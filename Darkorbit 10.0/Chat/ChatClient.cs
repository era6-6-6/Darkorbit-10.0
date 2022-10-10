using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Data;
using System.Net.Sockets;
using System.Text;
using static Darkorbit.Game.GameSession;
using Darkorbit.Game.Events;

namespace Darkorbit.Chat
{
    public enum Permissions
    {
        NORMAL = 0,
        ADMINISTRATOR = 3,
        CHAT_MODERATOR = 4,
        DONATOR = 1
    }
    //
    class ChatClient
    {
        public Socket Socket { get; set; }
        public int UserId { get; set; }
        public Permissions Permission { get; set; }
        public static int isStaff { get; set; }

        public List<int> ChatsJoined = new List<int>();

        public static JObject items;
        //public static string supportedLanguages;

        public ChatClient(Socket handler)
        {
            Socket = handler;

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void LoadChatRooms()
        {

            var rooms = new List<Room> {
                new Chat.Room(1, "Global", 0, -1),
                new Chat.Room(2, "MMO", 1, 1),
                new Chat.Room(3, "EIC", 1, 2),
                new Chat.Room(4, "VRU", 1, 3),
                new Chat.Room(5, "SEARCH CLAN", 2, -1),
                new Chat.Room(6, "GER", 3, -1),
                new Chat.Room(7, "TR", 4, -1),
            };

            foreach (var room in rooms)
                Chat.Room.Rooms.Add(room.Id, room);
        }

        public static void LoadCustomRoom(int isStaff, int roomId, string roomName)
        {

            var rooms = new List<Room> {
                new Chat.Room(roomId, roomName, 5, -1)
            };

            if (isStaff > 0)
            {
                if (!Room.Rooms.ContainsKey(roomId))
                {
                    foreach (var room in rooms)
                        Room.Rooms.Add(room.Id, room);
                }
            }
            else
            {
                if (Room.Rooms.ContainsKey(roomId))
                {
                    Room.Rooms.Remove(roomId);
                }
            }

        }

        public void Execute(string message)
        {
            try
            {
                string[] packet = message.Split(ChatConstants.MSG_SEPERATOR);
                switch (packet[0])
                {
                    case ChatConstants.CMD_USER_LOGIN:
                        var loginPacket = message.Replace("@", "%").Split('%');
                        UserId = Convert.ToInt32(loginPacket[3]);
                        if (!QueryManager.CheckSessionId(UserId, loginPacket[4]))
                        {
                            Close();
                            return;
                        }

                        var gameSession = GameManager.GetGameSession(UserId);
                        if (gameSession == null) return;

                        Permission = (Permissions)QueryManager.GetChatPermission(gameSession.Player.Id);
                        isStaff = QueryManager.GetChatPermission(gameSession.Player.Id);

                        LoadCustomRoom(isStaff, 10, "STAFF"); // Añadimos la sala de Staff.

                        if (GameManager.ChatClients.ContainsKey(UserId))
                            GameManager.ChatClients[gameSession.Player.Id]?.Close();

                        GameManager.ChatClients.TryAdd(gameSession.Player.Id, this);

                        Send("bv%" + gameSession.Player.Id + "#");
                        var servers = Room.Rooms.Aggregate(String.Empty, (current, chat) => current + chat.Value.ToString());
                        servers = servers.Remove(servers.Length - 1);
                        Send("by%" + servers + "#");

                        //Send($"dq%Group 2vs2 write /register (You need a group) . \n System  pushing is active, automatic ban .#");

                        ChatsJoined.Add(Room.Rooms.FirstOrDefault().Value.Id);

                        if (QueryManager.ChatFunctions.Banned(UserId))
                        {
                            Send($"{ChatConstants.CMD_BANN_USER}%#");
                            Close();
                            return;
                        }
                        break;
                    case ChatConstants.CMD_USER_MSG:
                        SendMessage(message);
                        break;
                    case ChatConstants.CMD_USER_JOIN:
                        var roomId = Convert.ToInt32(message.Split('%')[2].Split('@')[0]);
                        gameSession = GameManager.GetGameSession(UserId);

                        if (Room.Rooms.ContainsKey(roomId))
                        {
                            if (!ChatsJoined.Contains(roomId))
                                ChatsJoined.Add(roomId);
                        }
                        else
                        {
                            if (gameSession.Player.Storage.DuelInvites.ContainsKey(roomId))
                                AcceptDuel(gameSession.Player.Storage.DuelInvites[roomId], roomId);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Out.WriteLine("Exception: " + e, "ChatClient.cs");
                Logger.Log("error_log", $"- [ChatClient.cs] Execute void exception: {e}");
            }
        }

        public void AcceptDuel(Player inviterPlayer, int duelId)
        {
            var gameSession = GameManager.GetGameSession(UserId);

            if (!gameSession.Player.Storage.DuelInvites.ContainsKey(duelId))
            {
                Send($"dq%This invite is no longer available.#");
                return;
            }

            if (Duel.InDuel(gameSession.Player))
            {
                Send($"dq%You can't accept duels while you're in a duel.#");
                return;
            }

            if (Duel.InDuel(inviterPlayer))
            {
                Send($"dq%Your opponent is already fighting on duel with another player.#");
                return;
            }

            if (inviterPlayer != null && gameSession.Player != null)
            {
                if (gameSession.Player.Storage.IsInEquipZone && inviterPlayer.Storage.IsInEquipZone)
                {
                    gameSession.Player.Storage.DuelInvites.TryRemove(duelId, out inviterPlayer);
                    var players = new ConcurrentDictionary<int, Player>();
                    players.TryAdd(gameSession.Player.Id, gameSession.Player);
                    players.TryAdd(inviterPlayer.Id, inviterPlayer);

                    new Duel(players);
                }
            }
        }

        private Portal portal;
        public void SendMessage(string content)
        {
            Group group = null;
            var gameSession = GameManager.GetGameSession(UserId);
            if (gameSession == null) return;

            gameSession.LastActiveTime = DateTime.Now;
            string messagePacket = "";

            var packet = content.Replace("@", "%").Split('%');
            var roomId = packet[1];
            var message = packet[2];

            var cmd = message.Split(' ')[0];

            Logger.LogInDb(gameSession.Player.Name, gameSession.Player.playerId, message); // Guardar log en database.

            if (message.StartsWith("/reconnect"))
            {
                Close();
            }
            else if (cmd == "/w")
            {
                if (message.Split(' ').Length < 2)
                {
                    Send($"{ChatConstants.CMD_NO_WHISPER_MESSAGE}%#");
                    return;
                }

                var player = GameManager.GetPlayerByName(message.Split(' ')[1]);

                if (player == null || !GameManager.ChatClients.ContainsKey(player.Id))
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                if (player.Name == gameSession.Player.Name)
                {
                    Send($"dq%You can't whisper to yourself.#");
                    //Send($"{ChatConstants.CMD_CANNOT_WHISPER_YOURSELF}%#");
                    return;
                }

                message = message.Remove(0, player.Name.Length + 3);
                GameManager.ChatClients[player.Id].Send("cv%" + gameSession.Player.Name + "@" + message + "#");
                Send("cw%" + player.Name + "@" + message + "#");

                foreach (var client in GameManager.ChatClients.Values)
                {
                    if (gameSession.Player.Id != client.UserId && client.Permission == Permissions.ADMINISTRATOR && GameManager.ChatClients[player.Id].Permission != Permissions.ADMINISTRATOR)
                        client.Send($"dq%{gameSession.Player.Name} whispering to {player.Name}:{message}#");
                }

                //Logger.Log("chat_log", $"{gameSession.Player.Name} ({gameSession.Player.Id}) whispering to {player.Name} ({player.Id}):{message}");
            }
            else if (cmd == "/kick" && (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR))
            {
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var userId = 0;

                if (isNumeric)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                }

                var player = GameManager.GetPlayerById(userId);

                if (player != null && player.Name != gameSession.Player.Name)
                {
                    if (GameManager.ChatClients.ContainsKey(player.Id))
                    {
                        var client = GameManager.ChatClients[player.Id];
                        client.Send($"{ChatConstants.CMD_KICK_USER}%#");
                        client.Close();


                        player.GameSession.Disconnect(DisconnectionType.NORMAL);
                    }
                }
            }
            else if (cmd == "/duel" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2)
                {
                    Send($"dq%Use '/duel name' for invite someone to duel.#");
                    return;
                }

                var userName = message.Split(' ')[1];
                var duelPlayer = GameManager.GetPlayerByName(userName);

                if (duelPlayer == null || !GameManager.ChatClients.ContainsKey(duelPlayer.Id))
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                if (duelPlayer.Name == gameSession.Player.Name)
                {
                    Send($"{ChatConstants.CMD_CANNOT_INVITE_YOURSELF}%#");
                    return;
                }

                if (duelPlayer == null || duelPlayer == gameSession.Player || !GameManager.ChatClients.ContainsKey(duelPlayer.Id)) return;
                if (duelPlayer.Storage.DuelInvites.Any(x => x.Value == gameSession.Player))
                {
                    Send($"dq%{userName} already invited from you before.#");
                    return;
                }

                var duelId = Randoms.CreateRandomID();

                Send($"cr%{duelPlayer.Name}#");
                duelPlayer.Storage.DuelInvites.TryAdd(duelId, gameSession.Player);

                GameManager.ChatClients[duelPlayer.Id].Send("cj%" + duelId + "@" + "Duel" + "@" + 0 + "@" + gameSession.Player.Name + "#");
            }
            else if (cmd == "/msg" && Permission == Permissions.ADMINISTRATOR)
            {
                var msg = message.Remove(0, 4);
                GameManager.SendPacketToAll($"0|A|STD|{msg}");
            }
            else if (cmd == "/destroy" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var userId = 0;

                if (isNumeric)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                }

                var player = GameManager.GetPlayerById(userId);

                if (player == null)
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                player.Destroy(gameSession.Player, Game.DestructionType.PLAYER);
            }
            else if (cmd == "/ship" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var shipId = 0;

                if (isNumeric)
                {
                    shipId = Convert.ToInt32(message.Split(' ')[1]);
                }

                var ship = GameManager.GetShip(shipId);

                if (ship == null)
                {
                    Send($"dq%The ship that with entered doesn't exists.#");
                    return;
                }

                gameSession.Player.ChangeShip(shipId);

            }
            else if (cmd == "/jump" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var mapId = 0;

                if (isNumeric)
                {
                    mapId = Convert.ToInt32(message.Split(' ')[1]);
                }
                else
                {
                    string map = message.Split(' ')[1];
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        var query = $"SELECT mapID FROM server_maps WHERE name = '{map}'";

                        var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                        if (result.Rows.Count >= 1)
                        {
                            mapId = Int16.Parse(mySqlClient.ExecuteQueryRow(query)["mapID"].ToString());
                        }
                    }
                }

                if (GameManager.GetSpacemap(mapId) == null)
                {
                    Send($"dq%The map {mapId} doesn't exists.#");
                    return;
                }

                gameSession.Player.Jump(mapId, new Position(10300, 6300));

            }
            else if (cmd == "/move" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 3) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var isNumeric2 = int.TryParse(message.Split(' ')[1], out int n2);
                var userId = 0;
                var mapId = 0;

                if (isNumeric && isNumeric2)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                    mapId = Convert.ToInt32(message.Split(' ')[2]);
                }

                var player = GameManager.GetPlayerById(userId);
                var map = GameManager.GetSpacemap(mapId);

                if (player == null)
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                if (map == null)
                {
                    Send($"dq%The map that with entered doesn't exists.#");
                    return;
                }

                GameManager.GetPlayerById(player.Id)?.Jump(map.Id, new Position(0, 0));
            }
            else if (cmd == "/teleport" && Permission == Permissions.ADMINISTRATOR)
            {

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var userId = 0;

                if (isNumeric)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                }

                var player = GameManager.GetPlayerById(userId);

                if (player == null)
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                gameSession.Player?.Jump(player.Spacemap.Id, player.Position);
            }
            else if (cmd == "/pull" && Permission == Permissions.ADMINISTRATOR)
            {

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var userId = 0;

                if (isNumeric)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                }

                var player = GameManager.GetPlayerById(userId);

                if (player == null)
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                player?.Jump(gameSession.Player.Spacemap.Id, gameSession.Player.Position);
            }
            else if (cmd == "/speed+" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var speed = 0;

                if (isNumeric)
                {
                    speed = Convert.ToInt32(message.Split(' ')[1]);
                }

                gameSession.Player.SetSpeedBoost(speed);
            }
            else if (cmd == "/start-uba" && Permission == Permissions.ADMINISTRATOR)
            {
                var msg = message.Remove(0, 4);
                GameManager.SendPacketToAll($"0|A|STD|UBA Event Started! You can join with the button left up!");
                EventManager.UltimateBattleArena.UBAdisabled = false;
            }
            else if (cmd == "/stop-uba" && Permission == Permissions.ADMINISTRATOR)
            {
                var msg = message.Remove(0, 4);
                GameManager.SendPacketToAll($"0|A|STD|UBA event Ended!");
                EventManager.UltimateBattleArena.UBAdisabled = true;
            }
            else if (cmd == "/damage+" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var damage = 0;

                if (isNumeric)
                {
                    damage = Convert.ToInt32(message.Split(' ')[1]);
                }

                gameSession.Player.Storage.DamageBoost = damage;
            }
            else if (cmd == "/god" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var mod = message.Split(' ')[1];
                gameSession.Player.Storage.GodMode = mod == "on" ? true : mod == "off" ? false : false;

            }
            else if (cmd == "/dios" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var mod = message.Split(' ')[1];
                gameSession.Player.Storage.GodMode = mod == "on" ? true : mod == "off" ? false : false;
                gameSession.Player.Storage.DamageBoost = 1000000;
                gameSession.Player.SetSpeedBoost(1200);
            }

            /*else if (cmd == "/start-spaceball" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length >= 2)
                {
                    var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                    var limit = 0;

                    if (isNumeric)
                    {
                        limit = Convert.ToInt32(message.Split(' ')[1]);
                    }

                    EventManager.Spaceball.Limit = limit;
                }

                Console.WriteLine("Spaceball " + DateTime.Now + " ADMIN: " + gameSession.Player.Name);
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Spaceball'");
                EventManager.Spaceball.Start();
            }
            else if (cmd == "/stop-spaceball" && Permission == Permissions.ADMINISTRATOR)
            {
                EventManager.Spaceball.Stop();
                GameManager.SendChatSystemMessage("Spaceball ended");
            }*/


            else if (cmd == "/start-hitac" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length >= 2)
                {
                    var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                    var limit = 0;

                    if (isNumeric)
                    {
                        limit = Convert.ToInt32(message.Split(' ')[1]);
                    }

                    EventManager.Hitac.Limit = limit;
                }

                Console.WriteLine("Hitac " + DateTime.Now + " ADMIN: " + gameSession.Player.Name);
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Hitac'");
                EventManager.Hitac.Start();
            }

            else if (cmd == "/start_royal" && Permission == Permissions.ADMINISTRATOR)
            {

                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'B.royal'");
                EventManager.battleRoyal.Start();
            }

            else if (cmd == "/start_company" && Permission == Permissions.ADMINISTRATOR)
            {
                EventManager.BattleCompany.Start();
            }

            /*else if (cmd == "/start-invasion" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length >= 2)
                {
                    var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                    var seconds = 0;

                    if (isNumeric)
                    {
                        seconds = Convert.ToInt32(message.Split(' ')[1]);
                    }

                    EventManager.Invasion.seconds = seconds;
                }

                Console.WriteLine("Invasion " + DateTime.Now + " ADMIN: " + gameSession.Player.Name);
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Invasion'");
                EventManager.Invasion.Start();
            }
            else if (cmd == "/start-team" && Permission == Permissions.ADMINISTRATOR)
            {
                EventManager.TeamDeathmatch.Start();
            }
            else if (cmd == "/start_demaner" && Permission == Permissions.ADMINISTRATOR)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Demaner'");
                DemanerEvent.Start();
            }
            else if (cmd == "/start_meteorit" && Permission == Permissions.ADMINISTRATOR)
            {
                EventManager.SetIceMeteorid(true);
                //IceMetorit.ChangeIce(true);
            }
            else if (cmd == "/stop_meteorit" && Permission == Permissions.ADMINISTRATOR)
            {
                EventManager.SetIceMeteorid(false);
                //IceMetorit.ChangeIce(true);
            }*/

            else if (cmd == "/start_emperator" && Permission == Permissions.ADMINISTRATOR)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Emperator'");
                Emperator.Start();
            }
            else if (cmd == "/start_eggs" && Permission == Permissions.ADMINISTRATOR)
            {
                Game.Objects.Collectable.BoxenEvent = true;
                Game.Spacemap.BoxenEvent = true;
                GameManager.SendPacketToAll($"0|A|STD|Crazy Eggs Active!");
            }
            /*else if (cmd == "/start_rocketx2" && Permission == Permissions.ADMINISTRATOR)
            {
                EventManager.Setx2RocketEvent(true);
                GameManager.SendPacketToAll($"0|A|STD|Rocket Event activated");
            }
            else if (cmd == "/stop_rocketx2" && Permission == Permissions.ADMINISTRATOR)
            {
                EventManager.Setx2RocketEvent(false);
                GameManager.SendPacketToAll($"0|A|STD|Rocket Event deactivated");
            }
            else if (cmd == "/start_x2" && Permission == Permissions.ADMINISTRATOR)
            {
                Attackable.x2EventActive = true;
                GameManager.SendPacketToAll($"0|A|STD|X2 Rewards Event has been activated");
            }
            else if (cmd == "/stop_x2" && Permission == Permissions.ADMINISTRATOR)
            {
                Attackable.x2EventActive = false;
                GameManager.SendPacketToAll($"0|A|STD|X2 Rewards Event has been deactivated");
            }*/

            else if (cmd == "/start-jpb" && Permission == Permissions.ADMINISTRATOR)
            {
                Console.WriteLine("Jackpot " + DateTime.Now + " ADMIN: " + gameSession.Player.Name);
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Jpb'");
                EventManager.JackpotBattle.Start();
            }

            else if (cmd == "/register")
            {
                if (EventManager.GetTdmEvent() && !gameSession.Player.TdmDestroyed)
                {
                    if(!EventManager.TeamDeathmatchNew.waitingPlayers.Contains(gameSession.Player) && gameSession.Player.Level >= 12 && gameSession.Player.Storage.waitUBA == false && gameSession.Player.tdmLobby == null)
                    {
                        if(gameSession.Player.Spacemap.Id == 121)
                        {
                            gameSession.Player.SendPacket("0|A|STD|You are in the Uba you can't register now");
                            return;
                        }
                        gameSession.Player.Storage.TdmEnd = false;
                        EventManager.TeamDeathmatchNew.waitingPlayers.Add(gameSession.Player);
                        gameSession.Player.SendPacket($"0|A|STD|TDM Waiting Players: " + EventManager.TeamDeathmatchNew.waitingPlayers.Count);
                        gameSession.Player.Storage.waitTDM = true;
                        


                    }
                    else if (gameSession.Player.Level <= 11)
                    {
                        gameSession.Player.SendPacket("0|A|STD|You need Level 12 for Register");
                    }
                    else
                    {
                        gameSession.Player.SendPacket("0|A|STD|You are already on the waiting list in the tdm or you Match for UBA or TDM is running.");                        
                    }

                }
            }
            else if (cmd == "/unregister")
            {
                if (EventManager.GetTdmEvent())
                {
                    gameSession.Player.Storage.waitTDM = false;
                    EventManager.TeamDeathmatchNew.waitingPlayers.Remove(gameSession.Player);
                }
            }
            else if (cmd == "/check")
            {
                if (EventManager.GetTdmEvent())
                {
                    gameSession.Player.SendPacket($"0|A|STD|TDM Waiting Players: " + EventManager.TeamDeathmatchNew.waitingPlayers.Count);
                }
            }
            else if (cmd == "/add")
            {
                //var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                //var content2 = 0;

                //if (isNumeric)
                //{
                //content2 = Convert.ToInt32(message.Split(' ')[1]);
                //}

                //int playerID = content2;
            }
            /*else if (cmd == "/spaceballvote")
            {
                if (!VoteManager.checkVoteSP(gameSession.Player.Id))
                {
                    VoteManager.VotosSpaceball++;
                    VoteManager.checkStart();
                }
                else gameSession.Player.SendPacket("0|A|STD|You already voted.");

            }

            else if (cmd == "/invasionvote")
            {

                if (!VoteManager.checkVoteIN(gameSession.Player.Id))
                {
                    VoteManager.VotosInvasion++;
                    VoteManager.checkStart();
                }
                else gameSession.Player.SendPacket("0|A|STD|You already voted.");


            }
            else if (cmd == "/jackpotvote")
            {
                if (!VoteManager.checkVoteJP(gameSession.Player.Id))
                {
                    VoteManager.VotosJackpot++;
                    VoteManager.checkStart();
                }
                else gameSession.Player.SendPacket("0|A|STD|You already voted.");


            }*/
            /* else if (cmd == "/give_booster" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 3) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var isNumeric2 = int.TryParse(message.Split(' ')[2], out int n2);
                var isNumeric3 = int.TryParse(message.Split(' ')[3], out int n3);
                var userId = 0;
                var boosterType = 0;
                var time = 0;

                if (isNumeric && isNumeric2 && isNumeric3)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                    boosterType = Convert.ToInt32(message.Split(' ')[2]);
                    time = Convert.ToInt32(message.Split(' ')[3]);
                }

                var hours = message.Split(' ').Length == 4 ? time : 10;

                if (!new int[] { 0, 1, 2, 3, 8, 9, 10, 11, 12, 5, 6, 15, 16, 7, 4 }.Contains(boosterType)) return;

                var player = GameManager.GetPlayerById(userId);

                if (player != null)
                    player.BoosterManager.Add((BoosterType)boosterType, hours);
            }*/
            else if (cmd == "/ban" && (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR))
            {
                /* 0 CHAT BAN | 1 GAME BAN */
                if (message.Split(' ').Length < 4) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var isNumeric2 = int.TryParse(message.Split(' ')[2], out int n2);
                var isNumeric3 = int.TryParse(message.Split(' ')[2], out int n3);
                var userId = 0;
                var typeId = 0;
                var hours = 0;

                if (isNumeric && isNumeric2 && isNumeric3)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                    typeId = Convert.ToInt32(message.Split(' ')[2]);
                    hours = Convert.ToInt32(message.Split(' ')[3]);
                }

                var reason = message.Remove(0, (userId.ToString().Length + typeId.ToString().Length + hours.ToString().Length) + 7);

                if (typeId == 1 && Permission == Permissions.CHAT_MODERATOR) return;

                if (typeId == 0 || typeId == 1)
                {
                    QueryManager.ChatFunctions.AddBan(userId, gameSession.Player.Id, reason, typeId, (DateTime.Now.AddHours(hours)).ToString("yyyy-MM-dd HH:mm:ss"));

                    var player = GameManager.GetPlayerById(userId);

                    if (player != null)
                    {
                        if (GameManager.ChatClients.ContainsKey(player.Id))
                        {
                            var client = GameManager.ChatClients[player.Id];

                            if (client != null)
                            {
                                client.Send($"{ChatConstants.CMD_BANN_USER}%#");
                                
                                client.Close();
                            }
                        }

                        if (typeId == 1)
                        {
                            player.Destroy(null, DestructionType.MISC);
                            player.GameSession.Disconnect(DisconnectionType.NORMAL);
                        }
                    }
                }
            }
            else if (cmd == "/unban" && (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR))
            {
                /* 0 CHAT BAN | 1 GAME BAN */
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var isNumeric2 = int.TryParse(message.Split(' ')[1], out int n2);
                var userId = 0;
                var typeId = 0;

                if (isNumeric && isNumeric2)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                    typeId = Convert.ToInt32(message.Split(' ')[2]);
                }

                if (typeId == 1 && Permission == Permissions.CHAT_MODERATOR) return;

                if (typeId == 0 || typeId == 1)
                    QueryManager.ChatFunctions.UnBan(userId, gameSession.Player.Id, typeId);
            }
            else if (cmd == "/restart" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 2) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var seconds = 0;

                if (isNumeric)
                {
                    seconds = Convert.ToInt32(message.Split(' ')[1]);
                }

                var msg = message.Remove(0, seconds.ToString().Length + 9);
                GameManager.Restart(seconds, msg);
            }
            else if (cmd == "/users"/* && (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR)*/)
            {
                var users = GameManager.GameSessions.Values.Where(x => x.Player.RankId != 22).Aggregate(String.Empty, (current, user) => current + user.Player.Name + ", ");
                users = users.Remove(users.Length - 2);

                Send($"dq%Users online ({GameManager.GameSessions.Values.Where(x => x.Player.RankId != 22).Count()}): {users}#");
            }
            else if (cmd == "/system" && Permission == Permissions.ADMINISTRATOR)
            {
                message = message.Remove(0, 8);
                GameManager.SendChatSystemMessage(message);
            }
            else if (cmd == "/title" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 3) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var isNumeric2 = int.TryParse(message.Split(' ')[3], out int n2);
                var userId = 0;
                var perm = 0;

                if (isNumeric && isNumeric2)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                    perm = Convert.ToInt32(message.Split(' ')[3]);
                }

                var title = message.Split(' ')[2];
                var permanent = Convert.ToBoolean(perm);

                var player = GameManager.GetPlayerById(userId);
                if (player == null || !GameManager.ChatClients.ContainsKey(player.Id))
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                player.SetTitle(title, permanent);
            }
            else if (cmd == "/rmtitle" && Permission == Permissions.ADMINISTRATOR)
            {
                if (message.Split(' ').Length < 3) return;

                var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                var isNumeric2 = int.TryParse(message.Split(' ')[2], out int n2);
                var userId = 0;
                var perm = 0;

                if (isNumeric && isNumeric2)
                {
                    userId = Convert.ToInt32(message.Split(' ')[1]);
                    perm = Convert.ToInt32(message.Split(' ')[2]);
                }

                var permanent = Convert.ToBoolean(perm);
                var player = GameManager.GetPlayerById(userId);

                if (player == null || !GameManager.ChatClients.ContainsKey(player.Id))
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }

                player.SetTitle("", permanent);
            }
            else if (cmd == "/id" && (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR))
            {
                if (message.Split(' ').Length < 2) return;

                var userName = message.Split(' ')[1];

                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var query = $"SELECT userId FROM player_accounts WHERE pilotName = '{userName}'";

                    var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                    if (result.Rows.Count >= 1)
                    {
                        var userId = mySqlClient.ExecuteQueryRow(query)["userId"].ToString();

                        Send($"dq%{userName} id is: {userId}#");
                    }
                }

            }
            else if (cmd == "/at" && (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR))
            {
                if (message.Split(' ').Length < 2) return;
                var userId = Convert.ToInt32(message.Split(' ')[1]);
                var msg = message.Remove(0, userId.ToString().Length + 4);
                var player = GameManager.GetPlayerById(userId);
                if (player == null || !GameManager.ChatClients.ContainsKey(player.Id))
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                    return;
                }
                var color = (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR) ? "j" : "a";

                messagePacket = $"{color}%" + roomId + "@" + "FO-" + gameSession.Player.Name + "@" + " " + player.Name + " " + msg;
                Send(messagePacket + "#");
            }
            else if (cmd == "/sysmsg" && Permission == Permissions.ADMINISTRATOR)
            {
                var msg = message.Remove(0, 8);
                GameManager.SendPacketToAll($"0|n|KSMSG|{msg}");
            }

            /* else if (cmd == "/done" && Permission == Permissions.ADMINISTRATOR)
             {
                 if (message.Split(' ').Length < 4) return;

                 var isNumeric = int.TryParse(message.Split(' ')[1], out int n);
                 var isNumeric2 = int.TryParse(message.Split(' ')[2], out int n2);
                 var isNumeric3 = int.TryParse(message.Split(' ')[3], out int n3);
                 var userId = 0;
                 var typeId = 0; //1 uridium / 2 credits / 3 honor / 4 experience / 5 premium / 6 ec.
                 var amount = 0;

                 if (isNumeric && isNumeric2 && isNumeric3)
                 {
                     userId = Convert.ToInt32(message.Split(' ')[1]);
                     typeId = Convert.ToInt32(message.Split(' ')[2]);
                     amount = Convert.ToInt32(message.Split(' ')[3]);
                 }

                 var player = GameManager.GetPlayerById(userId);

                 if (player == null || !GameManager.ChatClients.ContainsKey(player.Id))
                 {
                     Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                     return;
                 }
                 if (player != null && new[] { 1, 2, 3, 4, 5, 6 }.Contains(typeId))
                 {
                     var rewardName = "";
                     switch (typeId)
                     {
                         case 1:
                             player.ChangeData(DataType.URIDIUM, amount);
                             rewardName = "uridium";
                             break;
                         case 2:
                             player.ChangeData(DataType.CREDITS, amount);
                             rewardName = "credits";
                             break;
                         case 3:
                             player.ChangeData(DataType.HONOR, amount);
                             rewardName = "honor";
                             break;
                         case 4:
                             player.ChangeData(DataType.EXPERIENCE, amount);
                             rewardName = "experience";
                             break;
                         case 5:
                             QueryManager.SavePlayer.changePremium(player, amount);
                             rewardName = "premium";
                             break;
                         case 6:
                             player.ChangeData(DataType.EC, amount);
                             rewardName = "E.C";
                             break;

                     }

                     player.SendPacket($"0|A|STD|You got {amount} {rewardName} from {gameSession.Player.Name}.");
                     Send($"dq%{player.Name} has got {amount} {rewardName} from you.#");
                     GameManager.ChatClients[player.Id].Send($"dq%You got {amount} {rewardName} from {gameSession.Player.Name}.#");
                 }
             } */

            /*else if (cmd == "/leave")
            {
                if ((gameSession.Player.Spacemap.Id == 51 || gameSession.Player.Spacemap.Id == 52 || gameSession.Player.Spacemap.Id == 53 || gameSession.Player.Spacemap.Id == 55 || gameSession.Player.Spacemap.Id == 55 || gameSession.Player.Spacemap.Id == 74))
                {
                    gameSession.Player.Jump(gameSession.Player.GetBaseMapId(true), gameSession.Player.GetBasePosition(true));
                }
            }*/
            else if (cmd == "/dronetest" && Permission == Permissions.ADMINISTRATOR)
            {
                var level = 6;

                string DronePacket = $"0|n|d|" + gameSession.Player.Id + $"|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1|2|{level}|1";
                gameSession.Player.SendPacket(DronePacket);
                gameSession.Player.SendPacketToInRangePlayers(DronePacket);
            }
            else if (cmd == "/test" && Permission == Permissions.ADMINISTRATOR)
            {
                GameManager.SendCommandToAll(VideoWindowRemoveCommand.write(1));
            }
            else if (cmd == "/test1" && Permission == Permissions.ADMINISTRATOR)
            {
                GameManager.SendCommandToAll(VideoWindowCreateCommand.write(1, "l", false, new List<string> { }, 7, 1));
            }
            else if (cmd == "/ghost" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.AddVisualModifier(VisualModifierCommand.GHOST_EFFECT, 0, "", 0, true);
            }
            else if (cmd == "/remghost" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.RemoveVisualModifier(VisualModifierCommand.GHOST_EFFECT);
            }
            else if (cmd == "/redglow" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.AddVisualModifier(VisualModifierCommand.RED_GLOW, 0, "", 0, true);
            }
            else if (cmd == "/remredglow" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.RemoveVisualModifier(VisualModifierCommand.RED_GLOW);
            }
            else if (cmd == "/greenglow" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.AddVisualModifier(VisualModifierCommand.GREEN_GLOW, 0, "", 0, true);
            }
            else if (cmd == "/remgreenglow" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.RemoveVisualModifier(VisualModifierCommand.GREEN_GLOW);
            }
            else if (cmd == "/legend" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.AddVisualModifier(VisualModifierCommand.LEGENDARY_NPC_NAME, 0, "", 0, true);
            }
            else if (cmd == "/remlegend" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.RemoveVisualModifier(VisualModifierCommand.LEGENDARY_NPC_NAME);
            }
            else if (cmd == "/emptarget" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.AddVisualModifier(VisualModifierCommand.ULTIMATE_EMP_TARGET, 0, "", 0, true);
            }
            else if (cmd == "/rememptarget" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.RemoveVisualModifier(VisualModifierCommand.ULTIMATE_EMP_TARGET);
            }
            else if (cmd == "/camera" && Permission == Permissions.ADMINISTRATOR)
            {
                gameSession.Player.AddVisualModifier(VisualModifierCommand.CAMERA, 0, "", 0, true);
            }
            else if (cmd == "/test333" && Permission == Permissions.ADMINISTRATOR)
            {
                
            }
            else if (cmd == "/spawn" && Permission == Permissions.ADMINISTRATOR)
            {
                var npc = Convert.ToInt32(message.Split(' ')[1]);
                var count = Convert.ToInt32(message.Split(' ')[2]);
                int i = 0;

                for (i = 0; i < count; i++)
                {
                    new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npc), gameSession.Player.Spacemap, gameSession.Player.Position);
                }

                gameSession.Player.SendPacket($"0|A|STD|Created NPC -> {GameManager.GetShip(npc).Name} {i} times");
            }
            else if (cmd == "/chatkick" && (Permission == Permissions.ADMINISTRATOR || Permission == Permissions.CHAT_MODERATOR))
            {
                var userid = Convert.ToInt32(message.Split(' ')[1]);

                var player = GameManager.GetPlayerById(userid);

                if (player == null)
                {
                    Send($"{ChatConstants.CMD_USER_NOT_EXIST}%#");
                }
                else
                {
                    if (GameManager.ChatClients.ContainsKey(player.Id))
                    {
                        var chatuser = GameManager.ChatClients[player.Id];
                        chatuser.Send($"{ChatConstants.CMD_KICK_BY_SYSTEM}%#");
                        chatuser.Close();
                        GameManager.SendChatSystemMessage($"dq%{player.Name} has been kicked by an Administrator#");
                    }
                }
            }
            else if (cmd == "/reloadfilter" && Permission == Permissions.ADMINISTRATOR)
            {
                items = null;
                LoadFilter();
            }
            else if (cmd == "/info" && Permission == Permissions.ADMINISTRATOR)
            {
                var user = GameManager.GetPlayerById(gameSession.Client.UserId);

                var selectedCharacter = user.SelectedCharacter;

                if (selectedCharacter == null)
                {
                    Send($"dq%Error. Select a Character first.#");
                }
                else
                {
                    //Send($"dq%Name: {selectedCharacter.Name}#Faction: {selectedCharacter.FactionId}#Map: {selectedCharacter.Spacemap.Id}#Position: {selectedCharacter.Position}#HP: {selectedCharacter.CurrentHitPoints} / {selectedCharacter.MaxHitPoints}#SP: {selectedCharacter.CurrentShieldPoints} / {selectedCharacter.MaxShieldPoints}#");
                    Send($"dq%Name: {selectedCharacter.Name}#");
                    Send($"dq%Faction: {selectedCharacter.FactionId}#");
                    Send($"dq%Map: {selectedCharacter.Spacemap.Id}#");
                    Send($"dq%PositionX: {selectedCharacter.Position.X} / Y: {selectedCharacter.Position.Y}#");
                    Send($"dq%HP: {selectedCharacter.CurrentHitPoints} / {selectedCharacter.MaxHitPoints}#");
                    Send($"dq%SP: {selectedCharacter.CurrentShieldPoints} / {selectedCharacter.MaxShieldPoints}#");
                    Send($"dq%Clan: ID: {selectedCharacter.Clan.Id}, Name: [{selectedCharacter.Clan.Tag}]{selectedCharacter.Clan.Name}");
                }
            }
            else if (cmd == "/spawnguard" && Permission == Permissions.ADMINISTRATOR)
            {
                //new Flagship(gameSession.Player);
                new NPCFlagship(Randoms.CreateRandomID(), GameManager.GetShip(56), gameSession.Player.Spacemap, gameSession.Player.Position);
            }

            else
            {
                if (!cmd.StartsWith("/"))
                {
                    /*foreach (var m in Filter)
                    {
                        if (message.ToLower().Contains(m.ToLower()) && Permission == Permissions.NORMAL)
                        {
                            Send($"{ChatConstants.CMD_KICK_BY_SYSTEM}%#");
                            Close();
                            return;
                        }
                    }*/

                    message = CheckFilter(message);

                    foreach (var pair in GameManager.ChatClients.Values)
                    {
                        if (pair.ChatsJoined.Contains(Convert.ToInt32(roomId)))
                        {
                            if (pair.Permission == Permissions.ADMINISTRATOR || pair.Permission == Permissions.CHAT_MODERATOR)
                            {
                                //Translate(message, pair, gameSession, roomId, Permission);
                                Task translate = Task.Factory.StartNew(() => Translate(message, pair, gameSession, roomId, Permission));
                            }
                            else
                            {
                                SendChatMessage(message, pair, gameSession, roomId, Permission);
                            }
                        }
                    }

                    //Logger.Log("chat_log", $"{gameSession.Player.Name} ({gameSession.Player.Id}): {message}");
                }
            }
        }

        static void SendChatMessage(string message, ChatClient pair, GameSession gameSession, string roomId, Permissions Permission)
        {
            string messagePacket = "";

            var name = gameSession.Player.Name + (pair.Permission == Permissions.ADMINISTRATOR || pair.Permission == Permissions.CHAT_MODERATOR ? $" ({gameSession.Player.Id})" : "");

            var color = "a"; // WHITE
            if (Permission == Permissions.CHAT_MODERATOR)
            {
                color = "j"; // RED
                messagePacket = $"{color}%" + roomId + "@CM-" + name + "@" + message + "@4";
            }
            else if (Permission == Permissions.ADMINISTRATOR)
            {
                color = "j"; // RED
                messagePacket = $"{color}%" + roomId + "@GM-" + name + "@" + message;
            }
            else
            {
                color = "a"; // WHITE
                messagePacket = $"{color}%" + roomId + "@" + name + "@" + message;
            }

            if (gameSession.Player.Clan.Tag != "")
                messagePacket += "@" + gameSession.Player.Clan.Tag;

            pair.Send(messagePacket + "#");
            //Console.WriteLine(messagePacket);
        }

        public void Close()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();

                GameManager.ChatClients.TryRemove(UserId, out var value);
            }
            catch (Exception)
            {
                //ignore
                //Logger.Log("error_log", $"- [ChatClient.cs] Close void exception: {e}");
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                if (Socket == null || !Socket.IsBound || !Socket.Connected) return;

                String content = String.Empty;

                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content = Encoding.UTF8.GetString(
                        state.buffer, 0, bytesRead);

                    if (content.Trim() != "")
                    {
                        Execute(content);

                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }
                else
                {
                    Close();
                }
            }
            catch
            {
                Close();
            }
        }

        public void Send(String data)
        {
            try
            {
                if (Socket == null || !Socket.IsBound || !Socket.Connected) return;

                byte[] byteData = Encoding.UTF8.GetBytes(data);

                Socket.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), Socket);
            }
            catch (Exception e)
            {
                //Logger.Log("error_log", $"- [ChatClient.cs] Send void exception: {e}");
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                handler.EndSend(ar);
            }
            catch (Exception)
            {
                //Logger.Log("error_log", $"- [ChatClient.cs] SendCallback void exception: {e}");
            }
        }

        public static void LoadFilter()
        {
            try
            {
                using (StreamReader r = new StreamReader(@"C:/Users/Administrator/Desktop/filter.json"))
                {
                    string json = r.ReadToEnd();
                    items = JObject.Parse(json);
                }

                /*var url = "http://45.10.24.45/languages";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                var result = "";
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                supportedLanguages = result;*/
            } catch(Exception ex)
            {
                Logger.Log("error_log", $"- [ChatClient.cs] Load filter list exception: {ex}");
            }
        }

        public static string CheckFilter(string msg)
        {
            try
            {
                string result = msg;
                string[] resultList = msg.Split(' ');

                foreach (var x in items)
                {
                    foreach (string y in items[x.Key])
                    {
                        //wenn länge kleiner gleich 2 zeichen, dann direkte prüfung ohne contains
                        /*if (msg.Length <= 2)
                        {
                            if(msg == y)
                            {
                                result = "*****";
                                break;
                            }
                        }
                        else
                        {
                            if (y.Contains(msg) || msg.Contains(y))
                            {
                                result = "*****";
                                break;
                            }
                        }*/
                        if (resultList.Length > 0)
                        {
                            for (int i = 0; i < resultList.Length; i++)
                            {
                                if (resultList[i].ToLower() == y.ToLower())
                                {
                                    resultList[i] = "*****";
                                }
                            }
                        }
                        else
                        {
                            if (msg.ToLower() == y.ToLower())
                            {
                                result = "*****";
                                break;
                            }
                        }
                    }
                }

                if (resultList.Length > 0)
                {
                    result = string.Join(" ", resultList);
                }

                return result;
            } catch(Exception ex)
            {
                Logger.Log("error_log", $"- [ChatClient.cs] Check filter exception: {ex}");
                return msg;
            }
        }

        static async Task Translate(string message, ChatClient pair, GameSession gameSession, string roomId, Permissions Permission)
        {
            /*try
            {
                // Input and output languages are defined as parameters.
                string route = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to=en";
                object[] body = new object[] { new { Text = message } };
                var requestBody = JsonConvert.SerializeObject(body);
                var text = "";

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", "b4d192494ef44b1dabcfb10bcf468993");
                    request.Headers.Add("Ocp-Apim-Subscription-Region", "northeurope");

                    // Send the request and get response.
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();

                    TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                    // Iterate over the deserialized results.
                    foreach (TranslationResult o in deserializedOutput)
                    {
                        // Print the detected input languge and confidence score.
                        //Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                        // Iterate over the results and print each translation.
                        foreach (Translation t in o.Translations)
                        {
                            //Console.WriteLine("Translated to {0}: {1}", t.To, t.Text);
                            text = t.Text;
                            break;
                        }
                    }
                }

                if (text == "")
                {
                    SendChatMessage(message, pair, gameSession, roomId, Permission);
                }
                else
                {
                    SendChatMessage(text, pair, gameSession, roomId, Permission);
                }
            } catch(Exception ex)
            {*/
                SendChatMessage(message, pair, gameSession, roomId, Permission);
            //}
        }

        /*public static string TranslateText(string msg)
        {
            try
            {
                var factory = new RankedLanguageIdentifierFactory();
                var identifier = factory.Load(@"C:\Users\Administrator\Desktop\Wiki82.profile.xml");
                var languages = identifier.Identify(msg);
                var mostCertainLanguage = languages.FirstOrDefault();

                if (mostCertainLanguage != null)
                    return Translate(msg, mostCertainLanguage.Item1.Iso639_2T);
                else
                    return Translate(msg, "auto");
            } catch(Exception ex)
            {
                Logger.Log("error_log", $"- [ChatClient.cs] Translate text exception: {ex}");
                return msg;
            }
        }

        public static string Translate(string msg, string lang)
        {
            try
            {
                bool langSupported = false;

                JArray translation = JArray.Parse(supportedLanguages);
                for (int i = 0; i < translation.Count; i++)
                {
                    string code = translation[i]["code"].ToString();
                    string name = translation[i]["name"].ToString();

                    if (code == lang)
                    {
                        langSupported = true;
                    }
                }

                if (langSupported)
                {
                    var url = "http://45.10.24.45/translate";

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Method = "POST";

                    httpRequest.Accept = "application/json";
                    httpRequest.ContentType = "application/json";

                    var data = @"{
                  ""q"": ""{{{XXX}}}"",
                  ""source"": ""{{{YYY}}}"",
                  ""target"": ""en""
                }";

                    data = data.Replace("{{{XXX}}}", msg);
                    data = data.Replace("{{{YYY}}}", lang);

                    using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                    }

                    var result = "";
                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }

                    return JObject.Parse(result)["translatedText"].ToString();
                }
                else
                {
                    return msg;
                }
            } catch(Exception ex)
            {
                Logger.Log("error_log", $"- [ChatClient.cs] Translate exception: {ex}");
                return msg;
            }
        }*/

        public class TranslationResult
        {
            public DetectedLanguage DetectedLanguage { get; set; }
            public TextResult SourceText { get; set; }
            public Translation[] Translations { get; set; }
        }

        public class DetectedLanguage
        {
            public string Language { get; set; }
            public float Score { get; set; }
        }

        public class TextResult
        {
            public string Text { get; set; }
            public string Script { get; set; }
        }

        public class Translation
        {
            public string Text { get; set; }
            public TextResult Transliteration { get; set; }
            public string To { get; set; }
            public Alignment Alignment { get; set; }
            public SentenceLength SentLen { get; set; }
        }

        public class Alignment
        {
            public string Proj { get; set; }
        }

        public class SentenceLength
        {
            public int[] SrcSentLen { get; set; }
            public int[] TransSentLen { get; set; }
        }
    }
}

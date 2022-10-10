global using Darkorbit.Game.Objects;
global using Darkorbit.Game;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static Darkorbit.Game.GameSession;
using Darkorbit;
using Darkorbit.Game.Events;
using Darkorbit.Chat;
using Pet = Darkorbit.Game.Objects.Pet;

public class StateObject
{
    public Socket workSocket = null;
    public const int BufferSize = 1024;
    public byte[] buffer = new byte[BufferSize];
    public StringBuilder sb = new StringBuilder();
}

class SocketServer
{
    public static ManualResetEvent allDone = new ManualResetEvent(false);
    public static int Port = 9001;

    public static void StartListening()
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);

        Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                allDone.Reset();

                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Logger.Log("error_log", $"- [SocketServer.cs] StartListening void exception: {e}");
        }
    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        try
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Connection(handler);
        }
        catch (Exception e)
        {
            Logger.Log("error_log", $"- [SocketServer.cs] AcceptCallback void exception: {e}");
        }
    }

    public static void Connection(Socket handler)
    {
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
    }

    public static void Execute(JObject json, JObject parameters, Socket handler)
    {
        string key = "aaaaaaaaaaaaaaaaaaaaaaaaaa23bbbbbbbbbbbbbcc3";
        switch (String(json["Action"]))
        {
            case "LockSync":
                GameManager.GetPlayerById(Int(parameters["UserId"])).LockSync = true;
                break;
            case "UnlockSync":
                GameManager.GetPlayerById(Int(parameters["UserId"])).LockSync = false;
                break;
            case "LoadUserData":
                GameManager.GetPlayerById(Int(parameters["UserId"])).LoadData();
                break;
            case "OnlineIds":
                Send(handler, JsonConvert.SerializeObject(GameManager.GameSessions.Keys).ToString());
                break;
            case "OnlineCount":
                Send(handler, GameManager.GameSessions.Count.ToString());
                break;
            case "OnlineAfkCount":
                Send(handler, GameManager.afkPlayers.Count.ToString());
                break;
            case "OnlineGGCount":
                Send(handler, GameManager.playersInGalaxyGates.Count.ToString());
                break;
            case "IsOnline":
                var player = GameManager.GetPlayerById(Int(parameters["UserId"]));
                var online = player?.GameSession != null ? true : false;
                Send(handler, online.ToString());
                break;
            case "IsInEquipZone":
                player = GameManager.GetPlayerById(Int(parameters["UserId"]));
                var inEquipZone = player?.GameSession != null ? player.Storage.IsInEquipZone : false;
                Send(handler, inEquipZone.ToString());
                break;
            case "GetPosition":
                player = GameManager.GetPlayerById(Int(parameters["UserId"]));
                var spacemapName = player?.GameSession != null ? player.Spacemap.Name : "";
                Send(handler, spacemapName.ToString());
                break;
            case "AvailableToChangeShip":
                player = GameManager.GetPlayerById(Int(parameters["UserId"]));
                var available = player?.Storage.lastChangeShipTime.AddSeconds(5) < DateTime.Now ? true : false;
                Send(handler, available.ToString());
                break;
            case "BanUser":
                BanUser(GameManager.GetPlayerById(Int(parameters["UserId"])));
                break;
            case "BuyItem":
                BuyItem(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["ItemType"]), (DataType)Short(parameters["DataType"]), Int(parameters["Amount"]));
                break;
            case "ChangeClanData":
                ChangeClanData(GameManager.GetClan(Int(parameters["ClanId"])), String(parameters["Name"]), String(parameters["Tag"]), Int(parameters["FactionId"]));
                break;
            case "ChangeShip":
                ChangeShip(GameManager.GetPlayerById(Int(parameters["UserId"])), GameManager.GetShip(Int(parameters["ShipId"])));
                break;
            case "ChangeCompany":
                ChangeCompany(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["UridiumPrice"]), Int(parameters["HonorPrice"]), Int(parameters["ExperiencePrice"]));
                break;
            case "UpdateLogfiles":
                UpdateLogfiles(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Logfiles"]));
                break;
            case "UpdateItems":
                UpdateItems(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Amount"]), parameters["LootId"].ToString());
                break;
            case "UpdateUridium":
                UpdateUridium(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["UridiumPrice"]), String(parameters["Type"]));
                break;
            case "UpdateCredits":
                UpdateCredits(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["CreditPrice"]), String(parameters["Type"]));
                break;
            case "UpdateTitle":
                UpdateTitle(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["title"]));
                break;
            case "UpdateStatus":
                UpdateStatus(GameManager.GetPlayerById(Int(parameters["UserId"])));
                break;
            case "JoinToClan":
                JoinToClan(GameManager.GetPlayerById(Int(parameters["UserId"])), GameManager.GetClan(Int(parameters["ClanId"])));
                break;
            case "LeaveFromClan":
                LeaveFromClan(GameManager.GetPlayerById(Int(parameters["UserId"])));
                break;
            case "CreateClan":
                CreateClan(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["ClanId"]), Int(parameters["FactionId"]), String(parameters["Name"]), String(parameters["Tag"]));
                break;
            case "DeleteClan":
                DeleteClan(GameManager.GetClan(Int(parameters["ClanId"])));
                break;
            case "StartDiplomacy":
                StartDiplomacy(GameManager.GetClan(Int(parameters["SenderClanId"])), GameManager.GetClan(Int(parameters["TargetClanId"])), Short(parameters["DiplomacyType"]));
                break;
            case "EndDiplomacy":
                EndDiplomacy(GameManager.GetClan(Int(parameters["SenderClanId"])), GameManager.GetClan(Int(parameters["TargetClanId"])));
                break;
            case "UpgradeSkillTree":
                UpgradeSkillTree(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["Skill"]));
                break;
            case "ResetSkillTree":
                ResetSkillTree(GameManager.GetPlayerById(Int(parameters["UserId"])));
                break;
            case "KickPlayer":
                KickPlayer(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["Reason"]));
                break;
            case "AddAmmo":
                AddAmmo(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["itemId"]), Int(parameters["amount"]));
                break;
            case "initializePet":
                initializePet(GameManager.GetPlayerById(Int(parameters["UserId"])));
                break;
            case "startSpaceball":
                var spaceball = startSpaceball();
                Send(handler, spaceball.ToString());
                break;
            case "startHitac":
                var Hitac = startHitac();
                Send(handler, Hitac.ToString());
                break;
            case "startRoyal":
                var royal = startRoyal();
                Send(handler, royal.ToString());
                break;
            case "startCompany":
                var company = startCompany();
                Send(handler, company.ToString());
                break;
            case "startInvasion":
                var invasion = startInvasion();
                Send(handler, invasion.ToString());
                break;
           /* case "startTeam":
                var team = startTeam();
                Send(handler, team.ToString());
                break;*/
            case "startDemaner":
                var demaner = startDemaner();
                Send(handler, demaner.ToString());
                break;
            case "startMeteorit":
                var meteorit = startMeteorit();
                Send(handler, meteorit.ToString());
                break;
            case "startJackpot":
                var jackpot = startJackpot();
                Send(handler, jackpot.ToString());
                break;
            case "startEmperator":
                var emperator = startEmperator();
                Send(handler, emperator.ToString());
                break;
            case "setUridium":
                setUridium(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Uridium"]));
                break;
            case "setCredits":
                setCredits(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Credits"]));
                break;
            case "setHonor":
                setHonor(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Honor"]));
                break;
            case "setExperience":
                setExperience(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Experience"]));
                break;
            case "updatePet":
                updatePet(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["PetName"]), Short(parameters["PetDesignn"]));
                break;
            case "updatePetFuel":
                updatePetFuel(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Amount"]));
                break;
            case "getPetFuel":
                int petFuel = getPetFuel(GameManager.GetPlayerById(Int(parameters["UserId"])));
                Send(handler, petFuel.ToString());
                break;
            case "setPetModule":
                setPetModule(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["TypeModule"]));
                break;
            case "updateEC":
                updateEC(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Amount"]), String(parameters["Type"]));
                break;
            case "setRewardEvent":
                setX2Event(String(parameters["status"]));
                break;
            case "setBoxenEvent":
                setCrazyEggs(String(parameters["status"]));
                break;
            case "kick":
                int data = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                if (GameManager.GetPlayerById(Int(parameters["UserId"])) != null)
                {
                    GameManager.GetPlayerById(Int(parameters["UserId"])).GameSession.Disconnect(DisconnectionType.NORMAL);
                    data = 1;

                    Send(handler, data.ToString());
                }
                else
                {
                    Send(handler, data.ToString());
                }
                break;
            case "msg":
                int dataMsg = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var msg = String(parameters["Message"]).Remove(0, 4);
                GameManager.SendPacketToAll($"0|A|STD|{msg}");
                dataMsg = 1;

                Send(handler, dataMsg.ToString());

                break;
            case "destroy":
                int dataDestroy = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var gameSession = GameManager.GetGameSession(Int(parameters["UserId"]));
                GameManager.GetPlayerById(Int(parameters["UserId"])).Destroy(gameSession.Player, DestructionType.PLAYER);
                dataDestroy = 1;

                Send(handler, dataDestroy.ToString());

                break;
            case "ship":
                int dataShip = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var ship = GameManager.GetShip(Int(parameters["ShipId"]));

                if (ship == null)
                {
                    Send(handler, dataShip.ToString());
                    return;
                }

                var gameSession2 = GameManager.GetGameSession(Int(parameters["UserId"]));
                gameSession2.Player.ChangeShip(Int(parameters["ShipId"]));
                dataShip = 1;

                Send(handler, dataShip.ToString());

                break;
            case "jump":
                int dataJump = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var gameSession3 = GameManager.GetGameSession(Int(parameters["UserId"]));
                gameSession3.Player.Jump(Int(parameters["MapId"]), new Position(10300, 6300));
                dataJump = 1;

                Send(handler, dataJump.ToString());

                break;
            case "speed":
                int dataSpeed = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var gameSession4 = GameManager.GetGameSession(Int(parameters["UserId"]));
                gameSession4.Player.SetSpeedBoost(Int(parameters["Speed"]));
                dataSpeed = 1;

                Send(handler, dataSpeed.ToString());

                break;
            case "damage":
                int dataDamage = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var gameSession5 = GameManager.GetGameSession(Int(parameters["UserId"]));
                gameSession5.Player.Storage.DamageBoost = Int(parameters["Damage"]);
                dataDamage = 1;

                Send(handler, dataDamage.ToString());

                break;
            case "god":
                int dataGod = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var gameSession6 = GameManager.GetGameSession(Int(parameters["UserId"]));
                gameSession6.Player.Storage.GodMode = String(parameters["Mod"]) == "on" ? true : String(parameters["Mod"]) == "off" ? false : false;
                dataGod = 1;

                Send(handler, dataGod.ToString());

                break;
            case "restart":
                int dataRestart = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                GameManager.Restart(Int(parameters["Seconds"]), String(parameters["Message"]));
                dataRestart = 1;

                Send(handler, dataRestart.ToString());

                break;
            case "users":
                string dataUsers;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var users = GameManager.GameSessions.Values.Where(x => x.Player.RankId != 22).Aggregate("", (current, user) => current + user.Player.Name + ", ");
                users = users.Remove(users.Length - 2);

                dataUsers = "Users online (" + GameManager.GameSessions.Values.Where(x => x.Player.RankId != 22).Count() + "): " + users;

                Send(handler, dataUsers);

                break;
            case "leave":
                int dataLeave;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var gameSession7 = GameManager.GetGameSession(Int(parameters["UserId"]));
                gameSession7.Player.Jump(gameSession7.Player.GetBaseMapId(), gameSession7.Player.GetBasePosition());
                dataLeave = 1;

                Send(handler, dataLeave.ToString());

                break;
            case "ban":
                int dataBan = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var userId = Int(parameters["UserId"]);
                var typeId = Int(parameters["TypeId"]);
                var hours = Int(parameters["Hours"]);
                var reason = String(parameters["Reason"]);

                if (typeId == 1)
                {
                    var gameSession8 = GameManager.GetGameSession(userId);

                    QueryManager.ChatFunctions.AddBan(userId, gameSession8.Player.Id, reason, typeId, (DateTime.Now.AddHours(hours)).ToString("yyyy-MM-dd HH:mm:ss"));

                    var player2 = GameManager.GetPlayerById(userId);

                    if (player2 != null)
                    {
                        if (typeId == 1)
                        {
                            player2.Destroy(null, DestructionType.MISC);
                            player2.GameSession.Disconnect(DisconnectionType.NORMAL);
                            dataBan = 1;
                        }
                    }
                }

                Send(handler, dataBan.ToString());

                break;
            case "unban":
                int dataUnBan = 0;

                if (String(parameters["Key"]) != key)
                {
                    return;
                }

                var userId2 = Int(parameters["UserId"]);
                var typeId2 = Int(parameters["TypeId"]);

                if (typeId2 == 0 || typeId2 == 1)
                {
                    var gameSession9 = GameManager.GetGameSession(userId2);
                    QueryManager.ChatFunctions.UnBan(userId2, gameSession9.Player.Id, typeId2);
                    dataUnBan = 1;
                }

                Send(handler, dataUnBan.ToString());

                break;
            case "timeOnline":
                TimeSpan timeSvOnline = DateTime.Now - Program.timeOnline;
                String dataSend;

                dataSend = "Gameserver is Online since " + timeSvOnline.Days + " Day(s) " + timeSvOnline.Hours + " Hour(s) and " + timeSvOnline.Minutes + " Minute(s)";

                Send(handler, dataSend);
                break;
            case "buyFormation":
                buyFormation(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["Formation"]));
                break;
            case "buyKey":
                buyKey(GameManager.GetPlayerById(Int(parameters["UserId"])), String(parameters["Key"]), Int(parameters["Amount"]));
                break;
            case "UpdateHonor":
                UpdateHonor(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Honor"]), String(parameters["Type"]));
                break;
            case "UpdateExperience":
                UpdateExperience(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["Experience"]), String(parameters["Type"]));
                break;
            case "SendMessageToUser":
                Player playerTU = GameManager.GetPlayerById(Int(parameters["UserId"]));
                var message = String(parameters["msg"]);
                playerTU.SendPacket($"0|A|STD|{message}");
                break;
            case "updateDroneEXP":
                Player playerDrone = GameManager.GetPlayerById(Int(parameters["UserId"]));
                int amount = Int(parameters["Amount"]);
                playerDrone.droneExp = amount;
                int level = playerDrone.DroneManager.droneLevel(amount);

                playerDrone.DroneManager.UpdateDrones(playerDrone.droneExp, true);
                QueryManager.SavePlayer.SaveDronesEXP(playerDrone);
                playerDrone.SendPacket($"0|A|STD|Your Drone level has uploaded to the level {level}.");
                break;
            case "SendBoosters":
                sendBoosterToUser(GameManager.GetPlayerById(Int(parameters["UserId"])), Int(parameters["typeBooster"]), Int(parameters["hoursBooster"]));
                break;
        }
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        try
        {
            String content = string.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                content = Encoding.UTF8.GetString(
                    state.buffer, 0, bytesRead);

                if (!string.IsNullOrEmpty(content))
                {
                    var json = Parse(content);
                    if (json != null)
                    {
                        if (json["Parameters"] != null)
                        {
                            var parameters = Parse(json["Parameters"]);

                            Execute(json, parameters, handler);

                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                                new AsyncCallback(ReadCallback), state);
                        }
                    }
                }
            }
            else
            {
                Close(handler);
            }
        }
        catch { }
    }

    public static void Close(Socket handler)
    {
        try
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        catch { }
    }

    private static void Send(Socket handler, String data)
    {
        try
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }
        catch (Exception e)
        {
            //Logger.Log("error_log", $"- [SocketServer.cs] Send void exception: {e}");
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket handler = (Socket)ar.AsyncState;

            handler.EndSend(ar);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        catch (Exception e)
        {
            //Logger.Log("error_log", $"- [SocketServer.cs] SendCallback void exception: {e}");
        }
    }

    public static void AddAmmo(Player player, string itemId, int amount)
    {
        player.AmmunitionManager.AddAmmo(itemId, amount);
    }

    public static void initializePet(Player player)
    {
        var CurrentHitPoints = 5000;

        player.Pet = new Pet(player);
        player.SendCommand(PetInitializationCommand.write(true, true, true));
        player.Pet.Fuel = 5000;
        player.SendCommand(PetStatusCommand.write(player.Pet.Id, 15, 27000000, 27000000, CurrentHitPoints, player.Pet.MaxHitPoints, player.Pet.CurrentShieldPoints, player.Pet.MaxShieldPoints, player.Pet.Fuel, 50000, player.Speed, player.Pet.Name));
        player.SettingsManager.SendMenuBarsCommand();
    }

    public static bool startSpaceball()
    {

        bool statusEvent = EventManager.Spaceball.Status();

        if (statusEvent)
        {
            Console.WriteLine("Spaceball Stoped" + DateTime.Now + " ADMIN: WebSite");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Spaceball'");
            EventManager.Spaceball.Stop();

            return false;
        }
        else
        {
            Console.WriteLine("Spaceball Started" + DateTime.Now + " ADMIN: WebSite");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Spaceball'");
            EventManager.Spaceball.Start();

            return true;
        }

    }

    public static bool startHitac()
    {

        bool statusEvent = EventManager.Hitac.Status();

        if (statusEvent)
        {
            Console.WriteLine("Hitac 2.0 Stoped" + DateTime.Now + " ADMIN: WebSite");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Hitac'");
            EventManager.Hitac.Stop();

            return false;
        }
        else
        {
            Console.WriteLine("Hitac 2.0 Started" + DateTime.Now + " ADMIN: WebSite");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Hitac'");
            EventManager.Hitac.Start();

            return true;
        }

    }

    public static bool startRoyal()
    {
        bool statusEvent = EventManager.battleRoyal.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            Console.WriteLine("B.royal Started" + DateTime.Now + " ADMIN: WebSite");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'B.royal'");
            EventManager.battleRoyal.Start();

            return true;
        }

    }

    public static bool startCompany()
    {
        bool statusEvent = EventManager.BattleCompany.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            Console.WriteLine("Company Started" + DateTime.Now + " ADMIN: WebSite");
            EventManager.BattleCompany.Start();

            return true;
        }

    }

    public static bool startInvasion()
    {
        bool statusEvent = EventManager.Invasion.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            Console.WriteLine("Invasion " + DateTime.Now + " ADMIN: Website");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Invasion'");
            EventManager.Invasion.Start();

            return true;
        }

    }

    /*public static bool startTeam()
    {
        bool statusEvent = EventManager.TeamDeathmatch.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            Console.WriteLine("Team " + DateTime.Now + " ADMIN: Website");
            EventManager.TeamDeathmatch.Start();

            return true;
        }

    }*/

    public static bool startDemaner()
    {
        bool statusEvent = DemanerEvent.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Demaner'");
            DemanerEvent.Start();

            return true;
        }

    }

    public static bool startMeteorit()
    {
        bool statusEvent = IceMetorit.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Meteorit'");
            IceMetorit.Start();

            return true;
        }

    }

    public static bool startJackpot()
    {
        bool statusEvent = EventManager.JackpotBattle.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            Console.WriteLine("Jackpot " + DateTime.Now + " ADMIN: Website");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Jpb'");
            EventManager.JackpotBattle.Start();

            return true;
        }

    }

    public static bool startEmperator()
    {
        bool statusEvent = Emperator.Status();

        if (statusEvent)
        {
            return false;
        }
        else
        {
            Console.WriteLine("Emperator " + DateTime.Now + " ADMIN: Website");
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '1'  WHERE eventoname = 'Emperator'");
            Emperator.Start();

            return true;
        }

    }

    public static void setUridium(Player player, int Uridium)
    {
        if (player?.GameSession != null)
        {
            player.SetData(DataType.URIDIUM, Uridium);
        }
    }

    public static void setCredits(Player player, int Credits)
    {
        if (player?.GameSession != null)
        {
            player.SetData(DataType.CREDITS, Credits);
        }
    }

    public static void setHonor(Player player, int Honor)
    {
        if (player?.GameSession != null)
        {
            player.SetData(DataType.HONOR, Honor);
        }
    }

    public static void setExperience(Player player, int Experience)
    {
        if (player?.GameSession != null)
        {
            player.SetData(DataType.EXPERIENCE, Experience);
        }
    }

    public static void updatePet(Player player, string PetName, short PetDesignn)
    {
        if (player?.GameSession != null)
        {
            player.PetName = PetName;
            player.Pet.Activate2(PetName, PetDesignn);
        }
    }

    public static void updatePetFuel(Player player, int Amount)
    {
        if (player?.GameSession != null)
        {
            player.Pet.Fuel += Amount;
            player.Pet.checkFuel(player);
        }
    }

    public static int getPetFuel(Player player)
    {
        if (player?.GameSession != null)
        {
            return player.Pet.Fuel;
        }
        return 0;
    }

    public static void setPetModule(Player player, String TypeModule)
    {
        if (player?.GameSession != null)
        {
            if (TypeModule == "GUARD")
                player.Pet.GUARD = true;
            if (TypeModule == "KAMIKAZE")
                player.Pet.KAMIKAZE = true;
            if (TypeModule == "AUTO_LOOT")
                player.Pet.AUTO_LOOT = true;
            if (TypeModule == "COMBO_SHIP_REPAIR")
                player.Pet.COMBO_SHIP_REPAIR = true;
            if (TypeModule == "REPAIR_PET")
                player.Pet.REPAIR_PET = true;

            player.Pet.Deactivate();
            player.Pet.Activate();
        }
    }

    public static void updateEC(Player player, int amount, String Type)
    {
        if (player?.GameSession != null)
        {
            player.ChangeData(DataType.EC, amount, (Type == "INCREASE" ? (ChangeType.INCREASE) : (ChangeType.DECREASE)));
        }
    }

    public static void buyFormation(Player player, String Formation)
    {
        if (player?.GameSession != null)
        {
            /*if (Formation == DroneManager.DEFAULT_FORMATION)
            {
                player.DEFAULT_FORMATION = Formation;
            }
            else*/ if (Formation == DroneManager.ARROW_FORMATION)
            {
                player.ARROW_FORMATION = Formation;
            }
            else if (Formation == DroneManager.BARRAGE_FORMATION)
            {
                player.BARRAGE_FORMATION = Formation;
            }
            else if (Formation == DroneManager.BAT_FORMATION)
            {
                player.BAT_FORMATION = Formation;
            }
            else if (Formation == DroneManager.CHEVRON_FORMATION)
            {
                player.CHEVRON_FORMATION = Formation;
            }
            else if (Formation == DroneManager.CRAB_FORMATION)
            {
                player.CRAB_FORMATION = Formation;
            }
            else if (Formation == DroneManager.DIAMOND_FORMATION)
            {
                player.DIAMOND_FORMATION = Formation;
            }
            else if (Formation == DroneManager.DOME_FORMATION)
            {
                player.DOME_FORMATION = Formation;
            }
            else if (Formation == DroneManager.DOUBLE_ARROW_FORMATION)
            {
                player.DOUBLE_ARROW_FORMATION = Formation;
            }
            else if (Formation == DroneManager.DRILL_FORMATION)
            {
                player.DRILL_FORMATION = Formation;
            }
            else if (Formation == DroneManager.HEART_FORMATION)
            {
                player.HEART_FORMATION = Formation;
            }
            else if (Formation == DroneManager.LANCE_FORMATION)
            {
                player.LANCE_FORMATION = Formation;
            }
            else if (Formation == DroneManager.MOTH_FORMATION)
            {
                player.MOTH_FORMATION = Formation;
            }
            else if (Formation == DroneManager.PINCER_FORMATION)
            {
                player.PINCER_FORMATION = Formation;
            }
            else if (Formation == DroneManager.RING_FORMATION)
            {
                player.RING_FORMATION = Formation;
            }
            else if (Formation == DroneManager.STAR_FORMATION)
            {
                player.STAR_FORMATION = Formation;
            }
            else if (Formation == DroneManager.TURTLE_FORMATION)
            {
                player.TURTLE_FORMATION = Formation;
            }
            else if (Formation == DroneManager.VETERAN_FORMATION)
            {
                player.VETERAN_FORMATION = Formation;
            }
            else if (Formation == DroneManager.WAVE_FORMATION)
            {
                player.WAVE_FORMATION = Formation;
            }
            else if (Formation == DroneManager.WHEEL_FORMATION)
            {
                player.WHEEL_FORMATION = Formation;
            }
            else if (Formation == DroneManager.X_FORMATION)
            {
                player.X_FORMATION = Formation;
            }

            player.SettingsManager.SendSlotBarCommand();
        }
    }

    public static void buyKey(Player player, String Key, int Amount)
    {
        if (player?.GameSession != null)
        {
            if (Key == "greenKeys")
            {
                player.bootyKeys.greenKeys += Amount;
                player.SendPacket($"0|A|BK|{player.bootyKeys.greenKeys}");
            }
            else if (Key == "redKeys")
            {
                player.bootyKeys.redKeys += Amount;
                player.SendPacket($"0|A|BKR|{player.bootyKeys.redKeys}");
            }
            else if (Key == "blueKeys")
            {
                player.bootyKeys.blueKeys += Amount;
                player.SendPacket($"0|A|BKB|{player.bootyKeys.blueKeys}");
            }
            else
            {
                Console.WriteLine("No key.");
            }
        }
    }

    public static void setX2Event(string arg = "false")
    {
        if (arg == "true")
        {
            Attackable.x2EventActive = true;
            GameManager.SendPacketToAll($"0|A|STD|x2 Reward Event Active!");
        }
        if (arg == "false")
        {
            Attackable.x2EventActive = false;
            GameManager.SendPacketToAll($"0|A|STD|x2 Reward Event disabled!");
        }
    }
    public static void setCrazyEggs(string arg = "false")
    {
        if (arg == "true")
        {
            Spacemap.BoxenEvent = true;
            GameManager.SendPacketToAll($"0|A|STD|Crazy Eggs Event Active!");
        }
        if (arg == "false")
        {
            Spacemap.BoxenEvent = false;
            GameManager.SendPacketToAll($"0|A|STD|Crazy Eggs Event disabled!");
        }
    }

    public static void KickPlayer(Player player, string reason)
    {
        if (player?.GameSession != null)
        {
            player.SendPacket($"0|A|STD|{reason}");
            player.GameSession.Disconnect(DisconnectionType.NORMAL);
        }
    }

    public static void UpgradeSkillTree(Player player, string skill)
    {
        if (player?.GameSession != null)
        {
            if (skill == "engineering")
                player.SkillTree.engineering++;
            else if (skill == "shieldEngineering")
                player.SkillTree.shieldEngineering++;
            else if (skill == "detonation1")
                player.SkillTree.detonation1++;
            else if (skill == "detonation2")
                player.SkillTree.detonation2++;
            else if (skill == "heatseekingMissiles")
                player.SkillTree.heatseekingMissiles++;
            else if (skill == "rocketFusion")
                player.SkillTree.rocketFusion++;
            else if (skill == "cruelty1")
                player.SkillTree.cruelty1++;
            else if (skill == "cruelty2")
                player.SkillTree.cruelty2++;
            else if (skill == "explosives")
                player.SkillTree.explosives++;
            else if (skill == "luck1")
                player.SkillTree.luck1++;
            else if (skill == "luck2")
                player.SkillTree.luck2++;
            else if (skill == "ishcd")
                player.SkillTree.ishcd++;
            else if (skill == "empcd")
                player.SkillTree.empcd++;
            else if (skill == "backupcd")
                player.SkillTree.backupcd++;
            else if (skill == "battlecd")
                player.SkillTree.battlecd++;
            else if (skill == "shieldMechanics")
                player.SkillTree.shieldMechanics++;
            else if (skill == "electroOptics")
                player.SkillTree.electroOptics++;
        }
    }

    public static void ResetSkillTree(Player player)
    {
        if (player?.GameSession != null)
        {
            player.SkillTree.engineering = 0;
            player.SkillTree.shieldEngineering = 0;
            player.SkillTree.detonation1 = 0;
            player.SkillTree.detonation2 = 0;
            player.SkillTree.heatseekingMissiles = 0;
            player.SkillTree.rocketFusion = 0;
            player.SkillTree.cruelty1 = 0;
            player.SkillTree.cruelty2 = 0;
            player.SkillTree.ishcd = 0;
            player.SkillTree.empcd = 0;
            player.SkillTree.explosives = 0;
            player.SkillTree.luck1 = 0;
            player.SkillTree.luck2 = 0;
            player.SkillTree.shieldMechanics = 0;
            player.SkillTree.electroOptics = 0;
        }
    }

    public static void BanUser(Player player)
    {
        if (player == null) return;

        var client = GameManager.ChatClients[player.Id];
        client.Send($"{ChatConstants.CMD_BANN_USER}%#");
        client.Close();

        player.GameSession.Disconnect(DisconnectionType.NORMAL);
        GameManager.SendChatSystemMessage($"{player.Name} has been banned.");
    }

    public static void BuyItem(Player player, string itemType, DataType dataType, int amount)
    {
        if (player?.GameSession != null)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var result = mySqlClient.ExecuteQueryRow($"SELECT data FROM player_accounts WHERE userId = {player.Id}");
                player.Data = JsonConvert.DeserializeObject<DataBase>(result["data"].ToString());
            }

            player.SendPacket($"0|LM|ST|{(dataType == DataType.URIDIUM ? "URI" : "CRE")}|-{amount}|{(dataType == DataType.URIDIUM ? player.Data.uridium : player.Data.credits)}");

            switch (itemType)
            {
                case "drones":
                    player.DroneManager.UpdateDrones(player.droneExp, true);
                    break;
                case "booster":
                    var oldBoosters = player.BoosterManager.Boosters;

                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        var result = mySqlClient.ExecuteQueryRow($"SELECT boosters FROM player_equipment WHERE userId = {player.Id}");
                        var newBoosters = JsonConvert.DeserializeObject<Dictionary<short, List<BoosterBase>>>(result["boosters"].ToString());
                        player.BoosterManager.Boosters = newBoosters.Concat(oldBoosters).GroupBy(b => b.Key).ToDictionary(b => b.Key, b => b.First().Value);
                    }

                    player.BoosterManager.Update();
                    break;
            }
        }
    }

    public static void ChangeClanData(Clan clan, string name, string tag, int factionId)
    {
        if (clan.Id != 0)
        {
            clan.Tag = tag;
            clan.Name = name;
            //clan.FactionId = factionId;

            foreach (GameSession gameSession in GameManager.GameSessions.Values.Where(x => x.Player.Clan.Id == clan.Id))
            {
                var player = gameSession.Player;
                if (player != null)
                    GameManager.SendCommandToMap(player.Spacemap.Id, ClanChangedCommand.write(clan.Tag, clan.Id, player.Id));
            }
        }
    }

    public static void JoinToClan(Player player, Clan clan)
    {
        if (player?.GameSession != null && clan != null)
        {
            player.Clan = clan;

            var command = ClanChangedCommand.write(clan.Tag, clan.Id, player.Id);
            player.SendCommand(command);
            player.SendCommandToInRangePlayers(command);
        }
    }

    public static void EndDiplomacy(Clan senderClan, Clan targetClan)
    {
        if (senderClan != null && targetClan != null)
        {
            senderClan.DiplomaciesSender.Remove(targetClan.Id);
            targetClan.DiplomaciesSender.Remove(senderClan.Id);

            senderClan.DiplomaciesTo.Remove(targetClan.Id);
            targetClan.DiplomaciesTo.Remove(senderClan.Id);

        }
    }

    public static void StartDiplomacy(Clan senderClan, Clan targetClan, short diplomacyType)
    {
        if (senderClan != null && targetClan != null)
        {
            senderClan.DiplomaciesSender.Add(targetClan.Id, (Diplomacy)diplomacyType);
            targetClan.DiplomaciesSender.Add(senderClan.Id, (Diplomacy)diplomacyType);

            senderClan.DiplomaciesTo.Add(targetClan.Id, (Diplomacy)diplomacyType);
            targetClan.DiplomaciesTo.Add(senderClan.Id, (Diplomacy)diplomacyType);

        }
    }

    public static void LeaveFromClan(Player player)
    {
        foreach (var battleStation in GameManager.BattleStations.Values)
        {
            if (battleStation.EquippedStationModule.ContainsKey(player.Clan.Id))
                battleStation.EquippedStationModule[player.Clan.Id].ForEach(x => { if (x.OwnerId == player.Id) { x.Destroy(null, DestructionType.MISC); } });
        }

        if (player?.GameSession != null)
        {
            if (player.Clan.Id != 0)
            {
                player.Clan = GameManager.GetClan(0);

                var command = ClanChangedCommand.write(player.Clan.Tag, player.Clan.Id, player.Id);
                player.SendCommand(command);
                player.SendCommandToInRangePlayers(command);
            }
        }
    }

    public static void DeleteClan(Clan deletedClan)
    {
        if (deletedClan != null)
        {
            foreach (var battleStation in GameManager.BattleStations.Values.Where(x => x.Clan.Id == deletedClan.Id))
                battleStation.Destroy(null, DestructionType.MISC);

            GameManager.Clans.TryRemove(deletedClan.Id, out deletedClan);

            foreach (var gameSession in GameManager.GameSessions.Values)
            {
                var member = gameSession?.Player;

                if (member != null && member.Clan.Id == deletedClan.Id)
                {
                    member.Clan = GameManager.GetClan(0);

                    var command = ClanChangedCommand.write(member.Clan.Tag, member.Clan.Id, member.Id);
                    member.SendCommand(command);
                    member.SendCommandToInRangePlayers(command);
                }
            }

            foreach (var clan in GameManager.Clans.Values)
            {
                clan.DiplomaciesSender.Remove(deletedClan.Id);
                clan.DiplomaciesTo.Remove(deletedClan.Id);
            }

        }
    }

    public static void CreateClan(Player player, int clanId, int factionId, string name, string tag)
    {
        if (!GameManager.Clans.ContainsKey(clanId))
        {
            var clan = new Clan(clanId, name, tag, factionId);
            GameManager.Clans.TryAdd(clan.Id, clan);

            if (player?.GameSession != null)
            {
                player.Clan = clan;

                var command = ClanChangedCommand.write(clan.Tag, clan.Id, player.Id);
                player.SendCommand(command);
                player.SendCommandToInRangePlayers(command);
            }
        }
    }

    public static void ChangeCompany(Player player, int uridiumPrice, int honorPrice, int ExperiencePrice)
    {
        if (player?.GameSession != null)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var result = mySqlClient.ExecuteQueryRow($"SELECT data, factionId FROM player_accounts WHERE userId = {player.Id}");
                player.Data = JsonConvert.DeserializeObject<DataBase>(result["data"].ToString());
                player.FactionId = Convert.ToInt32(result["factionId"]);
            }

            player.SendPacket($"0|LM|ST|URI|-{uridiumPrice}|{player.Data.uridium}");

            if (honorPrice > 0)
                player.SendPacket($"0|LM|ST|HON|-{honorPrice}|{player.Data.honor}");

            if (ExperiencePrice > 0)
                player.SendPacket($"0|LM|ST|EP|-{ExperiencePrice}|{player.Data.experience}");

            player.Jump(player.GetBaseMapId(), player.GetBasePosition());
        }
    }

    public static void UpdateLogfiles(Player player, int logfiles)
    {
        if(player?.GameSession != null)
        {
            player.SendPacket($"0|LM|ST|LOG|{logfiles}");
        }
    }
    public static void UpdateItems(Player player, int amount, string lootId)
    {
        if(player?.GameSession != null)
        {
            player.SendPacket($"0|LM|ST|LOT|{lootId}|{amount}");
        }
    }
    public static void UpdateUridium(Player player, int uridiumPrice, String Type)
    {
        if (player?.GameSession != null)
        {
            player.ChangeData(DataType.URIDIUM, uridiumPrice, (Type == "INCREASE" ? (ChangeType.INCREASE) : (ChangeType.DECREASE)));
        }
    }

    public static void UpdateHonor(Player player, int honor, String Type)
    {
        if (player?.GameSession != null)
        {
            player.ChangeData(DataType.HONOR, honor, (Type == "INCREASE" ? (ChangeType.INCREASE) : (ChangeType.DECREASE)));
        }
    }

    public static void UpdateExperience(Player player, int experience, String Type)
    {
        if (player?.GameSession != null)
        {
            player.ChangeData(DataType.EXPERIENCE, experience, (Type == "INCREASE" ? (ChangeType.INCREASE) : (ChangeType.DECREASE)));
        }
    }

    public static void sendBoosterToUser(Player playerBooster, int typeBooster, int hoursBooster)
    {

        if (!new int[] { 0, 1, 2, 3, 8, 9, 10, 11, 12, 5, 6, 15, 16, 7, 4 }.Contains(typeBooster)) return;

        if (playerBooster != null)
        {
            playerBooster.BoosterManager.Add((BoosterType)typeBooster, hoursBooster);
        }

    }

    public static void UpdateCredits(Player player, int creditPrice, String Type)
    {
        if (player?.GameSession != null)
        {
            player.ChangeData(DataType.CREDITS, creditPrice, (Type == "INCREASE" ? (ChangeType.INCREASE) : (ChangeType.DECREASE)));
        }
    }

    public static void UpdateTitle(Player player, string title)
    {
        if (player?.GameSession != null)
        {
            player.SetTitle(title, true);
        }
    }

    public static void ChangeShip(Player player, Ship ship)
    {
        if (player?.GameSession != null && ship != null)
        {
            player.ChangeShip(ship.Id);
            player.Storage.lastChangeShipTime = DateTime.Now;
        }
    }

    public static void UpdateStatus(Player player)
    {
        if (player?.GameSession != null)
        {
            QueryManager.SetEquipment(player);

            player.DroneManager.UpdateDrones(player.droneExp, true);
            player.UpdateStatus();
        }
        if (player.Storage.waitTDM == true)
        {
            player.Storage.TdmEnd = true;
        }
    }

    public static int Int(object value)
    {
        try
        {
            return Convert.ToInt32(value.ToString());

        }
        catch (Exception e)
        {
            return 0;
        }
    }

    public static short Short(object value)
    {
        try
        {
            return Convert.ToInt16(value.ToString());

        }
        catch (Exception e)
        {
            return 0;
        }

    }

    public static string String(object value)
    {
        try
        {
            return value.ToString();

        }
        catch (Exception e)
        {
            return "";
        }

    }

    public static JObject Parse(object value)
    {
        try
        {
            return JObject.Parse(value.ToString());

        }
        catch (Exception e)
        {
            return null;
        }

    }
}
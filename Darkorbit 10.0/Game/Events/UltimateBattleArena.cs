using Darkorbit.Game.Movements;
using Darkorbit.Game.Objects;
using Darkorbit.Game.Objects.Players.Stations;
using Darkorbit.Game.Ticks;
using Darkorbit.Managers;
using Darkorbit.Net.netty.commands;
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Darkorbit.Game.Events
{
    
    class UltimateBattleArenaLobby
    {
        public List<Player> players = new List<Player>();
        public int mapId = 0;
        public Task actualizePlayers = null;
        public Task lobbyAcceptWait = null;
        public int lobbyAcceptTime = 0;
        public Task lobbyWaitForPlayer = null;
        public int lobbyWaitForPlayerId = 0;
        public bool waiting = false;
        public bool initiateBattle = false;
        public int playersSpawnedOnMap = 0;
        public int currentRound = 1;
        public int p1RoundsWon = 0;
        public int p2RoundsWon = 0;
        public Task tick = null;
        public Task inactivityTick = null;
        public bool matchIsRunning = false;
        public int roundTime = 0;
        public int currentMode = 0;
        public int p1MapBeforeStart = 0;
        public Position p1MapPositionBeforeStart = null;
        public int p2MapBeforeStart = 0;
        public Position p2MapPositionBeforeStart = null;

        public UltimateBattleArenaLobby(List<Player> p, int m)
        {
            this.players = p;
            this.mapId = m;
        }
    }

    internal class UltimateBattleArena : Tick
    {
        public bool UBAdisabled = true;
        public List<Player> playerList = new List<Player>();
        private readonly Spacemap Spacemap = GameManager.GetSpacemap(121);

        public Position Position1 = new Position(4400, 3600);
        public Position Position2 = new Position(5600, 2400);

        private List<UltimateBattleArenaLobby> ubal = new List<UltimateBattleArenaLobby>();

        public UltimateBattleArena()
        {
            Program.TickManager.AddTick(this);
        }
        public void Tick()
        {
            if (!EventManager.JackpotBattle.Active)
            {
                if (playerList.Count >= 2)
                {
                    foreach (Player players in playerList)
                    {
                        if (players.GameSession == null || players.AttackingOrUnderAttack())
                        {
                            players.SendPacket($"0|A|STD|Cannot register, Event not active or You match for TDM.");
                            players.SendCommand(UbaWindowInitializationCommand.write(new UbaD26Module(), 0));
                            RemoveWaitingPlayer(players);
                            return;
                        }

                        if (!playerList.First().Equals(players))
                        {
                            if (playerList.First().Storage.Uba == null && players.Storage.Uba == null && !InEvent(playerList.First()) && !InEvent(players))
                            {
                                List<Player> tmp1 = new List<Player>();
                                tmp1.Add(playerList.First());
                                tmp1.Add(players);
                                var spacemap = new Spacemap(121, "UBA", -1, new List<NpcsBase>(), new List<PortalBase>(), new List<StationBase>(), new OptionsBase(), true);
                                int ubaId = int.Parse("88" + new Random().Next(1000000,9999999));
                                while(true)
                                {
                                    Spacemap s = GameManager.GetSpacemap(ubaId);
                                    if (s != null) ubaId = int.Parse("88" + new Random().Next(1000000, 9999999));
                                    else break;
                                }

                                Console.WriteLine("uba: " + ubaId);
                                GameManager.Spacemaps.TryAdd(ubaId, spacemap);
                                UltimateBattleArenaLobby tmp = new UltimateBattleArenaLobby(tmp1, ubaId);
                                ubal.Add(tmp);

                                tmp.tick = Task.Run(() => UbaLobbyTick(tmp));

                                playerList.First().Storage.ubal = tmp;
                                players.Storage.ubal = tmp;

                                Player p1 = playerList.First();
                                Player p2 = players;

                                p1.SendCommand(UbaWindowInitializationCommand.write(new Uba047Module(40000, new UbaM1tModule(false)), 3));
                                p2.SendCommand(UbaWindowInitializationCommand.write(new Uba047Module(40000, new UbaM1tModule(false)), 3));

                                EventManager.UltimateBattleArena.RemoveWaitingPlayer(playerList.First());
                                EventManager.UltimateBattleArena.RemoveWaitingPlayer(players);

                                tmp.lobbyAcceptWait = Task.Run(() => LobbyAcceptWait(p1, p2));

                                break;
                            }
                        }

                    }
                }
            }
        }

        public List<Ubaf3kModule> GetRewards(int[] minMax, UltimateBattleArenaLobby l, int index)
        {
            int rsb75 = Randoms.random.Next(minMax[0], minMax[1]);
            int ucb100 = Randoms.random.Next(minMax[2], minMax[3]);
            int emp01 = Randoms.random.Next(minMax[4], minMax[5]);
            int ish01 = Randoms.random.Next(minMax[6], minMax[7]);
            int smb01 = Randoms.random.Next(minMax[8], minMax[9]);

            List<Ubaf3kModule> tmp = new List<Ubaf3kModule>();
            tmp.Add(new Ubaf3kModule("RSB-75", rsb75));
            tmp.Add(new Ubaf3kModule("UCB-100", ucb100));
            tmp.Add(new Ubaf3kModule("EMP-01", emp01));
            tmp.Add(new Ubaf3kModule("ISH-01", ish01));
            tmp.Add(new Ubaf3kModule("SMB-01", smb01));

            if (rsb75 > 0) l.players[index].AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.RSB_75, rsb75);
            if (ucb100 > 0) l.players[index].AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.UCB_100, ucb100);
            if (emp01 > 0) l.players[index].AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.EMP_01, emp01);
            if (ish01 > 0) l.players[index].AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.ISH_01, ish01);
            if (smb01 > 0) l.players[index].AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.SMB_01, smb01);

            return tmp;
        }

        public async void ShowResult(UltimateBattleArenaLobby l, string state)
        {
            if (state.Contains("p1won"))
            {
                if (l.players[0].UbaPoints <= 1499)
                {
                    l.players[0].UbaPoints += 10;
                }
                if (l.players[0].UbaPoints >= 1500)
                {
                    l.players[0].UbaPoints += 5;
                }
                l.players[1].Ubabattel += 1;
                l.players[0].Ubabattel += 1;
                if (l.players[1].UbaPoints >= 50)
                {
                    l.players[1].UbaPoints -= 5;
                }
                if (l.players[1].UbaPoints >= 500)
                {
                    l.players[1].UbaPoints -= 5;
                }

                l.players[0].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 800, 1800, 1600, 3500, 2, 6, 2, 6, 2, 6 }, l, 0), (short)0, l.p1RoundsWon, l.p2RoundsWon, l.players[1].Name, 10, -10), 6));
                l.players[1].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 200, 500, 500, 1000, 0, 1, 0, 1, 0, 1 }, l, 1), (short)1, l.p2RoundsWon, l.p1RoundsWon, l.players[0].Name, -10, 10), 6));
            }
            else if (state.Contains("p2won"))
            {
                if (l.players[1].UbaPoints <= 1499)
                {
                    l.players[1].UbaPoints += 10;
                }
                if (l.players[1].UbaPoints >= 1500)
                {
                    l.players[1].UbaPoints += 5;
                }
                l.players[0].Ubabattel += 1;
                l.players[1].Ubabattel += 1;
                if (l.players[0].UbaPoints >= 50)
                {
                    l.players[0].UbaPoints -= 5;
                }
                if (l.players[0].UbaPoints >= 500)
                {
                    l.players[0].UbaPoints -= 5;
                }

                l.players[1].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 800, 1800, 1600, 3500, 2, 6, 2, 6, 2, 6 }, l, 1), (short)0, l.p2RoundsWon, l.p1RoundsWon, l.players[0].Name, 10, -10), 6));
                l.players[0].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 200, 500, 500, 1000, 0, 1, 0, 1, 0, 1 }, l, 0), (short)1, l.p1RoundsWon, l.p2RoundsWon, l.players[1].Name, -10, 10), 6));
            }
            else if (state.Contains("draw"))
            {
                l.players[0].Ubabattel += 1;
                l.players[1].Ubabattel += 1;
                l.players[0].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 200, 500, 500, 1000, 0, 1, 0, 1, 0, 1 }, l, 0), (short)2, l.p1RoundsWon, l.p2RoundsWon, l.players[1].Name, 0, 0), 6));
                l.players[1].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 200, 500, 500, 1000, 0, 1, 0, 1, 0, 1 }, l, 1), (short)2, l.p1RoundsWon, l.p2RoundsWon, l.players[0].Name, 0, 0), 6));
            }
            else if (state.Contains("doubleko"))
            {
                l.players[0].Ubabattel += 1;
                l.players[1].Ubabattel += 1;
                l.players[0].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 200, 500, 500, 1000, 0, 1, 0, 1, 0, 1 }, l, 0), (short)2, l.p1RoundsWon, l.p2RoundsWon, l.players[1].Name, 0, 0), 6));
                l.players[1].SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 200, 500, 500, 1000, 0, 1, 0, 1, 0, 1 }, l, 1), (short)2, l.p1RoundsWon, l.p2RoundsWon, l.players[0].Name, 0, 0), 6));
            }

            await Task.Delay(5000);

            foreach (Player p in l.players)
            {
                p.SetPosition(p.GetBasePosition());
                if(p.FactionId == 2)
                p.Jump(p.GetBaseMapId(true), p.UBAEIC);
                if (p.FactionId == 3)
                    p.Jump(p.GetBaseMapId(true), p.UBAVRU);
                if (p.FactionId == 1)
                    p.Jump(p.GetBaseMapId(true), p.UBAMMO);
                p.RemoveVisualModifier(VisualModifierCommand.CAMERA);
                DeleteUbaLobby(p);
            }

           /* l.players[0].Jump(l.p1MapBeforeStart, l.p1MapPositionBeforeStart);
            l.players[1].Jump(l.p2MapBeforeStart, l.p2MapPositionBeforeStart);*/
        }

        public async void UbaLobbyTick(UltimateBattleArenaLobby l)
        {
            try
            {
                {
                    while (l.players[0].Storage.ubal != null && l.players[1].Storage.ubal != null)
            {
                if (GameManager.GetSpacemap(l.mapId).Characters.Count == 1 && l.players[0].Storage.ubal.playersSpawnedOnMap == 2)
                {
                    if (GameManager.GetSpacemap(l.mapId).Characters.First().Value != null)
                    {
                        Player p = GameManager.GetSpacemap(l.mapId).Characters.First().Value as Player;
                        ShowWin(p);
                        foreach (Player p1 in p.Storage.ubal.players)
                        {
                            if (p1.Id != p.Id)
                            {
                                ShowLose(p1);
                            }
                        }
                        l.players[0].Storage.ubal.playersSpawnedOnMap = 0;
                        l.players[0].Storage.ubal.matchIsRunning = false;
                        InitNextRound(p.Storage.ubal.players[0], p.Storage.ubal.players[1]);
                    }
                } else if(GameManager.GetSpacemap(l.mapId).Characters.Count == 0 && l.players[0].Storage.ubal.playersSpawnedOnMap == 2)
                {
                    foreach (Player p1 in l.players[0].Storage.ubal.players)
                    {
                        ShowDoubleKO(p1);
                    }
                    l.players[0].Storage.ubal.playersSpawnedOnMap = 0;
                    l.players[0].Storage.ubal.matchIsRunning = false;
                    InitNextRound(l.players[0].Storage.ubal.players[0], l.players[0].Storage.ubal.players[1]);
                }
                await Task.Delay(1000);
            }
        }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [BLMaps.cs] Main void exception: {ex}");
            }
        }


        public async void ShowWin(Player p)
        {
            await Task.Delay(3000);
            UbaCountdown init1 = new UbaCountdown(0);
            UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
            p.SendCommand(init2.write());
            int index = p.Storage.ubal.players.IndexOf(p);
            if (index == 0) p.Storage.ubal.p1RoundsWon++;
            else if (index == 1) p.Storage.ubal.p2RoundsWon++;
        }

        public async void ShowLose(Player p)
        {
            await Task.Delay(3000);
            UbaCountdown init1 = new UbaCountdown(1);
            UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
            p.SendCommand(init2.write());
        }

        public async void ShowDraw(Player p)
        {
            await Task.Delay(3000);
            UbaCountdown init1 = new UbaCountdown(5);
            UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
            p.SendCommand(init2.write());
        }

        public async void ShowDoubleKO(Player p)
        {
            await Task.Delay(3000);
            UbaCountdown init1 = new UbaCountdown(4);
            UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
            p.SendCommand(init2.write());
        }

        public async void InitNextRound(Player player1, Player player2)
        {
            await Task.Delay(4000);
            player1.Storage.ubal.currentRound++;
            if (player1.Storage.ubal.currentRound >= 3)
            {
                string state = "";
                if (player1.Storage.ubal.p1RoundsWon == player1.Storage.ubal.p2RoundsWon && player1.Storage.ubal.currentRound > 3) state = "draw";
                else if (player1.Storage.ubal.p1RoundsWon >= 2) state = "p1wondouble";
                else if (player1.Storage.ubal.p2RoundsWon >= 2) state = "p2wondouble";
                else if (player1.Storage.ubal.p1RoundsWon > player1.Storage.ubal.p2RoundsWon) state = "p1won";
                else if (player1.Storage.ubal.p2RoundsWon > player1.Storage.ubal.p1RoundsWon) state = "p2won";
                else state = "";

                if (state == "")
                {
                    Uba(player1, player2, player1.Storage.ubal.mapId, player1.Storage.ubal);
                }
                else
                {
                    ShowResult(player1.Storage.ubal, state);
                }
            }
            else
            {
                Uba(player1, player2, player1.Storage.ubal.mapId, player1.Storage.ubal);
            }
        }

        public void AddWaitingPlayer(Player player)
        {
            if (UBAdisabled || player.Storage.waitTDM == true || player.AttackingOrUnderAttack())
            {
                player.SendPacket($"0|A|STD|Cannot register, Event not active or You match for TDM.");
                player.SendCommand(UbaWindowInitializationCommand.write(new UbaD26Module(), 0));
            }
            else if (player.Level <= 13)
            {
                player.SendPacket($"0|A|STD|Cannot register, you need Level 14.");
                player.SendCommand(UbaWindowInitializationCommand.write(new UbaD26Module(), 0));
            }
            else
            {
                if (!playerList.Contains((player)))
                {
                    if (!EventManager.JackpotBattle.Active || player.Storage.IsInDemilitarizedZone || player.AttackingOrUnderAttack())
                    {
                        player.SendCommand(UbaWindowInitializationCommand.write(new UbaD26Module(), 2));
                        playerList.Add(player);
                        player.Storage.waitUBA = true;
                        player.Storage.LogoutUba = false;
                        player.SendPacket($"Added in position[{playerList.Count}]");
                    }
                }
                else
                {
                    player.SendPacket($"0|A|STD|You are already registered or you do not have the necessary rank");
                }
            }
        }

        public void RemoveWaitingPlayer(Player player)
        {
            if (playerList.Contains((player)))
            {
                player.Storage.Uba = null;
                playerList.Remove(player);
                player.Storage.waitUBA = false;
                player.Storage.LogoutUba = true;
            }
            player.SendCommand(UbaWindowInitializationCommand.write(new UbaD26Module(), 0));
        }

        public void RemoveWaitingPlayer2(Player player)
        {
            if (playerList.Contains((player)))
            {
                player.Storage.Uba = null;
                playerList.Remove(player);
                player.Storage.waitUBA = false;
                player.Storage.LogoutUba = true;
            }
        }


        public bool InEvent(Player player)
        {
            if (player.GetPlayerActiveMap().ToString().StartsWith("88")) return true;
            else return false;
        }

        public async void Uba(Player player1, Player player2, int ubaId, UltimateBattleArenaLobby tmp)
        {
            tmp.currentMode = new Random().Next(0,100);
            if(tmp.currentMode >= 0 && tmp.currentMode < 60) tmp.currentMode = 0;
            else if(tmp.currentMode >= 60 && tmp.currentMode <= 100) tmp.currentMode = 1;

            tmp.roundTime = 315;

            Spacemap ubaMap = GameManager.GetSpacemap(ubaId);

            if(tmp.currentMode == 1)
            {

                //cloak mode enabled
                ubaMap.Options.CloakBlocked = true;

                player1.CpuManager.DisableCloak();
                player2.CpuManager.DisableCloak();

                await Task.Delay(20);

                player1.SendPacket($"0|n|KSMSG|Cloak Deactivated");
                player2.SendPacket($"0|n|KSMSG|Cloak Deactivated");

                //var poi1 = new POI("uba_poi1", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5000, 3000), new Position(4000, 4000), new Position(4000, 2000), new Position(6000, 2000) }, true, true);

                //ubaMap.POIs.TryAdd("uba_poi1", poi1);
            } else
            {
                //cloak mode disabled
                ubaMap.Options.CloakBlocked = false;

                POI tmp1 = null;
                ubaMap.POIs.TryRemove("uba_poi1", out tmp1);
                player1.SendCommand(MapRemovePOICommand.write("uba_poi1"));
                player2.SendCommand(MapRemovePOICommand.write("uba_poi1"));

                await Task.Delay(20);

                player1.SendPacket($"0|n|KSMSG|Cloak Activated");
                player2.SendPacket($"0|n|KSMSG|Cloak Activated");

            }

            var poi2 = new POI("uba_poi2", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(4400, 3600), new Position(200, 100) }, true, true);
            var poi3 = new POI("uba_poi3", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(5600, 2400), new Position(200, 100) }, true, true);

            ubaMap.POIs.TryAdd("uba_poi2", poi2);
            ubaMap.POIs.TryAdd("uba_poi3", poi3);

           /* if (player1.Storage.ubal.p1MapBeforeStart == 0)
            {
                player1.Storage.ubal.p1MapBeforeStart = player1.Spacemap.Id;
                player1.Storage.ubal.p1MapPositionBeforeStart = player1.Position;
            }
            if (player2.Storage.ubal.p2MapBeforeStart == 0)
            {
                player2.Storage.ubal.p2MapBeforeStart = player2.Spacemap.Id;
                player2.Storage.ubal.p2MapPositionBeforeStart = player2.Position;
            }*/

            player1.Storage.Uba = this;
            player2.Storage.Uba = this;
            if (player1.GameSession != null)
            {
                player1.Jump(ubaId, Position1);
                player1.CurrentHitPoints = player1.MaxHitPoints;
                player1.CurrentShieldConfig1 = player1.GetMaxShieldPoints(1);
                player1.CurrentShieldConfig2 = player1.GetMaxShieldPoints(2);
                player1.ResetCooldown();
            }
            if (player2.GameSession != null)
            {
                player2.Jump(ubaId, Position2);
                player2.CurrentHitPoints = player2.MaxHitPoints;
                player2.CurrentShieldConfig1 = player2.GetMaxShieldPoints(1);
                player2.CurrentShieldConfig2 = player2.GetMaxShieldPoints(2);
                player2.ResetCooldown();
            }

            player1.activeMapId = ubaId;
            player2.activeMapId = ubaId;

            tmp.actualizePlayers = Task.Run(() => ActualizePlayers(player1, player2));

            await Task.Delay(10000);

            if (!player1.Invisible)
            {
                player1.RemoveVisualModifier(VisualModifierCommand.CAMERA);
                player2.RemoveVisualModifier(VisualModifierCommand.CAMERA);
                player1.AddVisualModifier(VisualModifierCommand.CAMERA, 0, "", 0, true);
                player2.AddVisualModifier(VisualModifierCommand.CAMERA, 0, "", 0, true);
            } else
            {
                player1.RemoveVisualModifier(VisualModifierCommand.CAMERA);
                player2.RemoveVisualModifier(VisualModifierCommand.CAMERA);
            }

            startFigth(player1, player2);
        }

        public async void InactivityTick(UltimateBattleArenaLobby l)
            {
            while (l.matchIsRunning)
            {
                l.roundTime--;
                if (l.roundTime < 0)
                {
                    l.matchIsRunning = false;
                    //abort match because of inactivity or draw
                    foreach (Player p in l.players)
                    {
                        ShowDraw(p);
                    }
                    l.players[0].Storage.ubal.playersSpawnedOnMap = 0;
                    l.players[0].Storage.ubal.matchIsRunning = false;
                    InitNextRound(l.players[0].Storage.ubal.players[0], l.players[0].Storage.ubal.players[1]);
                }
                await Task.Delay(1000);
            }
        }

        public int countdownTime = 20;
        public async void startFigth(Player player1, Player player2)
            {
            for (int i = 0; i < 100; i++)
            {
                if (player1.GetPlayerActiveMap().ToString().StartsWith("88") && player2.GetPlayerActiveMap().ToString().StartsWith("88")) break;
                await Task.Delay(100);
            }

            player1.CurrentHitPoints = player1.MaxHitPoints;
            player2.CurrentHitPoints = player2.MaxHitPoints;

            if (player1.Storage.ubal.currentRound == 3)
            {
                UbaCountdown elm1_1 = new UbaCountdown(3);
                UbaCountdown1 elm2_1 = new UbaCountdown1(elm1_1, 3000);
                player1.SendCommand(elm2_1.write());
                player2.SendCommand(elm2_1.write());
            }
            else
            {
                UbaCountdown2 elm1_1 = new UbaCountdown2(player1.Storage.ubal.currentRound, 0);
                UbaCountdown1 elm2_1 = new UbaCountdown1(elm1_1, 3000);
                player1.SendCommand(elm2_1.write());
                player2.SendCommand(elm2_1.write());
            }
            await Task.Delay(3000);

            for (int i = 15; i >= 0; i--)
            {
                UbaCountdown2 elm1 = new UbaCountdown2(i, 1);
                UbaCountdown1 elm2 = new UbaCountdown1(elm1, 1000);
                if (i > 0)
                {
                    player1.SendCommand(elm2.write());
                    player2.SendCommand(elm2.write());
                }
                await Task.Delay(1000);
                if (i < 1)
                {
                    player1.SendCommand(MapRemovePOICommand.write("uba_poi2"));
                    player1.SendCommand(MapRemovePOICommand.write("uba_poi3"));
                    player2.SendCommand(MapRemovePOICommand.write("uba_poi2"));
                    player2.SendCommand(MapRemovePOICommand.write("uba_poi3"));
                    POI tmp1 = null;
                    if(player1.Storage.ubal.mapId != null)
                    {
                        GameManager.GetSpacemap(player1.Storage.ubal.mapId).POIs.TryRemove("uba_poi2", out tmp1);
                        GameManager.GetSpacemap(player1.Storage.ubal.mapId).POIs.TryRemove("uba_poi3", out tmp1);
                    }
                    UbaCountdown init1 = new UbaCountdown(2);
                    UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
                    player1.SendCommand(init2.write());
                    player2.SendCommand(init2.write());
                }
            }

            EventManager.UltimateBattleArena.RemoveWaitingPlayer2(player1);
            EventManager.UltimateBattleArena.RemoveWaitingPlayer2(player2);
        }

        public void ActualizePlayers(Player player1, Player player2)
        {
            ActualizePlayersSub(player1, player2);
        }

        public void ActualizePlayersSub(Player player1, Player player2)
        {
            if (player1.Storage.ubal.playersSpawnedOnMap >= 2)
            {
                player1.SendCommand(UbaWindowInitializationCommand.write(new command_e4W(1, 1, "", player2.Name, new class_oS(player2.CurrentHitPoints, player2.MaxHitPoints, player2.CurrentShieldPoints, player2.MaxShieldPoints, player1.Storage.ubal.p2RoundsWon, player1.CurrentHitPoints, player1.MaxHitPoints, player1.CurrentShieldPoints, player1.MaxShieldPoints, player1.Storage.ubal.p1RoundsWon), new class_s16(player1.Storage.ubal.currentRound, (short)0)), 5));
                player2.SendCommand(UbaWindowInitializationCommand.write(new command_e4W(1, 1, "", player1.Name, new class_oS(player1.CurrentHitPoints, player1.MaxHitPoints, player1.CurrentShieldPoints, player1.MaxShieldPoints, player2.Storage.ubal.p1RoundsWon, player2.CurrentHitPoints, player2.MaxHitPoints, player2.CurrentShieldPoints, player2.MaxShieldPoints, player2.Storage.ubal.p2RoundsWon), new class_s16(player2.Storage.ubal.currentRound, (short)0)), 5));
                player1.SendCommand(new Ubah6Module(player1.Storage.ubal.roundTime * 1000).write());
                player2.SendCommand(new Ubah6Module(player2.Storage.ubal.roundTime * 1000).write());
                ActualizePlayersSubSub(player1, player2);
                player1.Storage.ubal.matchIsRunning = true;
                player1.Storage.ubal.inactivityTick = Task.Run(() => InactivityTick(player1.Storage.ubal));
            }
        }

        public async void ActualizePlayersSubSub(Player player1, Player player2)
        {
            try
            {
                {
            while (player1.Storage.ubal != null && player2.Storage.ubal != null)
            {
                player1.SendCommand(new class_oS(player2.CurrentHitPoints, player2.MaxHitPoints, player2.CurrentShieldPoints, player2.MaxShieldPoints, player1.Storage.ubal.p2RoundsWon, player1.CurrentHitPoints, player1.MaxHitPoints, player1.CurrentShieldPoints, player1.MaxShieldPoints, player1.Storage.ubal.p1RoundsWon).write1());
                player2.SendCommand(new class_oS(player1.CurrentHitPoints, player1.MaxHitPoints, player1.CurrentShieldPoints, player1.MaxShieldPoints, player2.Storage.ubal.p1RoundsWon, player2.CurrentHitPoints, player2.MaxHitPoints, player2.CurrentShieldPoints, player2.MaxShieldPoints, player2.Storage.ubal.p2RoundsWon).write1());
                await Task.Delay(1000);
        }
            }
                 }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [UBA.cs] Main void exception: {ex}");
            }
        }

        public async void LobbyAcceptWait(Player player1, Player player2)
        {
            if (player1.Storage.ubal != null)
            {
                player1.Storage.ubal.lobbyAcceptTime = 40;
                while (player1.Storage.ubal.lobbyAcceptTime > 0)
                {
                    if (player1.Storage.ubal == null) break;

                    if (player1.Storage.ubal == null || player2.Storage.ubal == null || player1.Storage.ubal.initiateBattle || player2.Storage.ubal.initiateBattle || player1.AttackingOrUnderAttack() || player2.AttackingOrUnderAttack())
                    {
                        player2.SendPacket("0|A|STD|UBA Matchmakeing Disabled while you or your opponent are Under Attack");
                        player1.SendPacket("0|A|STD|UBA Matchmakeing Disabled while you or your opponent are Under Attack");
                        break;
                    }
                    if (player1.Storage.ubal.lobbyWaitForPlayerId != player1.Id)
                    {
                        player1.SendCommand(UbaWindowInitializationCommand.write(new Uba047Module(player1.Storage.ubal.lobbyAcceptTime * 1000, new UbaM1tModule(false)), 3));
                    }

                    if (player2.Storage.ubal.lobbyWaitForPlayerId != player2.Id || player2.AttackingOrUnderAttack())
                    {
                        player2.SendCommand(UbaWindowInitializationCommand.write(new Uba047Module(player1.Storage.ubal.lobbyAcceptTime * 1000, new UbaM1tModule(false)), 3));
                    }

                    await Task.Delay(1000);

                    if(player1.Storage.ubal != null)
                    {
                        player1.Storage.ubal.lobbyAcceptTime--;
                    } else
                    {
                        break;
                    }
                }

                if (player1.Storage.ubal != null && player2.Storage.ubal != null && !player1.Storage.ubal.initiateBattle && !player2.Storage.ubal.initiateBattle)
                {
                    EventManager.UltimateBattleArena.RemoveWaitingPlayer(player1);
                    EventManager.UltimateBattleArena.RemoveWaitingPlayer(player2);
                    EventManager.UltimateBattleArena.DeleteUbaLobby(player1);
                }
            }
        }

        public void DeleteUbaLobby(Player player)
        {
            UltimateBattleArenaLobby tmp = player.Storage.ubal;
            if (tmp != null)
            {
                foreach (Player p in tmp.players)
                {
                    p.Storage.ubal = null;
                    p.activeMapId = 0;
                    p.Storage.Uba = null;
                    p.Storage.waitUBA = false;
                }
                ubal.Remove(tmp);
                Spacemap tmp1 = null;
                GameManager.Spacemaps.TryRemove(tmp.mapId, out tmp1);
            }
        }
    }
}

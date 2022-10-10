using System.Data;
using Darkorbit;
using Darkorbit.Net;

namespace Darkorbit.Net.netty.handlers
{
    class LoginRequestHandler
    {
        public Player Player { get; set; }
        public GameSession GameSession { get; set; }

        public LoginRequestHandler(GameClient client, int userId)
        {
            try
            {
                client.UserId = userId;

                var gameSession = GameManager.GetGameSession(userId);
                if (gameSession != null)
                {
                    Player = gameSession.Player;
                    gameSession.Disconnect(GameSession.DisconnectionType.NORMAL);
                }
                else Player = QueryManager.GetPlayer(userId);

                if (Player != null)
                {
                    GameSession = new GameSession(Player)
                    {
                        Client = client,
                        LastActiveTime = DateTime.Now
                    };
                }
                else
                {
                    Out.WriteLine("Failed loading user ship / ShipInitializationHandler ERROR");
                    return;
                }

                Execute();

                if (Player.Destroyed) Player.KillScreen(null, DestructionType.MISC, true);
                else
                {
                    SendSettings(Player);
                    SendPlayer(Player);
                    Player.Spacemap.AddCharacter(Player);
                }
            }
            catch (Exception e)
            {
                Out.WriteLine("UID: " + Player.Id + " Exception: " + e, "LoginRequestHandler.cs");
                Logger.Log("error_log", $"- [LoginRequestHandler.cs] LoginRequestHandler class exception: {e}");
            }
        }

        public void Execute()
        {
            try
            {
                if (GameSession == null) return;

                if (!GameManager.GameSessions.ContainsKey(Player.Id))
                    GameManager.GameSessions.TryAdd(Player.Id, GameSession);
                else
                {
                    GameManager.GameSessions[Player.Id].Disconnect(GameSession.DisconnectionType.NORMAL);
                    GameManager.GameSessions[Player.Id] = GameSession;
                }

                if (Player.Spacemap == null || Player.Position == null || GameManager.GetSpacemap(Player.Spacemap.Id) == null)
                {
                    Player.CurrentHitPoints = Player.MaxHitPoints;
                    Player.CurrentShieldConfig1 = Player.MaxShieldPoints;
                    Player.CurrentShieldConfig2 = Player.MaxShieldPoints;

                    if (Player.RankId == 22)
                        Player.Premium = true;

                    if (Player.UbaPoints > 50)
                        Player.SetTitle("title_308", true);
                    if (Player.UbaPoints > 100)
                        Player.SetTitle("title_307", true);
                    if (Player.UbaPoints > 250)
                        Player.SetTitle("title_26", true);
                    if (Player.UbaPoints > 500)
                        Player.SetTitle("title_51", true);
                    if (Player.UbaPoints > 1000)
                        Player.SetTitle("title_202", true);
                    if (Player.UbaPoints > 1500)
                        Player.SetTitle("title_201", true);
                    if (Player.UbaPoints > 2500)
                        Player.SetTitle("title_200", true);
                    if (Player.UbaPoints < 50)
                        Player.SetTitle("title_24", true);
                                        // if (Player.Premium && Player.Title=="")
                    //   Player.SetTitle("title_2", false);

                    if (Player.positionInitializacion.mapID != 0 )
                    {
                        if(Player.positionInitializacion.mapID >= 51 && Player.positionInitializacion.mapID <= 1111)
                        {
                            Player.Spacemap = GameManager.GetSpacemap(Player.GetBaseMapId(true));
                            Player.SetPosition(Player.GetBasePosition(true));
                        } else if (Player.positionInitializacion.mapID > 29 && Player.positionInitializacion.mapID < 306)
                        {
                            Player.Spacemap = GameManager.GetSpacemap(Player.GetBaseMapId());
                            Player.SetPosition(Player.GetBasePosition());

                        }
                        else if (Player.positionInitializacion.isInMapPremium)
                        {
                            var sessionMapId = Int32.Parse("620" + Player.positionInitializacion.mapID);
                            Player.isInMapPremium = true;
                            Player.Spacemap = GameManager.GetSpacemap(sessionMapId);
                            Player.SetPosition(new Position(Player.positionInitializacion.x, Player.positionInitializacion.y));
                        }
                        else
                        {
                            //Player.Spacemap = GameManager.GetSpacemap(Player.FactionId == 1 ? 1 : Player.FactionId == 2 ? 5 : 9);
                            //if (Player.FactionId == 1)
                            //{
                            //    Player.SetPosition(Position.MMOPosition);
                            //}
                            //if (Player.FactionId == 2)
                            //{
                            //    Player.SetPosition(Position.EICPosition);
                            //}
                            //if (Player.FactionId == 3)
                            //{
                            //    Player.SetPosition(Position.VRUPosition);
                            //}

                            Player.Spacemap = GameManager.GetSpacemap(Player.positionInitializacion.mapID);
                            Player.SetPosition(new Position(Player.positionInitializacion.x, Player.positionInitializacion.y));
                        }
                    }
                    else {

                        Player.Spacemap = GameManager.GetSpacemap(Player.GetBaseMapId());
                        Player.SetPosition(Player.GetBasePosition());
                    }
                }

                Program.TickManager.AddTickPlayer(Player);

                QueryManager.SavePlayer.Information(Player);
                Console.Title = $"Emulator | {GameManager.GameSessions.Count} users online";
               
            }
            catch (Exception e)
            {
                Out.WriteLine("UID: "+Player.Id+" Execute void exception: " + e, "LoginRequestHandler.cs");
                Logger.Log("error_log", $"- [LoginRequestHandler.cs] Execute void exception: {e}");
            }
        }

        public static void SendPlayer(Player player, bool isLogin = true)
        {
            try
            {
                player.SendCommand(player.GetShipInitializationCommand());

                if (player.Title != "")
                    player.SendPacket($"0|n|t|{player.Id}|1|{player.Title}");

                player.SendPacket(player.DroneManager.GetDronesPacket(player.droneExp));
               // player.SendCommand(DroneFormationChangeCommand.write(player.Id, DroneManager.GetSelectedFormationId(player.Settings.InGameSettings.selectedFormation)));
                player.SendPacket("0|S|CFG|" + player.CurrentConfig);

                player.SendPacket($"0|A|BK|{player.bootyKeys.greenKeys}");
                player.SendPacket($"0|A|BKR|{player.bootyKeys.redKeys}");
                player.SendPacket($"0|A|BKB|{player.bootyKeys.blueKeys}");
                player.SendPacket("0|A|JV|0"); //jump coupon amount

                player.SendPacket($"0|n|m|0|0|0"); // CTB

                /*
                player.SendPacket("0|n|isi");
                player.SendPacket($"0 | A|CPU|C|0");
                player.SendPacket($"0|A|ITM|0|0|0|0|3|0|0|1|1|0|2|0|1|0|0|0|0");
                player.SendPacket($"0|TX|S|1|15|0|1|15|0|1|15|0|1|15|0|1|15|0");
                player.SendPacket($"0|ps|blk|0");
                player.SendPacket($"0|3|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100");
                player.SendPacket($"0|POI|RDY");
                */

                if (player.Group != null)
                    player.Group.InitializeGroup(player);
                
                var spaceball = EventManager.Spaceball.Character;
                if (EventManager.Spaceball.Active && spaceball != null)
                    player.SendPacket($"0|n|ssi|{spaceball.Mmo}|{spaceball.Eic}|{spaceball.Vru}");
                else
                    player.SendPacket($"0|n|ssi|0|0|0");
                if (EventManager.Invasion.Started)
                    player.SendPacket($"0|n|isi|{EventManager.Invasion.mmoKills}|{EventManager.Invasion.mmoKills}|{EventManager.Invasion.mmoKills}|{EventManager.Invasion.waveMMO}");
                else
                    player.SendPacket($"0|n|isi|0|0|0|0");
                /*
                var priceList = new List<JumpCPUPriceMappingModule>();
                var price = new List<int>();
                price.Add(1);
                price.Add(14);
                price.Add(15);
                price.Add(16);
                priceList.Add(new JumpCPUPriceMappingModule(0, price));
                player.SendCommand(JumpCPUUpdateCommand.write(priceList));

                player.SendCommand(CpuInitializationCommand.write(true, false));
                */
                //player.SendPacket("0|A|JCPU|S|20|1"); //JUMP BACK COUNT

                player.SendCommand(SetSpeedCommand.write(player.Speed, player.Speed));
                player.Spacemap.SendObjects(player);
                if (player.RankId == 22)
                {
                    //player.SendPacket($"0|n|fx|start|RAGE|{player.Id}");
                    //player.SendPacket($"0|n|ISH|" + player.Id);
                }

                //player.BoosterManager.Add(BoosterType.DMG_PVE_B01, 3 * 6000);

                player.BoosterManager.Update();
                
                if (player.Pet != null)
                {
                    player.SendCommand(PetInitializationCommand.write(true, true, !player.Settings.InGameSettings.petDestroyed));

                    if (player.Settings.InGameSettings.petDestroyed)
                        player.SendCommand(PetUIRepairButtonCommand.write(true, 15000));
                }


                //player.SendCommand(QuestInitializationCommand.write(true));
           
                player.UpdateStatus();
                player.SendCommand(player.SettingsManager.GetNewItemStatus("equipment_extra_cpu_cl04k-xl", "ttip_cloak_cpu", player.AmmunitionManager.GetAmmo("equipment_extra_cpu_cl04k-xl"), true, true, false));
               // if (EventManager.UltimateBattleArena.UBAdisabled)
               // player.SendCommand(UbaWindowInitializationCommand.write(new Ubas3wModule(new UbaG3FModule(player.UbaPoints / 10, player.Ubabattel, player.WarRank, player.UbaPoints), new Uba64iModule("", 0, new List<UbaHtModule>()), new UbahsModule(new List<Ubal4bModule>())), 0));


                //player.SendCommand(VideoWindowCreateCommand.write(1, "l", false, new List<string> { }, 7, 1));    
                //player.SendCommand(VideoWindowCreateCommand.write(1, "l", true, new List<string> { "start_head", "tutorial_video_msg_reward_premium_intro" }, 7, 1));    

                //player.SendPacket("0|n|KSMSG|start_head");


                //player.SendCommand(command_e4W.write(1, "", new class_oS(0, 0, 0, 0, 0, 0, 0, 0, 0, 0), "", new class_s16(1, class_s16.varC3p), 1));

                //player.SendCommand(command_z3Q.write(command_z3Q.varC2f));
                /*
                var contacts = new List<class_i1d>();

                var DDDDD = new List<class_84I>();


                var DDDDsD = new List<class_533>();
                DDDDsD.Add(new class_533(class_533.varN2g));

                contacts.Add(new class_i1d(new class_O4f(2, DDDDD), new class_y3i(2, DDDDsD)));
                contacts.Add(new class_i1d(new class_O4f(1, DDDDD), new class_y3i(1, DDDDsD)));

                player.SendCommand(ContactsListUpdateCommand.write(new class_o3q(contacts), new class_g1a(true, true, true, true), new class_H4Q(false)));
                */

                var shieldEngineering = player.SkillTree.shieldEngineering;
                var shieldEngineeringVal = false;
                if (shieldEngineering >= 8) shieldEngineeringVal = true;
                player.SetShieldSkillActivated(shieldEngineeringVal);

                if (isLogin)
                {
                //UBA SEASON

                    var ht = new List<UbaHtModule>();
                    var j3s = new List<command_j3s>();
                    j3s.Add(new Ubaf3kModule("currency_uridium", 250000));

                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {

                        var query = $"SELECT * FROM uba_rewards ORDER BY id ASC";
                        var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                        int position = 0;

                        if (result.Rows.Count >= 1)
                        {
                            foreach (DataRow row in result.Rows)
                            {
                                position++;
                                ht.Add(new UbaHtModule(position+"º   "+row["reward"], j3s));
                            }
                        }

                    }

                    var l4b = new List<Ubal4bModule>();

                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {

                        var query = $"SELECT * FROM player_accounts WHERE rankId != 22 ORDER BY warPoints DESC LIMIT 10";
                        var result = (DataTable)mySqlClient.ExecuteQueryTable(query);

                        if (result.Rows.Count >= 1)
                        {
                            foreach (DataRow row in result.Rows)
                            {
                                l4b.Add(new Ubal4bModule((string)row["pilotName"], (int)row["warPoints"]));
                            }
                        }

                    }

                    player.SendCommand(UbaWindowInitializationCommand.write(new Ubas3wModule(new UbaG3FModule(player.UbaPoints/10, player.Ubabattel, player.WarRank, player.UbaPoints), new Uba64iModule("Union season", 1, ht), new UbahsModule(l4b)), 0));

                    if (player.GetPlayerActiveMap().ToString().StartsWith("88") && player.Storage.ubal != null)
                    {
                        Player p1 = null;
                        Player p2 = null;
                        foreach(Player p in player.Storage.ubal.players)
                        {
                            if(p.Id != player.Id)
                            {
                                p2 = p;
                            } else
                            {
                                p1 = p;
                            }
                        }
                        p1.SendCommand(UbaWindowInitializationCommand.write(new command_e4W(1, 1, "", p2.Name, new class_oS(p2.CurrentHitPoints, p2.MaxHitPoints, p2.CurrentShieldPoints, p2.MaxShieldPoints, p1.Storage.ubal.p2RoundsWon, p1.CurrentHitPoints, p1.MaxHitPoints, p1.CurrentShieldPoints, p1.MaxShieldPoints, p1.Storage.ubal.p1RoundsWon), new class_s16(p1.Storage.ubal.currentRound, (short)0)), 5));
                        Task tmp = Task.Run(() => ActualizeUbaTime(p1));
                    }
                }
                

                player.SendCommand(player.GetBeaconCommand());

                player.GetLeonovEffect();
            }
            catch (Exception e)
            {
                Out.WriteLine("UID: " + player.Id + " SendPlayerItems void exception: " + e, "LoginRequestHandler.cs");
                Logger.Log("error_log", $"- [LoginRequestHandler.cs] SendPlayerItems void exception: {e}");
            }
        }

        public static async void ActualizeUbaTime(Player p1)
        {
            await Task.Delay(1000);
            p1.SendCommand(new Ubah6Module(p1.Storage.ubal.roundTime * 1000).write());
        }

        public static void SendSettings(Player player)
        {
            try
            {
                player.SetCurrentCooldowns();
                player.SettingsManager.SendUserKeyBindingsUpdateCommand();
                player.SettingsManager.SendUserSettingsCommand();
                player.SettingsManager.SendMenuBarsCommand();
                player.SettingsManager.SendSlotBarCommand();
                player.SettingsManager.SendHelpWindows();
            }
            catch (Exception e)
            {
                Out.WriteLine("UID: " + player.Id + " SendSettings void exception: " + e, "LoginRequestHandler.cs");
                Logger.Log("error_log", $"- [LoginRequestHandler.cs] SendSettings void exception: {e}");
            }
        }
    }
}

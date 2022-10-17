using Darkorbit.Game.GalaxyGates;
using Darkorbit.Game.Objects.Players.Stations;
using Darkorbit_10._0.Game.Events.Data;
using System.Data;

namespace Darkorbit.Game.Objects
{
    class PortalBase
    {
        public int TargetSpaceMapId { get; set; }
        public List<int> Position { get; set; }
        public List<int> TargetPosition { get; set; }
        public int GraphicId { get; set; }
        public int FactionId { get; set; }
        public bool Visible { get; set; }
        public bool Working { get; set; }
        public bool PremiumMap { get; set; }
    }

    class Portal : Activatable
    {
        public static int JUMP_DELAY = 3250;

        public Position TargetPosition { get; set; }
        public int TargetSpaceMapId { get; set; }
        public int GraphicsId { get; set; }
        public bool Visible { get; set; }
        public bool Working { get; set; }
        public bool PremiumMap { get; set; }

        public bool premiumMapActived = false;
        public bool blmap1Actived = true;
        public bool blmap2Actived = true;
        public bool blmap3Actived = true;

        public Portal(Spacemap spacemap, Position position, Position targetPosition, int targetSpacemapId, int graphicsId, int factionId, bool visible, bool working, bool premiummap) : base(spacemap, factionId, position, GameManager.GetClan(0))
        {
            TargetPosition = targetPosition;
            TargetSpaceMapId = targetSpacemapId;
            GraphicsId = graphicsId;
            FactionId = factionId;
            Visible = visible;
            Working = working;
            PremiumMap = premiummap;
        }

        public override async void Click(GameSession gameSession)
        {
            try
            {
                var player = gameSession.Player;
                
                if ((!Working || GameManager.GetSpacemap(TargetSpaceMapId) == null || TargetPosition == null) && TargetSpaceMapId != -1) return;
                if (player.Storage.Jumping || player.Destroyed) return;
                if (FactionId == 1)
                {
                    if(PortData.MMOPorts.Find(x => x.Item1 == TargetSpaceMapId).Item2 > player.Level)
                    {
                        player.SendPacket($"0|A|STM|You need level {PortData.MMOPorts.Find(x => x.Item1 == TargetSpaceMapId).Item2} to access this map");
                        return;
                    }
                }
                

                //if (TargetSpaceMapId == 306 && player.Level < 20 && (player.Spacemap.Id == 306 || player.Spacemap.Id == 307 || player.Spacemap.Id == 308)) { player.SendPacket($"0|A|STM|You need level 20 to access in BL-1"); return; }
                //if (TargetSpaceMapId == 307 && player.Level < 20 && (player.Spacemap.Id == 306 || player.Spacemap.Id == 307 || player.Spacemap.Id == 308)) { player.SendPacket($"0|A|STM|You need level 20 to access in BL-2"); return; }
                //if (TargetSpaceMapId == 308 && player.Level < 20 && (player.Spacemap.Id == 306 || player.Spacemap.Id == 307 || player.Spacemap.Id == 308)) { player.SendPacket($"0|A|STM|You need level 20 to access in BL-3"); return; }

                //if (TargetSpaceMapId == 306 && player.Level < 15 && (player.Spacemap.Id == 20 || player.Spacemap.Id == 24 || player.Spacemap.Id == 28)) { player.SendPacket($"0|A|STM|You need level 15 to access in BL-1"); return; }
                //if (TargetSpaceMapId == 307 && player.Level < 15 && (player.Spacemap.Id == 20 || player.Spacemap.Id == 24 || player.Spacemap.Id == 28)) { player.SendPacket($"0|A|STM|You need level 15 to access in BL-2"); return; }
                //if (TargetSpaceMapId == 308 && player.Level < 15 && (player.Spacemap.Id == 20 || player.Spacemap.Id == 24 || player.Spacemap.Id == 28)) { player.SendPacket($"0|A|STM|You need level 15 to access in BL-3"); return; }

                int AlphaGateId = 51;
                int BetaGateId = 52;
                int GammaGateId = 53;
               // int DeltaGateId = 55;
                int KappaGateId = 74;
                int KronosGateId = 76;
                int LambdaGateId = 75;

                //if (TargetSpaceMapId == 61 && player.FactionId!=1) { player.SendPacket($"0|A|STM|Only MMO Company"); return; }
               // if (TargetSpaceMapId == 61 && player.FactionId != 2) { player.SendPacket($"0|A|STM|Only EIC Company"); return; }
               // if (TargetSpaceMapId == 61 && player.FactionId != 3) { player.SendPacket($"0|A|STM|Only VRU Company"); return; }

                if (TargetSpaceMapId == 18 && PremiumMap == true && player.Premium == false) { player.SendPacket($"0|A|STM|Only Premium Users Access"); return; }
                if (TargetSpaceMapId == 22 && PremiumMap == true && player.Premium == false) { player.SendPacket($"0|A|STM|Only Premium Users Access"); return; }
                if (TargetSpaceMapId == 26 && PremiumMap == true && player.Premium == false) { player.SendPacket($"0|A|STM|Only Premium Users Access"); return; }


                //EVENTS
                if (TargetSpaceMapId==EventManager.JackpotBattle.Spacemap.Id) {
                    EventManager.JackpotBattle.Players.TryAdd(player.Id, player);
                    TargetPosition = Position.Random(Spacemap, 2000, 20000, 1500, 10000);
                   
                    player.CpuManager.DisableCloak();
                    player.Storage.noAttack = true;
                }
                if (TargetSpaceMapId == EventManager.battleRoyal.Spacemap.Id)
                {
                    EventManager.battleRoyal.Players.TryAdd(player.Id, player);
                    TargetPosition = Position.Random(Spacemap, 2000, 20000, 1500, 10000);
                 
                    player.CpuManager.DisableCloak();
                    player.Storage.noAttack = true;

                    EventManager.BattleCompany.Players.TryAdd(player.Id, player);
                    player.Storage.oldShip = player.Ship.Id;
                  
                    player.Storage.oldDamage = player.Storage.DamageBoost;
                    //player.ChangeShip(1);
                    player.Storage.DamageBoost = 100;
                   
                }

                if (TargetSpaceMapId == EventManager.BattleCompany.Spacemap.Id )
                {
                    EventManager.BattleCompany.Players.TryAdd(player.Id, player);

                    var mmoPosition = new Position(2000, 3500);
                    var eicPosition = new Position(15000, 3500);
                    var vruPosition = new Position(9500, 9500);
                    player.Storage.noAttack = true;
                    if (player.FactionId == 1)
                    {
                        EventManager.BattleCompany.playersMMO.Add(player);
                        TargetPosition = mmoPosition;
                    }

                    else if (player.FactionId == 2) { 
                        EventManager.BattleCompany.playersEIC.Add(player);
                    
                        TargetPosition = eicPosition;
                    }

                    else {
                        EventManager.BattleCompany.playersVRU.Add(player);
                    
                        TargetPosition = vruPosition;

                    }

                }

                if (TargetSpaceMapId == 200)
                {

                   Events.DemanerEvent.data.Players.TryAdd(player.Id,player);
                    player.Storage.noAttack = true;
                 

                }


                /*if (TargetSpaceMapId==EventManager.TeamDeathmatch.Spacemap.Id) {
                  
                        EventManager.TeamDeathmatch.AddWaitingPlayer(player.Group,player);
                    return;
                }*/

                //END EVENTS

                /*if (TargetSpaceMapId == AlphaGateId || TargetSpaceMapId == BetaGateId || TargetSpaceMapId == GammaGateId)
                {
                    player.SendPacket("0|A|STD|galaxygates are under maintenance");
                } else */if (TargetSpaceMapId == AlphaGateId)
                {
                    player.Storage.Jumping = true;

                    // Get gate from database
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        try
                        {
                            var query = $"SELECT * FROM player_galaxygates WHERE userId = '{player.GetPlayerId()}' AND gateId = '1'";
                            var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                            if (result.Rows.Count >= 1)
                            {
                                var prepared = Int32.Parse(mySqlClient.ExecuteQueryRow(query)["prepared"].ToString());
                                var wave = mySqlClient.ExecuteQueryRow(query)["wave"].ToString();
                                var lives = mySqlClient.ExecuteQueryRow(query)["lives"].ToString();

                                if (prepared == 1)
                                {
                                    var gateMapId = "99" + new Random().Next(1000000, 9999999);
                                    while (true)
                                    {
                                        Spacemap s = GameManager.GetSpacemap(Int32.Parse(gateMapId));
                                        if (s != null) gateMapId = "99" + new Random().Next(1000000, 9999999);
                                        else break;
                                    }
                                    player.activeMapId = Int32.Parse(gateMapId);

                                    Console.WriteLine(gateMapId);
                                    int mapId = Convert.ToInt32(51);
                                    string name = Convert.ToString("GG Alpha");
                                    int factionId = Convert.ToInt32(player.FactionId);
                                    var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                                    var portals = JsonConvert.DeserializeObject<List<PortalBase>>("".ToString());
                                    var stations = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                                    var options = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false,'PvpDisabled':false}".ToString());
                                    var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options, true);
                                    GameManager.Spacemaps.TryAdd(Int32.Parse(gateMapId), spacemap);

                                    player.SendCommand(ActivatePortalCommand.write(Int32.Parse(gateMapId), Id));
                                    await Task.Delay(JUMP_DELAY);

                                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                                    player.Spacemap.RemoveCharacter(player);
                                    player.CurrentInRangePortalId = -1;
                                    player.Deselection();
                                    player.Storage.InRangeAssets.Clear();
                                    player.InRangeCharacters.Clear();
                                    player.SetPosition(new Position(10500, 6500)); // Center of map

                                    player.Spacemap = GameManager.GetSpacemap(Int32.Parse(gateMapId));

                                    player.Spacemap.AddAndInitPlayer(player);
                                    player.Storage.Jumping = false;

                                    // Set active gate to Alpga gate
                                    player.playerActiveGate = "Alpha";
                                    // Create new Alpha gate
                                    player.AlphaGate = new AlphaGate(Int32.Parse(gateMapId), gameSession, Int32.Parse(wave), Int32.Parse(lives));

                                    foreach (Activatable activatable in player.Spacemap.Activatables.Values)
                                    {
                                        if (activatable is Portal portal) portal.Remove();
                                    }
                                }
                                else
                                {
                                    player.Storage.Jumping = false;
                                    player.SendPacket($"0|A|STM|You don't have Alpha gate active");
                                }
                            } else
                            {
                                player.Storage.Jumping = false;
                                player.SendPacket($"0|A|STM|You don't have Alpha gate active");
                            }
                        }
                        catch
                        {
                            player.Storage.Jumping = false;
                            player.SendPacket($"0|A|STM|You don't have Alpha gate active");
                        }
                    }

                }
                else if (TargetSpaceMapId == BetaGateId)
                {
                    player.Storage.Jumping = true;

                    // Get gate from database
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        try
                        {
                            var query = $"SELECT * FROM player_galaxygates WHERE userId = '{player.GetPlayerId()}' AND gateId = '2'";
                            var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                            if (result.Rows.Count >= 1)
                            {
                                var prepared = Int32.Parse(mySqlClient.ExecuteQueryRow(query)["prepared"].ToString());
                                var wave = mySqlClient.ExecuteQueryRow(query)["wave"].ToString();
                                var lives = mySqlClient.ExecuteQueryRow(query)["lives"].ToString();

                                if (prepared == 1)
                                {
                                    var gateMapId = "99" + new Random().Next(1000000, 9999999);
                                    while (true)
                                    {
                                        Spacemap s = GameManager.GetSpacemap(Int32.Parse(gateMapId));
                                        if (s != null) gateMapId = "99" + new Random().Next(1000000, 9999999);
                                        else break;
                                    }
                                    player.activeMapId = Int32.Parse(gateMapId);


                                    int mapId = Convert.ToInt32(52);
                                    string name = Convert.ToString("GG Beta");
                                    int factionId = Convert.ToInt32(player.FactionId);
                                    var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                                    var portals = JsonConvert.DeserializeObject<List<PortalBase>>("".ToString());
                                    var stations = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                                    var options = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                                    var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options, true);
                                    GameManager.Spacemaps.TryAdd(Int32.Parse(gateMapId), spacemap);

                                    player.SendCommand(ActivatePortalCommand.write(Int32.Parse(gateMapId), Id));
                                    await Task.Delay(JUMP_DELAY);

                                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                                    player.Spacemap.RemoveCharacter(player);
                                    player.CurrentInRangePortalId = -1;
                                    player.Deselection();
                                    player.Storage.InRangeAssets.Clear();
                                    player.InRangeCharacters.Clear();
                                    player.SetPosition(new Position(10500, 6500)); // Center of map

                                    player.Spacemap = GameManager.GetSpacemap(Int32.Parse(gateMapId));

                                    player.Spacemap.AddAndInitPlayer(player);
                                    player.Storage.Jumping = false;

                                    // Set active gate to Alpga gate
                                    player.playerActiveGate = "Beta";
                                    // Create new Alpha gate
                                    player.BetaGates = new BetaGates(Int32.Parse(gateMapId), gameSession, Int32.Parse(wave), Int32.Parse(lives));

                                    foreach (Activatable activatable in player.Spacemap.Activatables.Values)
                                    {
                                        if (activatable is Portal portal) portal.Remove();
                                    }
                                }
                                else
                                {
                                    player.Storage.Jumping = false;
                                    player.SendPacket($"0|A|STM|You don't have Beta gate active");
                                }
                            }
                            else
                            {
                                player.Storage.Jumping = false;
                                player.SendPacket($"0|A|STM|You don't have Beta gate active");
                            }
                        }
                        catch
                        {
                            player.Storage.Jumping = false;
                            player.SendPacket($"0|A|STM|You don't have Beta gate active");
                        }
                    }

                }
                else if (TargetSpaceMapId == GammaGateId)
                {
                    player.Storage.Jumping = true;

                    // Get gate from database
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        try
                        {
                            var query = $"SELECT * FROM player_galaxygates WHERE userId = '{player.GetPlayerId()}' AND gateId = '3'";
                            var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                            if (result.Rows.Count >= 1)
                            {
                                var prepared = Int32.Parse(mySqlClient.ExecuteQueryRow(query)["prepared"].ToString());
                                var wave = mySqlClient.ExecuteQueryRow(query)["wave"].ToString();
                                var lives = mySqlClient.ExecuteQueryRow(query)["lives"].ToString();

                                if (prepared == 1)
                                {
                                    var gateMapId = "99" + new Random().Next(1000000, 9999999);
                                    while (true)
                                    {
                                        Spacemap s = GameManager.GetSpacemap(Int32.Parse(gateMapId));
                                        if (s != null) gateMapId = "99" + new Random().Next(1000000, 9999999);
                                        else break;
                                    }
                                    player.activeMapId = Int32.Parse(gateMapId);


                                    int mapId = Convert.ToInt32(53);
                                    string name = Convert.ToString("GG Gamma");
                                    int factionId = Convert.ToInt32(player.FactionId);
                                    var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                                    var portals = JsonConvert.DeserializeObject<List<PortalBase>>("".ToString());
                                    var stations = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                                    var options = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                                    var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options, true);
                                    GameManager.Spacemaps.TryAdd(Int32.Parse(gateMapId), spacemap);

                                    player.SendCommand(ActivatePortalCommand.write(Int32.Parse(gateMapId), Id));
                                    await Task.Delay(JUMP_DELAY);

                                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                                    player.Spacemap.RemoveCharacter(player);
                                    player.CurrentInRangePortalId = -1;
                                    player.Deselection();
                                    player.Storage.InRangeAssets.Clear();
                                    player.InRangeCharacters.Clear();
                                    player.SetPosition(new Position(10500, 6500)); // Center of map

                                    player.Spacemap = GameManager.GetSpacemap(Int32.Parse(gateMapId));

                                    player.Spacemap.AddAndInitPlayer(player);
                                    player.Storage.Jumping = false;

                                    // Set active gate to Alpga gate
                                    player.playerActiveGate = "Gamma";
                                    // Create new Alpha gate
                                    player.GammaGates = new GammaGates(Int32.Parse(gateMapId), gameSession, Int32.Parse(wave), Int32.Parse(lives));

                                    foreach (Activatable activatable in player.Spacemap.Activatables.Values)
                                    {
                                        if (activatable is Portal portal) portal.Remove();
                                    }
                                }
                                else
                                {
                                    player.Storage.Jumping = false;
                                    player.SendPacket($"0|A|STM|You don't have Gamma gate active");
                                }
                            }
                            else
                            {
                                player.Storage.Jumping = false;
                                player.SendPacket($"0|A|STM|You don't have Gamma gate active");
                            }
                        }
                        catch
                        {
                            player.Storage.Jumping = false;
                            player.SendPacket($"0|A|STM|You don't have Gamma gate active");
                        }
                    }

                }
                /*else if (TargetSpaceMapId == DeltaGateId)
                {
                    player.Storage.Jumping = true;

                    /*   // Get gate from database
                       using (var mySqlClient = SqlDatabaseManager.GetClient())
                       {
                           try
                           {
                               var query = $"SELECT * FROM player_galaxygates WHERE userId = '{player.GetPlayerId()}' AND gateId = '4'";
                               var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                               if (result.Rows.Count >= 1)
                               {
                                   var prepared = Int32.Parse(mySqlClient.ExecuteQueryRow(query)["prepared"].ToString());
                                   var wave = mySqlClient.ExecuteQueryRow(query)["wave"].ToString();
                                   var lives = mySqlClient.ExecuteQueryRow(query)["lives"].ToString();

                                   if (prepared == 1)
                                   {
                                       var gateMapId = "99" + new Random().Next(1000000, 9999999);
                                       while (true)
                                       {
                                           Spacemap s = GameManager.GetSpacemap(Int32.Parse(gateMapId));
                                           if (s != null) gateMapId = "99" + new Random().Next(1000000, 9999999);
                                           else break;
                                       }
                                       player.activeMapId = Int32.Parse(gateMapId);


                                       int mapId = Convert.ToInt32(133);
                                       string name = Convert.ToString("GG Delta");
                                       int factionId = Convert.ToInt32(player.FactionId);
                                       var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                                       var portals = JsonConvert.DeserializeObject<List<PortalBase>>("".ToString());
                                       var stations = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                                       var options = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                                       var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options, true);
                                       GameManager.Spacemaps.TryAdd(Int32.Parse(gateMapId), spacemap);

                                       player.SendCommand(ActivatePortalCommand.write(Int32.Parse(gateMapId), Id));
                                       await Task.Delay(JUMP_DELAY);

                                       player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                                       player.Spacemap.RemoveCharacter(player);
                                       player.CurrentInRangePortalId = -1;
                                       player.Deselection();
                                       player.Storage.InRangeAssets.Clear();
                                       player.InRangeCharacters.Clear();
                                       player.SetPosition(new Position(10500, 6500)); // Center of map

                                       player.Spacemap = GameManager.GetSpacemap(Int32.Parse(gateMapId));

                                       player.Spacemap.AddAndInitPlayer(player);
                                       player.Storage.Jumping = false;

                                       // Set active gate to Delta gate
                                       player.playerActiveGate = "Delta";
                                       // Create new Delta gate
                                       player.DeltaGates = new DeltaGates(Int32.Parse(gateMapId), gameSession, Int32.Parse(wave), Int32.Parse(lives));
                                   }
                                   else
                                   {
                                       player.Storage.Jumping = false;
                                       player.SendPacket($"0|A|STM|You don't have Delta gate active");
                                   }
                               }
                               else
                               {
                                   player.Storage.Jumping = false;
                                   player.SendPacket($"0|A|STM|You don't have Delta gate active");
                               }
                           }
                           catch
                           {
                               player.Storage.Jumping = false;
                               player.SendPacket($"0|A|STM|You don't have Delta gate active");
                           }
                       }
                   
                }*/
                else if (TargetSpaceMapId == KappaGateId)
                {
                    player.Storage.Jumping = true;

                    // Get gate from database
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        try
                        {
                            var query = $"SELECT * FROM player_galaxygates WHERE userId = '{player.GetPlayerId()}' AND gateId = '7'";
                            var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                            if (result.Rows.Count >= 1)
                            {
                                var prepared = Int32.Parse(mySqlClient.ExecuteQueryRow(query)["prepared"].ToString());
                                var wave = mySqlClient.ExecuteQueryRow(query)["wave"].ToString();
                                var lives = mySqlClient.ExecuteQueryRow(query)["lives"].ToString();

                                if (prepared == 1)
                                {
                                    var gateMapId = "99" + new Random().Next(1000000, 9999999);
                                    while (true)
                                    {
                                        Spacemap s = GameManager.GetSpacemap(Int32.Parse(gateMapId));
                                        if (s != null) gateMapId = "99" + new Random().Next(1000000, 9999999);
                                        else break;
                                    }
                                    player.activeMapId = Int32.Parse(gateMapId);


                                    int mapId = Convert.ToInt32(74);
                                    string name = Convert.ToString("GG Kappa");
                                    int factionId = Convert.ToInt32(player.FactionId);
                                    var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                                    var portals = JsonConvert.DeserializeObject<List<PortalBase>>("".ToString());
                                    var stations = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                                    var options = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                                    var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options, true);
                                    GameManager.Spacemaps.TryAdd(Int32.Parse(gateMapId), spacemap);

                                    player.SendCommand(ActivatePortalCommand.write(Int32.Parse(gateMapId), Id));
                                    await Task.Delay(JUMP_DELAY);

                                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                                    player.Spacemap.RemoveCharacter(player);
                                    player.CurrentInRangePortalId = -1;
                                    player.Deselection();
                                    player.Storage.InRangeAssets.Clear();
                                    player.InRangeCharacters.Clear();
                                    player.SetPosition(new Position(10500, 6500)); // Center of map

                                    player.Spacemap = GameManager.GetSpacemap(Int32.Parse(gateMapId));

                                    player.Spacemap.AddAndInitPlayer(player);
                                    player.Storage.Jumping = false;

                                    // Set active gate to Kappa gate
                                    player.playerActiveGate = "Kappa";
                                    // Create new Kappa gate
                                    player.KappaGates = new KappaGates(Int32.Parse(gateMapId), gameSession, Int32.Parse(wave), Int32.Parse(lives));
                                }
                                else
                                {
                                    player.Storage.Jumping = false;
                                    player.SendPacket($"0|A|STM|You don't have Kappa gate active");
                                }
                            }
                            else
                            {
                                player.Storage.Jumping = false;
                                player.SendPacket($"0|A|STM|You don't have Kappa gate active");
                            }
                        }
                        catch
                        {
                            player.Storage.Jumping = false;
                            player.SendPacket($"0|A|STM|You don't have Kappa gate active");
                        }
                    }

                }
                else if (TargetSpaceMapId == KronosGateId)
                {
                    player.Storage.Jumping = true;

                    // Get gate from database
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        try
                        {
                            var query = $"SELECT * FROM player_galaxygates WHERE userId = '{player.GetPlayerId()}' AND gateId = '9'";
                            var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                            if (result.Rows.Count >= 1)
                            {
                                var prepared = Int32.Parse(mySqlClient.ExecuteQueryRow(query)["prepared"].ToString());
                                var wave = mySqlClient.ExecuteQueryRow(query)["wave"].ToString();
                                var lives = mySqlClient.ExecuteQueryRow(query)["lives"].ToString();

                                if (prepared == 1)
                                {
                                    var gateMapId = "99" + new Random().Next(1000000, 9999999);
                                    while (true)
                                    {
                                        Spacemap s = GameManager.GetSpacemap(Int32.Parse(gateMapId));
                                        if (s != null) gateMapId = "99" + new Random().Next(1000000, 9999999);
                                        else break;
                                    }
                                    player.activeMapId = Int32.Parse(gateMapId);


                                    int mapId = Convert.ToInt32(76);
                                    string name = Convert.ToString("GG Kronos");
                                    int factionId = Convert.ToInt32(player.FactionId);
                                    var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                                    var portals = JsonConvert.DeserializeObject<List<PortalBase>>("".ToString());
                                    var stations = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                                    var options = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                                    var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options, true);
                                    GameManager.Spacemaps.TryAdd(Int32.Parse(gateMapId), spacemap);

                                    player.SendCommand(ActivatePortalCommand.write(Int32.Parse(gateMapId), Id));
                                    await Task.Delay(JUMP_DELAY);

                                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                                    player.Spacemap.RemoveCharacter(player);
                                    player.CurrentInRangePortalId = -1;
                                    player.Deselection();
                                    player.Storage.InRangeAssets.Clear();
                                    player.InRangeCharacters.Clear();
                                    player.SetPosition(new Position(10500, 6500)); // Center of map

                                    player.Spacemap = GameManager.GetSpacemap(Int32.Parse(gateMapId));

                                    player.Spacemap.AddAndInitPlayer(player);
                                    player.Storage.Jumping = false;

                                    // Set active gate to Kronos gate
                                    player.playerActiveGate = "Kronos";
                                    // Create new Kronos gate
                                    player.KronosGates = new KronosGates(Int32.Parse(gateMapId), gameSession, Int32.Parse(wave), Int32.Parse(lives));
                                }
                                else
                                {
                                    player.Storage.Jumping = false;
                                    player.SendPacket($"0|A|STM|You don't have Kronos gate active");
                                }
                            }
                            else
                            {
                                player.Storage.Jumping = false;
                                player.SendPacket($"0|A|STM|You don't have Kronos gate active");
                            }
                        }
                        catch
                        {
                            player.Storage.Jumping = false;
                            player.SendPacket($"0|A|STM|You don't have Kronos gate active");
                        }
                    }

                }
                else if (TargetSpaceMapId == LambdaGateId)
                {
                    player.Storage.Jumping = true;

                    // Get gate from database
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        try
                        {
                            var query = $"SELECT * FROM player_galaxygates WHERE userId = '{player.GetPlayerId()}' AND gateId = '8'";
                            var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                            if (result.Rows.Count >= 1)
                            {
                                var prepared = Int32.Parse(mySqlClient.ExecuteQueryRow(query)["prepared"].ToString());
                                var wave = mySqlClient.ExecuteQueryRow(query)["wave"].ToString();
                                var lives = mySqlClient.ExecuteQueryRow(query)["lives"].ToString();

                                if (prepared == 1)
                                {
                                    var gateMapId = "99" + new Random().Next(1000000, 9999999);
                                    while (true)
                                    {
                                        Spacemap s = GameManager.GetSpacemap(Int32.Parse(gateMapId));
                                        if (s != null) gateMapId = "99" + new Random().Next(1000000, 9999999);
                                        else break;
                                    }
                                    player.activeMapId = Int32.Parse(gateMapId);


                                    int mapId = Convert.ToInt32(75);
                                    string name = Convert.ToString("GG Lambda");
                                    int factionId = Convert.ToInt32(player.FactionId);
                                    var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                                    var portals = JsonConvert.DeserializeObject<List<PortalBase>>("".ToString());
                                    var stations = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                                    var options = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                                    var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options, true);
                                    GameManager.Spacemaps.TryAdd(Int32.Parse(gateMapId), spacemap);

                                    player.SendCommand(ActivatePortalCommand.write(Int32.Parse(gateMapId), Id));
                                    await Task.Delay(JUMP_DELAY);

                                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                                    player.Spacemap.RemoveCharacter(player);
                                    player.CurrentInRangePortalId = -1;
                                    player.Deselection();
                                    player.Storage.InRangeAssets.Clear();
                                    player.InRangeCharacters.Clear();
                                    player.SetPosition(new Position(10500, 6500)); // Center of map

                                    player.Spacemap = GameManager.GetSpacemap(Int32.Parse(gateMapId));

                                    player.Spacemap.AddAndInitPlayer(player);
                                    player.Storage.Jumping = false;

                                    // Set active gate to Lambda gate
                                    player.playerActiveGate = "Lambda";
                                    // Create new Lambda gate
                                    player.LambdaGates = new LambdaGates(Int32.Parse(gateMapId), gameSession, Int32.Parse(wave), Int32.Parse(lives));
                                }
                                else
                                {
                                    player.Storage.Jumping = false;
                                    player.SendPacket($"0|A|STM|You don't have Lambda gate active");
                                }
                            }
                            else
                            {
                                player.Storage.Jumping = false;
                                player.SendPacket($"0|A|STM|You don't have Lambda gate active");
                            }
                        }
                        catch
                        {
                            player.Storage.Jumping = false;
                            player.SendPacket($"0|A|STM|You don't have Lambda gate active");
                        }
                    }

                }
                else if (TargetSpaceMapId == 18 && PremiumMap == true && player.Premium == true)
                {

                    if (!premiumMapActived)
                    {
                        player.SendPacket($"0|A|STM|Map premium deactived temporaly.");
                        return;
                    }

                    var sessionMapId = Int32.Parse("620" + 18);

                    player.Storage.Jumping = true;


                    player.SendCommand(ActivatePortalCommand.write(sessionMapId, Id));
                    await Task.Delay(JUMP_DELAY);

                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                    player.Spacemap.RemoveCharacter(player);
                    player.CurrentInRangePortalId = -1;
                    player.Deselection();
                    player.Storage.InRangeAssets.Clear();
                    player.InRangeCharacters.Clear();
                    player.SetPosition(TargetPosition);
                    player.Spacemap = GameManager.GetSpacemap(sessionMapId);

                    player.Spacemap.AddAndInitPlayer(player);
                    player.Storage.Jumping = false;

                }
                else if (TargetSpaceMapId == 22 && PremiumMap == true && player.Premium == true)
                {

                    if (!premiumMapActived)
                    {
                        player.SendPacket($"0|A|STM|Map premium deactived temporaly.");
                        return;
                    }

                    var sessionMapId = Int32.Parse("620" + 22);

                    player.Storage.Jumping = true;


                    player.SendCommand(ActivatePortalCommand.write(sessionMapId, Id));
                    await Task.Delay(JUMP_DELAY);

                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                    player.Spacemap.RemoveCharacter(player);
                    player.CurrentInRangePortalId = -1;
                    player.Deselection();
                    player.Storage.InRangeAssets.Clear();
                    player.InRangeCharacters.Clear();
                    player.SetPosition(TargetPosition);
                    player.Spacemap = GameManager.GetSpacemap(sessionMapId);

                    player.Spacemap.AddAndInitPlayer(player);
                    player.Storage.Jumping = false;

                }
                else if (TargetSpaceMapId == 26 && PremiumMap == true && player.Premium == true)
                {

                    if (!premiumMapActived)
                    {
                        player.SendPacket($"0|A|STM|Map premium deactived temporaly.");
                        return;
                    }

                    var sessionMapId = Int32.Parse("620" + 26);

                    player.Storage.Jumping = true;


                    player.SendCommand(ActivatePortalCommand.write(sessionMapId, Id));
                    await Task.Delay(JUMP_DELAY);

                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                    player.Spacemap.RemoveCharacter(player);
                    player.CurrentInRangePortalId = -1;
                    player.Deselection();
                    player.Storage.InRangeAssets.Clear();
                    player.InRangeCharacters.Clear();
                    player.SetPosition(TargetPosition);
                    player.Spacemap = GameManager.GetSpacemap(sessionMapId);

                    player.Spacemap.AddAndInitPlayer(player);
                    player.Storage.Jumping = false;

                }
                else
                {

                    if (TargetSpaceMapId == 306 && !blmap1Actived)
                    {
                        player.SendPacket($"0|A|STM|Map BL-1 deactived temporaly.");
                        return;
                    }

                    if (TargetSpaceMapId == 307 && !blmap2Actived)
                    {
                        player.SendPacket($"0|A|STM|Map BL-2 deactived temporaly.");
                        return;
                    }

                    if (TargetSpaceMapId == 308 && !blmap3Actived)
                    {
                        player.SendPacket($"0|A|STM|Map BL-3 deactived temporaly.");
                        return;
                    }

                    player.Storage.Jumping = true;
                    int tmpId = TargetSpaceMapId;

                    if (TargetSpaceMapId == -1) tmpId = player.GetBaseMapId();

                    player.SendCommand(ActivatePortalCommand.write(tmpId, Id));

                    if(player.activeMapId.ToString().StartsWith("99"))
                    {
                        Spacemap tmp = null;
                        GameManager.Spacemaps.TryRemove(player.activeMapId, out tmp);
                    }

                    await Task.Delay(JUMP_DELAY);

                    if(Spacemap.Options.PvpMap && player.AttackingOrUnderAttack())
                    {
                        player.Storage.Jumping = false;
                        player.SendPacket($"0|A|STM|jumpgate_failed_pvp_map");
                        return;
                    }

                    player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                    player.Spacemap.RemoveCharacter(player);
                    player.CurrentInRangePortalId = -1;
                    player.Deselection();
                    player.Storage.InRangeAssets.Clear();
                    player.InRangeCharacters.Clear();
                    player.SetPosition(TargetPosition);

                    player.Spacemap = GameManager.GetSpacemap(tmpId);
                    if (player.Spacemap.Id == 16)
                    {
                        foreach (var item in player.Spacemap.Activatables)
                        {
                            if (item.Value is BattleStation battle)
                            {
                                if ((player.Clan.Id == battle.Clan.Id && player.Clan.Id != 0 && battle.Clan.Id != 0 && !battle.InBuildingState))
                                {
                                    player.BoosterManager.Add(BoosterType.DMG_B03, 1);
                                    player.BoosterManager.Add(BoosterType.HP_B03, 1);
                                    player.BoosterManager.Add(BoosterType.HON_B03, 1);
                                }

                            }
                        }
                    }

                    //if ((player.Spacemap.Id == 306 || player.Spacemap.Id == 307 || player.Spacemap.Id == 308) && player.lf5lvl >= 16 && player.lf5lasers >= 1 && player.Level <= 23)
                    if ((player.Spacemap.Id == 306 || player.Spacemap.Id == 307 || player.Spacemap.Id == 308) && player.Level <= 23)
                    {
                        //player.SendPacket($"0|A|STM|You are on map {player.Spacemap.Name} and your level is <= {player.Level} and your Prometheus laser level is {player.lf5lvl}, your damage has been increasing in x3 only in BL maps.");
                        player.SendPacket($"0|A|STM|You are on map {player.Spacemap.Name} and your level is <= {player.Level}, your damage has been increasing in x3 only in BL maps.");
                    }

                    player.Spacemap.AddAndInitPlayer(player);
                    player.Storage.Jumping = false;

                    player.AttackManager.lastAttackTime = new DateTime();
                    player.AttackManager.lastRSBAttackTime = new DateTime();
                    player.AttackManager.lastRocketAttack = new DateTime();

                    if (player.Spacemap.CheckRadiation(player) || player.Spacemap.CheckActivatables(player))
                        player.SendCommand(player.GetBeaconCommand());

                    //Console.WriteLine(this.positionInitializacion.mapID);
                    //QueryManager.SavePlayer.Information(player);

                    if (TargetSpaceMapId == 61 || TargetSpaceMapId == 62 || TargetSpaceMapId == 63)
                    {
                        player.AttackManager.lastAttackTime = DateTime.Now;
                        player.AttackManager.lastRocketAttack = DateTime.Now;
                        player.AttackManager.lastRSBAttackTime = DateTime.Now;
                    }
                }



            }
            catch (Exception e)
            {
                Out.WriteLine("Click void exception: " + e, "Portal.cs");
                Logger.Log("error_log", $"- [Portal.cs] Click void exception: {e}");
            }
        }

        public void Remove()
        {
            var portal = this as Activatable;
            Spacemap.Activatables.TryRemove(Id, out portal);
            GameManager.SendCommandToMap(Spacemap.Id, RemovePortalCommand.write(Id));
        }

        public static int CalculateDistance(int mapId, int destinationMapId)
        {
            return Math.Abs(destinationMapId - mapId);
        }

        public override byte[] GetAssetCreateCommand(short clanRelationModule = ClanRelationModule.NONE, Player p = null)
        {
            return CreatePortalCommand.write(Id, FactionId, GraphicsId,
                                           Position.X, Position.Y, true,
                                           Visible, new List<int>());
        }
    }
}

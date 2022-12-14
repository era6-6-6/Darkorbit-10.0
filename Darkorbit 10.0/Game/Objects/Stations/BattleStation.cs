namespace Darkorbit.Game.Objects.Players.Stations
{
    public class EquippedModuleBase
    {
        public int ClanId { get; set; }
        public List<SatelliteBase> Modules { get; set; }

        public EquippedModuleBase(int clanId, List<SatelliteBase> satellites)
        {
            ClanId = clanId;
            Modules = satellites;
        }
    }

    class BattleStation : Activatable
    {
        public Dictionary<int, List<Satellite>> EquippedStationModule = new Dictionary<int, List<Satellite>>();

        public bool InBuildingState = false;
        public int BuildTimeInMinutes = 0;

        public bool DeflectorActive = false;
        public int DeflectorSecondsLeft = 0;
        public int DeflectorSecondsMax = 0;
        public int DeflectorTimeOffline = 0;
        public int DeflectorTimeDuration { get; set; }

        public string AsteroidName { get; set; }
        public int proteClanId { get; set; }
        public int idBSS { get; set; }
        public static Player player { get; set; }

        public BattleStation(string name, Spacemap spacemap, Position position, Clan clan,
            List<EquippedModuleBase> modules, bool inBuildingState, int buildTimeInMinutes, DateTime buildTime,
            bool deflectorActive, int deflectorSecondsLeft, DateTime deflectorTime,
            List<int> visualModifiers, int protectedClanId, int idBS) : base(spacemap, (clan.Id != 0 ? clan.FactionId : 0), position, clan, (clan.Id == 0 || inBuildingState ? AssetTypeModule.ASTEROID : AssetTypeModule.BATTLESTATION))
        {

            ShieldAbsorption = 0.8;

            MaxHitPoints = 10000000;
            CurrentHitPoints = MaxHitPoints;
            CurrentShieldPoints = 10000000;
            MaxShieldPoints = 10000000;

            InBuildingState = inBuildingState;
            BuildTimeInMinutes = buildTimeInMinutes;


            DeflectorActive = deflectorActive;
            DeflectorSecondsLeft = deflectorSecondsLeft;
            DeflectorSecondsMax = 86400;

            AsteroidName = name;
            proteClanId = protectedClanId;
            idBSS = idBS;

            Name = Clan.Id != 0 ? Clan.Name : name;

            if (deflectorSecondsLeft > 0)
                DeflectorActive = true;

            if (DeflectorActive)
            {
                DeflectorSecondsLeft = DeflectorSecondsLeft - (int)DateTime.Now.Subtract(deflectorTime).TotalMinutes;
                this.deflectorTime = DateTime.Now;
                Invincible = true;
                AddVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR, DeflectorSecondsLeft, "", 0, true);
                Program.TickManager.AddTick(this);
            }

            foreach (var modifier in visualModifiers)
                AddVisualModifier((short)modifier, 0, "", 0, true);

            foreach (var module in modules)
            {
                var satellite = new List<Satellite>();

                foreach (var m in module.Modules)
                {
                    var s = new Satellite(this, m.OwnerId, Satellite.GetName(m.Type), m.DesignId, m.ItemId, m.SlotId, m.Type, Satellite.GetPosition(Position, m.SlotId));
                    s.InstallationSecondsLeft = m.InstallationSecondsLeft;
                    s.Installed = m.Installed;
                    s.CurrentHitPoints = m.CurrentHitPoints;
                    s.MaxHitPoints = m.MaxHitPoints;
                    s.CurrentShieldPoints = m.CurrentShieldPoints;
                    s.MaxShieldPoints = m.MaxShieldPoints;

                    if (DeflectorActive)
                        s.AddVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR, 0, "", 0, true);
                    satellite.Add(s);
                }

                EquippedStationModule.Add(module.ClanId, satellite);
            }

            if (Clan.Id != 0 && InBuildingState)
            {
                BuildTimeInMinutes = BuildTimeInMinutes - (int)DateTime.Now.Subtract(buildTime).TotalMinutes;
                this.buildTime = DateTime.Now;
                Program.TickManager.AddTick(this);
            }
            else if (Clan.Id != 0 && !InBuildingState)
                Build();
        }

        public DateTime buildTime = new DateTime();
        public DateTime deflectorTime = new DateTime();
        public DateTime deflectorOffTime;
        public new void Tick()
        {

            if (DeflectorActive && DeflectorTimeOffline == 0)
            {
                //System.Threading.Thread.Sleep(1000);
                DeflectorSecondsLeft -= 1;
            }

            if (!DeflectorActive && DeflectorTimeOffline > 0)
            {
                //System.Threading.Thread.Sleep(1000);
                DeflectorSecondsLeft += 1;
            }

            if (InBuildingState && buildTime.AddMinutes(BuildTimeInMinutes) < DateTime.Now)
            {
                Build();
                QueryManager.BattleStations.BattleStation(this);
            }

            if (DeflectorActive && deflectorTime.AddSeconds(DeflectorSecondsLeft) < DateTime.Now)
            {
                DeactiveDeflector();
            }

            if (DeflectorTimeOffline > 0 && deflectorOffTime < DateTime.Now)
            {
                DeflectorTimeOffline = 0;
                ActiveDeflector();
            }

        }

        public void DeactiveDeflector()
        {
            RemoveVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR);

            foreach (var modules in EquippedStationModule.Values)
                foreach (var satellite in modules)
                    satellite.RemoveVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR);

            Invincible = false;
            DeflectorActive = false;
            DeflectorSecondsLeft = 0;
            QueryManager.BattleStations.BattleStation(this);

            if (player != null)
                updateDeflector(0);
        }

        public void ActiveDeflector()
        {
            AddVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR, DeflectorSecondsLeft, "", 0, true);

            foreach (var modules in EquippedStationModule.Values)
                foreach (var satellite in modules)
                    satellite.AddVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR, 0, "", 0, true);

            Invincible = true;
            DeflectorActive = true;
            QueryManager.BattleStations.BattleStation(this);
            updateDeflector(1);

            Program.TickManager.AddTick(this);
        }


        public void DeactiveDeflectorButton(int minutes, int cc1)
        {
            RemoveVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR);

            foreach (var modules in EquippedStationModule.Values)
                foreach (var satellite in modules)
                    satellite.RemoveVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR);

            Invincible = false;
            DeflectorActive = false;
            DeflectorTimeOffline = minutes;
            deflectorOffTime = DateTime.Now.AddMinutes(minutes);

            QueryManager.BattleStations.BattleStation(this);

            updateDeflector(0);
        }

        public void updateDeflector(int state)
        {
            var stationModuleModule = new List<StationModuleModule>();

            if (EquippedStationModule.ContainsKey(player.Clan.Id))
            {
                foreach (var mm in EquippedStationModule[player.Clan.Id])
                {
                    if (mm.Type == StationModuleModule.HULL || mm.Type == StationModuleModule.DEFLECTOR)
                    {
                        mm.CurrentHitPoints = CurrentHitPoints;
                        mm.MaxHitPoints = MaxHitPoints;
                        mm.CurrentShieldPoints = CurrentShieldPoints;
                        mm.MaxShieldPoints = MaxShieldPoints;
                    }

                    stationModuleModule.Add(new StationModuleModule(Id, mm.ItemId, mm.SlotId, mm.Type, mm.CurrentHitPoints,
                            mm.MaxHitPoints, mm.CurrentShieldPoints, mm.MaxShieldPoints, 16, QueryManager.GetUserPilotName(mm.OwnerId), 0, mm.InstallationSecondsLeft, 0, 0, 500));
                }
            }

            var playerModules = new List<StationModuleModule>();

            for (var i = 0; i < player.Storage.BattleStationModules.Count; i++)
            {
                if (!player.Storage.BattleStationModules[i].InUse)
                    playerModules.Add(new StationModuleModule(Id, player.Storage.BattleStationModules[i].Id, i, player.Storage.BattleStationModules[i].Type, 1, 1, 1, 1, 16, QueryManager.GetUserPilotName(player.Id), 0, 0, 0, 0, 500));
            }

            if (Clan.Id != 0 && player.Clan.Id == Clan.Id)
            {
                player.SendCommand(BattleStationManagementUiInitializationCommand.write(
                    Id,
                    Id,
                    Name,
                    Clan.Name,
                    new FactionModule((short)FactionId),
                    new BattleStationStatusCommand(Id, Id, Name, DeflectorActive, DeflectorSecondsLeft, 86400, 0, 0, 0, 0, 0, 0, DeflectorTimeOffline, 0, new EquippedModulesModule(stationModuleModule)),
                    new AvailableModulesCommand(playerModules),
                    1,
                    480,
                    0,
                    true));
            }
        }

        public void Build()
        {
            AssetTypeId = AssetTypeModule.BATTLESTATION;

            RemoveVisualModifier(VisualModifierCommand.BATTLESTATION_CONSTRUCTING);

            if (DeflectorActive)
            {
                this.deflectorTime = DateTime.Now;
                DeflectorSecondsLeft = DeflectorSecondsLeft - (int)DateTime.Now.Subtract(deflectorTime).TotalMinutes;
                Invincible = true;
                AddVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR, DeflectorSecondsLeft, "", 0, true);
                Program.TickManager.AddTick(this);
            }

            PrepareSatellites();

            GameManager.SendCommandToMap(Spacemap.Id, AssetRemoveCommand.write(GetAssetType(), Id));

            foreach (var character in Spacemap.Characters.Values)
            {
                if (character is Player player)
                {
                    short relationType = character.Clan.Id != 0 && Clan.Id != 0 ? Clan.GetRelation(character.Clan) : (short)0;
                    player.SendCommand(GetAssetCreateCommand(relationType));
                }
            }

            BuildTimeInMinutes = 0;
            InBuildingState = false;
        }

        public void PrepareSatellites()
        {
            foreach (var satellite in EquippedStationModule[Clan.Id])
            {
                if (DeflectorActive)
                    satellite.AddVisualModifier(VisualModifierCommand.BATTLESTATION_DEFLECTOR, 0, "", 0, true);
                if (satellite.Type != StationModuleModule.DEFLECTOR && satellite.Type != StationModuleModule.HULL)
                {
                    Spacemap.Activatables.TryAdd(satellite.Id, satellite);

                    foreach (var character in satellite.Spacemap.Characters.Values)
                    {
                        if (character is Player player)
                        {
                            short relationType = character.Clan.Id != 0 && satellite.Clan.Id != 0 ? satellite.Clan.GetRelation(character.Clan) : (short)0;
                            player.SendCommand(satellite.GetAssetCreateCommand(relationType));
                        }
                    }
                }
            }
        }

        public override void Click(GameSession gameSession)
        {

            player = gameSession.Player;

            int secondsLeft = (int)(TimeSpan.FromMinutes(BuildTimeInMinutes).TotalSeconds - (DateTime.Now - buildTime).TotalSeconds);

            if (InBuildingState)
                player.SendCommand(BattleStationBuildingStateCommand.write(Id, Id, Name, secondsLeft, 0, Clan.Name, new FactionModule((short)FactionId)));
            else
            {
                if (player.Clan.Id == 0)
                    player.SendCommand(BattleStationNoClanUiInitializationCommand.write(Id));
                else
                {
                    var stationModuleModule = new List<StationModuleModule>();

                    if (EquippedStationModule.ContainsKey(player.Clan.Id))
                    {
                        foreach (var mm in EquippedStationModule[player.Clan.Id])
                        {
                            if (mm.Type == StationModuleModule.HULL || mm.Type == StationModuleModule.DEFLECTOR)
                            {
                                mm.CurrentHitPoints = CurrentHitPoints;
                                mm.MaxHitPoints = MaxHitPoints;
                                mm.CurrentShieldPoints = CurrentShieldPoints;
                                mm.MaxShieldPoints = MaxShieldPoints;
                            }

                            stationModuleModule.Add(new StationModuleModule(Id, mm.ItemId, mm.SlotId, mm.Type, mm.CurrentHitPoints,
                                    mm.MaxHitPoints, mm.CurrentShieldPoints, mm.MaxShieldPoints, 16, QueryManager.GetUserPilotName(mm.OwnerId), 0, mm.InstallationSecondsLeft, 0, 0, 500));
                        }
                    }

                    var playerModules = new List<StationModuleModule>();

                    for (var i = 0; i < player.Storage.BattleStationModules.Count; i++)
                    {
                        if (!player.Storage.BattleStationModules[i].InUse)
                            playerModules.Add(new StationModuleModule(Id, player.Storage.BattleStationModules[i].Id, i, player.Storage.BattleStationModules[i].Type, 1, 1, 1, 1, 16, QueryManager.GetUserPilotName(player.Id), 0, 0, 0, 0, 500));
                    }

                    if (Clan.Id != 0 && player.Clan.Id == Clan.Id)
                    {

                        player.SendCommand(BattleStationManagementUiInitializationCommand.write(
                            Id,
                            Id,
                            Name,
                            Clan.Name,
                            new FactionModule((short)FactionId),
                            new BattleStationStatusCommand(Id, Id, Name, DeflectorActive, DeflectorSecondsLeft, 86400, 0, 0, 0, 0, 0, 0, DeflectorTimeOffline, 0, new EquippedModulesModule(stationModuleModule)),
                            new AvailableModulesCommand(playerModules),
                            1,
                            480,
                            0,
                            true));

                    }
                    else
                    {
                        var bestClan = EquippedStationModule.Values.Where(x => x.Where(y => y.Installed).ToList().Count() > 0).ToList().Count > 0 ? EquippedStationModule.OrderByDescending(x => x.Value.Count) : null;
                        player.SendCommand(BattleStationBuildingUiInitializationCommand.write(Id, Id, Name,
                                          new AsteroidProgressCommand(
                                                  Id,
                                                  (float)(EquippedStationModule.ContainsKey(player.Clan.Id) ? EquippedStationModule[player.Clan.Id].Where(x => x.Installed).ToList().Count : 0) / 10,
                                                  (float)(bestClan != null ? bestClan.FirstOrDefault().Value.Where(x => x.Installed).ToList().Count : 0) / 10,
                                                  player.Clan.Name,
                                                  bestClan != null ? GameManager.GetClan(bestClan.FirstOrDefault().Key)?.Name : "Leading clan's progress",
                                                  new EquippedModulesModule(EquippedStationModule.ContainsKey(player.Clan.Id) ? stationModuleModule : new List<StationModuleModule>()),
                                                  (EquippedStationModule.ContainsKey(player.Clan.Id) ? EquippedStationModule[player.Clan.Id].Where(x => x.Installed).ToList().Count : 0) == 10),
                                          new AvailableModulesCommand(playerModules),
                                          1,
                                          60,
                                          0));
                    }
                }
            }
        }

        public override byte[] GetAssetCreateCommand(short clanRelationModule = ClanRelationModule.NONE, Player p = null)
        {
            return AssetCreateCommand.write(GetAssetType(), Name,
                                          FactionId, Clan.Tag, Id, 0, 0,
                                          Position.X, Position.Y, Clan.Id, true, true, true, true,
                                          new ClanRelationModule(clanRelationModule),
                                          VisualModifiers.Values.ToList());
        }
    }
}

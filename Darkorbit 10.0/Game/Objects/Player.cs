using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using Darkorbit.Game.Movements;
using Darkorbit.Managers;
using Darkorbit.Game.Objects.Players.Managers;
using Darkorbit.Managers.MySQLManager;
using Darkorbit.Game.GalaxyGates;
using Darkorbit.Game.Events;
using Darkorbit.Game.Objects.Players.Stations;
using Darkorbit.Net.netty;

namespace Darkorbit.Game.Objects
{
    internal class Player : Character
    {
        public string PetName { get; set; }
        public int RankId { get; set; }
        public int UserId { get; set; }
        //public int WarRank { get; set; }
        public int UbaPoints { get; set; }
        public int reset { get; set; }
        public int Ubabattel { get; set; }
        public int Ubarank { get; set; }
        public int WarRank { get; set; }
        //public int bootyKeys { get; set; }
        public bootyKeys bootyKeys { get; set; }


        public int droneExp { get; set; }
        public int petExp { get; set; }
        public bool Premium { get; set; }
        public string Title { get; set; }

        public bool bountyHunterAquired { get; set; }
        public bool shieldmechanics { get; set; }

        public int activeMapId;

        public int playerId;
        public int GlobalId;

        public AlphaGate AlphaGate;
        public BetaGates BetaGates;
        public GammaGates GammaGates;
        public DeltaGates DeltaGates;
        public KappaGates KappaGates;
        public KronosGates KronosGates;
        public LambdaGates LambdaGates;
        public string playerActiveGate = "";
        private int nextLevel = 2;

        public bool LeonovEffect = false;

        public string premiumOld = null;

        public TDM_Lobby tdmLobby = null;
        private int DestroyedOnSpacemap = 0;


        public int NLevel
        {
            get
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var querySet = mySqlClient.ExecuteQueryRow($"SELECT nextLevel FROM player_accounts WHERE userId = {Id}");

                    string nextLevel = querySet["nextLevel"].ToString();

                    if (nextLevel != "")
                    {
                        this.nextLevel = (short)querySet["nextLevel"];
                    }

                }

                return this.nextLevel;
            }
            set
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var querySet = mySqlClient.ExecuteQueryRow($"SELECT nextLevel FROM player_accounts WHERE userId = {Id}");

                    string nextLevel = querySet["nextLevel"].ToString();

                    if (nextLevel != "")
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET nextLevel = '{value}' WHERE userId = {Id}");
                    }

                }
            }
        }

        public int Level
        {
            get
            {
                short lvl = 1;
                long expNext = 10000;

                while (Data.experience >= expNext)
                {
                    expNext *= 2;
                    lvl++;
                }

                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var querySet = mySqlClient.ExecuteQueryRow($"SELECT level, nextLevel FROM player_accounts WHERE userId = {Id}");

                    string level = querySet["level"].ToString();
                    string nextLevel = querySet["nextLevel"].ToString();

                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET level = '{lvl}' WHERE userId = {Id}");

                    if (nextLevel == "")
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET nextLevel = '{lvl + 1}' WHERE userId = {Id}");
                    }

                }

                return lvl;
            }
            set => Level = value;
        }

        public int CurrentInRangePortalId = -1;
        public int CurrentShieldConfig1 { get; set; }
        public int CurrentShieldConfig2 { get; set; }
        public int CurrentConfig { get; set; }

        public SettingsBase Settings = new SettingsBase();
        public DestructionsBase Destructions { get; set; }

        public positionInitializacion positionInitializacion { get; set; }
        public EquipmentBase Equipment { get; set; }
        public DataBase Data { get; set; }
        public SkillTreeBase SkillTree = new SkillTreeBase();
        public Group Group { get; set; }
        public Pet Pet { get; set; }
        public Flagship Flagschip { get; set; }
        public AttackManager AttackManager { get; set; }
        public SettingsManager SettingsManager { get; set; }
        public DroneManager DroneManager { get; set; }
        public CpuManager CpuManager { get; set; }
        public TechManager TechManager { get; set; }
        public SkillManager SkillManager { get; set; }
        public BoosterManager BoosterManager { get; set; }
        public string currentMapName { get; set; }
        public int ec { get; set; }
        public string DEFAULT_FORMATION { get; set; }
        public string TURTLE_FORMATION { get; set; }
        public string ARROW_FORMATION { get; set; }
        public string LANCE_FORMATION { get; set; }
        public string STAR_FORMATION { get; set; }
        public string PINCER_FORMATION { get; set; }
        public string DOUBLE_ARROW_FORMATION { get; set; }
        public string DIAMOND_FORMATION { get; set; }
        public string CHEVRON_FORMATION { get; set; }
        public string MOTH_FORMATION { get; set; }
        public string CRAB_FORMATION { get; set; }
        public string HEART_FORMATION { get; set; }
        public string BARRAGE_FORMATION { get; set; }
        public string BAT_FORMATION { get; set; }
        public string DOME_FORMATION { get; set; }
        public string DRILL_FORMATION { get; set; }
        public string RING_FORMATION { get; set; }
        public string VETERAN_FORMATION { get; set; }
        public string WHEEL_FORMATION { get; set; }
        public string WAVE_FORMATION { get; set; }
        public string X_FORMATION { get; set; }

        public AmmunitionManager AmmunitionManager { get; set; }
        public int lf4;
        public int lf4lasers;
        public int lf4Damage;
        public int conf1prome;
        public int conf2prome;
        public int lasers;

        public int lf5lasers;
        public int lf5Damage;
        public int lf5lvl;

        public Task sync;
        private static CancellationTokenSource tsSync = new CancellationTokenSource();
        private static CancellationToken ctSync;
        public bool LockSync;
        public bool RocketInProgress;
        public int SelectedRocket;
        public int OwnFactionNegativeHonor = 512;

        public bool TDMTeamMatched = false;

        public Player(int id, string name, Clan clan, int factionId, int rankId, int rank, Ship ship, short petDesign)
                     : base(id, name, factionId, ship, new Position(0, 0), null, clan, petDesign)
        {
            playerId = id;
            Name = name;
            Clan = clan;
            FactionId = factionId;
            RankId = rankId;
            //WarRank = warRank;
            InitiateManagers();

            ctSync = tsSync.Token;

            MaxNanoHull = ship.BaseHitpoints;

            LockSync = false;
        }

        public void InitiateManagers()
        {
            DroneManager = new DroneManager(this);
            AttackManager = new AttackManager(this);
            TechManager = new TechManager(this);
            SkillManager = new SkillManager(this);
            SettingsManager = new SettingsManager(this);
            AmmunitionManager = new AmmunitionManager(this);
            CpuManager = new CpuManager(this);
            BoosterManager = new BoosterManager(this);
        }

        public override void Tick()
        {
            positionInitializacion.mapID = Spacemap.Id;
            positionInitializacion.x = Position.X;
            positionInitializacion.y = Position.Y;
            positionInitializacion.isInMapPremium = isInMapPremium;
            Movement.ActualPosition(this);
            CheckHitpointsRepair();
            CheckShieldPointsRepair();
            CheckRadiation();
            AttackManager.LaserAttack();
            AttackManager.RocketLauncher.Tick();
            RefreshAttackers();
            TDMCheck();
            Logout();

           

            Storage.Tick();
            DroneManager.Tick();
            TechManager.Tick();
            SkillManager.Tick();
            BoosterManager.Tick();
        }

        public void StartSync()
        {
            sync = Task.Factory.StartNew(() => Sync());
        }

        private void Sync()
        {
            try
            {
                while(!ctSync.IsCancellationRequested)
                {
                    if (!LockSync)
                    {
                        LoadData();

                        QueryManager.SavePlayer.SaveData(this);

                        if (Invisible && Spacemap.Options.CloakBlocked)
                        {
                            CpuManager.DisableCloak();
                        }

                        int kickTime = 6;
                        int step1 = 3;
                        int step2 = 4;
                        int step3 = 5;

                        if (Spacemap != null)
                        {
                            if ((Spacemap.Id == 61 || Spacemap.Id == 62 || Spacemap.Id == 63) && LastAttackTimeInvasion(step1 * 60))
                            {
                                SendPacket($"0|A|STD|[INVASION] You're inactive and would be kicked out in {kickTime - step1} minutes");
                            }
                            else if ((Spacemap.Id == 61 || Spacemap.Id == 62 || Spacemap.Id == 63) && LastAttackTimeInvasion(step2 * 60))
                            {
                                SendPacket($"0|A|STD|[INVASION] You're inactive and would be kicked out in {kickTime - step2} minutes");
                            }
                            else if ((Spacemap.Id == 61 || Spacemap.Id == 62 || Spacemap.Id == 63) && LastAttackTimeInvasion(step3 * 60))
                            {
                                SendPacket($"0|A|STD|[INVASION] You're inactive and would be kicked out in {kickTime - step3} minute");
                            }
                            else if ((Spacemap.Id == 61 || Spacemap.Id == 62 || Spacemap.Id == 63) && LastAttackTimeInvasion(kickTime * 60))
                            {
                                SendPacket($"0|A|STD|[INVASION] You're inactive and has been kicked out of the invasion");
                                Jump(GetBaseMapId(), Position.Random(Spacemap, 2000, 20000, 1500, 10000));
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Player.cs] Main void exception: {ex}");
            }
        }

        public DateTime lastHpRepairTime = new DateTime();

        private void CheckHitpointsRepair()
        {
            if (CurrentHitPoints >= MaxHitPoints || AttackingOrUnderAttack() || Moving)
            {
                if (Storage.RepairBotActivated)
                {
                    RepairBot(false);
                }

                return;
            }

            if (lastHpRepairTime.AddSeconds(1) >= DateTime.Now)
            {
                return;
            }

            if (!Storage.RepairBotActivated)
            {
                RepairBot(true);
            }

            int repairHitpoints = MaxHitPoints / 40;
            repairHitpoints += Maths.GetPercentage(repairHitpoints, BoosterManager.GetPercentage(BoostedAttributeType.REPAIR));
            repairHitpoints += Maths.GetPercentage(repairHitpoints, GetSkillPercentage("Engineering"));

            Heal(repairHitpoints);

            lastHpRepairTime = DateTime.Now;
        }

        public DateTime lastShieldRepairTime = new DateTime();
        private void CheckShieldPointsRepair()
        {
            if (LastCombatTime.AddSeconds(10) >= DateTime.Now || lastShieldRepairTime.AddSeconds(1) >= DateTime.Now ||
                CurrentShieldPoints >= MaxShieldPoints || Settings.InGameSettings.selectedFormation == DroneManager.MOTH_FORMATION
                || Settings.InGameSettings.selectedFormation == DroneManager.WHEEL_FORMATION)
            {
                return;
            }
            int repairShield = MaxShieldPoints / 25;
            CurrentShieldPoints += repairShield;
            UpdateStatus();

            lastShieldRepairTime = DateTime.Now;
        }

        public DateTime lastRadiationDamageTime = new DateTime();
        public void CheckRadiation()
        {
            if (Storage.Jumping || !Storage.IsInRadiationZone || Storage.invincibilityEffectTime.AddSeconds(5) >= DateTime.Now || lastRadiationDamageTime.AddSeconds(1) >= DateTime.Now)
            {
                return;
            }

            AttackManager.Damage(this, this, DamageType.RADIATION, 20000, true, true, false);
            lastRadiationDamageTime = DateTime.Now;
        }

        public void SetSpeedBoost(int speed)
        {
            Storage.SpeedBoost = speed;
            SendCommand(SetSpeedCommand.write(Speed, Speed));
        }

        public void RepairBot(bool activated)
        {
            Storage.RepairBotActivated = activated;
            SendCommand(GetBeaconCommand());
        }

        public void SetShieldSkillActivated(bool pShieldSkillActivated)
        {
            Storage.ShieldSkillActivated = pShieldSkillActivated;

            if (pShieldSkillActivated)
            {
                //SendCommand(AttributeSkillShieldUpdateCommand.write(1, 15, 0));
                SendCommand(AttributeSkillShieldUpdateCommand.write(0, 0, 0));
            }
            else
            {
                SendCommand(AttributeSkillShieldUpdateCommand.write(0, 0, 0));
            }
        }

        public override int Speed
        {
            get
            {
                int value = CurrentConfig == 1 ? Equipment.Configs.Config1Speed : Equipment.Configs.Config2Speed;
                if (LeonovEffect) value += 40;

                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.DOME_FORMATION:
                        value -= Maths.GetPercentage(value, 50);
                        break;
                    case DroneManager.CRAB_FORMATION:
                        value -= Maths.GetPercentage(value, 15);
                        break;
                    case DroneManager.BAT_FORMATION:
                        value -= Maths.GetPercentage(value, 15);
                        break;
                    case DroneManager.RING_FORMATION:
                        value -= Maths.GetPercentage(value, 5);
                        break;
                    case DroneManager.DRILL_FORMATION:
                        value -= Maths.GetPercentage(value, 5);
                        break;
                    case DroneManager.WHEEL_FORMATION:
                        value += Maths.GetPercentage(value, 5);
                        break;
                }
                if (Storage.underDCR_250)
                {                   
                    value -= Maths.GetPercentage(value, 30);
                }

                if (Storage.underSLM_01)
                {
                    value -= Maths.GetPercentage(value, 50);
                }

                if (Storage.underR_IC3)
                {
                    value -= value;
                }

                if (Storage.Lightning)
                {
                    value += Maths.GetPercentage(value, 30);
                }

                value += Storage.SpeedBoost;

                return value;
            }
        }

        public override int MaxHitPoints
        {
            get
            {
                int value = CurrentConfig == 1 ? Equipment.Configs.Config1Hitpoints : Equipment.Configs.Config2Hitpoints;

                if (LeonovEffect) value *= 2;

                value += Maths.GetPercentage(value, BoosterManager.GetPercentage(BoostedAttributeType.MAXHP));
                value += GetSkillPercentage("Ship Hull");

                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.CHEVRON_FORMATION:
                        value -= Maths.GetPercentage(value, 20);
                        break;
                    case DroneManager.DIAMOND_FORMATION:
                        value -= Maths.GetPercentage(value, 30);
                        break;
                    case DroneManager.MOTH_FORMATION:
                        value += Maths.GetPercentage(value, 20);
                        break;
                    case DroneManager.HEART_FORMATION:
                        value += Maths.GetPercentage(value, 20);
                        break;
                    case DroneManager.VETERAN_FORMATION:
                        value -= Maths.GetPercentage(value, 20);
                        break;
                    case DroneManager.X_FORMATION:
                        value += Maths.GetPercentage(value, 8);
                        break;
                }

                if (Ship == null)
                {
                    return 0;
                }

                value = Ship.GetHitPointsBoost(value);
                return value;
            }
        }

        public double RocketSpeed
        {
            get
            {
                double value =  1;

                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.DOME_FORMATION:
                        value -= 0.25;
                        break;
                    case DroneManager.RING_FORMATION:
                        value += 0.25;
                        break;
                }

                return value;
            }
        }

        public double RocketLauncherSpeed
        {
            get
            {
                double value = 1.5;

                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.DOME_FORMATION:
                        value -= 0.25;
                        break;
                    case DroneManager.STAR_FORMATION:
                        value += 0.33;
                        break;
                    case DroneManager.RING_FORMATION:
                        value += 0.25;
                        break;
                }

                return value;
            }
        }

        public override int CurrentShieldPoints
        {
            get
            {
                int value = CurrentConfig == 1 ? CurrentShieldConfig1 : CurrentShieldConfig2;
                return value;
            }
            set
            {
                if (CurrentConfig == 1)
                {
                    CurrentShieldConfig1 = value;
                }
                else
                {
                    CurrentShieldConfig2 = value;
                }
            }
        }

        public int GetMaxShieldPoints(int config)
        {
            int value = config == 1 ? Equipment.Configs.Config1Shield : Equipment.Configs.Config2Shield;

            if (LeonovEffect)
            {
                value = int.Parse((Math.Round(value * 1.5)).ToString());
            }

            //value += Maths.GetPercentage(value, 25); //Seprom
            value += Maths.GetPercentage(value, BoosterManager.GetPercentage(BoostedAttributeType.SHIELD));
            value += Maths.GetPercentage(value, GetSkillPercentage("Shield Engineering"));

            switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
            {
                case DroneManager.TURTLE_FORMATION:
                    value += Maths.GetPercentage(value, 10);
                    break;
                case DroneManager.RING_FORMATION:
                    value += Maths.GetPercentage(value, 85);
                    break;
                case DroneManager.DRILL_FORMATION:
                    value -= Maths.GetPercentage(value, 25);
                    break;
                case DroneManager.DOME_FORMATION:
                    value += Maths.GetPercentage(value, 30);
                    break;
                case DroneManager.HEART_FORMATION:
                    value += Maths.GetPercentage(value, 20);
                    break;
                case DroneManager.DOUBLE_ARROW_FORMATION:
                    value -= Maths.GetPercentage(value, 20);
                    break;
                case DroneManager.VETERAN_FORMATION:
                    value -= Maths.GetPercentage(value, 20);
                    break;
            }

            //Console.WriteLine("before_shield: " + value);
            value = Ship.GetShieldPointsBoost(value);
            //Console.WriteLine("after_shield: " + value);
            //value += Maths.GetPercentage(value, DroneManager.droneLevel(droneExp) / 2);
            //value += (int)(value * 1);
            //value -= Maths.GetPercentage(value, 40);

            return value;
        }

        public override int MaxShieldPoints
        {
            get
            {
                return GetMaxShieldPoints(CurrentConfig);
            }
        }

        public override double ShieldAbsorption
        {
            get
            {
                double value = 0.97;
                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.CRAB_FORMATION:
                        value += 0.2;
                        break;
                    case DroneManager.BARRAGE_FORMATION:
                        value -= 0.15;
                        break;
                }
                return value;
            }
        }

        public override double ShieldPenetration
        {
            get
            {
                var value = 0.8;
                switch (SkillTree.shieldmechanics)
                {
                    case 1:
                        value += 0.02;
                        break;
                    case 2:
                        value += 0.04;
                        break;
                    case 3:
                        value += 0.06;
                        break;
                    case 4:
                        value += 0.08;
                        break;
                    case 5:
                        value += 0.12;
                        break;
                }
                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.MOTH_FORMATION:
                        return 0.2; // 0.2
                    case DroneManager.DOUBLE_ARROW_FORMATION:
                        return 0.1;
                    case DroneManager.PINCER_FORMATION:
                        return -0.1;
                    default:
                        return 0;
                }
            }
        }

        public override int Damage
        {
            get
            {
                int value = CurrentConfig == 1 ? Equipment.Configs.Config1Damage : Equipment.Configs.Config2Damage;

                if (LeonovEffect) value += Maths.GetPercentage(value, 30);

                value += Maths.GetPercentage(value, BoosterManager.GetPercentage(BoostedAttributeType.DAMAGE));

                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.DOME_FORMATION:
                        value -= Maths.GetPercentage(value, 50);
                        break;
                    case DroneManager.TURTLE_FORMATION:
                        value -= Maths.GetPercentage(value, (int)7.5);
                        break;
                    case DroneManager.ARROW_FORMATION:
                        value -= Maths.GetPercentage(value, 3);
                        break;
                    case DroneManager.PINCER_FORMATION:
                        value += Maths.GetPercentage(value, 3);
                        break;
                    case DroneManager.HEART_FORMATION:
                        value -= Maths.GetPercentage(value, 5);
                        break;
                    case DroneManager.RING_FORMATION:
                        value -= Maths.GetPercentage(value, 25);
                        break;
                    case DroneManager.DRILL_FORMATION:
                        value += Maths.GetPercentage(value, 20);
                        break;
                    case DroneManager.WHEEL_FORMATION:
                        value -= Maths.GetPercentage(value, 20);
                        break;
                    case DroneManager.VETERAN_FORMATION:
                        value -= Maths.GetPercentage(value, 20);
                        break;
                }

                value = Ship.GetLaserDamageBoost(value, FactionId, (Selected != null ? Selected.FactionId : 0));

                //admin command to add damage on top
                value += Storage.DamageBoost;

                //value += Maths.GetPercentage(value, DroneManager.droneLevel(droneExp) / 2);
                return value;
            }
        }

        public int GetHonorBoost(int honor)
        {
            switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
            {
                case DroneManager.PINCER_FORMATION:
                    return honor += Maths.GetPercentage(honor, 5);
                case DroneManager.VETERAN_FORMATION:
                    return honor += Maths.GetPercentage(honor, 20);
                case DroneManager.X_FORMATION:
                    return 0;
                default:
                    return honor;
            }
        }

        public override int RocketDamage
        {
            get
            {
                int value = AttackManager.GetRocketDamage();

                if (LeonovEffect) value *= 2;

                value += Maths.GetPercentage(value, GetSkillPercentage("Rocket Fusion"));

                switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
                {
                    case DroneManager.TURTLE_FORMATION:
                        value -= Maths.GetPercentage(value, (int)7.5);
                        break;
                    case DroneManager.ARROW_FORMATION:
                        value += Maths.GetPercentage(value, 20);
                        break;
                    case DroneManager.STAR_FORMATION:
                        value += Maths.GetPercentage(value, 25);
                        break;
                    case DroneManager.DOUBLE_ARROW_FORMATION:
                        value += Maths.GetPercentage(value, 30);
                        break;
                    case DroneManager.CHEVRON_FORMATION:
                        value += Maths.GetPercentage(value, 65);
                        break;
                }

                value += (int)(value * 1);
                return value;
            }
        }

        public double RocketMissProbability
        {
            get
            {
                double value = 0.1;
                value -= Maths.GetDoublePercentage(value, GetSkillPercentage("Heat-seeking Missiles"));

                if (Storage.PrecisionTargeter)
                {
                    value = 0;
                }

                return value;
            }
        }
        public int GetExperienceBoost(int experience)
        {
            switch (SettingsManager.Player.Settings.InGameSettings.selectedFormation)
            {
                case DroneManager.BAT_FORMATION:
                    return experience += Maths.GetPercentage(experience, 8);
                case DroneManager.BARRAGE_FORMATION:
                    return experience += Maths.GetPercentage(experience, 5);
                case DroneManager.X_FORMATION:
                    return experience += Maths.GetPercentage(experience, 5);
                default:
                    return experience;
            }
        }

        public bool UpdateActivatable(Activatable pEntity, bool pInRange)
        {
            if (Storage.InRangeAssets.ContainsKey(pEntity.Id))
            {
                if (!pInRange)
                {
                    if (pEntity is Portal portal && portal.Working)
                    {
                        CurrentInRangePortalId = -1;
                    }

                    Storage.InRangeAssets.TryRemove(pEntity.Id, out pEntity);
                    return true;
                }
            }
            else
            {
                if (pInRange)
                {
                    if (pEntity is Portal portal && portal.Working)
                    {
                        CurrentInRangePortalId = pEntity.Id;
                    }

                    Storage.InRangeAssets.TryAdd(pEntity.Id, pEntity);
                    return true;
                }
            }
            return false;
        }

        public DateTime ConfigCooldown = new DateTime();
        public void ChangeConfiguration(string LootID)
        {
            if (ConfigCooldown.AddSeconds(5) < DateTime.Now || Storage.GodMode)
            {
                
                SendPacket("0|S|CFG|" + LootID);
                SetCurrentConfiguration(Convert.ToInt32(LootID));
                ConfigCooldown = DateTime.Now;
                //Movement.ActualPosition(this);
                //UpdateStatus();
                
            }
            else
            {
                SendPacket("0|A|STM|config_change_failed_time");
            }
        }

        public void SetCurrentConfiguration(int pCurrentConfiguration)
        {
            CurrentConfig = Convert.ToInt32(pCurrentConfiguration);
            Settings.InGameSettings.currentConfig = CurrentConfig;
            DroneManager.UpdateDrones(droneExp);
            UpdateStatus();

            if(this.Moving)
            {
                Position destination = this.Destination;
                Movement.Move(this, destination);
            }
        }

        public void SetTitle(string title, bool permanent = false)
        {
            Title = title;
            string packet = Title != "" ? $"0|n|t|{Id}|1|{Title}" : $"0|n|trm|{Id}";
            SendPacket(packet);
            SendPacketToInRangePlayers(packet);

            if (permanent)
            {
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET title = '{Title}' WHERE userId = {Id}");
                }
            }
        }

        public byte[] GetBeaconCommand()
        {
            return BeaconCommand.write(1, 1, 1, 1, Storage.IsInDemilitarizedZone, Storage.RepairBotActivated, (SkillTree.engineering == 5),
                         "equipment_extra_repbot_rep-4", Storage.IsInRadiationZone);
        }

        public byte[] GetShipCreateCommand(Player otherPlayer, short relationType)
        {
            return ShipCreateCommand.write(
                Id,
                Ship.LootId,
                3,
                !EventManager.JackpotBattle.InEvent(this) ? Clan.Tag : "",
                !EventManager.JackpotBattle.InEvent(this) ? (otherPlayer.RankId == 22 ? $"{Name}" : Name) : EventManager.JackpotBattle.Name,
                Position.X,
                Position.Y,
                !EventManager.JackpotBattle.InEvent(this) ? FactionId : 4,
                !EventManager.JackpotBattle.InEvent(this) ? Clan.Id : 0,
                RankId,
                false,
                new ClanRelationModule(!EventManager.JackpotBattle.InEvent(this) ? relationType : ClanRelationModule.AT_WAR),
                GetRingsCount(),
                false,
                false,
                Invisible,
                !EventManager.JackpotBattle.InEvent(this) ? relationType : ClanRelationModule.NONE,
                !EventManager.JackpotBattle.InEvent(this) ? relationType : ClanRelationModule.NONE,
                VisualModifiers.Values.ToList(),
                new class_11d(class_11d.DEFAULT));
        }

        public byte[] GetShipInitializationCommand()
        {
            return ShipInitializationCommand.write(
                Id,
                Name,
                Ship.LootId,
                Speed,
                CurrentShieldPoints,
                MaxShieldPoints,
                CurrentHitPoints,
                MaxHitPoints,
                0,
                0,
                CurrentNanoHull,
                MaxNanoHull,
                Position.X,
                Position.Y,
                Spacemap.Id,
                FactionId,
                Clan.Id,
                3,
                Premium,
                Data.experience,
                Data.honor,
                (short)Level,
                Data.credits,
                Data.uridium,
                0,
                RankId,
                Clan.Tag,
                GetRingsCount(),
                true,
                Invisible,
                true,
                VisualModifiers.Values.ToList());
        }

        public int GetPlayerId()
        {
            return playerId;
        }
        public int GetPlayerActiveMap()
        {
            return activeMapId;
        }

        public string GetPlayerActiveGate()
        {
            return playerActiveGate;
        }

        public int GetRingsCount()
        {//

            return RankId >= 1 ? 100 : RankId >= 14 ? 63 : RankId >= 12 ? 31 : RankId >= 8 ? 15 : RankId >= 5 ? 7 : RankId >= 1 ? 3 : RankId < 0 ? 0 : 0;
        }

        public bool Attackable()
        {
            return (AttackManager.IshCooldown.AddMilliseconds(TimeManager.ISH_DURATION) > DateTime.Now || Invincible || Storage.GodMode) ? false : true;
        }

        public void ResetCooldown()
        {
            Player p = this;

            p.SendCooldown(AmmunitionManager.ISH_01, 0);
            p.SendCooldown(AmmunitionManager.SMB_01, 0);
            p.SendCooldown(AmmunitionManager.EMP_01, 0);
            p.SendCooldown(AmmunitionManager.PLD_8, 0);
            p.SendCooldown(AmmunitionManager.DCR_250, 0);
            p.SendCooldown(AmmunitionManager.R_IC3, 0);
            p.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 0);
            p.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, 0);
            p.SendCooldown(TechManager.TECH_CHAIN_IMPULSE, 0);
            //p.SendCooldown(TechManager.TECH_ENERGY_LEECH, 0);
            //p.SendCooldown(TechManager.TECH_PRECISION_TARGETER, 0);
            p.AttackManager.IshCooldown = new DateTime();
            p.AttackManager.SmbCooldown = new DateTime();
            p.AttackManager.EmpCooldown = new DateTime();
            p.AttackManager.pld8Cooldown = new DateTime();
            p.AttackManager.dcr_250Cooldown = new DateTime();
            p.AttackManager.r_ic3Cooldown = new DateTime();
            p.TechManager.BattleRepairBot.cooldown = new DateTime();
            //p.TechManager.EnergyLeech.cooldown = new DateTime();
            p.TechManager.BackupShields.cooldown = new DateTime();
            p.TechManager.ChainImpulse.cooldown = new DateTime();
            //p.TechManager.PrecisionTargeter.cooldown = new DateTime();
        }

        public void SendCooldown(string itemId, int time, bool countdown = false)
        {
            SendCommand(UpdateMenuItemCooldownGroupTimerCommand.write(
            SettingsManager.GetCooldownType(itemId),
            new ClientUISlotBarCategoryItemTimerStateModule(countdown ? ClientUISlotBarCategoryItemTimerStateModule.ACTIVE : ClientUISlotBarCategoryItemTimerStateModule.short_2168), time, time));
        }

        public void UpdateCurrentCooldowns()
        {
            Settings.Cooldowns[AmmunitionManager.SMB_01] = AttackManager.SmbCooldown.ToString("yyyy-MM-dd HH:mm:ss");
            Settings.Cooldowns[AmmunitionManager.ISH_01] = AttackManager.IshCooldown.ToString("yyyy-MM-dd HH:mm:ss");
            Settings.Cooldowns[AmmunitionManager.EMP_01] = AttackManager.EmpCooldown.ToString("yyyy-MM-dd HH:mm:ss");
            Settings.Cooldowns["ammunition_mine"] = AttackManager.mineCooldown.ToString("yyyy-MM-dd HH:mm:ss");
            Settings.Cooldowns[AmmunitionManager.DCR_250] = AttackManager.dcr_250Cooldown.ToString("yyyy-MM-dd HH:mm:ss");
            Settings.Cooldowns[AmmunitionManager.PLD_8] = AttackManager.pld8Cooldown.ToString("yyyy-MM-dd HH:mm:ss");
            Settings.Cooldowns[AmmunitionManager.R_IC3] = AttackManager.r_ic3Cooldown.ToString("yyyy-MM-dd HH:mm:ss");

            foreach (Players.Skills.Skill skill in Storage.Skills.Values)
            {
                Settings.Cooldowns[skill.LootId] = Storage.Skills[skill.LootId].cooldown.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public void SetCurrentCooldowns()
        {
            if (Settings.Cooldowns[AmmunitionManager.SMB_01] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[AmmunitionManager.SMB_01]))).TotalSeconds;
                AttackManager.SmbCooldown = DateTime.Now.AddSeconds(-seconds);
            }

            if (Settings.Cooldowns[AmmunitionManager.ISH_01] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[AmmunitionManager.ISH_01]))).TotalSeconds;
                AttackManager.IshCooldown = DateTime.Now.AddSeconds(-seconds);
            }
            if (Settings.Cooldowns[AmmunitionManager.ISH_01] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[AmmunitionManager.ISH_01]))).TotalSeconds;
                AttackManager.IshCooldownSkill = DateTime.Now.AddSeconds(-seconds);
            }

            if (Settings.Cooldowns[AmmunitionManager.EMP_01] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[AmmunitionManager.EMP_01]))).TotalSeconds;
                AttackManager.EmpCooldown = DateTime.Now.AddSeconds(-seconds);
            }

            if (Settings.Cooldowns["ammunition_mine"] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns["ammunition_mine"]))).TotalSeconds;
                AttackManager.mineCooldown = DateTime.Now.AddSeconds(-seconds);
            }


            if (Settings.Cooldowns[AmmunitionManager.PLD_8] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[AmmunitionManager.PLD_8]))).TotalSeconds;
                AttackManager.pld8Cooldown = DateTime.Now.AddSeconds(-seconds);
            }
            if (Settings.Cooldowns[AmmunitionManager.DCR_250] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[AmmunitionManager.DCR_250]))).TotalSeconds;
                AttackManager.dcr_250Cooldown = DateTime.Now.AddSeconds(-seconds);
            }

            if (Settings.Cooldowns[AmmunitionManager.R_IC3] != "")
            {
                var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[AmmunitionManager.R_IC3]))).TotalSeconds;
                AttackManager.r_ic3Cooldown = DateTime.Now.AddSeconds(-seconds);
            }

            foreach (var skill in Storage.Skills.Values)
            {
                if (Settings.Cooldowns.ContainsKey(skill.LootId))
                {
                    var seconds = (int)(DateTime.Now.Subtract(DateTime.Parse(Settings.Cooldowns[skill.LootId]))).TotalSeconds;
                    skill.cooldown = DateTime.Now.AddSeconds(-seconds);
                }
            }
        }

        public void GetLeonovEffect()
        {
            Player player = this;

            if (((player.FactionId == 1 && (player.Spacemap.Id == 1 || player.Spacemap.Id == 2 || player.Spacemap.Id == 3 || player.Spacemap.Id == 4)) ||
                    (player.FactionId == 2 && (player.Spacemap.Id == 5 || player.Spacemap.Id == 6 || player.Spacemap.Id == 7 || player.Spacemap.Id == 8)) ||
                    (player.FactionId == 3 && (player.Spacemap.Id == 9 || player.Spacemap.Id == 10 || player.Spacemap.Id == 11 || player.Spacemap.Id == 12))) && player.Ship.Id == 3)
            {
                player.AddVisualModifier(VisualModifierCommand.LEONOV_EFFECT, 0, "", 0, true);
                player.LeonovEffect = true;
            }
            else
            {
                player.RemoveVisualModifier(VisualModifierCommand.LEONOV_EFFECT);
                player.LeonovEffect = false;
            }
        }

        public void SelectEntity(int entityId)
        {
            
            if (AttackManager.Attacking)
            {
                DisableAttack(SettingsManager.Player.Settings.InGameSettings.selectedLaser);
            }
            

            try
            {
                if (InRangeCharacters.ContainsKey(entityId))
                {
                    Character character = InRangeCharacters.Values.Where(x => x.Id == entityId).FirstOrDefault();

                    if (character != null && !character.Destroyed)
                    {
                        if (character is Player player && (player.AttackManager.EmpCooldown.AddMilliseconds(TimeManager.EMP_DURATION) > DateTime.Now))
                        {
                            return;
                        }
                        if (character.Position.DistanceTo(this.Position) > 1500)
                        {
                            return;
                        }

                        Selected = character;

                        bool shieldSkill = character is Player ? true : false;
                        if(character is Player)
                        {
                            Player tmp = (Player)character;
                            shieldSkill = tmp.SkillTree.shieldengineering == 5;
                        }

                        SendCommand(ShipSelectionCommand.write(
                            character.Id,
                            character.Ship.Id,
                            character.CurrentShieldPoints,
                            character.MaxShieldPoints,
                            character.CurrentHitPoints,
                            character.MaxHitPoints,
                            character.CurrentNanoHull,
                            character.MaxNanoHull,
                            shieldSkill));
                    }
                }
                else if (Storage.InRangeAssets.ContainsKey(entityId))
                {
                    Activatable asset = Storage.InRangeAssets.Values.Where(x => x.Id == entityId).FirstOrDefault();

                    if (asset != null && (asset is BattleStation || asset is Satellite) && !asset.Destroyed)
                    {
                        Selected = asset;

                        SendCommand(AssetInfoCommand.write(
                            asset.Id,
                            asset.GetAssetType(),
                            asset is Satellite satellite ? satellite.DesignId : 0,
                            3,
                            asset.CurrentHitPoints,
                            asset.MaxHitPoints,
                            asset.MaxShieldPoints > 0 ? true : false,
                            asset.CurrentShieldPoints,
                            asset.MaxShieldPoints
                            ));
                    }
                }

                if (Selected != null)
                {
                    Group?.UpdateTarget(this, new List<command_i3O> { new GroupPlayerTargetModule(new GroupPlayerShipModule(Selected is Player player ? player.Ship.GroupShipId : GroupPlayerShipModule.WRECK), Selected.Name, new GroupPlayerInformationsModule(Selected.CurrentHitPoints, Selected.MaxHitPoints, Selected.CurrentShieldPoints, Selected.MaxShieldPoints, Selected.CurrentNanoHull, Selected.MaxNanoHull)) });
                }
            }
            catch (Exception e)
            {
                Out.WriteLine("SelectEntity void exception " + e, "Player.cs");
                Logger.Log("error_log", $"- [Player.cs] SelectEntity void exception: {e}");
            }
        }

        public void ChangeShip(int shipId)
        {
            SkillManager.DisableAllSkills();
            Ship = GameManager.GetShip(shipId);
            QueryManager.SetEquipment(this);
            SkillManager.InitiateSkills(true);
            LastCombatTime = DateTime.Now.AddSeconds(-999);
            Spacemap.RemoveCharacter(this);
            CurrentInRangePortalId = -1;
            Deselection();
            Storage.InRangeAssets.Clear();
            InRangeCharacters.Clear();

            Spacemap.AddAndInitPlayer(this);
            UpdateStatus();
        }

        public async void Jump(int mapId, Position targetPosition)
        {
            Storage.Jumping = true;
            await Task.Delay(Portal.JUMP_DELAY);

            LastCombatTime = DateTime.Now.AddSeconds(-999);
            Spacemap.RemoveCharacter(this);
            CurrentInRangePortalId = -1;
            Deselection();
            Storage.InRangeAssets.Clear();
            InRangeCharacters.Clear();
            SetPosition(targetPosition);

            Spacemap = GameManager.GetSpacemap(mapId);

            Spacemap.AddAndInitPlayer(this);
            Storage.Jumping = false;

            if(GetPlayerActiveMap().ToString().StartsWith("88") && Storage.ubal != null)
            {
                Storage.ubal.playersSpawnedOnMap++;

                EventManager.UltimateBattleArena.ActualizePlayersSub(Storage.ubal.players[0], Storage.ubal.players[1]);
            }

            //QueryManager.SavePlayer.Information(this);
        }

        public void KillScreen(Attackable killerEntity, DestructionType destructionType, bool killedLogin = false)
        {
            List<KillScreenOptionModule> killScreenOptionModules = new List<KillScreenOptionModule>();
            KillScreenOptionModule basicRepair =
                   new KillScreenOptionModule(new KillScreenOptionTypeModule(KillScreenOptionTypeModule.BASIC_REPAIR),
                                              new PriceModule(PriceModule.URIDIUM, 0), true, 0,
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()));

            int portalRepairTime = (int)(60 - ((DateTime.Now - Storage.KillscreenPortalRepairTime).TotalSeconds));
            int portalRepairPrice = 2500;
            KillScreenOptionModule portalRepair =
                  new KillScreenOptionModule(new KillScreenOptionTypeModule(KillScreenOptionTypeModule.AT_JUMPGATE_REPAIR),
                                             new PriceModule(PriceModule.URIDIUM, portalRepairPrice), Data.uridium >= portalRepairPrice, portalRepairTime,
                                             new MessageLocalizedWildcardCommand("desc_killscreen_repair_gate", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule> { new MessageWildcardReplacementModule("%COUNT%", portalRepairPrice.ToString(), new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED)) }),
                                             new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                             new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                             new MessageLocalizedWildcardCommand(Data.uridium >= portalRepairPrice ? "btn_killscreen_repair_for_uri" : "btn_killscreen_payment", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule> { new MessageWildcardReplacementModule("%COUNT%", portalRepairPrice.ToString(), new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED)) }));

            /*int deathLocationRepairTime = (int)(30 - ((DateTime.Now - Storage.KillscreenDeathLocationRepairTime).TotalSeconds));
            int deathLocationRepairPrice = 2500;
            KillScreenOptionModule deathLocationRepair =
                  new KillScreenOptionModule(new KillScreenOptionTypeModule(KillScreenOptionTypeModule.AT_DEATHLOCATION_REPAIR),
                                             new PriceModule(PriceModule.URIDIUM, deathLocationRepairPrice), Data.uridium >= deathLocationRepairPrice, deathLocationRepairTime,
                                             new MessageLocalizedWildcardCommand("desc_killscreen_repair_location", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule> { new MessageWildcardReplacementModule("%COUNT%", deathLocationRepairPrice.ToString(), new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED)) }),
                                             new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                             new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                             new MessageLocalizedWildcardCommand(Data.uridium >= deathLocationRepairPrice ? "btn_killscreen_repair_for_uri" : "btn_killscreen_payment", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule> { new MessageWildcardReplacementModule("%COUNT%", deathLocationRepairPrice.ToString(), new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED)) }));*/

            KillScreenOptionModule fullRepair =
                   new KillScreenOptionModule(new KillScreenOptionTypeModule(KillScreenOptionTypeModule.BASIC_FULL_REPAIR),
                                              new PriceModule(PriceModule.URIDIUM, 0), true, 0,
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()),
                                              new MessageLocalizedWildcardCommand("btn_killscreen_repair_for_free", new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), new List<MessageWildcardReplacementModule>()));
            killScreenOptionModules.Add(basicRepair);

            if (!killedLogin)
            {
                if (Spacemap.Activatables.FirstOrDefault(x => x.Value is Portal).Value is Portal portal && portal.Working && Data.uridium >= portalRepairPrice)
                {
                    killScreenOptionModules.Add(portalRepair);
                }

                /*if (Spacemap.Options.DeathLocationRepair && Data.uridium >= deathLocationRepairPrice)
                {
                    killScreenOptionModules.Add(deathLocationRepair);
                }*/

                //killScreenOptionModules.Add(fullRepair);
            }

            byte[] killScreenPostCommand =
                    KillScreenPostCommand.write(killerEntity != null ? killerEntity.Name : "", "http://127.0.0.1",
                                              "MISC", new DestructionTypeModule((short)destructionType),
                                              killScreenOptionModules);

            SendCommand(killScreenPostCommand);
        }

        public void Respawn(bool basicRepair = false, bool deathLocation = false, bool atNearestPortal = false, bool fullRepair = false)
        {
            LastCombatTime = DateTime.Now.AddSeconds(-999);
            //isInMapPremium = false;
            DestroyedOnSpacemap = Spacemap.Id;

            
            AddVisualModifier(VisualModifierCommand.INVINCIBILITY, 0, "", 0, true);

            Storage.IsInDemilitarizedZone = basicRepair || fullRepair ? true : false;
            Storage.IsInEquipZone = basicRepair || fullRepair ? true : false;
            Storage.IsInRadiationZone = false;

            if (atNearestPortal)
            {
                SetPosition(GetNearestPortalPosition());
                CurrentHitPoints = Maths.GetPercentage(MaxHitPoints, 15);
                CurrentShieldConfig1 = Maths.GetPercentage(MaxShieldPoints, 15);
                CurrentShieldConfig2 = Maths.GetPercentage(MaxShieldPoints, 15);
            }
            else if (deathLocation)
            {
                CurrentHitPoints = Maths.GetPercentage(MaxHitPoints, 15);
                CurrentShieldConfig1 = Maths.GetPercentage(MaxShieldPoints, 15);
                CurrentShieldConfig2 = Maths.GetPercentage(MaxShieldPoints, 15);
            }
            else
            {
                CurrentHitPoints = Maths.GetPercentage(MaxHitPoints, 15);
                CurrentShieldConfig1 = Maths.GetPercentage(MaxShieldPoints, 15);
                CurrentShieldConfig2 = Maths.GetPercentage(MaxShieldPoints, 15);

                SetPosition(GetBasePosition());
            }

            if (basicRepair || fullRepair)
            {
                
                Spacemap = GameManager.GetSpacemap(GetBaseMapId());
            }

            if (fullRepair)
            {
                CurrentHitPoints = MaxHitPoints;
                CurrentShieldConfig1 = MaxShieldPoints;
                CurrentShieldConfig2 = MaxShieldPoints;
            }

            Spacemap.AddAndInitPlayer(this, Destroyed);

            Group?.UpdateTarget(this, new List<command_i3O> { new GroupPlayerDisconnectedModule(false) });

            Destroyed = false;
        }

        public int GetBaseMapId(bool gate = false)
        {
            
            if(gate || activeMapId.ToString().StartsWith("99") || activeMapId.ToString().StartsWith("88"))
            {
                return FactionId == 1 ? 1 : FactionId == 2 ? 5 : 9;
            }
            if (Spacemap == null)
            {
                return FactionId == 1 ? 1 : FactionId == 2 ? 5 : 9;
            }

            if (Spacemap.Id >= 16 && Spacemap.Id <= 29 && Level >= 12)
            {
                return FactionId == 1 ? 20 : FactionId == 2 ? 24 : 28;
            }
            if (Spacemap.Id <= 16)
            {
                return FactionId == 1 ? 1 : FactionId == 2 ? 5 : 9;
            }
            return FactionId == 1 ? 1 : FactionId == 2 ? 5 : 9;
        }

        public Position GetBasePosition(bool gate = false)
        {
                if (gate || activeMapId.ToString().StartsWith("99") || activeMapId.ToString().StartsWith("88")) return FactionId == 1 ? Position.MMOPosition : FactionId == 2 ? Position.EICPosition : Position.VRUPosition;
                if (Spacemap == null)
                {
                    return FactionId == 1 ? Position.newMMOPosition : FactionId == 2 ? Position.newEICPosition : Position.newVRUPosition;
                }
                if (Spacemap == null && TdmDestroyed == true)
                {
                    return FactionId == 1 ? Position.TDMMMO : FactionId == 2 ? Position.TDMEIC : Position.TDMVRU;
                }
                if (Spacemap.Id >= 16 && Spacemap.Id <= 29 )
                {
                    return FactionId == 1 ? Position.newMMOPosition : FactionId == 2 ? Position.newEICPosition : Position.newVRUPosition;
                }
                if (Spacemap.Id <= 16)
                {
                    return FactionId == 1 ? Position.MMOPosition : FactionId == 2 ? Position.EICPosition : Position.VRUPosition;
                }
                if (Spacemap.Id == 41)
                {
                    return FactionId == 1 ? Position.TDMMMO : FactionId == 2 ? Position.TDMEIC : Position.TDMVRU;
                }
                return FactionId == 1 ? Position.MMOPosition : FactionId == 2 ? Position.EICPosition : Position.VRUPosition;
            
           
        }
        
        public void LoadData()
        {
            try
            {
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    System.Data.DataRow result = mySqlClient.ExecuteQueryRow($"SELECT data, premium, premiumUntil FROM player_accounts WHERE userId = {Id}");
                    Data = JsonConvert.DeserializeObject<DataBase>(result["data"].ToString());

                    if (premiumOld == null)
                    {
                        premiumOld = result["premium"].ToString();
                    }

                    if ((bool)result["premium"] && result["premiumUntil"].ToString() != "")
                    {
                        DateTime premiumUntil = DateTime.Parse(result["premiumUntil"].ToString());
                        if (DateTime.Now >= premiumUntil)
                        {
                            mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET premium = 0, premiumUntil = NULL WHERE userId = {Id}");
                            Premium = false;
                        }
                    }

                    if (result["premium"].ToString() != premiumOld)
                    {
                        if ((bool)result["premium"])
                        {
                            SendPacket($"0|A|STD|Premium activated");
                            SendCommand(PremiumStateCommand.write(Premium));
                        }
                        else
                        {
                            SendPacket($"0|A|STD|Premium deactivated");
                            SendCommand(PremiumStateCommand.write(Premium));
                        }
                    }

                    premiumOld = result["premium"].ToString();
                }
            } catch(Exception ex)
            {
            }
        }

        public void AddNpcToDatabase(int npc)
        {
            try
            {
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var query = $"SELECT npc FROM log_player_pve_kills WHERE userId = {this.Id} AND npc = {npc}";

                    var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                    if (result.Rows.Count >= 1)
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE log_player_pve_kills SET amount = amount+1 WHERE userId = {this.Id} AND npc = {npc}");
                    }
                    else
                    {
                        mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_pve_kills (userId, npc, amount) VALUES ({this.Id}, {npc}, 1)");
                    }
                }
            } catch(Exception ex) { }
        }

        public void AddShipToDatabase(int npc)
        {
            try {
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var query = $"SELECT ship FROM log_player_pvp_kills WHERE userId = {this.Id} AND ship = {npc}";

                    var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                    if (result.Rows.Count >= 1)
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE log_player_pvp_kills SET amount = amount+1 WHERE userId = {this.Id} AND ship = {npc}");
                    }
                    else
                    {
                        mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_pvp_kills (userId, ship, amount) VALUES ({this.Id}, {npc}, 1)");
                    }
                }
            }
            catch (Exception ex) { }
        }

        public void AddLogfiles(int amount)
        {
            using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
            {
                var query = $"SELECT items FROM player_equipment WHERE userId = {this.Id}";

                var result = (DataTable)mySqlClient.ExecuteQueryTable(query);
                if (result.Rows.Count >= 1)
                {
                    dynamic items = null;

                    foreach (DataRow row in result.Rows)
                    {
                        items = JsonConvert.DeserializeObject(row["items"].ToString());
                    }

                    items["skillTree"]["logdisks"] += amount;

                    items = JsonConvert.SerializeObject(items);

                    mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET items = '{items}' WHERE userId = {this.Id}");
                }
            }
        }

        public void ChangeData(DataType dataType, int amount, ChangeType changeType = ChangeType.INCREASE)
        {
            if (amount == 0)
            {
                return;
            }

            amount = Convert.ToInt32(amount);

            switch (dataType)
            {
                case DataType.URIDIUM:
                    Data.uridium = (changeType == ChangeType.INCREASE ? (Data.uridium + amount) : (Data.uridium - amount));
                    if (Data.uridium < 0)
                    {
                        Data.uridium = 0;
                    }
                    SendPacket($"0|LM|ST|URI|{(changeType == ChangeType.DECREASE ? "-" : "")}{amount}|{Data.uridium}");
                    break;
                case DataType.CREDITS:
                    Data.credits = (changeType == ChangeType.INCREASE ? (Data.credits + amount) : (Data.credits - amount));
                    if (Data.credits < 0)
                    {
                        Data.credits = 0;
                    }

                    SendPacket($"0|LM|ST|CRE|{(changeType == ChangeType.DECREASE ? "-" : "")}{amount}|{Data.credits}");
                    break;
                case DataType.HONOR:
                    Data.honor = (changeType == ChangeType.INCREASE ? (Data.honor + amount) : (Data.honor - amount));
                    if (Data.honor < 0)
                    {
                        Data.honor = 0;
                    }

                    SendPacket($"0|LM|ST|HON|{(changeType == ChangeType.DECREASE ? "-" : "")}{amount}|{Data.honor}");
                    break;
                case DataType.EXPERIENCE:
                    Data.experience = (changeType == ChangeType.INCREASE ? (Data.experience + amount) : (Data.experience - amount));
                    if (Data.experience < 0)
                    {
                        Data.experience = 0;
                    }

                    SendPacket($"0|LM|ST|EP|{(changeType == ChangeType.DECREASE ? "-" : "")}{amount}|{Data.experience}|{Level}");
                    CheckNextLevel(Data.experience);
                    break;
                case DataType.JACKPOT:
                    break;
                case DataType.EC:
                    ec = (changeType == ChangeType.INCREASE ? (ec + amount) : (ec - amount));
                    if (ec < 0)
                    {
                        ec = 0;
                    }

                    if (changeType == ChangeType.INCREASE)
                    {
                        SendPacket($"0|A|STD|You received {String.Format("{0:n0}", amount)} E.C.");
                    }
                    else
                    {
                        SendPacket($"0|A|STD|You have spent {String.Format("{0:n0}", amount)} E.C.");
                    }

                    break;
            }

            QueryManager.SavePlayer.Information(this);
            QueryManager.SavePlayer.saveEC(this);
        }

        public void SetData(DataType dataType, int amount)
        {
            if (amount == 0)
            {
                return;
            }

            amount = Convert.ToInt32(amount);

            using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
            {
                System.Data.DataRow result = mySqlClient.ExecuteQueryRow($"SELECT data FROM player_accounts WHERE userId = {Id}");
                Data = JsonConvert.DeserializeObject<DataBase>(result["data"].ToString());
            }

            switch (dataType)
            {
                case DataType.URIDIUM:
                    Data.uridium = amount;

                    if (Data.uridium < 0)
                    {
                        Data.uridium = 0;
                    }

                    SendPacket($"0|LM|ST|URI|{""}{amount}|{Data.uridium}");
                    break;
                case DataType.CREDITS:
                    Data.credits = amount;

                    if (Data.credits < 0)
                    {
                        Data.credits = 0;
                    }

                    SendPacket($"0|LM|ST|CRE|{""}{amount}|{Data.credits}");
                    break;
                case DataType.HONOR:
                    Data.honor = amount;

                    if (Data.honor < 0)
                    {
                        Data.honor = 0;
                    }

                    SendPacket($"0|LM|ST|HON|{""}{amount}|{Data.honor}");
                    break;
                case DataType.EXPERIENCE:
                    Data.experience = amount;

                    if (Data.experience < 0)
                    {
                        Data.experience = 0;
                    }

                    SendPacket($"0|LM|ST|EP|{""}{amount}|{Data.experience}|{Level}");
                    CheckNextLevel(Data.experience);
                    break;
                case DataType.JACKPOT:
                    break;
            }

            QueryManager.SavePlayer.Information(this);
        }

        public void CheckNextLevel(long experience)
        {
            short lvl = 1;
            long expNext = 10000;

            while (experience >= expNext)
            {
                expNext *= 2;
                lvl++;
            }

            if (lvl >= NLevel)
            {
                SendPacket($"0|{ServerCommands.SET_ATTRIBUTE}|{ServerCommands.LEVEL_UPDATE}|{lvl}|{expNext - experience}");
                byte[] levelUpCommand = LevelUpCommand.write(Id, lvl);
                SendCommandToInRangePlayers(levelUpCommand);
                NLevel = lvl + 1;
            }

        }

        public bool AttackingOrUnderAttack(int combatSecond = 10)
        {
            if (LastCombatTime.AddSeconds(combatSecond) > DateTime.Now) return true;
            if (LastAttackTime(combatSecond)) return true;
            return false;
        }

        public bool LastAttackTime(int combatSecond = 10)
        {
            if (AttackManager.lastAttackTime.AddSeconds(combatSecond) > DateTime.Now) return true;
            else
            {
                return false;
            }
            if (AttackManager.lastRSBAttackTime.AddSeconds(combatSecond) > DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }

            if (AttackManager.lastRocketAttack.AddSeconds(combatSecond) > DateTime.Now) return true;
            else
            {
                return false;
            }
        }

        public bool LastAttackTimeInvasion(int combatSecond = 10)
        {
            DateTime lastAttack = AttackManager.lastAttackTime;
            if (AttackManager.lastRSBAttackTime > lastAttack) lastAttack = AttackManager.lastRSBAttackTime;
            else if (AttackManager.lastRocketAttack > lastAttack) lastAttack = AttackManager.lastRocketAttack;

            if ((int)(DateTime.Now - lastAttack).TotalSeconds == combatSecond) return true;
            //if ((int)(DateTime.Now - AttackManager.lastRSBAttackTime).TotalSeconds == combatSecond) return true;
            //if ((int)(DateTime.Now - AttackManager.lastRocketAttack).TotalSeconds == combatSecond) return true;
            return false;
        }

        public void EnableAttack(string itemId)
        {
            AttackManager.Attacking = true;
            //SendCommand(AddMenuItemHighlightCommand.write(new class_h2P(class_h2P.ITEMS_CONTROL), itemId, new class_K18(class_K18.ACTIVE), new class_I1W(false, 0)));
        }

        public void DisableAttack(string itemId)
        {
            AttackManager.Attacking = false;
            //SendCommand(RemoveMenuItemHighlightCommand.write(new class_h2P(class_h2P.ITEMS_CONTROL), itemId, new class_K18(class_K18.ACTIVE)));

            SendCommand(AttackLaserRunSlotCommand.write(Id));
            SendCommandToInRangePlayers(AttackLaserRunSlotCommand.write(Id));

            //TODO give user free
            /*var tmp = Selected;
            Selected = null;
            SendCommand(ShipDeselectionCommand.write());
            SelectEntity(tmp.Id);*/
        }

        public Position GetNearestPortalPosition()
        {
            IOrderedEnumerable<Activatable> activatablesOrdered = Spacemap.Activatables.Values.OrderBy(x => x.Position.DistanceTo(Position));
            Activatable nearestPortal = activatablesOrdered.FirstOrDefault(x => x is Portal);

            return nearestPortal.Position;
        }

        public void SaveSettings()
        {
            QueryManager.SavePlayer.Settings(this, "audio", Settings.Audio);
            QueryManager.SavePlayer.Settings(this, "quality", Settings.Quality);
            QueryManager.SavePlayer.Settings(this, "classY2T", Settings.ClassY2T);
            QueryManager.SavePlayer.Settings(this, "display", Settings.Display);
            QueryManager.SavePlayer.Settings(this, "gameplay", Settings.Gameplay);
            QueryManager.SavePlayer.Settings(this, "window", Settings.Window);
            QueryManager.SavePlayer.Settings(this, "boundKeys", Settings.BoundKeys);
            QueryManager.SavePlayer.Settings(this, "inGameSettings", Settings.InGameSettings);
            QueryManager.SavePlayer.Settings(this, "cooldowns", Settings.Cooldowns);
            QueryManager.SavePlayer.Settings(this, "slotbarItems", Settings.SlotBarItems);
            QueryManager.SavePlayer.Settings(this, "premiumSlotbarItems", Settings.PremiumSlotBarItems);
            QueryManager.SavePlayer.Settings(this, "proActionBarItems", Settings.ProActionBarItems);
        }

        public void SendPacket(string packet)
        {
            try
            {
                GameSession gameSession = GameManager.GetGameSession(Id);

                if (gameSession == null)
                {
                    return;
                }

                if (!Program.TickManager.Exists(this))
                {
                    return;
                }

                if (gameSession.Client.Socket == null || !gameSession.Client.Socket.IsBound || !gameSession.Client.Socket.Connected)
                {
                    return;
                }

                gameSession.Client.Send(LegacyModule.write(packet));
            }
            catch (Exception e)
            {
                Out.WriteLine("SendPacket void exception " + e, "Player.cs");
                Logger.Log("error_log", $"- [Player.cs] SendPacket void exception: {e}");
            }
        }

        public void SendCommand(byte[] command)
        {
            try
            {
                GameSession gameSession = GameManager.GetGameSession(Id);

                if (gameSession == null)
                {
                    return;
                }

                if (!Program.TickManager.Exists(this))
                {
                    return;
                }

                if (gameSession.Client.Socket == null || !gameSession.Client.Socket.IsBound || !gameSession.Client.Socket.Connected)
                {
                    return;
                }

                gameSession.Client.Send(command);
            }
            catch (Exception e)
            {
                Out.WriteLine("SendCommand void exception " + e, "Player.cs");
                Logger.Log("error_log", $"- [Player.cs] SendCommand void exception: {e}");
            }
        }

        public bool LoggingOut = false;
        private DateTime LogoutStartTime = new DateTime();

        public void Logout(bool start = false)
        {
            if (start)
            {
                 EventManager.TeamDeathmatchNew.waitingPlayers.Remove(this);
                 LoggingOut = true;
                 LogoutStartTime = DateTime.Now;
                 return;
            }


            if (!LoggingOut) return;

            if (!Storage.IsInDemilitarizedZone && (AttackingOrUnderAttack() || Moving || Spacemap.Options.LogoutBlocked || activeMapId.ToString().StartsWith("88") || activeMapId.ToString().StartsWith("77")))
            {
                AbortLogout();
                return;
            }

            //if (LogoutStartTime.AddSeconds((Premium || Id == 21) ? 5 : 5) < DateTime.Now)
            if (LogoutStartTime.AddSeconds((Premium || RankId == 22) ? 5 : 10) < DateTime.Now)
            {
                SendPacket("0|l|" + Id);
                if (Pet != null && Pet.Activated)
                    Pet.Deactivate(true);
                GameSession.Disconnect(GameSession.DisconnectionType.NORMAL);
                tsSync.Cancel();
                LoggingOut = false;
            }
        }
        public bool TDMEnd = false;
        public void TDMCheck (bool start = false)
        {
            if (TDMEnd || TeamDeathmatchNew.Active == false)
            if(GameSession.Player.Storage.waitTDM == true)
            {
                if (GameSession.Player.Storage.waitTDM == true)
                {
                   
                    EventManager.TeamDeathmatchNew.waitingPlayers.Remove(this);
                    SendPacket($"0|A|STD|Removed from TDM");
                    GameSession.Player.Storage.waitTDM = false;
                    GameSession.Player.TDMleft = false;
                    GameSession.Player.TDMright = false;
                    return;
                }
                 if (!TDMEnd) return;
            }
        }
             public void AbortLogout()
        {
            LoggingOut = false;
            SendPacket("0|t");
        }

        public int GetSkillPercentage(string skillName)
        {
            int value = 0;
            int shiphull1 = SkillTree.shiphull1;
            int shiphull2 = SkillTree.shiphull2;
            int engineering = SkillTree.engineering;
            int shieldengineering = SkillTree.shieldengineering;
            int shieldmechanics = SkillTree.shieldmechanics;
            int bountyhunter1 = SkillTree.bountyhunter1;
            int bountyhunter2 = SkillTree.bountyhunter2;
            int rocketfusion = SkillTree.rocketfusion;
            int alienhunter = SkillTree.alienhunter;
            int tactics = SkillTree.tactics;
            int luck1 = SkillTree.luck1;
            int luck2 = SkillTree.luck2;
            int cruelty1 = SkillTree.cruelty1;
            int cruelty2 = SkillTree.cruelty2;
            int greed = SkillTree.greed;

            if (skillName == "Ship Hull")
            {
                value += shiphull2 >= 1 ? (shiphull2 == 1 ? 15000 : shiphull2 == 2 ? 25000 : shiphull2 == 3 ? 50000 : 0) : (shiphull1 == 1 ? 5000 : shiphull1 == 2 ? 10000 : 0);
            }
            else if (skillName == "Engineering")
            {
                value += engineering == 1 ? 5 : engineering == 2 ? 10 : engineering == 3 ? 15 : engineering == 4 ? 20 : engineering == 5 ? 30 : 0;
            }
            else if (skillName == "Shield Engineering")
            {
                value += shieldengineering == 1 ? 4 : shieldengineering == 2 ? 8 : shieldengineering == 3 ? 12 : shieldengineering == 4 ? 18 : shieldengineering == 5 ? 25 : 0;
            }
            else if (skillName == "Shield Mechanics")
            {
                value += shieldmechanics == 1 ? 2 : shieldmechanics == 2 ? 4 : shieldmechanics == 3 ? 6 : shieldmechanics == 4 ? 8 : shieldmechanics == 5 ? 12 : 0;
            }
            else if (skillName == "Bounty Hunter")
            {
                value += bountyhunter2 >= 1 ? (bountyhunter2 == 1 ? 6 : bountyhunter2 == 2 ? 8 : bountyhunter2 == 3 ? 12 : 0) : (bountyhunter1 == 1 ? 2 : bountyhunter1 == 2 ? 4 : 0);
            }
            else if (skillName == "Rocket Fusion")
            {
                value += rocketfusion == 1 ? 2 : rocketfusion == 2 ? 4 : rocketfusion == 3 ? 6 : rocketfusion == 4 ? 8 : rocketfusion == 5 ? 15 : 0;
            }
            else if (skillName == "Alien Hunter")
            {
                value += alienhunter == 1 ? 2 : alienhunter == 2 ? 4 : alienhunter == 3 ? 6 : alienhunter == 4 ? 8 : alienhunter == 5 ? 12 : 0;
            }
            else if (skillName == "Tactics")
            {
                value += tactics == 1 ? 2 : tactics == 2 ? 4 : tactics == 3 ? 6 : tactics == 4 ? 8 : tactics == 5 ? 12 : 0;
            }
            else if (skillName == "Luck")
            {
                value += luck2 >= 1 ? (luck2 == 1 ? 6 : luck2 == 2 ? 8 : luck2 == 3 ? 12 : 0) : (luck1 == 1 ? 2 : luck1 == 2 ? 4 : 0);
            }
            else if (skillName == "Cruelty")
            {
                value += cruelty2 >= 1 ? (cruelty2 == 1 ? 12 : cruelty2 == 2 ? 18 : cruelty2 == 3 ? 25 : 0) : (cruelty1 == 1 ? 4 : cruelty1 == 2 ? 8 : 0);
            }
            else if (skillName == "Greed")
            {
                value += greed == 1 ? 4 : greed == 2 ? 8 : greed == 3 ? 12 : greed == 4 ? 15 : greed == 5 ? 25 : 0;
            }


            return value;
        }

        public GameSession GameSession => GameManager.GetGameSession(Id);

        public int IdCubikon { get; set; }

        public override byte[] GetShipCreateCommand() { return null; }

        public string GetMapName(string mapID)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT * FROM server_maps WHERE mapID = {mapID}");
                foreach (DataRow row in data.Rows)
                {
                    return Convert.ToString(row["name"]);
                }
                return "-";
            }
        }
    }
}
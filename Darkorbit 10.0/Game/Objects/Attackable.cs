using Darkorbit.Game.Events;
using Darkorbit.Game.Objects.Players;
using System.Collections.Concurrent;
using Darkorbit.Game.Objects.Players.Stations;
using Darkorbit.Game.Objects.Collectables;

namespace Darkorbit.Game.Objects
{

    class Damage
    {
        public int damage;
        public DateTime LastAttack;

        public Damage(int dmg, DateTime la)
        {
            damage = dmg;
            LastAttack = la;
        }
    }
    internal abstract class Attackable : Tick
    {
        public int Id { get; }

        public abstract string Name { get; set; }

        public abstract Clan Clan { get; set; }

        public abstract Position Position { get; set; }

        public abstract Spacemap Spacemap { get; set; }


        public abstract int FactionId { get; set; }

        public abstract int CurrentHitPoints { get; set; }

        public abstract int MaxHitPoints { get; set; }

        public abstract int CurrentNanoHull { get; set; }

        public abstract int MaxNanoHull { get; set; }

        public abstract int CurrentShieldPoints { get; set; }

        public abstract int MaxShieldPoints { get; set; }

        public abstract double ShieldAbsorption { get; set; }

        public abstract double ShieldPenetration { get; set; }

        public DateTime LastCombatTime { get; set; }

        public virtual int AttackRange => 670;

        public virtual int RenderRange => 2000;
        public virtual int RenderRange1 => 1000;

        public virtual int RenderRange2 => 300000;


        public bool Invisible { get; set; }
        public bool Invincible { get; set; }

        public bool Destroyed = false;
        public bool SpaceballDestroyed = false;
        public bool TdmDestroyed = false;
        public bool TDMleft = false;
        public bool TDMright = false;
        public bool CR = false;
        public bool cubiPremium = false;

        public Character MainAttacker { get; set; }
        public ConcurrentDictionary<int, Attacker> Attackers = new ConcurrentDictionary<int, Attacker>();

        public ConcurrentDictionary<int, Character> InRangeCharacters = new ConcurrentDictionary<int, Character>();
        public ConcurrentDictionary<int, VisualModifierCommand> VisualModifiers = new ConcurrentDictionary<int, VisualModifierCommand>();
        public ConcurrentDictionary<Attackable, Damage> DamageRecieved = new ConcurrentDictionary<Attackable, Damage>();

        public Attackable Selected { get; set; }

        public static List<int> BLACKLIGHT = new List<int>
        {
            90
        };
        public static List<int> BLACK = new List<int>
        {
            45
        };
        public static List<int> BLACKI = new List<int>
        {
            41
        };

        public static bool x2EventActive = false;

        protected Attackable(int id)
        {
            Id = id;
            Invisible = false;
            Invincible = false;

            if (Clan == null || !GameManager.Clans.ContainsKey(Clan.Id))
            {
                Clan = GameManager.GetClan(0);
            }
        }

        public abstract void Tick();

        public bool InRange(Attackable attackable, int range = 2000)
        {
            if (attackable == null || attackable.Spacemap.Id != Spacemap.Id)
            {
                return false;
            }

            if (attackable is Character character)
            {
                if (character == null || character.Destroyed)
                {
                    return false;
                }

                if (this is Player player)
                {
                    if (Duel.InDuel(player) && player.Storage.Duel?.GetOpponent(player) != attackable)
                    {
                        return false;
                    }
                }
            }
            if (range == -1 || attackable.Spacemap.Options.RangeDisabled)
            {
                return true;
            }

            if(this is Player player1)
            {
                if(player1.activeMapId.ToString().StartsWith("88") && player1.Spacemap.Id == 121)
                {
                    return true;
                }
            }

            return attackable.Id != Id && Position.DistanceTo(attackable.Position) <= range;
        }


        public async void AddDamage(Attackable player, int damage)
        {
            if (DamageRecieved.ContainsKey(player))
            {

                DamageRecieved[player].damage += damage;
                DamageRecieved[player].LastAttack = DateTime.Now;

            }
            else
            {
                Damage dmg = new Damage(damage, DateTime.Now);
                DamageRecieved.TryAdd(player, dmg);
                await Task.Delay(500);
            }
        }

        public void Deselection(bool emp = false)
        {
            Selected = null;

            if (this is Player player)
            {
                player.SendCommand(ShipDeselectionCommand.write());
                player.DisableAttack(player.Settings.InGameSettings.selectedLaser);
                player.Group?.UpdateTarget(player, new List<command_i3O> { new GroupPlayerTargetModule(new GroupPlayerShipModule(GroupPlayerShipModule.NONE), "", new GroupPlayerInformationsModule(0, 0, 0, 0, 0, 0)) });

                if (emp)
                {
                    player.SendPacket("0|A|STM|msg_own_targeting_harmed");
                    player.SendPacket("0|UI|MM|NOISE|1");
                }
            }
            else if (this is Npc npc && npc.NpcAI.AIOption != NpcAIOption.RANDOM_POSITION_MOVE && npc.NpcAI.AIOption != NpcAIOption.CUBIKON_POSITION_MOVE && npc.NpcAI.AIOption != NpcAIOption.PROTI_POSITION_MOVE)
            {
                npc.Attacking = false;
                npc.NpcAI.AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
            }
            else if (this is Npcx2 npcx2)
            {
                npcx2.Attacking = false;
                npcx2.NpcAi2.AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
            }
            else if (this is NPCFlagship pCFlagship)
            {
                pCFlagship.Attacking = false;
                pCFlagship.Selected = null;
                pCFlagship.FlagshipAI.AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
            }
        }

        public void UpdateStatus()
        {
            if (CurrentHitPoints > MaxHitPoints) CurrentHitPoints = MaxHitPoints;
            if (CurrentHitPoints < 0) CurrentHitPoints = 0;
            if (CurrentShieldPoints > MaxShieldPoints) CurrentShieldPoints = MaxShieldPoints;
            if (CurrentShieldPoints < 0) CurrentShieldPoints = 0;

            if (this is Player player)
            {
                player.SendCommand(AttributeHitpointUpdateCommand.write(CurrentHitPoints, MaxHitPoints, CurrentNanoHull, MaxNanoHull));
                player.SendCommand(AttributeShieldUpdateCommand.write(player.CurrentShieldPoints, player.MaxShieldPoints));
                player.SendCommand(SetSpeedCommand.write(player.Speed, player.Speed));
                player.Group?.UpdateTarget(player, new List<command_i3O> { new GroupPlayerInformationsModule(player.CurrentHitPoints, player.MaxHitPoints, player.CurrentShieldPoints, player.MaxShieldPoints, player.CurrentNanoHull, player.Speed) });
            }
            else if (this is Pet pet)
            {
                var owner = pet.Owner;
                //owner.SendCommand(PetHitpointsUpdateCommand.write(pet.CurrentHitPoints, pet.MaxHitPoints, false));
                //owner.SendCommand(PetShieldUpdateCommand.write(pet.CurrentShieldPoints, pet.MaxShieldPoints));

                //owner.SendCommand(PetFuelUpdateCommand.write(pet.fuel, 50000));
                owner.SendCommand(PetStatusCommand.write(pet.Id, 15, 27000000, 27000000, pet.CurrentHitPoints, pet.MaxHitPoints, pet.CurrentShieldPoints, pet.MaxShieldPoints, pet.Fuel, 50000, pet.Speed, pet.Name));
            }

            foreach (Character otherCharacter in Spacemap?.Characters.Values)
            {
                if (otherCharacter is Player otherPlayer && otherPlayer.Selected == this)
                {
                    if (this is Character character)
                    {
                        if (!otherPlayer.AttackingOrUnderAttack())
                        {
                            otherPlayer.SendCommand(ShipSelectionCommand.write(Id, character.Ship.Id, CurrentShieldPoints, MaxShieldPoints, CurrentHitPoints, MaxHitPoints, CurrentNanoHull, MaxNanoHull, (this is Player && (this as Player).SkillTree.shieldEngineering == 5)));
                        }
                        else
                        {
                            otherPlayer.SendCommand(ShipSelectionCommand.write(Id, character.Ship.Id, CurrentShieldPoints, MaxShieldPoints, CurrentHitPoints, MaxHitPoints, CurrentNanoHull, MaxNanoHull, false));
                        }
                    }
                    else if (this is Activatable activatable)
                    {
                        otherPlayer.SendCommand(AssetInfoCommand.write(
                            activatable.Id,
                            activatable.GetAssetType(),
                            activatable is Satellite ? (activatable as Satellite).DesignId : 0,
                            0,
                            activatable.CurrentHitPoints,
                            activatable.MaxHitPoints,
                            activatable.MaxShieldPoints > 0 ? true : false,
                            activatable.CurrentShieldPoints,
                            activatable.MaxShieldPoints
                            ));
                    }
                }
            }
        }

        public void SendPacketToInRangePlayers(string packet)
        {
            foreach (Character character in InRangeCharacters.Values)
            {
                if (character is Player player)
                {
                    player.SendPacket(packet);
                }
            }
        }

        public void SendCommandToInRangePlayers(byte[] command)
        {
            foreach (Character character in InRangeCharacters.Values)
            {
                if (character is Player player)
                {
                    player.SendCommand(command);
                }
            }
        }

        public void SendCommandToSelected(Attackable att, byte[] command)
        {
            if (att is Player pla)
            {
                pla.SendCommand(command);
            }
            foreach (var character in InRangeCharacters.Values)
                if (character is Player player)
                {
                    if (player.Selected == att)
                    {
                        player.SendCommand(command);

                    }
                }
        }

        public void BookReward(Player d, int c, int u, int l, int e, int h, ChangeType ch = ChangeType.INCREASE)
        {
            d.LoadData();
            if (c > 0) d.ChangeData(DataType.CREDITS, c, ch);
            if (u > 0) d.ChangeData(DataType.URIDIUM, u, ch);
            if (e > 0) d.ChangeData(DataType.EXPERIENCE, e, ch);
            if (h > 0) d.ChangeData(DataType.HONOR, h, ch);
            if (ch == ChangeType.INCREASE) if (l > 0)
                {
                    d.SendPacket($"0|LM|ST|LOG|{l}");
                    d.AddLogfiles(l);
                }
        }

        public void Destroy(Attackable destroyer, DestructionType destructionType, bool noRewards = false)
        {

            if (this is Spaceball || Destroyed)
            {
                SpaceballDestroyed = true;
                return;
            }

            if (this is Hitac || Destroyed)
            {
                return;
            }

            if (MainAttacker != null && MainAttacker is Player)
            {
                destroyer = MainAttacker;
                destructionType = DestructionType.PLAYER;
            }

            Destroyed = true;

            byte[] destroyCommand = ShipDestroyedCommand.write(Id, 0);

            if (this is Activatable)
            {
                GameManager.SendCommandToMap(Spacemap.Id, destroyCommand);
            }
            else if (this is Character)
            {
                SendCommandToInRangePlayers(destroyCommand);
            }

            if (this is Npc npc1)
            {
                if (npc1.mother != null)
                {
                    List<Npc> childs = npc1.mother.childs;
                    if (childs.Count > 0)
                    {
                        
                        npc1.Spacemap.RemoveCharacter(npc1);
                        npc1.Attacking = false;
                        npc1.MainAttacker = null;
                        Program.TickManager.RemoveTick(this);
                        Attackers.Clear();
                        childs.Remove(npc1);
                    }
                    if (npc1.mother == null)
                    {
                        npc1.mother.Spacemap.RemoveCharacter(npc1);
                        npc1.mother.Attacking = false;
                        npc1.mother.MainAttacker = null;
                        Program.TickManager.RemoveTick(this);
                        Attackers.Clear();
                    }
                    Destroyed = true;
                }
            }

            if (this is Player player)
            {

                if (player == null) return;

                if (EventManager.battleRoyal.InEvent(player) || player.Spacemap.Id == 224)
                {
                    player.ChangeShip(player.Storage.oldShip);
                    player.Storage.DamageBoost = player.Storage.oldDamage;
                    EventManager.battleRoyal.Players.TryRemove(player.Id, out player);
                }

                if (EventManager.BattleCompany.InEvent(player))
                {
                    if (player.FactionId == 1)
                    {
                        EventManager.BattleCompany.playersMMO.Remove(player);
                    }
                    else if (player.FactionId == 2)
                    {
                        EventManager.BattleCompany.playersEIC.Remove(player);
                    }
                    else if (player.FactionId == 3)
                    {
                        EventManager.BattleCompany.playersVRU.Remove(player);
                    }
                }

                if (EventManager.JackpotBattle.Players.ContainsKey(player.Id))
                {
                    EventManager.JackpotBattle.Players.TryRemove(player.Id, out player);
                }

                if (destroyer is NpcGG)
                {

                    if (player.GetPlayerActiveGate() == "Alpha")
                        player.AlphaGate.RemoveLife();

                    if (player.GetPlayerActiveGate() == "Beta")
                        player.BetaGates.RemoveLife();

                    if (player.GetPlayerActiveGate() == "Gamma")
                        player.GammaGates.RemoveLife();

                    if (player.GetPlayerActiveGate() == "Delta")
                        player.DeltaGates.RemoveLife();

                    if (player.GetPlayerActiveGate() == "Kappa")
                        player.KappaGates.RemoveLife();

                }

                if (EventManager.JackpotBattle.InEvent(player))
                {
                    GameManager.SendPacketToMap(EventManager.JackpotBattle.Spacemap.Id, $"0|A|STM|msg_jackpot_players_left|%COUNT%|{(EventManager.JackpotBattle.Spacemap.Characters.Count - 1)}");
                }

                if (destroyer is Player && (destroyer as Player).Storage.KilledPlayerIds.Where(x => x == player.Id).Count() <= 7 && player.Storage.ubal == null)
                {
                    (destroyer as Player).Storage.KilledPlayerIds.Add(player.Id);
                }

                player.Group?.UpdateTarget(player, new List<command_i3O> { new GroupPlayerDisconnectedModule(true) });
                player.SkillManager.DisableAllSkills();
                player.SendCommand(destroyCommand);
                player.DisableAttack(player.Settings.InGameSettings.selectedLaser);
                if (player.Storage.ubal == null && player.Spacemap.Id != 81)
                {
                    if (player.activeMapId.ToString().StartsWith("99"))
                    {
                        Spacemap tmp = null;
                        GameManager.Spacemaps.TryRemove(player.activeMapId, out tmp);
                    }

                    player.CurrentInRangePortalId = -1;
                    player.Storage.InRangeAssets.Clear();
                    player.KillScreen(destroyer, destructionType);
                }

                else if (player.Spacemap.Id == 81)
                {
                    TdmDestroyed = true;
                    //player.SetPosition(player.GetBasePosition());
                    // player.Jump(Duel.Spacemap.Id, Position.TDMEIC);
                    player.SetPosition(player.GetBasePosition());
                    player.Jump(player.GetBaseMapId(true), player.Position);
                    Destroyed = false;
                }
                else
                {
                    Destroyed = false;
                }

            }
            else if (this is BattleStation battleStation)
            {

                if (destroyer.Clan.Id != 0)
                {
                    GameManager.SendPacketToAll($"0|A|STM|msg_station_destroyed_by_clan|%DESTROYER%|{destroyer.Clan.Name}|%MAP%|{Spacemap.Name}|%LOSER%|{battleStation.Clan.Name}|%STATION%|{battleStation.AsteroidName}");
                }
                else
                {
                    GameManager.SendPacketToAll($"0|A|STM|msg_station_destroyed|%MAP%|{Spacemap.Name}|%LOSER%|{battleStation.Clan.Name}|%STATION%|{battleStation.AsteroidName}");
                }

                foreach (var satellites in battleStation.EquippedStationModule[battleStation.Clan.Id])
                {
                    satellites.Remove(true, true, true);
                    satellites.Type = StationModuleModule.NONE;
                    satellites.CurrentHitPoints = 0;
                    satellites.CurrentShieldPoints = 0;
                    satellites.DesignId = 0;
                    satellites.Destroyed = true;

                    if (satellites.BattleStation.Destroyed)
                    {
                        Spacemap.Activatables.TryRemove(satellites.Id, out Activatable activatable);
                        GameManager.SendCommandToMap(Spacemap.Id, AssetRemoveCommand.write(satellites.GetAssetType(), satellites.Id));
                    }
                    else if (satellites.BattleStation.AssetTypeId == AssetTypeModule.BATTLESTATION)
                    {
                        GameManager.SendCommandToMap(Spacemap.Id, satellites.GetAssetCreateCommand(0));
                    }

                    QueryManager.BattleStations.Modules(satellites.BattleStation);
                }

                battleStation.EquippedStationModule.Remove(battleStation.Clan.Id);
                battleStation.Clan = GameManager.GetClan(0);
                battleStation.Name = battleStation.AsteroidName;
                battleStation.InBuildingState = false;
                battleStation.FactionId = 0;
                battleStation.BuildTimeInMinutes = 0;
                battleStation.AssetTypeId = AssetTypeModule.ASTEROID;
                battleStation.CurrentHitPoints = battleStation.MaxHitPoints;
                battleStation.CurrentShieldPoints = battleStation.MaxShieldPoints;
                battleStation.proteClanId = 0;

                Program.TickManager.RemoveTick(battleStation);

                GameManager.SendCommandToMap(Spacemap.Id, AssetRemoveCommand.write(battleStation.GetAssetType(), battleStation.Id));
                GameManager.SendCommandToMap(Spacemap.Id, battleStation.GetAssetCreateCommand(0));

                destroyer.Selected = null;
                QueryManager.BattleStations.BattleStation(battleStation);
                QueryManager.BattleStations.Modules(battleStation);
            }
            else if (this is Satellite satellite)
            {

                if (!satellite.BattleStation.Destroyed && satellite.Type != StationModuleModule.HULL && satellite.Type != StationModuleModule.DEFLECTOR)
                {
                    GameManager.SendPacketToClan($"0|A|STM|msg_station_module_destroyed|%STATION%|{satellite.BattleStation.AsteroidName}|%MAP%|{Spacemap.Name}|%MODULE%|{satellite.Name}|%LEVEL%|16", satellite.Clan.Id);
                }

                satellite.Remove(true, true, true);
                satellite.Type = StationModuleModule.NONE;
                satellite.CurrentHitPoints = 0;
                satellite.CurrentShieldPoints = 0;
                satellite.DesignId = 0;
                satellite.Destroyed = true;
                if (satellite.BattleStation.Destroyed)
                {
                    Spacemap.Activatables.TryRemove(satellite.Id, out Activatable activatable);
                    GameManager.SendCommandToMap(Spacemap.Id, AssetRemoveCommand.write(satellite.GetAssetType(), satellite.Id));
                }
                else if (satellite.BattleStation.AssetTypeId == AssetTypeModule.BATTLESTATION)
                {
                    GameManager.SendCommandToMap(Spacemap.Id, satellite.GetAssetCreateCommand(0));
                }

                destroyer.Selected = null;
                QueryManager.BattleStations.Modules(satellite.BattleStation);
            }

            if (destroyer is Player destroyerPlayer)
            {
                int experience = 0;
                int honor = 0;
                int uridium = 0;
                int credits = 0;
                int logfiles = 0;

                bool reward = true;
                ChangeType changeType = ChangeType.INCREASE;

                if (this is Pet && (this as Pet).Owner == destroyerPlayer)
                {
                    changeType = ChangeType.DECREASE;
                }

                if (this is Character)
                {
                    if (this is Player && destroyerPlayer is Player && Spacemap.Name.ToString() != "UBA" && Spacemap.Name.ToString() != "TDM I")
                    {
                        GameManager.SendPacketToAll($"0|A|STD|MAP [{Spacemap.Name.ToString()}]  {destroyerPlayer.Name.ToString()} Destroyed {(this as Character).Name.ToString()}   ");
                    }

                    experience = destroyerPlayer.GetExperienceBoost(destroyerPlayer.Ship.GetExperienceBoost((this as Character).Ship.Rewards.Experience));
                    honor = destroyerPlayer.GetHonorBoost(destroyerPlayer.Ship.GetHonorBoost((this as Character).Ship.Rewards.Honor));
                    uridium = (this as Character).Ship.Rewards.Uridium;
                    credits = (this as Character).Ship.Rewards.Credits;
                    logfiles = (this as Character).Ship.Rewards.Logfiles;

                    short relationType = 0;
                    if (this is Player)
                    {
                        relationType = destroyerPlayer.Clan.Id != 0 && (this as Character).Clan.Id != 0 ? destroyerPlayer.Clan.GetRelation((this as Character).Clan) : (short)0;
                    }
                    int count = destroyerPlayer.Storage.KilledPlayerIds.Where(x => x == Id).Count();
                    if (this is Player && count >= 7)
                    {

                        destroyerPlayer.SendPacket($"0|A|STM|If you keep killing this player will be counted as pushing");
                    }
                    if (this is Player && count >= 10)
                    {
                        reward = false;
                        destroyerPlayer.OwnFactionNegativeHonor *= 4;
                        destroyerPlayer.SendPacket($"0|A|STM|pusher_info_no_reward|%NAME%|{Name}");
                    }
                    else if (this is Player && Duel.InDuel(this as Player))
                    {
                        reward = false;
                    }
                }
                else if (this is Activatable)
                {
                    credits = 3000;
                    experience = 500000;
                    honor = 1012;
                    uridium = 30000;

                }

                experience += Maths.GetPercentage(experience, destroyerPlayer.BoosterManager.GetPercentage(BoostedAttributeType.EP));
                honor += Maths.GetPercentage(honor, destroyerPlayer.BoosterManager.GetPercentage(BoostedAttributeType.HONOUR));
                honor += Maths.GetPercentage(honor, destroyerPlayer.GetSkillPercentage("Cruelty"));

                if (this is NpcGG NpcGG)
                {
                    NpcGG.Destroy(GameManager.GetGameSession(destroyerPlayer.GetPlayerId()));
                    NpcGG.Spacemap.RemoveCharacter(NpcGG);
                    NpcGG.Attacking = false;
                    NpcGG.MainAttacker = null;
                    Program.TickManager.RemoveTick(this);
                }

                if (reward)
                {
                    var beginnerLevel = 14;

                    IEnumerable<Player> groupMembers = destroyerPlayer.Group?.Members.Values.Where(x => x.AttackingOrUnderAttack());

                    if (this is Npc n && n.overallCausedDamage > 0)
                    {
                        for (int i = n.causedDamage.Count - 1; i >= 0; i--)
                        {
                            NpcDamageCause facade = n.causedDamage[i];
                            double p = 0f;
                            p = facade.damage / n.overallCausedDamage;
                            int beginnerBonus = 1;

                            if (facade.player.Level <= beginnerLevel || x2EventActive) beginnerBonus = 2;

                            BookReward(facade.player, (int)(Convert.ToDouble(credits) * p) * beginnerBonus, (int)(Convert.ToDouble(uridium) * p) * beginnerBonus, (int)(Convert.ToDouble(logfiles) * p) * beginnerBonus, (int)(Convert.ToDouble(experience) * p), (int)(Convert.ToDouble(honor) * p));
                        }
                    }
                    else if ((this is Npc n1 && n1.overallCausedDamage <= 0) || this is NpcGG n2)
                    {
                        if ((destroyerPlayer.Group == null || (destroyerPlayer.Group != null && groupMembers.Count() == 0)) && !noRewards)
                        {
                            int beginnerBonus = 1;

                            if (destroyerPlayer.Level <= beginnerLevel || x2EventActive) beginnerBonus = 2;

                            BookReward(destroyerPlayer, credits * beginnerBonus, uridium * beginnerBonus, logfiles * beginnerBonus, experience, honor);
                        }
                        else
                        {
                            credits = credits / groupMembers.Count();
                            experience = experience / groupMembers.Count();
                            honor = honor / groupMembers.Count();
                            uridium = uridium / groupMembers.Count();
                            logfiles = logfiles / groupMembers.Count();

                            int beginnerBonus = 1;

                            if (destroyerPlayer.Level <= beginnerLevel || x2EventActive) beginnerBonus = 2;

                            foreach (Player member in groupMembers)
                            {
                                if (destroyerPlayer.Spacemap.Id == member.Spacemap.Id)
                                {
                                    //if player is on a normal map
                                    if (!destroyerPlayer.activeMapId.ToString().StartsWith("99") && destroyerPlayer.Spacemap.Id != 81)
                                    {
                                        BookReward(member, credits * beginnerBonus, uridium * beginnerBonus, logfiles * beginnerBonus, experience, honor);
                                    } else
                                    {
                                        //if groupmembers are on the same mapinstance
                                        if(destroyerPlayer.activeMapId == member.activeMapId)
                                        {
                                            BookReward(member, credits * beginnerBonus, uridium * beginnerBonus, logfiles * beginnerBonus, experience, honor);
                                        } else
                                        {
                                            //if destroyer is same as group member, then book the full reward instead of splitting it for each member
                                            if(destroyerPlayer.Id == member.Id)
                                            {
                                                credits = credits * groupMembers.Count();
                                                experience = experience * groupMembers.Count();
                                                honor = honor * groupMembers.Count();
                                                uridium = uridium * groupMembers.Count();
                                                logfiles = logfiles * groupMembers.Count();

                                                BookReward(member, credits * beginnerBonus, uridium * beginnerBonus, logfiles * beginnerBonus, experience, honor);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } else if (this is Player p && destroyerPlayer.Spacemap.Id != 121 && destroyerPlayer.Spacemap.Id != 81)
                    {
                        if (p.FactionId != destroyerPlayer.FactionId)
                        {
                            BookReward(destroyerPlayer, credits, uridium, logfiles, experience, honor);
                            destroyerPlayer.bootyKeys.greenKeys++;
                            destroyerPlayer.SendPacket($"0|A|BK|{destroyerPlayer.bootyKeys.greenKeys}");
                            destroyerPlayer.AmmunitionManager.AddAmmo(AmmunitionManager.RSB_75, Randoms.random.Next(200, 600));
                            destroyerPlayer.AmmunitionManager.AddAmmo(AmmunitionManager.UCB_100, Randoms.random.Next(600, 1500));
                            destroyerPlayer.AmmunitionManager.AddAmmo(AmmunitionManager.SAB_50, Randoms.random.Next(200, 1000));
                            destroyerPlayer.AmmunitionManager.AddAmmo(AmmunitionManager.EMP_01, Randoms.random.Next(0, 1));
                            destroyerPlayer.AmmunitionManager.AddAmmo(AmmunitionManager.ISH_01, Randoms.random.Next(0, 1));
                            destroyerPlayer.AmmunitionManager.AddAmmo(AmmunitionManager.SMB_01, Randoms.random.Next(0, 1));
                        }
                        else
                        {
                            var relationType = destroyerPlayer.Clan.Id != 0 && p.Clan.Id != 0 ? destroyerPlayer.Clan.GetRelation(p.Clan) : (short)0;

                            if(relationType == ClanRelationModule.AT_WAR)
                            {
                                BookReward(destroyerPlayer, credits, uridium, logfiles, 0, 0);
                            } else
                            {
                                BookReward(destroyerPlayer, 0, 0, 0, 0, destroyerPlayer.OwnFactionNegativeHonor, ChangeType.DECREASE);
                                destroyerPlayer.OwnFactionNegativeHonor *= 2;
                            }
                        }
                    }
                }

                if (this is Npc && !noRewards)
                {
                    if (destroyerPlayer.droneExp < 2800)
                    {
                        destroyerPlayer.droneExp += Randoms.random.Next(1, 15);
                    }

                    using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        if (destroyerPlayer.Group != null)
                        {
                            IEnumerable<Player> groupMembers = destroyerPlayer.Group?.Members.Values.Where(x => x.AttackingOrUnderAttack());
                            if (groupMembers.Count<Player>() > 0)
                            {
                                foreach (Player tmp in groupMembers) tmp.AddNpcToDatabase((this as Character).Ship.Id);
                            }
                            //mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_npc (killer_id, target_id, group) VALUES ({destroyerPlayer.Id}, '{(this as Character).Ship.LootId}', {destroyerPlayer.Group.Id})");
                        }
                        else destroyerPlayer.AddNpcToDatabase((this as Character).Ship.Id);

                        mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_killhistory (killer_id, target_id, type) VALUES ({destroyerPlayer.Id}, {(this as Character).Ship.Id}, 'pve')");
                    }
                    if ((this as Character).Ship.Id == 80 && ((this as Character).Spacemap.Id == 3 || (this as Character).Spacemap.Id == 11 ))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.CubiThread> ct = c.Spacemap.GetCubikonThreadList();
                        foreach (Spacemap.CubiThread cubi in ct)
                        {
                            int threadId = cubi.GetId();
                            Task thread = cubi.GetThread();                           

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;                              
                            }
                        }
                    }
                    if ((this as Character).Ship.Id == 119 && ((this as Character).Spacemap.Id == 58))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.BossCubiThread> ct = c.Spacemap.GetBossCubikonThreadList();
                        foreach (Spacemap.BossCubiThread bosscubi in ct)
                        {
                            int threadId = bosscubi.GetId();
                            Task thread = bosscubi.GetThread();

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;
                            }
                        }
                    }
                    else if ((this as Character).Ship.Id == 90 && ((this as Character).Spacemap.Id == 3))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.CentaurThread> ct = c.Spacemap.GetCentaurThreadList();
                        foreach (Spacemap.CentaurThread centaur in ct)
                        {
                            int threadId = centaur.GetId();
                            Task thread = centaur.GetThread();

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;
                            }
                        }
                    }
                    else if ((this as Character).Ship.Id == 90 && ((this as Character).Spacemap.Id == 11))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.CentaurThread> ct = c.Spacemap.GetCentaurThreadList();
                        foreach (Spacemap.CentaurThread centaur in ct)
                        {
                            int threadId = centaur.GetId();
                            Task thread = centaur.GetThread();

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;
                            }
                        }
                    }
                    else if ((this as Character).Ship.Id == 115 && ((this as Character).Spacemap.Id == 55))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.BattlerayThread> ct = c.Spacemap.GetBattlerayThreadList();
                        foreach (Spacemap.BattlerayThread battleray in ct)
                        {
                            int threadId = battleray.GetId();
                            Task thread = battleray.GetThread();

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;
                            }
                        }
                    }
                    else if ((this as Character).Ship.Id == 101 && ((this as Character).Spacemap.Id == 2))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.MeteoritThread> ct = c.Spacemap.GetMeteoritThreadList();
                        foreach (Spacemap.MeteoritThread meteorit in ct)
                        {
                            int threadId = meteorit.GetId();
                            Task thread = meteorit.GetThread();

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;
                            }
                        }
                    }
                    else if ((this as Character).Ship.Id == 101 && ((this as Character).Spacemap.Id == 10))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.MeteoritThread> ct = c.Spacemap.GetMeteoritThreadList();
                        foreach (Spacemap.MeteoritThread meteorit in ct)
                        {
                            int threadId = meteorit.GetId();
                            Task thread = meteorit.GetThread();

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;
                            }
                        }
                    }
                    else if ((this as Character).Ship.Id == 83 && ((this as Character).Spacemap.Id == 16))
                    {
                        //if cubikon destroyed, then abort the thread as is it not needed anymore
                        Character c = (this as Character);
                        List<Spacemap.KukuThread> ct = c.Spacemap.GetKukuThreadList();
                        foreach (Spacemap.KukuThread kuku in ct)
                        {
                            int threadId = kuku.GetId();
                            Task thread = kuku.GetThread();

                            Npc n = this as Npc;

                            int i = 1;
                            foreach (Npc t in n.childs)
                            {
                                t.diesAfter = DateTime.Now.AddSeconds(2 * i);
                                i++;
                            }
                        }
                    }
                    /* else if ((this as Character).Ship.Id == 101)
                     {
                         int i = 1;
                         foreach (Npc n in IceMetorit.data.minions)
                         {
                             n.diesAfter = DateTime.Now.AddSeconds(2 * i);
                             i++;
                         }
                         IceMetorit.data.minions = new List<Npc>();
                     }*/
                    else if ((this as Character).Ship.Id == 33)
                    {
                        int i = 1;
                        foreach (Npc n in SuperIceMetorit.data.Minions)
                        {
                            n.diesAfter = DateTime.Now.AddSeconds(2 * i);
                            i++;
                        }
                        SuperIceMetorit.data.SetMinions(new List<Npc>());
                    }
                    if (BLACKLIGHT.Contains((this as Character).Ship.Id))
                    {
                        var ran = Randoms.random.Next(1, 100);
                        if (ran > 0 && ran <= 100)
                        {
                            new IceBox(Position, Spacemap, false, destroyerPlayer);
                        }
                        if (BLACK.Contains((this as Character).Ship.Id))
                        {
                            var ran2 = Randoms.random.Next(1, 100);
                            if (ran2 > 0 && ran <= 100)
                            {
                                new GoldBooty(Position, Spacemap, false, destroyerPlayer);
                            }

                        }
                        if (BLACKI.Contains((this as Character).Ship.Id))
                        {
                            var ran2 = Randoms.random.Next(1, 100);
                            if (ran2 > 0 && ran <= 100)
                            {
                                new SilverBooty(Position, Spacemap, false, destroyerPlayer);
                            }

                        }

                    }
                }
                /*if (this is Npcx2 && !noRewards)
                {
                    if (destroyerPlayer.droneExp < 2800)
                    {
                        destroyerPlayer.droneExp += Randoms.random.Next(1, 15);
                    }
                    using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        if (destroyerPlayer.Group != null)
                        {
                            IEnumerable<Player> groupMembers = destroyerPlayer.Group?.Members.Values.Where(x => x.AttackingOrUnderAttack());
                            if (groupMembers.Count<Player>() > 0)
                            {
                                foreach (Player tmp in groupMembers) mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_npc (killer_id, target_id, groupId) VALUES ({tmp.Id}, '{(this as Character).Ship.LootId}', {destroyerPlayer.Group.Id})");
                            }
                            //mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_npc (killer_id, target_id, group) VALUES ({destroyerPlayer.Id}, '{(this as Character).Ship.LootId}', {destroyerPlayer.Group.Id})");
                        }
                        else mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_npc (killer_id, target_id) VALUES ({destroyerPlayer.Id}, '{(this as Character).Ship.LootId}')");
                    }
                }*/
                if (this is NPCFlagship && !noRewards)
                {
                    if (destroyerPlayer.FactionId == this.FactionId)
                    {
                        destroyerPlayer.LoadData();
                        destroyerPlayer.ChangeData(DataType.CREDITS, 250000, ChangeType.DECREASE);
                        destroyerPlayer.ChangeData(DataType.EXPERIENCE, 100000, ChangeType.DECREASE);
                        destroyerPlayer.ChangeData(DataType.HONOR, 10000, ChangeType.DECREASE);
                        destroyerPlayer.ChangeData(DataType.URIDIUM, 10000, ChangeType.DECREASE);
                        if (logfiles > 0) destroyerPlayer.SendPacket($"0|LM|ST|LOG|{logfiles}");
                    }
                }
                if (this is Player otherPlayer)
                {

                        //if (EventManager.UltimateBattleArena.InEvent(this as Player) || EventManager.groupEvent.InEvent(this as Player))
                        if (EventManager.UltimateBattleArena.InEvent(this as Player))
                    {
                        //using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                        //{
                        //mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_kills (killer_id, target_id) VALUES ({destroyerPlayer.Id}, {Id})");
                        //}
                        /*if (otherPlayer.UbaPoints < 10)
                        {
                            destroyerPlayer.UbaPoints += 10;

                            otherPlayer.Ubabattel += 1;

                            destroyerPlayer.Ubabattel += 1;

                            destroyerPlayer.SendPacket("0|A|STD|You WIN 10 UBA points.");
                        }
                        else
                        {
                            otherPlayer.UbaPoints -= 10;

                            destroyerPlayer.UbaPoints += 10;

                            otherPlayer.Ubabattel += 1;

                            destroyerPlayer.Ubabattel += 1;

                            destroyerPlayer.SendPacket("0|A|STD|You WIN 10 UBA points.");
                            otherPlayer.SendPacket("0|A|STD|You LOST 10 UBA points.");
                        }*/
                    }


                    else
                    {
                            if (destroyerPlayer.FactionId != otherPlayer.FactionId && destroyerPlayer.Spacemap.Id != 121 && destroyerPlayer.Spacemap.Id != 81)
                        {


                            destroyerPlayer.Destructions.fpd += 1;
                            if (destroyerPlayer.droneExp < 3000)
                            {
                                destroyerPlayer.droneExp += 50;
                            }




                            //using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                            //{
                            //    mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_pvp_kills (userId, ship) VALUES ({destroyerPlayer.Id}, {Id})");
                            //}
                            using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                            {
                                mySqlClient.ExecuteNonQuery($"INSERT INTO log_player_killhistory (killer_id, target_id, type) VALUES ({destroyerPlayer.Id}, {Id}, 'pvp')");
                            }
                            destroyerPlayer.AddShipToDatabase(otherPlayer.Ship.Id);
                            int count = destroyerPlayer.Storage.KilledPlayerIds.Where(x => x == Id).Count();
                            /*if (this is Player && count <= 2)

                            new CargoBox(Position, Spacemap, false, false, false, destroyerPlayer);*/
                        }

                    }

                }
            }


            if (this is Character character)
            {
                if (this is Player && Duel.InDuel(this as Player))
                {
                    Duel.RemovePlayer(this as Player);
                }

                /* ========================================= */

                /*if (this is NPCFlagship fNPCFlagship)
                {
                    //var faction = FactionId;
                    int mapId = Spacemap.Id;

                    if (new int[] { 1, 2, 3, 4 }.Contains(Id))
                        NPCFlagship.Add(Spacemap, 1);
                    if (new int[] { 5, 6, 7, 8 }.Contains(Id))
                        NPCFlagship.Add(Spacemap, 2);
                    if (new int[] { 9, 10, 11, 12 }.Contains(Id))
                        NPCFlagship.Add(Spacemap, 3);
                    else
                        NPCFlagship.Add(Spacemap, Randoms.random.Next(1, 4));

                    Program.TickManager.RemoveTick(this);
                }

                /* ========================================= */

                Spacemap.RemoveCharacter(character);

                CurrentHitPoints = 0;
            }

            if (this is Npc npc)
            {
                if (npc.Ship.Respawnable && npc.respawnable && !npc.CR)
                {
                    npc.Respawn();
                }

                if (npc.Ship.Id == 80 && !npc.CR)
                {
                    //Logger.Log("error_log", $"- [CubiconDebug] Cubicon respawned");
                    Task cubiThread = Task.Run(async () => await Cubikon.RespawnCubicon(npc));
                }
                if (npc.Ship.Id == 119 && !npc.CR)
                {
                    //Logger.Log("error_log", $"- [CubiconDebug] Cubicon respawned");
                    Task bosscubiThread = Task.Factory.StartNew(() => BossCubikon.RespawnBossCubicon(npc));
                }
                if (npc.Ship.Id == 90 && !npc.CR)
                {
                    //Logger.Log("error_log", $"- [CubiconDebug] Cubicon respawned");
                    Task centaurThread = Task.Factory.StartNew(() => Centaur.RespawnCentaur(npc));
                }
                if (npc.Ship.Id == 115 && !npc.CR)
                {
                    //Logger.Log("error_log", $"- [CubiconDebug] Cubicon respawned");
                    Task battlerayThread = Task.Factory.StartNew(() => Battleray.RespawnBattleray(npc));
                }
                if (npc.Ship.Id == 83 && !npc.CR)
                {
                    //Logger.Log("error_log", $"- [CubiconDebug] Cubicon respawned");
                    Task kukuThread = Task.Factory.StartNew(() => Kuku.RespawnKuku(npc));
                }
                if (npc.Ship.Id == 101 && !npc.CR)
                {
                    //Logger.Log("error_log", $"- [CubiconDebug] Cubicon respawned");
                    Task meteoritThread = Task.Factory.StartNew(() => Meteorit.RespawnMeteorit(npc));
                }

            }

            if (this is Npcx2 npcx2)
            {
                if (npcx2.Ship.Respawnable && !npcx2.CR)
                {
                    npcx2.Respawn();
                }
            }

            if (destroyer is Character)
            {
                if (destroyer.CurrentHitPoints <= 0)
                    destroyer.Deselection();
            }

            Deselection();
            InRangeCharacters.Clear();
            VisualModifiers.Clear();

            if (this is Pet pet)
            {
                pet.Deactivate(true, true);
            }

            if (this is Npc npcTmp)
            {
                if (!npcTmp.respawnable && !npcTmp.Ship.Respawnable) Program.TickManager.RemoveTick(this);
            }
        }


        public DateTime outOfRangeCooldown = new DateTime();
        public DateTime inAttackCooldown = new DateTime();
        public DateTime peaceAreaCooldown = new DateTime();
        public bool TargetDefinition(Attackable target, bool sendMessage = true, bool isPlayerRocketAttack = false, bool pet = false)
        {
            if (target == null) return false;

            short relationType = Clan.Id != 0 && target.Clan.Id != 0 ? Clan.GetRelation(target.Clan) : (short)0;

            if (this is Player player)
            {
                if (Spacemap.Options.PvpDisabled && target is Player)
                {
                    player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                    if (sendMessage)
                    {
                        player.SendPacket("0|A|STD|PvP is disabled on this map");
                    }

                    return false;
                }

                if (relationType != ClanRelationModule.AT_WAR)
                {

                    bool attackable = true;

                    /*if (Spacemap.Id == 61 || Spacemap.Id == 62 || Spacemap.Id == 63)
                    {
                        attackable = false;
                    }*/

                    string packet = "";

                    if (target is Pet pet2)
                    {

                        if (pet2.Owner.Storage.IsInDemilitarizedZone)
                        {
                            attackable = false;
                        }

                        if ((pet2.Owner.Spacemap.Id == 1) || (pet2.Owner.Spacemap.Id == 2) || (pet2.Owner.Spacemap.Id == 3) || (pet2.Owner.Spacemap.Id == 4) || (pet2.Owner.Spacemap.Id == 5) || (pet2.Owner.Spacemap.Id == 6) || (pet2.Owner.Spacemap.Id == 7) || (pet2.Owner.Spacemap.Id == 8) || (pet2.Owner.Spacemap.Id == 9) || (pet2.Owner.Spacemap.Id == 10) || (pet2.Owner.Spacemap.Id == 11) || (pet2.Owner.Spacemap.Id == 12) || (pet2.Owner.Spacemap.Id == 17) || (pet2.Owner.Spacemap.Id == 18) || (pet2.Owner.Spacemap.Id == 19) || (pet2.Owner.Spacemap.Id == 20) || (pet2.Owner.Spacemap.Id == 21) || (pet2.Owner.Spacemap.Id == 22) || (pet2.Owner.Spacemap.Id == 23) || (pet2.Owner.Spacemap.Id == 24) || (pet2.Owner.Spacemap.Id == 25) || (pet2.Owner.Spacemap.Id == 26) || (pet2.Owner.Spacemap.Id == 27) || (pet2.Owner.Spacemap.Id == 28) || (pet2.Owner.Spacemap.Id == 29))
                        {

                            if (pet2.Owner.Level <= 13)
                            {
                                attackable = false;
                            }

                            if (player.Level <= 12 && pet2.Owner.Level >= 13)
                            {
                                attackable = false;
                            }

                        }

                        if (pet2.Owner.Spacemap.Id == 306)
                        {

                            if (pet2.Owner.Level <= 22)
                            {
                                attackable = false;
                            }

                            if (player.Level <= 22 && pet2.Owner.Level >= 22)
                            {
                                attackable = false;
                            }

                        }

                        if (!attackable)
                        {
                            player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                            if (sendMessage)
                            {
                                player.SendPacket(packet);
                            }

                            return false;
                        }
                    }
                    if (target is Player target2)
                    {
                        if (!EventManager.BattleCompany.InEvent(player) && !EventManager.JackpotBattle.InEvent(player) && !Duel.InDuel(player) && !EventManager.UltimateBattleArena.InEvent(player) && !EventManager.battleRoyal.InEvent(player) && !EventManager.groupEvent.InEvent(player) && !(Spacemap.Id == 1) && !(Spacemap.Id == 2) && !(Spacemap.Id == 3) && !(Spacemap.Id == 4) && !(Spacemap.Id == 5) && !(Spacemap.Id == 6) && !(Spacemap.Id == 7) && !(Spacemap.Id == 8) && !(Spacemap.Id == 9) && !(Spacemap.Id == 10) && !(Spacemap.Id == 11) && !(Spacemap.Id == 12) && !(Spacemap.Id == 13) && !(Spacemap.Id == 14) && !(Spacemap.Id == 15) && !(Spacemap.Id == 16) && !(Spacemap.Id == 17) && !(Spacemap.Id == 18) && !(Spacemap.Id == 19) && !(Spacemap.Id == 20) && !(Spacemap.Id == 21) && !(Spacemap.Id == 22) && !(Spacemap.Id == 23) && !(Spacemap.Id == 24) && !(Spacemap.Id == 25) && !(Spacemap.Id == 26) && !(Spacemap.Id == 27) && !(Spacemap.Id == 28) && !(Spacemap.Id == 29) && !(Spacemap.Id == 42) && !(Spacemap.Id == 81))
                        {
                            if (FactionId == target.FactionId)
                            {
                                packet = "0|A|STD|You can't attack members of your company!";
                            }
                            else if (Clan.Id != 0 && target.Clan.Id != 0 && Clan.Id == target.Clan.Id)
                            {
                                packet = "0|A|STD|You can't attack members of your own clan!";
                            }
                            else if (FactionId == target.FactionId && Clan.Id != 0 && target.Clan.Id != 0 && Clan.Id == target.Clan.Id)
                            {
                                packet = "0|A|STD|You can't attack members of your own clan!";
                            }
                            if (packet != "")
                            {
                                attackable = false;
                            }
                        }

                        if (target2.Storage.IsInDemilitarizedZone)
                        {
                            attackable = false;
                        }

                        if (!attackable)
                        {
                            player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                            if (sendMessage)
                            {
                                player.SendPacket(packet);
                            }

                            return false;
                        }
                    }
                }
                else if (EventManager.BattleCompany.InEvent(player))
                {
                    if (FactionId == target.FactionId)
                    {
                        player.DisableAttack(player.Settings.InGameSettings.selectedLaser);
                    }
                }
                else if (relationType == ClanRelationModule.AT_WAR)
                {
                    if (!EventManager.BattleCompany.InEvent(player) && !EventManager.JackpotBattle.InEvent(player) && !Duel.InDuel(player) && !EventManager.UltimateBattleArena.InEvent(player) && !EventManager.battleRoyal.InEvent(player) && !EventManager.groupEvent.InEvent(player) && !(Spacemap.Id == 1) && !(Spacemap.Id == 2) && !(Spacemap.Id == 3) && !(Spacemap.Id == 4) && !(Spacemap.Id == 5) && !(Spacemap.Id == 6) && !(Spacemap.Id == 7) && !(Spacemap.Id == 8) && !(Spacemap.Id == 9) && !(Spacemap.Id == 10) && !(Spacemap.Id == 11) && !(Spacemap.Id == 12) && !(Spacemap.Id == 13) && !(Spacemap.Id == 14) && !(Spacemap.Id == 15) && !(Spacemap.Id == 16) && !(Spacemap.Id == 17) && !(Spacemap.Id == 18) && !(Spacemap.Id == 19) && !(Spacemap.Id == 20) && !(Spacemap.Id == 21) && !(Spacemap.Id == 22) && !(Spacemap.Id == 23) && !(Spacemap.Id == 24) && !(Spacemap.Id == 25) && !(Spacemap.Id == 26) && !(Spacemap.Id == 27) && !(Spacemap.Id == 28) && !(Spacemap.Id == 29) && (Spacemap.Id == 42))
                    {
                        var packet = "";
                        bool attackable = true;
                        if (FactionId == target.FactionId)
                        {
                            packet = "0|A|STD|You can't attack members of your company!";
                        }
                        if (packet != "")
                        {
                            attackable = false;
                        }
                        if (!attackable)
                        {
                            player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                            if (sendMessage)
                            {
                                player.SendPacket(packet);
                            }

                            return false;
                        }
                    }
                }


                if (target is Player targetPlayer && targetPlayer.Group != null)
                {
                    if (player.Group == targetPlayer.Group)
                    {
                        player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                        if (sendMessage)
                        {
                            player.SendPacket("0|A|STD|You can't attack members of your group!");
                        }

                        return false;
                    }
                }
                if (target is Player targetPlayer2)
                    {
                    if (player.TDMleft == true && targetPlayer2.TDMleft == true)
                    {
                        player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                        if (sendMessage)
                        {
                            player.SendPacket("0|A|STD|You can't attack members of your own Team!");
                        }

                        return false;
                    }
                    if (TDMright == true && targetPlayer2.TDMright == true)
                    {
                        player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                        if (sendMessage)
                        {
                            player.SendPacket("0|A|STD|You can't attack members of your own Team!");
                        }

                        return false;
                    }
                }

                if ((target is Player && (target as Player).Storage.IsInDemilitarizedZone) || (Duel.InDuel(player) && player.Storage.Duel.PeaceArea) || (player.Storage.noAttack && target is Player))
                {
                    player.DisableAttack(player.Settings.InGameSettings.selectedLaser);

                    if (peaceAreaCooldown.AddSeconds(10) < DateTime.Now)
                    {
                        if (sendMessage)
                        {
                            player.SendPacket("0|A|STM|peacearea");

                            if (target is Player)
                            {
                                (target as Player).SendPacket("0|A|STM|peacearea");
                            }

                            peaceAreaCooldown = DateTime.Now;
                        }
                    }
                    return false;
                }

                if (inAttackCooldown.AddSeconds(10) < DateTime.Now)
                {
                    if (sendMessage)
                    {
                        player.SendPacket("0|A|STM|oppoatt|%!|" + (target is Player && EventManager.JackpotBattle.InEvent(target as Player) ? EventManager.JackpotBattle.Name : target.Name));
                        inAttackCooldown = DateTime.Now;
                    }
                }
            }
            else if (this is Satellite)
            {
                if (target.Clan.Id == Clan.Id || relationType == ClanRelationModule.ALLIED)
                {
                    return false;
                }
            }

            int range = this is Player ? (isPlayerRocketAttack ? (this as Player).AttackManager.GetRocketRange() : AttackRange) : this is Satellite ? (this as Satellite).GetRange() : this is Npc ? 450 : this is NpcGG ? 450 : this is Npcx2 ? 450 : AttackRange;

            if (Position.DistanceTo(target.Position) > range && pet == false)
            {
                if (outOfRangeCooldown.AddSeconds(5) < DateTime.Now)
                {
                    if (sendMessage)
                    {
                        if (this is Player && !isPlayerRocketAttack)
                            (this as Player).SendPacket("0|A|STM|outofrange");

                        if (target is Player)
                            (target as Player).SendPacket("0|A|STM|attescape");

                        outOfRangeCooldown = DateTime.Now;
                    }
                }
                return false;
            }
            return true;
        }

        public void Heal(int amount, int healerId = 0, HealType healType = HealType.HEALTH)
        {
            if (amount < 0)
                return;

            switch (healType)
            {
                case HealType.HEALTH:
                    if (CurrentHitPoints + amount > MaxHitPoints)
                        amount = MaxHitPoints - CurrentHitPoints;
                    CurrentHitPoints += amount;
                    break;
                case HealType.SHIELD:
                    if (CurrentShieldPoints + amount > MaxShieldPoints)
                        amount = MaxShieldPoints - CurrentShieldPoints;
                    CurrentShieldPoints += amount;
                    break;
            }

            var healPacket = "0|A|HL|" + healerId + "|" + Id + "|" + (healType == HealType.HEALTH ? "HPT" : "SHD") + "|" + CurrentHitPoints + "|" + amount;

            if (this is Player player)
            {
                if (!Invisible)
                {
                    foreach (var otherPlayers in InRangeCharacters.Values)
                        if (otherPlayers.Selected == this)
                            if (otherPlayers is Player)
                                (otherPlayers as Player).SendPacket(healPacket);
                }

                player.SendPacket(healPacket);
            }
            else if (this is Activatable)
            {
                foreach (var character in Spacemap.Characters.Values)
                    if (character.Selected == this && character is Player && character.Position.DistanceTo(Position) < RenderRange)
                        (character as Player).SendPacket(healPacket);
            }

            UpdateStatus();
        }

        public void AddVisualModifier(short modifier, int attribute, string shipLootId, int count, bool activated)
        {
            if (!VisualModifiers.ContainsKey(modifier))
            {
                VisualModifierCommand visualModifier = new VisualModifierCommand(Id, modifier, attribute, shipLootId, count, activated);

                VisualModifiers.TryAdd(visualModifier.modifier, visualModifier);

                if (this is Character)
                {
                    SendCommandToInRangePlayers(visualModifier.writeCommand());
                }
                else if (this is Activatable)
                {
                    GameManager.SendCommandToMap(Spacemap.Id, visualModifier.writeCommand());
                }

                if (this is Player player)
                {
                    player.SendCommand(visualModifier.writeCommand());

                    switch (visualModifier.modifier)
                    {
                        case VisualModifierCommand.INVINCIBILITY:
                            player.Invincible = true;
                            player.Storage.invincibilityEffectTime = DateTime.Now;
                            break;
                        case VisualModifierCommand.MIRRORED_CONTROLS:
                            player.Storage.mirroredControlEffect = true;
                            player.Storage.mirroredControlEffectTime = DateTime.Now;
                            break;
                        case VisualModifierCommand.WIZARD_ATTACK:
                            player.Storage.wizardEffect = true;
                            player.Storage.wizardEffectTime = DateTime.Now;
                            break;
                    }
                }
            }
        }

        public void RemoveVisualModifier(int modifier)
        {
            VisualModifierCommand visualModifier = VisualModifiers.FirstOrDefault(x => x.Value.modifier == modifier).Value;

            if (visualModifier != null)
            {
                VisualModifiers.TryRemove(visualModifier.modifier, out visualModifier);

                if (this is Character character)
                {
                    SendCommandToInRangePlayers(new VisualModifierCommand(visualModifier.userId, visualModifier.modifier, 0, character.Ship.LootId, 0, false).writeCommand());
                }
                else if (this is Activatable)
                {
                    GameManager.SendCommandToMap(Spacemap.Id, new VisualModifierCommand(visualModifier.userId, visualModifier.modifier, 0, "", 0, false).writeCommand());
                }

                if (this is Player player)
                {
                    player.SendCommand(new VisualModifierCommand(visualModifier.userId, visualModifier.modifier, 0, player.Ship.LootId, 0, false).writeCommand());
                }
            }
        }
    }
}

using Ow.Managers;
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Darkorbit.Game.Objects
{
    class Pet : Character
    {
        public Player Owner { get; set; }

        public override int Speed
        {
            get
            {
                return (int)(Owner.Speed);
            }

        }
        public int CurrentShieldConfig1 { get; set; }
        public int CurrentShieldConfig2 { get; set; }
        public int CurrentConfig { get; set; }

        public bool Activated = false;
        public bool AUTO_LOOTModeActive = false;
        public bool GuardModeActive = false;
        public bool KamikazeModeActive = false;
        public short GearId = PetGearTypeModule.PASSIVE;
        public int kamDamage;
        public DateTime lastTimeKami;
        public DateTime test;
        public bool kami = false;
        public bool RepairComboActive = false;
        public bool kamistarted = false;
        public bool kamiavai = true;
        public bool repavai = true;

        public bool k = true;
        public bool HPModeActive = false;
        public bool AUTOLOOTModeActive = false;
        public Character target;
        public double distance;
        public int Fuel { get; set; }
        public bool GUARD = false;
        public bool KAMIKAZE = false;
        public bool COMBO_SHIP_REPAIR = false;
        public bool REPAIR_PET = false;
        public bool AUTO_LOOT = false;

        public Pet(Player player) : base(Randoms.CreateRandomID(), "P.E.T 1", player.FactionId, GameManager.GetShip(15), player.Position, player.Spacemap, player.Clan, player.petDesign)
        {
            Name = player.PetName;
            Owner = player;

            ShieldAbsorption = 0.8;
            Damage = 8500;
            kamDamage = 75000;
            CurrentHitPoints = MaxHitPoints;
            MaxHitPoints = 150000;
            MaxShieldPoints = 300000;
            CurrentShieldConfig1 = MaxShieldPoints;
            CurrentShieldConfig2 = MaxShieldPoints;
            CurrentConfig = Owner.CurrentConfig;
            CurrentHitPoints = 5000;
            Fuel = Fuel;
        }

        public override void Tick()
        {
            if (Activated)
            {
                checkFuel(Owner);
                CheckShieldPointsRepair();
                CheckGuardMode();
                CheckAutoLoot();
                CheckKamikazeMode();
                CheckhpPointsmode();
                Follow(Owner);
                Movement.ActualPosition(this);
                CheckCloak();
                CheckRepairCombo();
                if (Owner.AttackManager.Attacking)
                {
                    // laser();

                }
            }
            if (!kamiavai)
            {


                if (lastTimeKami.AddSeconds(30) <= DateTime.Now)
                {

                    kamiavai = true;
                    Owner.SendPacket("0|A|STD|Kamikaze is now available again!");

                }

            }
            if (kamistarted && target != null)
            {
                distance = Position.DistanceTo(target.Position);
            }




            if (this.LastCombatTime.AddSeconds(1) >= DateTime.Now)
            {
                AddVisualModifier(VisualModifierCommand.FORTRESS, 0, GameManager.GetShip(22).LootId, 0, true);
            }
            else
            {
                RemoveVisualModifier(VisualModifierCommand.FORTRESS);
            }
        }
        public DateTime lastl = DateTime.Now;
        public void laser()
        {
            if (GuardModeActive)
            {
                if (lastl.AddSeconds(5) < DateTime.Now)
                {
                    var laserRunCommand = AttackLaserRunCommand.write(Id, Owner.Selected.Id, Owner.AttackManager.GetSelectedLaser(), false, false);
                    SendCommandToInRangePlayers(laserRunCommand);
                    lastl = DateTime.Now;
                }
            }
        }

        public void checkFuel(Player player)
        {
            if (player.Pet.Fuel <= 0)
            {
                Deactivate();
                player.SendPacket($"0|A|STD|Your pet not have fuel. Fuel: {player.Pet.Fuel}");
            }
            UpdateStatus();
        }

        public void startFuel()
        {
            var task = Task.Run(async () => {
                for (; ; )
                {
                    await Task.Delay(60000);

                    if (!Activated)
                        break;

                    if (Owner.Premium)
                    {
                        Owner.Pet.Fuel -= 25;
                    }
                    else
                    {
                        Owner.Pet.Fuel -= 50;
                    }

                    UpdateStatus();
                }
            });
        }

        public void CheckhpPointsmode()
        {
            if (HPModeActive)
            {
                if (LastCombatTime.AddSeconds(10) >= DateTime.Now || lastShieldRepairTime.AddSeconds(1) >= DateTime.Now || CurrentHitPoints == MaxHitPoints) return;

                int vida = 5000;
                CurrentHitPoints += vida;
                UpdateStatus();
                lastShieldRepairTime = DateTime.Now;

            }
            else
            {
                //HPModeActive = OFF;


            }
        }

        public void CheckCloak()
        {
            if (lastAttackTime.AddSeconds(1) >= DateTime.Now)
            {
                Owner.Pet.Invisible = false;
                Owner.Pet.SendPacketToInRangePlayers("0|n|INV|" + Owner.Pet.Id + "|0");
            }
            else
            {
                if (Owner.Pet != null && Owner.Pet.Activated)
                {
                    if (Owner.Invisible)
                    {
                        Owner.Pet.Invisible = true;
                        Owner.Pet.SendPacketToInRangePlayers("0|n|INV|" + Owner.Pet.Id + "|1");
                    }
                    else
                    {
                        Owner.Pet.Invisible = false;
                        Owner.Pet.SendPacketToInRangePlayers("0|n|INV|" + Owner.Pet.Id + "|0");
                    }
                }
            }

        }
        public override int CurrentShieldPoints
        {
            get
            {
                var value = CurrentConfig == 1 ? CurrentShieldConfig1 : CurrentShieldConfig2;
                return value;
            }
            set
            {
                if (CurrentConfig == 1)
                    CurrentShieldConfig1 = value;
                else
                    CurrentShieldConfig2 = value;
            }
        }



        public DateTime lastRepair = new DateTime();
        public void CheckRepairCombo()
        {

            if (RepairComboActive)
            {
                if (lastRepair.AddSeconds(30) < DateTime.Now || lastRepair == new DateTime())
                {
                    if (!Owner.AttackingOrUnderAttack())
                    {
                        RepairComboAsync();
                        lastRepair = DateTime.Now;
                    }
                    else
                    {
                        GearId = PetGearTypeModule.GUARD;
                        Owner.SendCommand(PetGearSelectCommand.write(new PetGearTypeModule(PetGearTypeModule.GUARD), new List<int>()));
                        RepairComboActive = false;
                    }
                }
                else
                {
                    GearId = PetGearTypeModule.GUARD;
                    Owner.SendCommand(PetGearSelectCommand.write(new PetGearTypeModule(PetGearTypeModule.GUARD), new List<int>()));
                    RepairComboActive = false;
                }
            }
        }

        public async Task RepairComboAsync()
        {

            for (int i = 0; i < 10; i++)
            {
                if (!Owner.AttackingOrUnderAttack(1))
                {
                    Owner.Heal(25000, Id);
                    await Task.Delay(1000);
                }
            }
            GearId = PetGearTypeModule.GUARD;
            Owner.SendCommand(PetGearSelectCommand.write(new PetGearTypeModule(PetGearTypeModule.GUARD), new List<int>()));

        }

        public void CheckAutoLoot()
        {
            //TODO
        }

        public DateTime lastShieldRepairTime = new DateTime();
        private void CheckShieldPointsRepair()
        {
            if (LastCombatTime.AddSeconds(5) >= DateTime.Now || lastShieldRepairTime.AddSeconds(1) >= DateTime.Now || CurrentShieldPoints == MaxShieldPoints) return;

            int repairShield = MaxShieldPoints / 15;
            CurrentShieldPoints += repairShield;
            UpdateStatus();

            lastShieldRepairTime = DateTime.Now;
        }

        public DateTime lastAttackTime = new DateTime();
        public DateTime lastRSBAttackTime = new DateTime();
        public void CheckGuardMode()
        {
            if (GuardModeActive)
            {
                if (Owner.AttackManager.Attacking && Owner.Selected != this)
                {
                    Attack(Owner.Selected as Character);
                }
                else
                {
                    foreach (var enemy in Owner.InRangeCharacters.Values)
                    {
                        if (Owner.SelectedCharacter != null && Owner.SelectedCharacter != this && Owner.AttackManager.Attacking && Owner.Selected == enemy)
                        {
                            Attack(Owner.SelectedCharacter);
                        }
                        else
                        {
                            if (enemy.LastCombatTime.AddSeconds(1) <= DateTime.Now && enemy.Selected == Owner && Owner.AttackingOrUnderAttack(3) && Owner.DamageRecieved.ContainsKey(enemy) && Owner.DamageRecieved[enemy].LastAttack.AddSeconds(3) >= DateTime.Now)
                                Attack(enemy);
                        }
                    }
                }
            }
        }

        public void CheckKamikazeMode()
        {
            if (KamikazeModeActive)
            {
                if (Owner.AttackingOrUnderAttack(5) && Owner.MainAttacker != null)
                {
                    Kamikaze(Owner.MainAttacker);
                }
                else
                {
                    foreach (var c in Owner.InRangeCharacters.Values)
                    {
                        if (c.Selected == Owner && !c.Destroyed && Owner.AttackingOrUnderAttack(3) && Owner.DamageRecieved.Any(x => x.Key == c))
                        {
                            Kamikaze(c);
                        }
                    }
                }
            }
        }

        private async void Attack(Character target)
        {
            Damage = 8500;

            if (target is Pet pet2)
            {
                if (pet2.Owner.Storage.IsInDemilitarizedZone)
                {
                    return;
                }

                if ((pet2.Owner.Spacemap.Id == 1) || (pet2.Owner.Spacemap.Id == 2) || (pet2.Owner.Spacemap.Id == 3) || (pet2.Owner.Spacemap.Id == 4) || (pet2.Owner.Spacemap.Id == 5) || (pet2.Owner.Spacemap.Id == 6) || (pet2.Owner.Spacemap.Id == 7) || (pet2.Owner.Spacemap.Id == 8) || (pet2.Owner.Spacemap.Id == 9) || (pet2.Owner.Spacemap.Id == 10) || (pet2.Owner.Spacemap.Id == 11) || (pet2.Owner.Spacemap.Id == 12) || (pet2.Owner.Spacemap.Id == 17) || (pet2.Owner.Spacemap.Id == 18) || (pet2.Owner.Spacemap.Id == 19) || (pet2.Owner.Spacemap.Id == 20) || (pet2.Owner.Spacemap.Id == 21) || (pet2.Owner.Spacemap.Id == 22) || (pet2.Owner.Spacemap.Id == 23) || (pet2.Owner.Spacemap.Id == 24) || (pet2.Owner.Spacemap.Id == 25) || (pet2.Owner.Spacemap.Id == 26) || (pet2.Owner.Spacemap.Id == 27) || (pet2.Owner.Spacemap.Id == 28) || (pet2.Owner.Spacemap.Id == 29))
                {

                    if (pet2.Owner.Level < 18)
                    {
                        return;
                    }

                    if (Owner.Level < 18 && pet2.Owner.Level > 18)
                    {
                        return;
                    }

                }

            }

            if (target is Player target2)
            {
                if (target2.Storage.IsInDemilitarizedZone)
                {
                    return;
                }

                if ((target2.Spacemap.Id == 1) || (target2.Spacemap.Id == 2) || (target2.Spacemap.Id == 3) || (target2.Spacemap.Id == 4) || (target2.Spacemap.Id == 5) || (target2.Spacemap.Id == 6) || (target2.Spacemap.Id == 7) || (target2.Spacemap.Id == 8) || (target2.Spacemap.Id == 9) || (target2.Spacemap.Id == 10) || (target2.Spacemap.Id == 11) || (target2.Spacemap.Id == 12) || (target2.Spacemap.Id == 17) || (target2.Spacemap.Id == 18) || (target2.Spacemap.Id == 19) || (target2.Spacemap.Id == 20) || (target2.Spacemap.Id == 21) || (target2.Spacemap.Id == 22) || (target2.Spacemap.Id == 23) || (target2.Spacemap.Id == 24) || (target2.Spacemap.Id == 25) || (target2.Spacemap.Id == 26) || (target2.Spacemap.Id == 27) || (target2.Spacemap.Id == 28) || (target2.Spacemap.Id == 29))
                {

                    if (target2.Level < 18)
                    {
                        return;
                    }

                    if (Owner.Level < 18 && target2.Level > 18)
                    {
                        return;
                    }

                }

            }

            if (!TargetDefinition(target, false)) return;
            if (target is Player player && (player.AttackManager.EmpCooldown.AddMilliseconds(TimeManager.EMP_DURATION) > DateTime.Now)) return;
            if (Position.DistanceTo(target.Position) < 200)
            {
                Follow(target);
            }


            if ((Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75 ? lastRSBAttackTime : lastAttackTime).AddSeconds(Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75 ? 3 : 1) < DateTime.Now)
            {
                if (Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75)
                {
                    Damage = 10000;
                }
                else if (Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 || Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.MCB_50)
                {
                    Damage = 8500;
                }

                if (Owner.AttackManager.scatter)
                {
                    Damage = (int)(Damage * 1.5);
                }

                int damageShd = 0, damageHp = 0;

                if (target is Spaceball)
                {
                    var spaceball = target as Spaceball;
                    spaceball.AddDamage(this, Damage);
                }

                if (target is Hitac)
                {
                    var spaceball = target as Hitac;
                    spaceball.AddDamage(this, Damage);
                }

                double shieldAbsorb = 0.8;

                if (shieldAbsorb > 1)
                    shieldAbsorb = 1;

                if ((target.CurrentShieldPoints - Damage) >= 0)
                {
                    damageShd = (int)(Damage * shieldAbsorb);
                    damageHp = Damage - damageShd;
                }
                else
                {
                    int newDamage = Damage - target.CurrentShieldPoints;
                    damageShd = target.CurrentShieldPoints;
                    damageHp = (int)(newDamage + (damageShd * shieldAbsorb));
                }

                if ((target.CurrentHitPoints - damageHp) < 0)
                {
                    damageHp = target.CurrentHitPoints;
                }

                if (target is Player && !(target as Player).Attackable())
                {
                    Damage = 0;
                    damageShd = 0;
                    damageHp = 0;
                }

                if (Invisible)
                {
                    Invisible = false;
                    string cloakPacket = "0|n|INV|" + Id + "|0";
                    SendPacketToInRangePlayers(cloakPacket);
                }

                if (target is Player && (target as Player).Storage.Sentinel)
                    damageShd -= Maths.GetPercentage(damageShd, 30);



                var laserRunCommand = AttackLaserRunCommand.write(Id, target.Id, Owner.AttackManager.scatter ? 0 : Owner.AttackManager.GetSelectedLaser(), false, false);
                SendCommandToInRangePlayers(laserRunCommand);
                if (Owner.AttackManager.scatter)
                {
                    Owner.AttackManager.scatter = false;
                    Owner.AttackManager.lastScatter = DateTime.Now;
                }

                var attackHitCommand =
                        AttackHitCommand.write(new AttackTypeModule(AttackTypeModule.LASER), Id,
                                             target.Id, target.CurrentHitPoints,
                                             target.CurrentShieldPoints, target.CurrentNanoHull,
                                             Damage > AttackManager.RandomizeDamage(damageShd, 0.1) ? AttackManager.RandomizeDamage(Damage, 0.1) : AttackManager.RandomizeDamage(damageShd, 0.1), false);

                SendCommandToSelected(target, attackHitCommand);
                damageHp = AttackManager.RandomizeDamage(damageHp, 0.1);
                damageShd = AttackManager.RandomizeDamage(damageShd, 0.1);
                if (damageHp >= target.CurrentHitPoints || target.CurrentHitPoints == 0)
                {

                    if (target is NpcGG)
                    {
                        target.Destroy(Owner, DestructionType.NPC);
                    }
                    else if (target is Npcx2)
                    {
                        target.Destroy(Owner, DestructionType.NPC);
                    }
                    else
                    {
                        target.Destroy(this, DestructionType.PLAYER);
                    }

                }
                else
                {
                    target.CurrentHitPoints -= AttackManager.RandomizeDamage(damageHp, 0.1);
                }

                target.CurrentShieldPoints -= AttackManager.RandomizeDamage(damageShd, 0.1);
                target.LastCombatTime = DateTime.Now;

                if (Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75)
                    lastRSBAttackTime = DateTime.Now;
                else
                    lastAttackTime = DateTime.Now;

                target.UpdateStatus();
                target.AddDamage(this, damageShd);

                await Task.Delay(500);

            }
        }

        private void Kamikaze(Character target)
        {

            if (lastTimeKami == test)
            {
                FollowEnemyAsync(target);
            }
            else if (lastTimeKami.AddSeconds(30) <= DateTime.Now)
            {
                FollowEnemyAsync(target);
            }
            else
            {
                GearId = PetGearTypeModule.GUARD;
                Owner.SendCommand(PetGearSelectCommand.write(new PetGearTypeModule(PetGearTypeModule.GUARD), new List<int>()));
            }


        }

        private async Task FollowEnemyAsync(Character target)
        {
            if (kamiavai && !kamistarted)
            {


                var startkami = DateTime.Now;
                kamistarted = true;

                this.target = target;

                DateTime start = DateTime.Now;
                GameManager.SendPacketToAll($"0|n|fx|start|RAGE|{Id}");
                int a = 0;
                bool b = true;
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        a = 1300;
                    }
                    else if (i > 0)
                    {
                        a = 1200;
                    }

                    if (Owner.Speed > 450)
                    {
                        a += 250;
                    }
                    //Movement.Move(this, target.Position, a);
                    if (i >= 2 && distance < 150)
                    {
                        Explode(target);
                        b = false;
                        break;

                    }
                    if (b)
                        await Task.Delay(1000);


                }
                if (b)
                    Explode(target);
            }

        }

        public void Explode(Character target)
        {
            kamDamage = 75000;

            foreach (var c in this.InRangeCharacters.Where(n => Position.DistanceTo(n.Value.Position) < 1000))
            {
                if (c.Value != Owner)
                {

                    if (c.Value is Player && !(c.Value as Player).Attackable())
                    {
                        kamDamage = 0;
                        Damage = 0;

                    }
                    var distance = Position.DistanceTo(c.Value.Position);
                    if (distance > 400)
                    {
                        if (distance < 850)
                        {
                            kamDamage = Maths.GetPercentage(kamDamage, 85);
                            kamDamage = AttackManager.RandomizeDamage(kamDamage, 0);
                        }
                        else
                        {
                            kamDamage = Maths.GetPercentage(kamDamage, 65);
                            kamDamage = AttackManager.RandomizeDamage(kamDamage, 0);
                        }
                    }

                    c.Value.CurrentHitPoints -= Maths.GetPercentage(kamDamage, 95);
                    c.Value.CurrentShieldPoints -= Maths.GetPercentage(kamDamage, 5);
                    if (c.Value.CurrentHitPoints - Maths.GetPercentage(kamDamage, 95) <= 0 || c.Value.CurrentHitPoints <= 0)
                    {
                        c.Value.Destroy(Owner, DestructionType.PLAYER);
                    }
                    c.Value.UpdateStatus();
                    var attackHitCommand =
                            AttackHitCommand.write(new AttackTypeModule(AttackTypeModule.KAMIKAZE), Id,
                                                    c.Value.Id, c.Value.CurrentHitPoints,
                                                        c.Value.CurrentShieldPoints, c.Value.CurrentNanoHull,
                                                    Damage > kamDamage ? Damage : kamDamage, false);

                    SendCommandToSelected(c.Value, attackHitCommand);

                }
            }

            Destroy(this, DestructionType.PET);
            kamistarted = false;
            kamiavai = false;
            target = null;
            lastTimeKami = DateTime.Now;


        }
        public void Activate()
        {
            if (!Activated && !Owner.Settings.InGameSettings.petDestroyed && (Owner.Spacemap.Id != 42 || Owner.Spacemap.Id != 224))
            {
                Activated = true;

                SetPosition(Owner.Position);
                Spacemap = Owner.Spacemap;
                Invisible = Owner.Invisible;

                Owner.SendPacket("0|A|STM|msg_pet_activated");

                Initialization(GearId);

                Spacemap.AddCharacter(this);
                Program.TickManager.AddTick(this);

                startFuel();
            }
            else
            {
                Deactivate();
            }
        }

        public void Activate2(String NamePet, short PetDesignn)
        {
            if ((Owner.Spacemap.Id != 42 || Owner.Spacemap.Id != 224))
            {
                if (Activated == true)
                    Deactivate();

                Activated = true;
                Name = NamePet;
                petDesign = PetDesignn;

                SetPosition(Owner.Position);
                Spacemap = Owner.Spacemap;
                Invisible = Owner.Invisible;

                Owner.SendPacket("0|A|STM|msg_pet_activated");

                Initialization(GearId);

                Spacemap.AddCharacter(this);
                Program.TickManager.AddTick(this);

                startFuel();

            }
        }

        public void RepairDestroyed()
        {
            if (Owner.Settings.InGameSettings.petDestroyed)
            {
                var cost = Owner.Premium ? 0 : 250;

                if (Owner.Data.uridium >= cost)
                {
                    Destroyed = false;
                    Owner.LoadData();
                    Owner.ChangeData(DataType.URIDIUM, cost, ChangeType.DECREASE);
                    Owner.SendCommand(PetRepairCompleteCommand.write());
                    Owner.Settings.InGameSettings.petDestroyed = false;
                    QueryManager.SavePlayer.Settings(Owner, "inGameSettings", Owner.Settings.InGameSettings);
                }
                else Owner.SendPacket("0|A|STM|ttip_pet_repair_disabled_through_money");
            }
        }

        public void Deactivate(bool direct = false, bool destroyed = false)
        {
            if (Activated)
            {
                if (LastCombatTime.AddSeconds(10) < DateTime.Now || direct)
                {
                    Owner.SendPacket("0|PET|D");

                    if (destroyed)
                    {
                        Owner.Settings.InGameSettings.petDestroyed = true;
                        QueryManager.SavePlayer.Settings(Owner, "inGameSettings", Owner.Settings.InGameSettings);

                        Owner.SendPacket("0|PET|Z");
                        CurrentHitPoints = 5000;
                        UpdateStatus();

                        Owner.SendCommand(PetInitializationCommand.write(true, true, false));
                        Owner.SendCommand(PetUIRepairButtonCommand.write(true, 250));
                    }
                    else Owner.SendPacket("0|A|STM|msg_pet_deactivated");

                    Activated = false;

                    Deselection();
                    Spacemap.RemoveCharacter(this);
                    InRangeCharacters.Clear();
                    Program.TickManager.RemoveTick(this);
                }
                else
                {
                    Owner.SendPacket("0|A|STM|msg_pet_in_combat");
                }
            }
            GearId = PetGearTypeModule.PASSIVE;
        }

        private void Initialization(short gearId = PetGearTypeModule.PASSIVE)
        {
            Owner.SendCommand(PetStatusCommand.write(Id, 15, 27000000, 27000000, CurrentHitPoints, MaxHitPoints, CurrentShieldPoints, MaxShieldPoints, Fuel, 50000, Speed, Name));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.PASSIVE), 0, 0, true));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.GUARD), 0, 0, GUARD));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.KAMIKAZE), 3, 0, KAMIKAZE));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.COMBO_SHIP_REPAIR), 3, 0, COMBO_SHIP_REPAIR));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.REPAIR_PET), 0, 0, REPAIR_PET));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.AUTO_LOOT), 0, 0, false));
            SwitchGear(gearId);
        }

        private void Follow(Character character)
        {
            var distance = Position.DistanceTo(character.Position);
            if (distance < 450 && character.Moving) return;
            if (KamikazeModeActive && lastTimeKami.AddSeconds(30) <= DateTime.Now && kamistarted) return;
            if (character.Moving)
            {
                Movement.Move(this, character.Position);
            }
            else if (Math.Abs(distance - 300) > 250 && !Moving)
                Movement.Move(this, Position.GetPosOnCircle(character.Position, 50));
        }



        public void SwitchGear(short gearId)
        {
            if (!Activated)
                Activate();

            switch (gearId)
            {
                case PetGearTypeModule.PASSIVE:
                    GuardModeActive = false;
                    KamikazeModeActive = false;
                    RepairComboActive = false;
                    HPModeActive = false;
                    break;
                case PetGearTypeModule.GUARD:
                    GuardModeActive = true;
                    KamikazeModeActive = false;
                    RepairComboActive = false;
                    HPModeActive = false;
                    break;
                case PetGearTypeModule.KAMIKAZE:
                    KamikazeModeActive = true;
                    RepairComboActive = false;
                    HPModeActive = false;
                    GuardModeActive = false;
                    break;
                case PetGearTypeModule.COMBO_SHIP_REPAIR:
                    RepairComboActive = true;
                    GuardModeActive = false;
                    HPModeActive = false;
                    KamikazeModeActive = false;
                    break;
                case PetGearTypeModule.REPAIR_PET:
                    HPModeActive = true;
                    KamikazeModeActive = false;
                    RepairComboActive = false;
                    GuardModeActive = false;
                    break;
                case PetGearTypeModule.AUTO_LOOT:
                    AUTOLOOTModeActive = true;
                    KamikazeModeActive = false;
                    RepairComboActive = false;
                    GuardModeActive = false;
                    break;
            }
            GearId = gearId;

            Owner.SendCommand(PetGearSelectCommand.write(new PetGearTypeModule(gearId), new List<int>()));
        }

        public override byte[] GetShipCreateCommand() { return null; }
    }
}

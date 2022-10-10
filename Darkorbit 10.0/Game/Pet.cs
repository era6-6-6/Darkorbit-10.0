using Darkorbit.Game.Movements;
using Darkorbit.Game.Objects;
using Darkorbit.Game.Objects.Players.Managers;
using Darkorbit.Managers;
using Darkorbit.Net.netty.commands;
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game
{
    class Pet : Character
    {
        public Player Owner { get; set; }

        public override int Speed
        {
            get
            {
                return (int)(Owner.Speed * 1.25);
            }
        }

        public bool Activated = false;
        public bool GuardModeActive = false;
        public short GearId = PetGearTypeModule.PASSIVE;

        public Pet(Player player) : base(Randoms.CreateRandomID(), "P.E.T 15", player.FactionId, GameManager.GetShip(22), player.Position, player.Spacemap, player.Clan , 1)
        {
            Name = player.PetName;
            Owner = player;

            ShieldAbsorption = 0.8;
            Damage = 2000;
            //Damage = 5000;
            CurrentHitPoints = 250000;
            //CurrentHitPoints = 2500;
            MaxHitPoints = 250000;
            //MaxHitPoints = 50000;
            MaxShieldPoints = 250000;
            //MaxShieldPoints = 50000;
            CurrentShieldPoints = MaxShieldPoints;
        }

        public override void Tick()
        {
            if (Activated)
            {
                CheckShieldPointsRepair();
                CheckHitPointsRepair();
                CheckGuardMode();
                CheckAutoLoot();
                Follow(Owner);
                Movement.ActualPosition(this);
            }
        }

        public void CheckAutoLoot()
        {
            //TODO
        }

        public DateTime lastShieldRepairTime = new DateTime();
        private void CheckShieldPointsRepair()
        {
            if (LastCombatTime.AddSeconds(10) >= DateTime.Now || lastShieldRepairTime.AddSeconds(1) >= DateTime.Now || CurrentShieldPoints == MaxShieldPoints) return;

            int repairShield = MaxShieldPoints / 25;
            CurrentShieldPoints += repairShield;
            UpdateStatus();

            lastShieldRepairTime = DateTime.Now;
        }

        public DateTime lastHitPointsRepairTime = new DateTime();
        private void CheckHitPointsRepair()
        {
            if (LastCombatTime.AddSeconds(10) >= DateTime.Now || lastHitPointsRepairTime.AddSeconds(1) >= DateTime.Now || CurrentHitPoints == MaxHitPoints) return;

            int repairHitPoints = MaxHitPoints / 25;
            CurrentHitPoints += repairHitPoints;
            UpdateStatus();

            lastHitPointsRepairTime = DateTime.Now;
        }

        public DateTime lastAttackTime = new DateTime();
        public DateTime lastRSBAttackTime = new DateTime();
        public void CheckGuardMode()
        {
            if (GuardModeActive)
            {
                foreach (var enemy in Owner.InRangeCharacters.Values)
                {
                    if (Owner.SelectedCharacter != null && Owner.SelectedCharacter != this)
                    {
                        if ((Owner.AttackingOrUnderAttack(5) || Owner.LastAttackTime(5)) || ((enemy is Player && (enemy as Player).LastAttackTime(5)) && enemy.SelectedCharacter == Owner))
                            Attack(Owner.SelectedCharacter);
                    }
                    else
                    {
                        if (((enemy is Player && (enemy as Player).LastAttackTime(5)) && enemy.SelectedCharacter == Owner))
                            Attack(enemy);
                    }
                }
            }
        }

        private void Attack(Character target)
        {
            if (!Owner.TargetDefinition(target, false)) return;
            if ((Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75 ? lastRSBAttackTime : lastAttackTime).AddSeconds(Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75 ? 3 : 1) < DateTime.Now)
            {
                int damageShd = 0, damageHp = 0, randomKeyNumber = 0;
                Random random = new Random();
                var selectedLaser = Owner.Settings.InGameSettings.selectedLaser;

                //because the PET cannot be equipped for now on equipment, It have a minimum of 1920 of damage per shot with x1 and it multiplies as well depending on the selected ammo.
                //and the damage will be random following these values below
                int[] damageNumbers = { 1920, 1985, 2049, 2074, 2129, 2287, 2320, 2399, 2441, 2539, 2610, 2774, 2856, 2929, 2980, 3055, 3120 };

                switch (selectedLaser) {
                    case AmmunitionManager.MCB_25:
                        randomKeyNumber = random.Next(damageNumbers.Length);
                        Damage = damageNumbers[randomKeyNumber] * 2;
                        break;
                    case AmmunitionManager.MCB_50:
                        randomKeyNumber = random.Next(damageNumbers.Length);
                        Damage = damageNumbers[randomKeyNumber] * 3;
                        break;
                    case AmmunitionManager.UCB_100:
                        randomKeyNumber = random.Next(damageNumbers.Length);
                        Damage = damageNumbers[randomKeyNumber] * 4;
                        break;
                    case AmmunitionManager.SAB_50:
                        randomKeyNumber = random.Next(damageNumbers.Length);
                        Damage = damageNumbers[randomKeyNumber] * 2;
                        break;
                    case AmmunitionManager.RSB_75:
                        randomKeyNumber = random.Next(damageNumbers.Length);
                        Damage = damageNumbers[randomKeyNumber] * 6;
                        break;
                    default:
                        randomKeyNumber = random.Next(damageNumbers.Length);
                        Damage = damageNumbers[randomKeyNumber];
                        break;
                }

                if (target is Spaceball)
                {
                    var spaceball = target as Spaceball;
                    spaceball.AddDamage(this, Damage);
                }

                //get target shield absorption
                double shieldAbsorb = System.Math.Abs(target.ShieldAbsorption);

                if (shieldAbsorb > 1)
                    shieldAbsorb = 1;

                if ((target.CurrentShieldPoints - Damage) >= 0)
                {
                    //if the target still has shield hit points, it will take the appropriate damage according their absorption.
                    damageShd = (int)(Damage * shieldAbsorb);
                    if (Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50) 
                    {
                        damageHp = 0;
                        damageShd = Damage;
                        CurrentShieldPoints += Damage;
                    } else 
                    {
                        damageHp = Damage - damageShd;
                    }
                }
                else
                {
                    //the shield receives as damage the current shield hit points (to avoid values below 0) and the HP of the target receives the rest.
                    int newDamage = Damage - target.CurrentShieldPoints;
                    damageShd = target.CurrentShieldPoints;
                    if (Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50)
                    {
                        damageHp = 0;
                        CurrentShieldPoints += damageShd;
                    } else
                    {
                        damageHp = (int)(newDamage + (damageShd * shieldAbsorb));
                    }
                }

                if ((target.CurrentHitPoints - damageHp) < 0)
                {
                    //to avoid values below 0.
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

                var laserRunCommand = AttackLaserRunCommand.write(Id, target.Id, Owner.AttackManager.GetSelectedLaser(), false, false);
                SendCommandToInRangePlayers(laserRunCommand);

                var attackHitCommand =
                        AttackHitCommand.write(new AttackTypeModule(AttackTypeModule.LASER), Id,
                                             target.Id, target.CurrentHitPoints,
                                             target.CurrentShieldPoints, target.CurrentNanoHull,
                                             Damage > damageShd ? Damage : damageShd, false);

                SendCommandToInRangePlayers(attackHitCommand);

                if (damageHp >= target.CurrentHitPoints || target.CurrentHitPoints == 0)
                    target.Destroy(this, DestructionType.PET);
                else
                    target.CurrentHitPoints -= damageHp;

                target.CurrentShieldPoints -= damageShd;
                target.LastCombatTime = DateTime.Now;

                if (Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75)
                    lastRSBAttackTime = DateTime.Now;
                else
                    lastAttackTime = DateTime.Now;

                target.UpdateStatus();
                UpdateStatus();
            }
        }

        public void Activate()
        {
            if (!Activated && !Owner.Settings.InGameSettings.petDestroyed)
            {
                Activated = true;

                CurrentHitPoints = 250000;

                SetPosition(Owner.Position);
                Spacemap = Owner.Spacemap;
                Invisible = Owner.Invisible;

                Owner.SendPacket("0|A|STM|msg_pet_activated");

                Initialization(GearId);

                Spacemap.AddCharacter(this);
                Program.TickManager.AddTick(this);
            }
            else
            {
                Deactivate();
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
                    Owner.ChangeData(DataType.URIDIUM, cost, ChangeType.DECREASE);
                    Owner.SendCommand(PetRepairCompleteCommand.write());
                    Owner.Settings.InGameSettings.petDestroyed = false;
                    QueryManager.SavePlayer.Settings(Owner, "inGameSettings", Owner.Settings.InGameSettings);
                } else Owner.SendPacket("0|A|STM|ttip_pet_repair_disabled_through_money");
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
                        CurrentShieldPoints = 0;
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
        }

        private void Initialization(short gearId = PetGearTypeModule.PASSIVE)
        {
            Owner.SendCommand(PetStatusCommand.write(Id, 15, 27000000, 27000000, CurrentHitPoints, MaxHitPoints, CurrentShieldPoints, MaxShieldPoints, 50000, 50000, Speed, Name));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.PASSIVE), 0, 0, true));
            Owner.SendCommand(PetGearAddCommand.write(new PetGearTypeModule(PetGearTypeModule.GUARD), 0, 0, true));
            SwitchGear(gearId);
        }

        private void Follow(Character character)
        {
            var distance = Position.DistanceTo(character.Position);
            if (distance < 450 && character.Moving) return;

            if (character.Moving)
            {
                Movement.Move(this, character.Position);
            }
            else if (Math.Abs(distance - 300) > 250 && !Moving)
                Movement.Move(this, Position.GetPosOnCircle(character.Position, 250));
        }

        public void SwitchGear(short gearId)
        {
            if (!Activated)
                Activate();

            switch (gearId)
            {
                case PetGearTypeModule.PASSIVE:
                    GuardModeActive = false;
                    break;
                case PetGearTypeModule.GUARD:
                    GuardModeActive = true;
                    break;
            }
            GearId = gearId;

            Owner.SendCommand(PetGearSelectCommand.write(new PetGearTypeModule(gearId), new List<int>()));
        }

        public override byte[] GetShipCreateCommand() { return null; }
    }
}

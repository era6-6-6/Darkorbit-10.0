using Darkorbit.Game.Objects.AI;

namespace Darkorbit.Game.Objects
{
    internal class Npcx2 : Character
    {
        public NpcAi2 NpcAi2 { get; set; }
        public bool Attacking = false;

        public bool UnderEmp = false;
        public DateTime startemp = new DateTime();
        public Player EmpFrom;

        public Npcx2(int id, Ship ship, Spacemap spacemap, Position position) : base(id, ship.Name, 0, ship, position, spacemap, GameManager.GetClan(0), 22)
        {
            Spacemap.AddCharacter(this);

            ShieldAbsorption = 0.8;

            Damage = ship.Damage;
            MaxHitPoints = ship.BaseHitpoints;
            CurrentHitPoints = MaxHitPoints;
            MaxShieldPoints = ship.BaseShieldPoints;
            CurrentShieldPoints = MaxShieldPoints;

            NpcAi2 = new NpcAi2(this);

            Program.TickManager.AddTick(this);
        }

        public override void Tick()
        {
            Movement.ActualPosition(this);
            NpcAi2.TickAI();
            CheckShieldPointsRepair();
            Storage.Tick();
            RefreshAttackers();

            if (Attacking)
            {
                Attack();
            }
        }

        public DateTime lastAttackTime = new DateTime();
        public void Attack()
        {
            int damage = AttackManager.RandomizeDamage(Damage, (Storage.underPLD8 ? 0.5 : 0.1));
            Character target = SelectedCharacter;

            if (!TargetDefinition(target, false))
            {
                return;
            }

            if (target is Player player && player.AttackManager.EmpCooldown.AddMilliseconds(TimeManager.EMP_DURATION) > DateTime.Now)
            {
                return;
            }

            if (lastAttackTime.AddSeconds(1) < DateTime.Now)
            {
                if (target is Player && (target as Player).Storage.Spectrum)
                {
                    damage -= Maths.GetPercentage(damage, 50);
                }

                int damageShd = 0, damageHp = 0;

                double shieldAbsorb = System.Math.Abs(target.ShieldAbsorption - 0);

                if (shieldAbsorb > 1)
                {
                    shieldAbsorb = 1;
                }

                if ((target.CurrentShieldPoints - damage) >= 0)
                {
                    damageShd = (int)(damage * shieldAbsorb);
                    damageHp = damage - damageShd;
                }
                else
                {
                    int newDamage = damage - target.CurrentShieldPoints;
                    damageShd = target.CurrentShieldPoints;
                    damageHp = (int)(newDamage + (damageShd * shieldAbsorb));
                }

                if ((target.CurrentHitPoints - damageHp) < 0)
                {
                    damageHp = target.CurrentHitPoints;
                }

                if (target is Player && !(target as Player).Attackable())
                {
                    damage = 0;
                    damageShd = 0;
                    damageHp = 0;
                }

                if (target is Player && (target as Player).Storage.Sentinel)
                    damageShd -= Maths.GetPercentage(damageShd, 30);

                var laserRunCommand = AttackLaserRunCommand.write(Id, target.Id, 0, false, false);
                SendCommandToInRangePlayers(laserRunCommand);

                if (damage == 0)
                {
                    byte[] attackMissedCommandToInRange = AttackMissedCommand.write(new AttackTypeModule(AttackTypeModule.LASER), target.Id, 1);
                    SendCommandToInRangePlayers(attackMissedCommandToInRange);
                }
                else
                {
                    byte[] attackHitCommand =
                        AttackHitCommand.write(new AttackTypeModule(AttackTypeModule.LASER), Id,
                             target.Id, target.CurrentHitPoints,
                             target.CurrentShieldPoints, target.CurrentNanoHull,
                             damage > damageShd ? damage : damageShd, false);

                    SendCommandToInRangePlayers(attackHitCommand);
                }

                if (damageHp >= target.CurrentHitPoints || target.CurrentHitPoints <= 0)
                    target.Destroy(this, DestructionType.NPC);
                else
                    target.CurrentHitPoints -= damageHp;

                target.CurrentShieldPoints -= damageShd;
                target.LastCombatTime = DateTime.Now;

                lastAttackTime = DateTime.Now;
                target.AddDamage(this, damage);
                target.UpdateStatus();
            }
        }
        public DateTime lastShieldRepairTime = new DateTime();
        private void CheckShieldPointsRepair()
        {
            if (LastCombatTime.AddSeconds(10) >= DateTime.Now || lastShieldRepairTime.AddSeconds(1) >= DateTime.Now || CurrentShieldPoints == MaxShieldPoints)
            {
                return;
            }

            int repairShield = MaxShieldPoints / 10;
            CurrentShieldPoints += repairShield;
            UpdateStatus();

            lastShieldRepairTime = DateTime.Now;
        }

        public void Respawn()
        {
            LastCombatTime = DateTime.Now.AddSeconds(-999);
            CurrentHitPoints = MaxHitPoints;
            CurrentShieldPoints = MaxShieldPoints;
            SetPosition(Position.Random(Spacemap, 0, 41800, 0, 26000));

            Spacemap.AddCharacter(this);
            Attackers.Clear();
            MainAttacker = null;
            Destroyed = false;

        }

        public void ReceiveAttack(Character character)
        {
            Selected = character;
            Attacking = true;
        }

        public override int Speed
        {
            get
            {
                var value = Ship.BaseSpeed;

                if (Storage.underR_IC3)
                    value = 0;

                return value;
            }
        }

        public override byte[] GetShipCreateCommand()
        {
            return ShipCreateCommand.write(
                Id,
                Convert.ToString(Ship.Id),
                3,
                "",
                Ship.Name,
                Position.X,
                Position.Y,
                FactionId,
                0,
                0,
                false,
                new ClanRelationModule(ClanRelationModule.AT_WAR),
                0,
                false,
                true,
                false,
                ClanRelationModule.AT_WAR,
                ClanRelationModule.AT_WAR,
                new List<VisualModifierCommand>(),
                new class_11d(class_11d.DEFAULT)
                );
        }
    }
}

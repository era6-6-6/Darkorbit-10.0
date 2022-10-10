using Darkorbit.Game.Objects.AI;

namespace Darkorbit.Game.Objects
{
    class NpcDamageCause
    {
        public Player player = null;
        public double damage = 0f;

        public NpcDamageCause(Player p, double d)
        {
            player = p;
            damage = d;
        }
    }

    internal class Npc : Character
    {
        public NpcAI NpcAI { get; set; }
        public bool Attacking = false;

        public bool UnderEmp = false;
        public bool Empowered = false;
        public DateTime lastEmpowered = new DateTime();
        public DateTime startemp = new DateTime();
        public Player EmpFrom;

        public bool Proti = true;
        public int tmpId;
        public DateTime diesAfter;
        public List<Npc> childs = new List<Npc>();
        public Npc mother;
        public Position destPosition = null;
        public static int switchEnemyDefault = 5;
        public int switchEnemy = switchEnemyDefault;
        public Player cubiMainAttacker;
        public List<NpcDamageCause> causedDamage = new List<NpcDamageCause>();
        public double overallCausedDamage = 0f;

        public bool respawnable = true;
        public bool aggressive;

        public Npc(int id, Ship? ship, Spacemap spacemap, Position position) : base(id, ship.Name, 0, ship, position, spacemap, GameManager.GetClan(0), 22)
        {
            Spacemap.AddCharacter(this);

            ShieldAbsorption = 0.8;

            Damage = ship.Damage;
            MaxHitPoints = ship.BaseHitpoints;
            CurrentHitPoints = MaxHitPoints;
            MaxShieldPoints = ship.BaseShieldPoints;
            CurrentShieldPoints = MaxShieldPoints;

            NpcAI = new NpcAI(this);

            Program.TickManager.AddTick(this);
        }
       
        

        public override void Tick()
        {
            Movement.ActualPosition(this);
            NpcAI.TickAI();
            CheckShieldPointsRepair();
            Storage.Tick();
            RefreshAttackers();

            if (Attacking)
            {
                Attack();
            }

            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && (this as Character).Ship.Id == 81 && ((this as Character).Spacemap.Id == 3))   
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }

            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }
            } else if(Ship.Id == 81 && mother != null && mother.Ship.Id == 80 && (Spacemap.Id == 3 ))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5,10));               
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 3))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
           else if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && (this as Character).Ship.Id == 81 && ((this as Character).Spacemap.Id == 11))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }

            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }
            }
            else if (Ship.Id == 81 && mother != null && mother.Ship.Id == 80 && (Spacemap.Id == 11))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 11))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
            else if (Ship.Id == 94 && mother != null && mother.Ship.Id == 90 && ( Spacemap.Id == 11))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" &&  ((this as Character).Spacemap.Id == 11))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
            else if (Ship.Id == 94 && mother != null && mother.Ship.Id == 90 && (Spacemap.Id == 3))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 3))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
            else if (Ship.Id == 111 && mother != null && mother.Ship.Id == 115 && (Spacemap.Id == 55))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 55))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
            else if (Ship.Id == 81 && mother != null && mother.Ship.Id == 119 && (Spacemap.Id == 58))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 58))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
            else if (Ship.Id == 103 && mother != null && mother.Ship.Id == 101 && (Spacemap.Id == 2))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 2))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
            else if (Ship.Id == 103 && mother != null && mother.Ship.Id == 101 && (Spacemap.Id == 10))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 10))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
            else if (Ship.Id == 82 && mother != null && mother.Ship.Id == 83 && (Spacemap.Id == 16))
            {
                DateTime lastAttack = mother.LastCombatTime;
                if ((DateTime.Now - lastAttack).TotalSeconds > 30)
                {
                    diesAfter = DateTime.Now.AddSeconds(StaticRandom.Instance.Next(5, 10));
                }
            }
            if (MainAttacker != null && diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00" && ((this as Character).Spacemap.Id == 16))
            {
                diesAfter = DateTime.Now.AddMinutes(5);
            }
            if (diesAfter != null && diesAfter.ToString() != "01.01.0001 00:00:00")
            {
                if (DateTime.Now >= diesAfter)
                {
                    Destroy(this, DestructionType.NPC, true);
                }

            }
        }

        public static class StaticRandom
        {
            private static int seed;

            private static ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
                (() => new Random(Interlocked.Increment(ref seed)));

            static StaticRandom()
            {
                seed = Environment.TickCount;
            }

            public static Random Instance { get { return threadLocal.Value; } }
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

            //if (Destroyed) return;

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
                if(target is Player p)
                {
                    laserRunCommand = AttackLaserRunCommand.write(Id, target.Id, 0, p.shieldMechanics, false);
                }
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

        public async void empower()
        {
            AddVisualModifier(VisualModifierCommand.SINGULARITY, 0, "", 1, true);
            Empowered = true;
            await Task.Delay(10000);
            Empowered = false;
            lastEmpowered = DateTime.Now;
            RemoveVisualModifier(VisualModifierCommand.SINGULARITY);

        }

        public void Respawn(Position pos = null)
        {
            LastCombatTime = DateTime.Now.AddSeconds(-999);
            CurrentHitPoints = MaxHitPoints;
            CurrentShieldPoints = MaxShieldPoints;
            if(pos != null) SetPosition(pos);
            else
            {
                /*if (Spacemap.Id == 29) SetPosition(Position.Random(Spacemap, 0, 41600, 0, 25600));
                else */SetPosition(Position.Random(Spacemap, 0, 20800, 0, 12800));
            }

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

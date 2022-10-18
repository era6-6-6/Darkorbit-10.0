using Darkorbit.Game.Objects.AI;

namespace Darkorbit.Game.Objects
{
    internal class NPCFlagship : Character
    {
        public FlagshipAI FlagshipAI;
        public bool Attacking = false;
        public bool canBeAttacked = true;

        public bool UnderEmp = false;
        public DateTime startemp = new DateTime();
        public Player EmpFrom;

        public static List<int> PoliceShips = new List<int>
        {
            9, 4, 6
        };

        public static List<int> MilitaryShips = new List<int>
        {
            10, 56, 8
        };

        public static List<int> localMaps = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        };

        public NPCFlagship(int id, Ship ship, Spacemap Spacemap, Position position) : base(id, ship.Name, Randoms.random.Next(1, 4), ship, position, Spacemap, GameManager.GetClan(0), 22)
        {
            Spacemap.AddCharacter(this);

            ShieldAbsorption = 0.8;

            //Damage = MilitaryShips.Contains(ship.Id) ? 65000 : 30000;
            Damage = 15000;
            MaxHitPoints = ship.BaseHitpoints;
            CurrentHitPoints = MaxHitPoints;
            MaxShieldPoints = 456000;
            CurrentShieldPoints = MaxShieldPoints;
            ship.BaseSpeed = 470;
            //Console.WriteLine($"Ship base Speed = {ship.BaseSpeed}, Boosted: {Speed}");
            //Console.WriteLine($"FactionId = {FactionId}");


            FlagshipAI = new FlagshipAI(this);

            Program.TickManager.AddTick(this);
        }

        public override void Tick()
        {
            Movement.ActualPosition(this);
            FlagshipAI.TickAI();
            Storage.Tick();
            RefreshAttackers();
            CheckShieldPointsRepair();
            //Console.WriteLine($"AIOption: {FlagshipAI.AIOption}");

            if (Attacking)
            {
                Attack();
            }
        }

        public static void Add(Spacemap sMap, int faction)
        {
            int polShips = Randoms.random.Next(PoliceShips.Count);
            int milShips = Randoms.random.Next(MilitaryShips.Count);
            var ship = new NPCFlagship(Randoms.CreateRandomID(), GameManager.GetShip(localMaps.Contains(sMap.Id) ? PoliceShips[polShips] : MilitaryShips[milShips]), sMap, Position.Random(sMap, 0, 20800, 0, 12800))
            {
                FactionId = faction
            };
        }

        public DateTime lastAttackTime = new DateTime();
        public DateTime lastRLTime = new DateTime();
        public DateTime lastRocketTime = new DateTime();
        public DateTime lastRSBTime = new DateTime();
        public DateTime ISHcooldown = new DateTime();
        public DateTime ISHcooldownSkill = new DateTime();
        public void Attack()
        {
            int damage = AttackManager.RandomizeDamage(Damage, (Storage.underPLD8 ? 0.5 : 0.1));
            Character target = SelectedCharacter;

            if (Invincible)
            {
                AddVisualModifier(VisualModifierCommand.INVINCIBILITY, 0, "", 0, true);
            }

            if (!TargetDefinition(target, false))
            {
                return;
            }

            if (target is Player player && player.AttackManager.EmpCooldown.AddMilliseconds(TimeManager.EMP_DURATION) > DateTime.Now)
            {
                return;
            }

            if (lastRLTime.AddSeconds(6) < DateTime.Now)
            {
                rocketLauncherAttack();
            }

            if (lastRocketTime.AddSeconds(1) < DateTime.Now)
            {
                rocketAttack();
            }

            if (CurrentHitPoints < MaxHitPoints && ISHcooldown.AddSeconds(30) < DateTime.Now)
            {
                ISH();
            }
            if (CurrentHitPoints < MaxHitPoints && ISHcooldownSkill.AddSeconds(30) < DateTime.Now)
            {
                ISH();
            }

            if (lastAttackTime.AddSeconds(1) < DateTime.Now)
            {
                if (target is Player && (target as Player).Storage.Spectrum)
                {
                    damage -= Maths.GetPercentage(damage, 50);
                }

                int damageShd = 0, damageHp = 0;

                double shieldAbsorb = Math.Abs(target.ShieldAbsorption - 0);

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

                var laserRunCommand = AttackLaserRunCommand.write(Id, target.Id, lastRSBTime.AddSeconds(3.6) < DateTime.Now ? 6 : 3, false, true);
                SendCommandToInRangePlayers(laserRunCommand);
                if (lastRSBTime.AddSeconds(3.6) < DateTime.Now)
                {
                    damage += Maths.GetPercentage(damage, 10);
                }

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

                if (lastRSBTime.AddSeconds(3.6) < DateTime.Now)
                {
                    lastRSBTime = DateTime.Now;
                }

                lastAttackTime = DateTime.Now;
                target.AddDamage(this, damage);
                target.UpdateStatus();
                if (target is NPCFlagship)
                {
                    target.Selected = this;
                    (target as NPCFlagship).Attacking = true;
                }
            }
        }

        public async void rocketLauncherAttack()
        {
            /* DEBUG */

            var enemy = Selected;
            if (enemy == null)
            {
                return;
            }
            if (!TargetDefinition(enemy, false))
            {
                return;
            }

            int damage = 15000;
            lastRLTime = DateTime.Now;

            SendPacketToInRangePlayers("0|RL|A|" + Id + "|" + Selected.Id + "|5|7");

            await Task.Delay(1000);

            byte[] attackHitCommand =
                    AttackHitCommand.write(new AttackTypeModule(AttackTypeModule.ROCKET), Id,
                         enemy.Id, enemy.CurrentHitPoints,
                         enemy.CurrentShieldPoints, enemy.CurrentNanoHull,
                         damage, false);

            SendCommandToInRangePlayers(attackHitCommand);

            enemy.AddDamage(this, damage);
            enemy.UpdateStatus();
            enemy.LastCombatTime = DateTime.Now;

            
            /*
            var rocketRunPacket = $"0|v|{Player.Id}|{enemy.Id}|H|{GetSelectedRocket()}|{(Player.SkillTree.rocketFusion == 5 ? 1 : 0)}|{(Player.Storage.PrecisionTargeter || Player.SkillTree.heatseekingMissiles == 5 ? 1 : 0)}";
            Player.SendPacket(rocketRunPacket);
            Player.SendPacketToInRangePlayers(rocketRunPacket);
             
             */
        }

        public async void rocketAttack()
        {
            var enemy = Selected;
            if (enemy == null)
            {
                return;
            }
            if (!TargetDefinition(enemy, false))
            {
                return;
            }

            int damage = 7500;

            lastRocketTime = DateTime.Now;

            SendPacketToInRangePlayers($"0|v|{Id}|{enemy.Id}|H|4|0|0");

            await Task.Delay(1000);

            byte[] attackHitCommand =
                    AttackHitCommand.write(new AttackTypeModule(AttackTypeModule.ROCKET), Id,
                         enemy.Id, enemy.CurrentHitPoints,
                         enemy.CurrentShieldPoints, enemy.CurrentNanoHull,
                         damage, false);

            SendCommandToInRangePlayers(attackHitCommand);

            enemy.AddDamage(enemy, damage);
            enemy.UpdateStatus();
            enemy.LastCombatTime = DateTime.Now;

        }

        public async void ISH()
        {
            var ishPacket = "0|n|ISH|" + Id;
            SendPacketToInRangePlayers(ishPacket);

            canBeAttacked = false;

            ISHcooldown = DateTime.Now;

            await Task.Delay(3000);

            canBeAttacked = true;

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
            SetPosition(Position.Random(Spacemap, 0, 20800, 0, 12800));
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
                Convert.ToString(Ship.LootId),
                3,
                "",
                PoliceShips.Contains(Ship.Id) ? "[POLICE] " + Ship.Name : MilitaryShips.Contains(Ship.Id) ? "[MILITARY] " + Ship.Name : Ship.Name,
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

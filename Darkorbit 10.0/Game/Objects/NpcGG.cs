using Darkorbit.Game.Objects.AI;

namespace Darkorbit.Game.Objects
{
    internal class NpcGG : Character
    {
        public NpcAIGG NpcAIGG { get; set; }
        public bool Attacking = false;
        private readonly string Gate = "";
        private readonly int Count;
        public Player Session;
        public Position destPosition = null;
        public int gateId = 0;
        public static Dictionary<int, List<NpcGG>> Npcs = new Dictionary<int, List<NpcGG>>();

        public NpcGG(int id, Ship ship, Spacemap spacemap, Position position, string gate, int count, Player session, int mapID, int gateId = 0) : base(id, ship.Name + " " + gate + " " + count, 0, ship, position, spacemap, GameManager.GetClan(0), 22)
        {
            System.Collections.Generic.List<NpcGG> npcs = new System.Collections.Generic.List<NpcGG>();
            Spacemap.AddCharacter(this);

            npcs.Add(this);
            Npcs.Add(id, npcs);

            ShieldAbsorption = 0.8;

            Damage = (mapID == 52) ? ship.Damage * 2 : (mapID == 53) ? ship.Damage * 3 : (mapID == 1111) ? ship.Damage * 2 : (mapID == 74) ? ship.Damage * 1 : (mapID == 75) ? ship.Damage * 25 : (mapID == 76) ? ship.Damage * 30 : ship.Damage;
            MaxHitPoints = (mapID == 52) ? ship.BaseHitpoints * 2 : (mapID == 53) ? ship.BaseHitpoints * 3 : (mapID == 1111) ? ship.BaseHitpoints * 2 : (mapID == 74) ? ship.BaseHitpoints * 1 : (mapID == 75) ? ship.BaseHitpoints * 25 : (mapID == 76) ? ship.BaseHitpoints * 30 : ship.BaseHitpoints;
            CurrentHitPoints = MaxHitPoints;
            MaxShieldPoints = ship.BaseShieldPoints;
            CurrentShieldPoints = MaxShieldPoints;
            Gate = gate;
            Count = count;
            Session = session;

            NpcAIGG = new NpcAIGG(this);

            Program.TickManager.AddTick(this);
            this.gateId = gateId;
        }

        public override void Tick()
        {
            Movement.ActualPosition(this);
            NpcAIGG.TickAI();
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
                {
                    damageShd -= Maths.GetPercentage(damageShd, 30);
                }

                byte[] laserRunCommand = AttackLaserRunCommand.write(Id, target.Id, 0, false, false);
                if (target is Player p)
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
                {
                    target.Destroy(this, DestructionType.NPC);
                }
                else
                {
                    target.CurrentHitPoints -= damageHp;
                }

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

        static public void Destroy(GameSession gameSession)
        {
            Player player = gameSession.Player;

            if (player.positionInitializacion.mapID == 51)
                player.AlphaGate.WaveCheck(player);

            if (player.positionInitializacion.mapID == 52)
                player.BetaGates.WaveCheck(player);

            if (player.positionInitializacion.mapID == 53)
                player.GammaGates.WaveCheck(player);

            if (player.positionInitializacion.mapID == 1111)
                player.DeltaGates.WaveCheck(player);

            if (player.positionInitializacion.mapID == 74)
                player.KappaGates.WaveCheck(player);

            if (player.positionInitializacion.mapID == 76)
                player.KronosGates.WaveCheck(player);

            if (player.positionInitializacion.mapID == 75)
                player.LambdaGates.WaveCheck(player);

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
                int value = Ship.BaseSpeed;

                if (Storage.underR_IC3)
                {
                    value -= value;
                }

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
                //Ship.Name,
                Ship.Name + " " + Gate + " " + Count,
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

namespace Darkorbit.Game.Objects
{
    class Flagship : Character
    {
        public Player Owner { get; set; }
        public bool Activated = true;



        public override int Speed
        {
            get
            {
                return (int)(Owner.Speed);
            }

        }

        public Flagship(Player player) : base(Randoms.CreateRandomID(), "Guard", player.FactionId, GameManager.GetShip(273), player.Position, player.Spacemap, player.Clan, 22)
        {
            Spacemap.AddCharacter(this);
            Name = player.Name + " - Guard";
            Owner = player;

            ShieldAbsorption = 0.8;
            Damage = 60000;
            MaxHitPoints = 256000;
            MaxShieldPoints = 400000;
            CurrentHitPoints = MaxHitPoints;
            CurrentShieldPoints = MaxShieldPoints;
            Program.TickManager.AddTick(this);
            //SendDrones();

        }

        public void SendDrones()
        {
            var dronepacket = $"2|6|0|2|6|0|2|6|0|2|6|0|2|6|0|2|6|0|2|6|0|2|6|0|";
            var drones = $"0|n|d|{Id}|" + dronepacket;
            Console.WriteLine(drones);
            Owner.SendPacketToInRangePlayers(drones);
            Owner.SendPacket(drones);

            Owner.SendCommand(DroneFormationChangeCommand.write(Id, 0));
            SendCommandToInRangePlayers(DroneFormationChangeCommand.write(Id, 0));
        }

        public override void Tick()
        {
            if (Activated)
            {
                Follow(Owner);
                Movement.ActualPosition(this);
                if (Owner.AttackManager.Attacking)
                {
                    Attack(Owner.SelectedCharacter);
                }
            }
        }

        public void Activate()
        {
            SetPosition(Owner.Position);
            Spacemap = Owner.Spacemap;
            Invisible = Owner.Invisible;

            Spacemap.AddCharacter(this);
            Program.TickManager.AddTick(this);
        }

        public void Deactivate()
        {
            Deselection();
            Spacemap.RemoveCharacter(this);
            InRangeCharacters.Clear();
            Program.TickManager.RemoveTick(this);
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
                Movement.Move(this, Position.GetPosOnCircle(character.Position, 50));
        }

        public DateTime lastl = DateTime.Now;
        public void laser()
        {
            int damageShd = 0, damageHp = 0;
            double shieldAbsorb = 0.8;
            if (lastl.AddSeconds(1) < DateTime.Now)
            {

                if (shieldAbsorb > 1)
                    shieldAbsorb = 1;

                if ((Owner.SelectedCharacter.CurrentShieldPoints - Damage) >= 0)
                {
                    damageShd = (int)(Damage * shieldAbsorb);
                    damageHp = Damage - damageShd;
                }
                else
                {
                    int newDamage = Damage - Owner.SelectedCharacter.CurrentShieldPoints;
                    damageShd = Owner.SelectedCharacter.CurrentShieldPoints;
                    damageHp = (int)(newDamage + (damageShd * shieldAbsorb));
                }

                if ((Owner.SelectedCharacter.CurrentHitPoints - damageHp) < 0)
                {
                    damageHp = Owner.SelectedCharacter.CurrentHitPoints;
                }

                var laserRunCommand = AttackLaserRunCommand.write(Id, Owner.Selected.Id, 3, false, false);
                SendCommandToInRangePlayers(laserRunCommand);
                lastl = DateTime.Now;

                var attackHitCommand =
                        AttackHitCommand.write(new AttackTypeModule(AttackTypeModule.LASER), Id,
                                             Owner.SelectedCharacter.Id, Owner.SelectedCharacter.CurrentHitPoints,
                                             Owner.SelectedCharacter.CurrentShieldPoints, Owner.SelectedCharacter.CurrentNanoHull,
                                             Damage > AttackManager.RandomizeDamage(damageShd, 0.1) ? AttackManager.RandomizeDamage(Damage, 0.1) : AttackManager.RandomizeDamage(damageShd, 0.1), false);

                SendCommandToSelected(Owner.SelectedCharacter, attackHitCommand);
                damageHp = AttackManager.RandomizeDamage(damageHp, 0.1);
                damageShd = AttackManager.RandomizeDamage(damageShd, 0.1);
                if (damageHp >= Owner.SelectedCharacter.CurrentHitPoints || Owner.SelectedCharacter.CurrentHitPoints == 0)
                {

                    if (Owner.SelectedCharacter is NpcGG)
                    {
                        Owner.SelectedCharacter.Destroy(Owner, DestructionType.NPC);
                    }
                    else if (Owner.SelectedCharacter is Npcx2)
                    {
                        Owner.SelectedCharacter.Destroy(Owner, DestructionType.NPC);
                    }
                    else
                    {
                        Owner.SelectedCharacter.Destroy(this, DestructionType.PLAYER);
                    }

                }
                else
                {
                    Owner.SelectedCharacter.CurrentHitPoints -= AttackManager.RandomizeDamage(damageHp, 0.1);
                }

                Owner.SelectedCharacter.CurrentShieldPoints -= AttackManager.RandomizeDamage(damageShd, 0.1);
                Owner.SelectedCharacter.LastCombatTime = DateTime.Now;

                Owner.SelectedCharacter.UpdateStatus();
                Owner.SelectedCharacter.AddDamage(this, damageShd);

            }
        }


        /* ATTACK COMMAND */

        public DateTime lastAttackTime = new DateTime();
        public DateTime lastRSBAttackTime = new DateTime();

        private void Attack(Character target)
        {

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
                }
                else if (Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 || Owner.Settings.InGameSettings.selectedLaser == AmmunitionManager.MCB_50)
                {
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

            }
        }

        public override byte[] GetShipCreateCommand()
        {
            return ShipCreateCommand.write(
                Id,
                Ship.LootId,
                3,
                Owner.Clan.Tag,
                Owner.Name + "'s Guard",
                Position.X,
                Position.Y,
                FactionId,
                0,
                0,
                false,
                new ClanRelationModule(ClanRelationModule.NONE),
                0,
                false,
                false,
                false,
                ClanRelationModule.AT_WAR,
                ClanRelationModule.AT_WAR,
                new List<VisualModifierCommand>(),
                new class_11d(class_11d.DEFAULT)
                );
        }

        //public override void Tick()
        //{
        //    //throw new NotImplementedException();
        //}
    }
}

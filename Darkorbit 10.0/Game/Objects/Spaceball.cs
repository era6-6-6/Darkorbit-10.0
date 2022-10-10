namespace Darkorbit.Game.Objects
{
    class Spaceball : Character
    {
        public Spacemap Spacemap = GameManager.GetSpacemap(16);

        private int SelectedFactionId = 0;
        public override int RenderRange => -1;

        public int Mmo = 0;
        public int Eic = 0;
        public int Vru = 0;

        public List<Player> players = new List<Player>();

        private readonly static Position CurrentPosition = new Position(10500, 6500);
        private int MMODamage = 0;
        public Position MMOPosition = new Position(2000, 6400);
        private int EICDamage = 0;
        public Position EICPosition = new Position(18600, 1900);
        private int VRUDamage = 0;
        public Position VRUPosition = new Position(18700, 6400);

        public DateTime LastDamagedTime = new DateTime();

        public Spaceball(int id, int typeId) : base(id, GameManager.GetShip(typeId).Name, 0, GameManager.GetShip(typeId), CurrentPosition, GameManager.GetSpacemap(16), GameManager.GetClan(0), 22)
        {
            Speed = 100;
        }
        List<Character> AttackingPlayers = new List<Character>();
        
        public override void Tick()
        {
            if (EventManager.Spaceball.Active)
            {
                Movement.ActualPosition(this);
                CheckDamage();
                CheckSpeed();
                if ((Position.DistanceTo(MMOPosition) <= 100) || (Position.DistanceTo(EICPosition) <= 100) || (Position.DistanceTo(VRUPosition) <= 100))
                    SendReward();
            }
        }

        public void CheckDamage()
        {
            if (LastDamagedTime.AddSeconds(5) < DateTime.Now && Position != CurrentPosition && SelectedFactionId != 0)
            {
                GameManager.SendPacketToAll("0|n|sss|1|0");
                GameManager.SendPacketToAll("0|n|sss|2|0");
                GameManager.SendPacketToAll("0|n|sss|3|0");
                ReInitialization();
                Movement.Move(this, CurrentPosition);
            }
        }

        public DateTime lastCheckSpeedTime = new DateTime();
        public void CheckSpeed()
        {
            if (lastCheckSpeedTime.AddSeconds(2) < DateTime.Now)
            {
                switch (SelectedFactionId)
                {
                    case 1:
                        if (MMODamage < 500000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|1|1");
                        }
                        else if (MMODamage < 1000000 && MMODamage > 500000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|1|2");
                            Speed = 125;
                        }
                        else if (MMODamage > 1000000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|1|3");
                            Speed = 175;
                        }
                        break;
                    case 2:
                        if (EICDamage < 500000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|2|1");
                        }
                        else if (EICDamage < 1000000 && EICDamage > 500000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|2|2");
                            Speed = 125;
                        }
                        else if (EICDamage > 1000000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|2|3");
                            Speed = 175;
                        }
                        break;
                    case 3:
                        if (VRUDamage < 500000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|3|1");
                        }
                        else if (VRUDamage < 1000000 && VRUDamage > 500000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|3|2");
                            Speed = 125;
                        }
                        else if (VRUDamage > 1000000)
                        {
                            GameManager.SendPacketToAll("0|n|sss|3|3");
                            Speed = 175;
                        }
                        break;
                }
                lastCheckSpeedTime = DateTime.Now;
            }
        }

        public void SendReward()
        {
            //var portal = Spacemap.Activatables.Values.FirstOrDefault(x => x is Portal && (x as Portal).Position == (SelectedFactionId == 1 ? MMOPosition : SelectedFactionId == 2 ? EICPosition : VRUPosition));
            //GameManager.SendCommandToMap(Spacemap.Id, ActivatePortalCommand.write((portal as Portal).TargetSpaceMapId, portal.Id));

            if (SelectedFactionId == 1)
            {
                Mmo++;
                GameManager.SendPacketToAll($"0|A|STM|msg_spaceball_company_scored|%COMPANY%|MMO");
            }
            else if (SelectedFactionId == 2)
            {
                Eic++;
                GameManager.SendPacketToAll($"0|A|STM|msg_spaceball_company_scored|%COMPANY%|EIC");
            }
            else if (SelectedFactionId == 3)
            {
                Vru++;
                GameManager.SendPacketToAll($"0|A|STM|msg_spaceball_company_scored|%COMPANY%|VRU");
            }
         
            
            foreach (Character item in AttackingPlayers.Where(x => x is Player && x.FactionId == SelectedFactionId))
            {
                if (item is Player player && player.FactionId == SelectedFactionId)
                {
                    int experience = 0;
                    int honor = 0;
                    int uridium = 0;
                    int credits = 0;
                    int ec = 0;

                    experience = player.Ship.GetExperienceBoost(Randoms.random.Next(25000, 50000));
                    honor = player.Ship.GetHonorBoost(Randoms.random.Next(500, 1000));
                    uridium = Randoms.random.Next(1000, 2500);
                    ec = Randoms.random.Next(0, 5);
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.MCB_25, Randoms.random.Next(1000, 3000));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.MCB_50, Randoms.random.Next(0, 3000));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.UCB_100, Randoms.random.Next(0, 1900));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.RSB_75, Randoms.random.Next(0, 600));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.ISH_01, Randoms.random.Next(0, 2));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.SMB_01, Randoms.random.Next(0, 2));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.EMP_01, Randoms.random.Next(0, 2));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.DCR_250, Randoms.random.Next(0, 2));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.PLD_8, Randoms.random.Next(0, 2));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.HSTRM_01, Randoms.random.Next(0, 20));
                    player.AmmunitionManager.AddAmmo(AmmunitionManager.SAR_02, Randoms.random.Next(0, 20));

                    player.LoadData();
                    player.ChangeData(DataType.EXPERIENCE, experience);
                    player.ChangeData(DataType.HONOR, honor);
                    player.ChangeData(DataType.URIDIUM, uridium);
                    player.ChangeData(DataType.CREDITS, credits);
                    player.ChangeData(DataType.EC, ec);
                }
            }
            GameManager.SendPacketToAll($"0|n|ssi|{Mmo}|{Eic}|{Vru}");

            if (Mmo >= EventManager.Spaceball.Limit || Eic >= EventManager.Spaceball.Limit || Vru >= EventManager.Spaceball.Limit)
                EventManager.Spaceball.Stop();
            else
            {
                Respawn();
                AttackingPlayers.Clear();
            }

            
        }

        public async void Respawn()
        {
            Spacemap.RemoveCharacter(this);
            
            SetPosition(CurrentPosition);
            ReInitialization();
            await Task.Delay(2500);
            Spacemap.AddCharacter(this);
        }

        public void ReInitialization()
        {
            Speed = 100;
            MMODamage = 0;
            EICDamage = 0;
            VRUDamage = 0;
            SelectedFactionId = 0;
        }

        public void AddDamage(Character character, int damage)
        {
            if(!AttackingPlayers.Contains(character))
            {
                AttackingPlayers.Add(character);
            }
            
            switch (character.FactionId)
            {
                case 1:
                    MMODamage += damage;
                    break;
                case 2:
                    EICDamage += damage;
                    break;
                case 3:
                    VRUDamage += damage;
                    break;
            }
            LastDamagedTime = DateTime.Now;
            Move();
        }

        public void Move()
        {
            if (MMODamage > EICDamage && MMODamage > VRUDamage)
            {
                SelectedFactionId = 1;
                Movement.Move(this, MMOPosition);
            }
            else if (EICDamage > MMODamage && EICDamage > VRUDamage)
            {
                SelectedFactionId = 2;
                Movement.Move(this, EICPosition);
            }
            else if (VRUDamage > MMODamage && VRUDamage > EICDamage)
            {
                SelectedFactionId = 3;
                Movement.Move(this, VRUPosition);
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

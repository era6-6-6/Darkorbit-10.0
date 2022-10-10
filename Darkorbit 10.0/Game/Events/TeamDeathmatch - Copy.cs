using Pet = Darkorbit.Game.Pet;
using Darkorbit;

namespace Ow.Game.Events
{
    internal class TeamDeathmatch : Tick
    {

        public bool active = false;
        public bool peaceArea = true;
        public bool
            ed = false;
        public bool ActiveUBA = false;
        public bool activeTournament = false;
        public List<Player> playerAlone = new List<Player>();
        public List<Group> listGroup = new List<Group>();
        public List<Player> playerGroup1 = new List<Player>();
        public List<Player> playerGroup2 = new List<Player>();

        public readonly Spacemap Spacemap = GameManager.GetSpacemap(42);

        public Position Position1 = new Position(9000, 6700);
        public Position Position2 = new Position(12000, 6700);
        public List<Player> Finalists = new List<Player>();


        int inEventGrouId1 = 0;
        int inEventGrouId2 = 0;
        public List<Portal> portal = new List<Portal>();
        private readonly Position positionPortal = new Position(10300, 4300);
        public async void Start()
        {
            activeTournament = true;
            int seconds = 30;
            int k = seconds - 5;
            for (int i = seconds; i > 0; i--)
            {
                if (i == k)
                {
                    string packet = $"0|A|STD|[TDM] You can register with command /register ";

                    GameManager.SendPacketToAll(packet);
                    k += -10;
                }
                await Task.Delay(1000);

            }
            GameManager.SendPacketToAll($"0|n|KSMSG|TEAM DEATH MATCH STARTED");



            Program.TickManager.AddTick(this);
        }
        public void Tick()
        {
            if (playerAlone.Count >= 4 || listGroup.Count > 0)
            {
                if (playerAlone.Count >= 4)
                {
                    Group group1 = new Group(playerAlone[0], playerAlone[1]);
                    Group group2 = new Group(playerAlone[2], playerAlone[3]);
                    //group1.AddToGroup(playerAlone[2]);
                    //group1.AddToGroup(playerAlone[3]);

                    foreach (var players in group1.Members.Values)
                    {
                        playerAlone.Remove(players);

                    }
                    foreach (var players in group2.Members.Values)
                    {
                        playerAlone.Remove(players);

                    }


                    group1.SendInitToAll();
                    group2.SendInitToAll();
                    listGroup.Add(group1);
                    listGroup.Add(group2);
                }
            }

            if (listGroup.Count >= 2 && !active)
            {

                start(listGroup[0], listGroup[1]);
                inEventGrouId1 = listGroup[0].Id;
                inEventGrouId2 = listGroup[1].Id;

            }




            if (ActiveUBA)
            {
                if (Spacemap.Characters.Count <= 10)
                {
                    foreach (Character player in Spacemap.Characters.Values)
                    {
                        if (player is Pet pet)
                        {
                            pet.Destroy(pet.Owner, DestructionType.PLAYER);
                        }
                    }
                }
            }
        }

        public void AddWaitingPlayer(Group group, Player player)
        {


            if (listGroup.Contains(group))
            {

                player.SendPacket($"0|A|STD|Please wait, searching players.. {listGroup.Count}/2");
                return;

            }

            if (playerAlone.Contains(player))
            {

                player.SendPacket($"0|A|STD|Please wait, searching players.. {playerAlone.Count}/4");
                return;
            }
            if (group != null)
            {
                if (group.Members.Count < 4 && group.Members.Count > 4)
                    player.SendPacket($"0|A|STD|Group invalid. Only 4 players");
                return;
            }

            if (group != null)
            {


                if (GameManager.Groups.Contains(player.Group))
                {
                    foreach (var players in group.Members.Values)
                    {
                        if (playerAlone.Contains(players))
                            playerAlone.Remove(players);
                    }
                    listGroup.Add(player.Group);
                    player.SendPacket($"0|A|STD|Group Registered . searching opponent...");

                }
            }
            else
            {
                System.Console.WriteLine(player.Id);
                playerAlone.Add(player);
                player.SendPacket($"0|A|STD|Registered. searching group...");
            }



        }

        public void RemoveWaitingPlayer(Group group, Player player)
        {
            if (group != null)
            {
                if (listGroup.Contains(group))
                {
                    listGroup.Remove(group);
                }
            }

            if (player != null)
            {
                if (playerAlone.Contains(player))
                {
                    playerAlone.Remove(player);
                }
            }
        }



        public bool InEvent(Player player)
        {
            Spacemap Spacemap = GameManager.GetSpacemap(42);
            return player.Spacemap.Id == Spacemap.Id && Spacemap.Characters.ContainsKey(player.Id);
        }



        public async void start(Group players1, Group players2)
        {
            active = true;

            GameManager.SendPacketToAll($"0|A|STD|-= Team Death Match Start=-");
            for (int i = 10; i >= 1; i--)
            {


                foreach (Player player in players1.Members.Values)
                {
                    player.SendPacket($"0|A|STD|-={i}=-");
                }
                foreach (Player player in players2.Members.Values)
                {
                    player.SendPacket($"0|A|STD|-={i}=-");
                }
                await Task.Delay(1000);
                if (i <= 1)
                {



                    foreach (Player player in players1.Members.Values)
                    {
                        player.CurrentHitPoints = player.MaxHitPoints;
                        player.CurrentShieldConfig1 = player.MaxShieldPoints;
                        player.CurrentShieldConfig2 = player.MaxShieldPoints;

                        player.Jump(Spacemap.Id, Position1);
                        player.CpuManager.DisableCloak();

                    }
                    foreach (Player player in players2.Members.Values)
                    {
                        player.CurrentHitPoints = player.MaxHitPoints;
                        player.CurrentShieldConfig1 = player.MaxShieldPoints;
                        player.CurrentShieldConfig2 = player.MaxShieldPoints;

                        player.Jump(Spacemap.Id, Position2);
                        player.CpuManager.DisableCloak();

                    }
                    Thread thread = new Thread(Status);
                    thread.Start();

                }
            }

            ActiveUBA = true;

            EventManager.TeamDeathmatch.listGroup.Remove(players1);
            EventManager.TeamDeathmatch.listGroup.Remove(players2);
        }

        public void Status()
        {
            bool winGroup1 = false;
            bool winGroup2 = false;
            int group1members = 2;
            int group2members = 2;
            bool activestatus = true;
            while (activestatus == true)
            {
                foreach (var item in Spacemap.Characters.Values)
                {
                    if (item is Player)
                    {
                        Player player = item as Player;
                        if (player.Destroyed)
                        {
                            if (player.Group.Id == inEventGrouId1)
                            {
                                Console.WriteLine("DEBUG: GRUPPE 1 " + player.Name + " zerstört - 1 ALTER WERT = " + group1members);
                                group1members--;
                                Console.WriteLine("DEBUG: GRUPPE 1 - 1 NEUER WERT = " + group1members);
                                player.Respawn(basicRepair: true);
                            }
                            if (player.Group.Id == inEventGrouId2)
                            {
                                Console.WriteLine("DEBUG: GRUPPE 2 " + player.Name + " zerstört - 1 ALTER WERT = " + group2members);
                                group2members--;
                                Console.WriteLine("DEBUG: GRUPPE 2 - 1 NEUER WERT = " + group2members);
                                player.Respawn(basicRepair: true);
                            }
                        }
                    }
                }

                if (group2members == 0)
                {
                    foreach (var itemGroup in GameManager.Groups)
                    {

                        if (itemGroup.Id == inEventGrouId1)
                        {
                            SendReward(itemGroup);
                            break;
                        }
                    }
                    break;
                }
                if (group1members == 0)
                {
                    foreach (var itemGroup in GameManager.Groups)
                    {

                        if (itemGroup.Id == inEventGrouId2)
                        {
                            SendReward(itemGroup);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public void SendReward(Group group)

        {

            int uridium = 5000;
            int experience = 50000;
            int credits = 5000;
            int honor = 512;
            foreach (Player player in group.Members.Values)
            {

                player.SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                player.ChangeData(DataType.URIDIUM, uridium);
                player.ChangeData(DataType.EXPERIENCE, experience);
                player.ChangeData(DataType.HONOR, honor);
                player.ChangeData(DataType.CREDITS, credits);


                player.SetPosition(player.GetBasePosition());
                player.Jump(player.GetBaseMapId(), player.Position);


            }

            Spacemap.Characters.Clear();
            Spacemap.Objects.Clear();


            inEventGrouId1 = 0;
            inEventGrouId2 = 0;
            active = false;

        }

        public async void Stop()
        {

            {
                GameManager.SendPacketToAll("0|A|STD|TeamDeathmatch event ended!");
                activeTournament = false;
            }
        }
    }
}
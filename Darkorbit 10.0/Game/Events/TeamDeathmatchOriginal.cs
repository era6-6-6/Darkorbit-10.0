
using Darkorbit.Game.Ticks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
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
       
        public readonly Spacemap Spacemap = GameManager.GetSpacemap(121);

        public Position Position1 = new Position(3700, 3200);
        public Position Position2 = new Position(6400, 3200);
        public List<Player> Finalists = new List<Player>();


        int inEventGrouId1 = 0;
        int inEventGrouId2 = 0;
        public List<Portal> portal = new List<Portal>();
        private readonly Position positionPortal = new Position(10300, 4300);
        public async void Start()
        {
            activeTournament = true;
            foreach (Spacemap item in GameManager.Spacemaps.Values)
            {
                if (item.Id == 20 || item.Id == 24 || item.Id == 28)
                    portal.Add(new Portal(item, positionPortal, Position.Random(Spacemap, 2000, 20000, 1500, 10000), Spacemap.Id, 4, 1, true, true, false));

            }

            foreach (Portal portal in portal)
            {
                GameManager.SendCommandToMap(portal.Spacemap.Id, portal.GetAssetCreateCommand());
            }
            int seconds = 30;
            int k = seconds - 5;
            for (int i = seconds; i > 0; i--)
            {
                if (i == k)
                {
                    string packet = $"0|A|STD|[TDM-Event] You can register with command in chat /register or use portal in base";

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
            if (playerAlone.Count>=4 || listGroup.Count>0) {
                if (playerAlone.Count >= 4)
                {
                    Group group1 = new Group(playerAlone[0], playerAlone[1]);
                    Group group2 = new Group(playerAlone[2], playerAlone[3]);

                    foreach (var players in group1.Members.Values) 
                    {
                        playerAlone.Remove(players);

                    }
                    foreach (var players in group2.Members.Values)
                    {
                        playerAlone.Remove(players);

                    }

                    group2.SendInitToAll();
                    group1.SendInitToAll();
                    listGroup.Add(group1);
                    listGroup.Add(group2);
                }
            }

            if (listGroup.Count>=2  && !active)
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

                bool winGroup1 = false;
                bool winGroup2 = false;
                foreach (var item in Spacemap.Characters.Values)
                {
                    if (item is Player)
                    {
                        Player player = item as Player;
                        if (player.Group.Id == inEventGrouId1)
                        {

                            winGroup1 = true;
                        }
                        else
                        {
                            winGroup1 = false;

                        }


                    }

                    if (item is Player)
                    {
                        Player player = item as Player;
                        if (player.Group.Id != inEventGrouId2)
                        {

                            winGroup2 = true;
                        }
                        else
                        {
                            winGroup2 = false;

                        }






                    }


                }
                if (winGroup1) {
                    foreach (var itemGroup in GameManager.Groups)
                    {

                        if (itemGroup.Id == inEventGrouId1)
                        {
                            SendReward(itemGroup);
                        }
                    }
                }
                if (winGroup2)
                {
                    foreach (var itemGroup in GameManager.Groups)
                    {

                        if (itemGroup.Id == inEventGrouId2)
                        {
                            SendReward(itemGroup);
                            break;
                        }
                    }
                }

            }
        }

        public void AddWaitingPlayer(Group group,Player player)
        {


            if (listGroup.Contains(group) )
            {

                player.SendPacket($"0|A|STD|Please wait, searching players.. {listGroup.Count}/2");
                return;

            }

            if (playerAlone.Contains(player)) {

                player.SendPacket($"0|A|STD|Please wait, searching players.. {playerAlone.Count}/4");
                return;
            }
            if (group!=null ) { 
            if(group.Members.Count<2 && group.Members.Count>2)
                    player.SendPacket($"0|A|STD|Group invalid. Only 2 players");
                return;
            }
            
            if (group != null)
            {
              

                if (GameManager.Groups.Contains(player.Group))
                {
                    foreach (var players in group.Members.Values) {
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
                //hola
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
            Spacemap Spacemap = GameManager.GetSpacemap(121);
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

                        player.Jump(Spacemap.Id, Position1);

                    }
                    foreach (Player player in players2.Members.Values)
                    {
                        player.CurrentHitPoints = player.MaxHitPoints;

                        player.Jump(Spacemap.Id, Position2);

                    }

                }
            }

            ActiveUBA = true;
        }

        public bool Status()
        {
            return activeTournament;
        }

        public void SendReward(Group group)

        {
            ActiveUBA = false;
         
            int uridium = 5000;
            int experience = 50000;
            int credits = 5000;
            int honor = 512;
            foreach (Player player in group.Members.Values)
            {

                player.SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                player.LoadData();
                player.ChangeData(DataType.URIDIUM, uridium);
                player.ChangeData(DataType.EXPERIENCE, experience);
                player.ChangeData(DataType.HONOR, honor);
                player.ChangeData(DataType.CREDITS, credits);
                player.ChangeData(DataType.EC, 10);


                player.SetPosition(player.GetBasePosition());
                player.Jump(player.GetBaseMapId(), player.Position);


            }

            Spacemap.Characters.Clear();
            Spacemap.Objects.Clear();
          
          
            inEventGrouId1 = 0;
            inEventGrouId2 = 0;
            active = false;

        }

    }
}

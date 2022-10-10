using Darkorbit.Game.Objects;
using Darkorbit.Game.Ticks;
using Darkorbit.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class GroupEvent : Tick
    {

        public bool active = false;
        public bool peaceArea = true;
        public bool
            ed = false;
        public bool ActiveUBA = false;


        public List<int> listGroup = new List<int>();
        public List<Player> playerGroup1 = new List<Player>();
        public List<Player> playerGroup2 = new List<Player>();
        private readonly Spacemap Spacemap = GameManager.GetSpacemap(101);

        public Position Position1 = new Position(3700, 3200);
        public Position Position2 = new Position(6400, 3200);
        public List<Player> Finalists = new List<Player>();
        public GroupEvent()
        {
            Program.TickManager.AddTick(this);

        }
        public void Tick()
        {

            if ((playerGroup1.Count <= 2 || playerGroup2.Count <= 2) && !active)
            {
                foreach (Group groupPlayers in GameManager.Groups)
                {

                    if (listGroup.Contains(groupPlayers.Id))
                    {
                        if (groupPlayers.Members.Count > 2)
                        {
                            RemoveWaitingPlayer(groupPlayers.Leader);
                            groupPlayers.Leader.SendPacket($"0|A|STD|ERROR, Only two players per group");
                            return;
                        }
                        Player[] player = groupPlayers.Members.Values.ToArray<Player>();

                        if (playerGroup1.Count <= 2)
                        {
                            playerGroup1.Add(player[0]);
                            playerGroup1.Add(player[1]);


                        }
                        else if (playerGroup2.Count <= 2 && !playerGroup1.Contains(player[0]) && !playerGroup1.Contains(player[1]))
                        {
                            playerGroup2.Add(player[0]);
                            playerGroup2.Add(player[1]);

                        }


                    }


                }
            }
            else if (playerGroup1.Count == 4 && playerGroup2.Count == 4 && !active)
            {



                start(playerGroup1, playerGroup2);





            }

            if (ActiveUBA)
            {
                if (Spacemap.Characters.Count <= 6)
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


            if (Spacemap.Characters.Count == 2 && Finalists.Count <= 2)
            {
                foreach (Character player in Spacemap.Characters.Values)
                {


                    Finalists.Add(player as Player);


                }

            }

            if (Finalists.Count == 2 && Finalists[0].Group.Id == Finalists[1].Group.Id)
            {
                SendReward();
            }
            else
            {
                if (Spacemap.Characters.Count == 1)
                {
                    SendReward();
                }
            }



        }

        public void AddWaitingPlayer(Player player)
        {


            if (!listGroup.Contains(player.Group.Id))
            {
                if (GameManager.Groups.Contains(player.Group))
                {

                    listGroup.Add(player.Group.Id);
                    player.SendPacket($"0|A|STD|Registered group");
                }

                else
                {
                    player.SendPacket($"0|A|STD|You have no group");
                }
            }
            else
            {
                player.SendPacket($"0|A|STD|Your group already registered");
            }
        }

        public void RemoveWaitingPlayer(Player player)
        {
            if (listGroup.Contains(player.Group.Id))
            {

                if (playerGroup1.Contains(player))
                {
                    playerGroup1.Clear();
                }

                if (playerGroup2.Contains(player))
                {
                    playerGroup2.Clear();
                }

                listGroup.Remove(player.Group.Id);
            }


        }
        public void RemoveGroup(Player player)
        {
            if (listGroup.Contains(player.Group.Id))
            {
                listGroup.Remove(player.Group.Id);
            }


        }


        public bool InEvent(Player player)
        {
            Spacemap Spacemap = GameManager.GetSpacemap(101);
            return player.Spacemap.Id == Spacemap.Id && Spacemap.Characters.ContainsKey(player.Id);
        }



        public async void start(List<Player> players1, List<Player> players2)
        {
            active = true;

            GameManager.SendPacketToAll($"0|A|STD|-= Teams Battle Arena Start=-");
            for (int i = 10; i >= 1; i--)
            {


                players1[0].SendPacket($"0|A|STD|-={i}=-");
                players1[1].SendPacket($"0|A|STD|-={i}=-");




                players2[0].SendPacket($"0|A|STD|-={i}=-");
                players2[1].SendPacket($"0|A|STD|-={i}=-");

                await Task.Delay(1000);
                if (i <= 1)
                {


                    players1[0].CurrentHitPoints = players1[0].MaxHitPoints;
                    players1[0].Jump(Spacemap.Id, Position1);
                    players1[1].CurrentHitPoints = players1[0].MaxHitPoints;
                    players1[1].Jump(Spacemap.Id, Position1);




                    players2[0].CurrentHitPoints = players2[0].MaxHitPoints;
                    players2[0].Jump(Spacemap.Id, Position2);
                    players2[1].CurrentHitPoints = players2[0].MaxHitPoints;
                    players2[1].Jump(Spacemap.Id, Position2);

                }
            }

            ActiveUBA = true;









            EventManager.groupEvent.RemoveGroup(players1.First());
            EventManager.groupEvent.RemoveGroup(players2.First());
        }

        public void SendReward()

        {
            ActiveUBA = false;
            Player playerLeader = Spacemap.Characters.First().Value as Player;
            int uridium = 5000;
            int experience = 50000;
            int honor = 512;
            if (playerGroup1.Contains(playerLeader))
            {

                playerGroup1[0].SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                playerGroup1[0].LoadData();
                playerGroup1[0].ChangeData(DataType.URIDIUM, uridium);
                playerGroup1[0].ChangeData(DataType.EXPERIENCE, experience);
                playerGroup1[0].ChangeData(DataType.HONOR, honor);
                playerGroup1[0].ChangeData(DataType.EC, 15);



                playerGroup1[0].SetPosition(playerGroup1[0].GetBasePosition());
                playerGroup1[0].Jump(playerGroup1[0].GetBaseMapId(), playerGroup1[0].Position);


                playerGroup1[1].SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                playerGroup1[1].LoadData();
                playerGroup1[1].ChangeData(DataType.URIDIUM, uridium);
                playerGroup1[1].ChangeData(DataType.EXPERIENCE, experience);
                playerGroup1[1].ChangeData(DataType.HONOR, honor);
                playerGroup1[1].ChangeData(DataType.EC, 15);



                playerGroup1[1].SetPosition(playerGroup1[1].GetBasePosition());
                playerGroup1[1].Jump(playerGroup1[1].GetBaseMapId(), playerGroup1[1].Position);



            }
            else if (playerGroup2.Contains(playerLeader))
            {

                playerGroup2[0].SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                playerGroup2[0].LoadData();
                playerGroup2[0].ChangeData(DataType.URIDIUM, uridium);
                playerGroup2[0].ChangeData(DataType.EXPERIENCE, experience);
                playerGroup2[0].ChangeData(DataType.HONOR, honor);
                playerGroup2[0].ChangeData(DataType.EC, 15);



                playerGroup2[0].SetPosition(playerGroup2[0].GetBasePosition());
                playerGroup2[0].Jump(playerGroup2[0].GetBaseMapId(), playerGroup2[0].Position);


                playerGroup2[1].SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                playerGroup2[1].LoadData();
                playerGroup2[1].ChangeData(DataType.URIDIUM, uridium);
                playerGroup2[1].ChangeData(DataType.EXPERIENCE, experience);
                playerGroup2[1].ChangeData(DataType.HONOR, honor);
                playerGroup2[1].ChangeData(DataType.EC, 15);



                playerGroup2[1].SetPosition(playerGroup2[1].GetBasePosition());
                playerGroup2[1].Jump(playerGroup2[1].GetBaseMapId(), playerGroup2[1].Position);





            }

            Spacemap.Characters.Clear();
            Spacemap.Objects.Clear();
            Finalists.Clear();
            playerGroup1.Clear();
            playerGroup2.Clear();

            active = false;

        }


    }
}
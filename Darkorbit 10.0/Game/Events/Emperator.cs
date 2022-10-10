using Darkorbit.Data;
using Darkorbit.Game.Objects.Collectables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Emperator
    {
        private static Spacemap Spacemap = GameManager.GetSpacemap(200);
        private static readonly int[] maps = new int[] { 13, 14, 15 };
        public static DataDemaner data = new DataDemaner();

        public static bool Active = false;
        public static bool Aggresive = false;
        public static int Uridium = 0;
        public static int Credits = 0;
        public static int Experience = 0;
        public static int Honor = 0;

        public static Position CurrentPosition = new Position(10300, 6300);
        private static Npc demaner = null;
        private static int wait = 1;
        private static Task proccesPlayer;
        private static Task update;
        private static Task feedback;


        public static Random ran = new Random();
        public static List<Portal> portal = new List<Portal>();
        private static readonly Position positionPortal = new Position(10300, 6300);

        private static CancellationTokenSource tsUpdate = new CancellationTokenSource();
        private static CancellationToken ctUpdate;
        private static CancellationTokenSource tsFeedback = new CancellationTokenSource();
        private static CancellationToken ctFeedback;

        public static void Start()
        {
            if (!Active)
            {
                Spacemap = GameManager.GetSpacemap(maps[ran.Next(maps.Length)]);
                Active = true;

                ctUpdate = tsUpdate.Token;
                ctFeedback = tsFeedback.Token;

                proccesPlayer = Task.Factory.StartNew(() => Starting());
                update = new Task(loop);
                feedback = new Task(feedBack);


            }
        }
        public static void Starting()
        {
            try
            {
                if (Active)
                {
                    ChooseNpc();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Emperator.cs] Main void exception: {ex}");
            }
        }

        public static bool Status()
        {
            return Active;
        }

        public static void ChooseNpc()
        {
            Random random = new Random();
            int numberRandom = random.Next(2);
            int shipId;
            int lacayos = 0;

            if (numberRandom == 0)
            {
                shipId = 122;
                lacayos = 78;
                Aggresive = true;
                Uridium = 50500;
                Credits = 2000000;
                Honor = 10250;
                Experience = 200000;
            } 
            else
            {
                shipId = 123;
                lacayos = 77;
                Aggresive = true;
                Uridium = 80500;
                Credits = 2000000;
                Honor = 10250;
                Experience = 200000;
            }

            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var dataShip = mySqlClient.ExecuteQueryTable($"SELECT * FROM server_ships WHERE shipID = {shipId} LIMIT 1");
                foreach(DataRow row in dataShip.Rows)
                {

                    int seconds = 60;
                    int k = seconds - 10;

                    GameManager.SendPacketToAll($"0|n|KSMSG|{row["name"]} will land in map {Spacemap.Name} in {seconds} seconds.");
                    for (int i = seconds; i > 0; i--)
                    {
                        if (i == k)
                        {
                            string packet = $"0|A|STD| [MAP {Spacemap.Name}]{row["name"]} land in {i} seconds.";

                            GameManager.SendPacketToAll(packet);
                            k += -10;
                        }
                        Thread.Sleep(1000);


                    }

                    GameManager.SendPacketToAll($"0|n|KSMSG|{row["name"]} land in map {Spacemap.Name}");
                    demaner = createNPC(shipId, 1, Spacemap.Id, CurrentPosition);
                    demaner.Name = row["name"].ToString();
                    demaner.Ship.Name = row["name"].ToString();
                    demaner.MaxHitPoints = 80000000;
                    demaner.CurrentHitPoints = 80000000;
                    demaner.MaxShieldPoints = 7000000;
                    demaner.CurrentShieldPoints = 75000000;
                    update.Start();
                    feedback.Start();

                    for (int i = wait; i > 0; i--)
                    {


                        Thread.Sleep(1000);

                        if (i <= 1 && data.Minions.Count <= 35)
                        {

                            createNPC(lacayos, 1, Spacemap.Id, demaner.Position);
                            demaner.Heal(250000);
                            i = wait + 5;

                        }
                    }
                }
            }
        }

        public static void feedBack()
        {
            try
            {
                if (Active)
                {
                    while (Active)
                    {
                        if (ctFeedback.IsCancellationRequested) break;
                        Thread.Sleep(30000);
                        GameManager.SendPacketToAll($"0|n|KSMSG|{demaner.Name} IN MAP {Spacemap.Name} POSITION {demaner.Position.X / 100}/{demaner.Position.Y / 100}");

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Emperator.cs] Main void exception: {ex}");
            }
        }

        public static void loop()
        {
            try
            {
                while (Active)
                {
                    if (ctUpdate.IsCancellationRequested) break;
                    if (data.Minions.Count > 0)
                    {


                        for (int i = 0; i < data.Minions.Count; i++)
                        {


                            if (data.Minions[i].Destroyed)
                            {
                                for (int j = 0; j <= 4; j++)
                                {
                                    new CargoBox(Position.Random(Spacemap, data.Minions[i].Position.X - 200, data.Minions[i].Position.X + 200, data.Minions[i].Position.Y - 200, data.Minions[i].Position.Y + 200), Spacemap, false, false, false);
                                    data.Minions.Remove(data.Minions[i]);
                                    break;
                                }


                            }
                            else
                            {
                                Movement.Move(data.Minions[i], Position.Random(demaner.Spacemap, demaner.Position.X - 1300, demaner.Position.X + 1300, demaner.Position.Y - 1300, demaner.Position.Y + 1300));
                            }
                        }
                    }
                    if (demaner != null)
                    {
                        if (demaner.Destroyed)
                        {
                            for (int i = 0; i <= GameManager.GameSessions.Count + 10; i++)
                            {
                                new IceBox(Position.Random(Spacemap, demaner.Position.X - 1000, demaner.Position.X + 1000, demaner.Position.Y - 1000, demaner.Position.Y + 1000), Spacemap, false);
                            }

                            sendReward();

                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Emperator.cs] Main void exception: {ex}");
            }
        }


        public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;
            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                npc.Ship.Respawnable = false;
                npc.Ship.Aggressive = Aggresive;
                if (npcID == 78)
                {
                    data.Minions.Add(npc);
                }

            }
            return npc;
        }

        public static void sendReward()
        {

            wait = 1;

            Active = false;

            foreach (Character item in Spacemap.Characters.Values)
            {
                if (item is Player playerWin)
                {


                    playerWin.LoadData();
                    playerWin.ChangeData(DataType.URIDIUM, Uridium);
                    playerWin.ChangeData(DataType.EXPERIENCE, Experience);
                    playerWin.ChangeData(DataType.HONOR, Honor);
                    playerWin.ChangeData(DataType.CREDITS, Credits);
                    playerWin.ChangeData(DataType.EC, 15);


                }
            }

            GameManager.SendPacketToAll($"0|n|KSMSG| {demaner.Name} DESTROYED ");

            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Emperator'");
            }

            tsUpdate.Cancel();
            tsFeedback.Cancel();

        }

    }



}
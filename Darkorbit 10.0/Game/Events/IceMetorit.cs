using Darkorbit.Data;
using Darkorbit.Game.Objects.Collectables;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class IceMetorit
    {
        private static Spacemap Spacemap = GameManager.GetSpacemap(200);
        private static readonly int[] maps = new int[] { 17, 18, 19, 21, 22, 23, 25, 26, 27 };
        public static DataDemaner data = new DataDemaner();

        public static bool Active = false;
        public static bool Spawned = false;
        public static bool SpawnedOnMap = false;

        public static Position CurrentPosition = new Position(new Random().Next(0, 20600), new Random().Next(0, 12600));
        private static Npc demaner = null;
        private static int wait = 1;
        private static Task proccesPlayer;
        private static Task update;
        private static Task feedback;
        private static Task restart;


        public static Random ran = new Random();
        public static List<Portal> portal = new List<Portal>();
        private static readonly Position positionPortal = new Position(10300, 6300);

        private static CancellationTokenSource tsUpdate = new CancellationTokenSource();
        private static CancellationToken ctUpdate;
        private static CancellationTokenSource tsFeedback = new CancellationTokenSource();
        private static CancellationToken ctFeedback;

        public static void Start()
        {
            try
            {
                if (!Active)
                {
                    Active = true;

                    Spacemap = GameManager.GetSpacemap(maps[ran.Next(maps.Length)]);

                    tsUpdate = new CancellationTokenSource();
                    tsFeedback = new CancellationTokenSource();
                    ctUpdate = tsUpdate.Token;
                    ctFeedback = tsFeedback.Token;

                    proccesPlayer = Task.Factory.StartNew(() => Starting());
                    update = Task.Run(async() => await loop());
                    feedback = new Task(feedBack);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.ToString());
            }
        }

        public static bool Status()
        {
            return Active;
        }

        public static void Starting()
        {
            try
            {
                if (Active)
                {
                    Spawned = true;

                    int seconds = 60;
                    int k = seconds;
                    GameManager.SendPacketToAll($"0|n|KSMSG|METEORIT WILL LAND IN MAP {Spacemap.Name} IN {seconds} SECONDS");
                    for (int i = seconds; i > 0; i--)
                    {
                        if (i == k)
                        {
                            string packet = $"0|A|STD|[Map {Spacemap.Name}] Meteorit land in {i} seconds...";

                            GameManager.SendPacketToAll(packet);
                            k += -10;
                        }
                        Thread.Sleep(1000);


                    }

                    GameManager.SendPacketToAll($"0|n|KSMSG|METEORIT LAND IN MAP {Spacemap.Name}");
                    CurrentPosition = new Position(new Random().Next(0, 20600), new Random().Next(0, 12600));
                    demaner = createNPC(101, 1, Spacemap.Id, CurrentPosition);
                    /*demaner.MaxHitPoints = 4500000;
                    demaner.CurrentHitPoints = 6500000;
                    demaner.MaxShieldPoints = 2500000;
                    demaner.CurrentShieldPoints = 6500000;*/
                    demaner.NpcAI.AIOption = NpcAIOption.RANDOM_POSITION_MOVE;
                    update.Start();
                    feedback.Start();
                    for (int i = wait; i > 0; i--)
                    {
                        
                        Thread.Sleep(1000);

                        if (i <= 1 && data.Minions.Count <= 25 && !demaner.Destroyed)
                        {

                            createNPC(103, 1, Spacemap.Id, demaner.Position);
                            demaner.Heal(25000);
                            i = wait + 1;

                        }
                    }

                    SpawnedOnMap = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [IceMeteorit.cs] Main void exception: {ex}");
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
                        if (Active && Spawned && SpawnedOnMap)
                        {
                            GameManager.SendPacketToAll($"0|n|KSMSG|METEORIT IN MAP {Spacemap.Name} POSITION {demaner.Position.X / 100}/{demaner.Position.Y / 100}");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [IceMeteorit.cs] Main void exception: {ex}");
            }
        }

        public static async Task loop()
        {
            try
            {
                while (true)
                {
                    if (ctUpdate.IsCancellationRequested) break;
                    if (data.Minions.Count > 0)
                    {

                        for (int i = 0; i < data.Minions.Count; i++)
                        {
                            //data.Minions[i].Selected = demaner.MainAttacker;
                            //data.Minions[i].Attack();
                            if (!data.Minions[i].Destroyed)
                            {
                                Movement.Move(data.Minions[i], Position.Random(demaner.Spacemap, demaner.Position.X - 1300, demaner.Position.X + 1300, demaner.Position.Y - 1300, demaner.Position.Y + 1300));
                                //Spacemap.RemoveCharacter(data.Minions[i]);
                                /*//new CargoBox(Position.Random(Spacemap, data.Minions[i].Position.X - 200, data.Minions[i].Position.X + 200, data.Minions[i].Position.Y - 200, data.Minions[i].Position.Y + 200), Spacemap, false, false, false);
                                Console.WriteLine(data.Minions[i]);
                                data.Minions.Remove(data.Minions[i]);
                                Spacemap.RemoveCharacter(data.Minions[i]);
                                Console.WriteLine(data.Minions[i]);
                                Program.TickManager.RemoveTick(data.Minions[i]);*/
                            }
                        }
                    }
                    if (demaner != null)
                    {
                        if (demaner.Destroyed)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                new IceBox(Position.Random(Spacemap, demaner.Position.X - 400, demaner.Position.X + 400, demaner.Position.Y - 400, demaner.Position.Y + 400), Spacemap, false);
                            }
                            Spacemap.RemoveCharacter(demaner);
                            sendReward();

                            tsUpdate.Cancel();
                            tsFeedback.Cancel();
                        }
                    }
                    await Task.Delay(500);
                }
            }
            catch (ThreadAbortException ex) { }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [IceMeteorit.cs] Main void exception: {ex}");
            }
        }


        public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;
            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                npc.Ship.Respawnable = false;
                if (npcID == 103)
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
            Spawned = false;
            /*foreach (Character item in Spacemap.Characters.Values)
            {
                if (item is Player playerWin)
                {
                    playerWin.LoadData();
                    playerWin.ChangeData(DataType.URIDIUM, 1000);
                    playerWin.ChangeData(DataType.EXPERIENCE, 7000);
                    playerWin.ChangeData(DataType.HONOR, 700);
                    playerWin.ChangeData(DataType.CREDITS, 100000);
                    playerWin.ChangeData(DataType.EC, 1);
                }
            }*/

            GameManager.SendPacketToAll($"0|n|KSMSG|ICE METEORIT DESTROYED");

            SpawnedOnMap = false;

            tsUpdate.Cancel();
            tsFeedback.Cancel();

            if (EventManager.GetIceMeteorid() == true)
            {
                if (!Active && !Spawned)
                {
                    restart = Task.Factory.StartNew(() => Restart());
                }
            }
        }

        public static void Restart()
        {
            Thread.Sleep(15000);
            Start();
        }

    }



}

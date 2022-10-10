
using Darkorbit.Data;
using Darkorbit.Game.Objects.Collectables;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class DemanerEvent
    {
        private static readonly Spacemap Spacemap = GameManager.GetSpacemap(58);
        public static DataDemaner data = new DataDemaner();

        public static bool Active = false;

        public static Position CurrentPosition = new Position(10300, 6300);
        private static Position Map16Position = new Position(20800, 12900);
        private static Npc demaner = null;
        private static int wait = 2;
        private static Task proccesPlayer;
        private static Task update;
        private static Task finishedEvent;

        private static int MMODamage = 0;
        public static Position MMOPosition = new Position(7000, 13500);
        private static int EICDamage = 0;
        public static Position EICPosition = new Position(28000, 1200);
        private static int VRUDamage = 0;
        public static Position VRUPosition = new Position(28000, 25000);

        public static List<Portal> portal = new List<Portal>();

        private static readonly Position positionPortal = new Position(10300, 6300);

        private static CancellationTokenSource tsUpdate = new CancellationTokenSource();
        private static CancellationToken ctUpdate;

        public static void Start()
        {
            if (!Active)
            {
                Active = true;
                proccesPlayer = Task.Factory.StartNew(() => Starting());
                ctUpdate = tsUpdate.Token;
                update = new Task(loop);


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
                    int seconds = 10;
                    int k = seconds - 10;
                    for (int i = seconds; i > 0; i--)
                    {
                        if (i == k)
                        {
                            string packet = $"0|A|STD| [DEMANER-EVENT] The portal will appears in {i} seconds in center of map...";

                            GameManager.SendPacketToAll(packet);
                            k += -10;
                        }
                        Thread.Sleep(1000);
                        if (i <= 1)
                        {

                            foreach (Spacemap item in GameManager.Spacemaps.Values)
                            {
                                if (item.Id != Spacemap.Id)
                                {
                                    portal.Add(new Portal(item, positionPortal, Position.Random(Spacemap, 2000, 20000, 1500, 10000), Spacemap.Id, 4, 1, true, true, false));
                                }
                            }

                            foreach (Portal portal in portal)
                            {
                                GameManager.SendCommandToMap(portal.Spacemap.Id, portal.GetAssetCreateCommand());
                            }
                        }

                    }
                    GameManager.SendPacketToAll($"0|n|KSMSG|DemaNer Started");
                    demaner = createNPC(126, 1, Spacemap.Id, CurrentPosition);
                    update.Start();
                    for (int i = wait; i > 0; i--)
                    {


                        Thread.Sleep(1000);

                        if (i <= 1 && data.Minions.Count <= 20)
                        {

                            createNPC(11, 1, Spacemap.Id, demaner.Position);
                            i = wait + 5;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [DemanerEvent.cs] Main void exception: {ex}");
            }
        }




        public static void loop()
        {
            try
            {
                while (Active)
                {
                    if (tsUpdate.IsCancellationRequested) break;

                    if (data.Minions.Count > 0)
                    {


                        for (int i = 0; i < data.Minions.Count; i++)
                        {
                            data.Minions[i].Selected = demaner.MainAttacker;
                            data.Minions[i].Attack();

                            if (data.Minions[i].Destroyed)
                            {
                                for (int j = 0; j <= 4; j++)
                                {
                                    new CargoBox(Position.Random(Spacemap, data.Minions[i].Position.X - 200, data.Minions[i].Position.X + 200, data.Minions[i].Position.Y - 200, data.Minions[i].Position.Y + 200), Spacemap, false, false, false);
                                    data.Minions.Remove(data.Minions[i]);
                                    Spacemap.RemoveCharacter(data.Minions[i]);
                                    Program.TickManager.RemoveTick(data.Minions[i]);
                                    break;
                                }


                            }
                            else
                            {
                                Movement.Move(data.Minions[i], Position.Random(demaner.Spacemap, demaner.Position.X - 1000, demaner.Position.X + 1000, demaner.Position.Y - 1000, demaner.Position.Y + 1000));
                            }
                        }
                    }
                    if (demaner != null)
                    {
                        if (demaner.Destroyed)
                        {
                            for (int i = 0; i <= data.Players.Count * 2; i++)
                            {
                                new DemanerBox(Position.Random(Spacemap, demaner.Position.X - 1000, demaner.Position.X + 1000, demaner.Position.Y - 1000, demaner.Position.Y + 1000), Spacemap, false);
                            }
                            sendReward();

                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [DemanerEvent.cs] Main void exception: {ex}");
            }
        }


        public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;
            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                if (npcID == 11)
                {
                    data.Minions.Add(npc);
                }

            }
            return npc;
        }
        public static void AddDamage(Character character, int damage)
        {
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

        }

        public static void sendReward()
        {
            string factionWinner = "";
            wait = 15;

            foreach (Portal portal in portal)
            {
                portal.Remove();
            }
            portal.Add(new Portal(Spacemap, positionPortal, Map16Position, 16, 1, 1, true, true, false));
            foreach (Portal portal in portal)
            {
                GameManager.SendCommandToMap(portal.Spacemap.Id, portal.GetAssetCreateCommand());
            }

            finishedEvent = Task.Run(async()=> await FinishEvent());

            foreach (Player item in data.Players.Values)
            {
                item.Storage.noAttack = false;
            }
            Active = false;
            foreach (Character item in Spacemap.Characters.Values)
            {
                if (item is Player playerWin)
                {


                    playerWin.LoadData();
                    playerWin.ChangeData(DataType.URIDIUM, 150000);
                    playerWin.ChangeData(DataType.EXPERIENCE, 1000000);
                    playerWin.ChangeData(DataType.HONOR, 8000);
                    playerWin.ChangeData(DataType.CREDITS, 20000000);
                    playerWin.ChangeData(DataType.EC, 15);


                }
                if (item is Npc npc)
                {
                    npc.Destroy(item, DestructionType.PLAYER);
                    Spacemap.RemoveCharacter(npc);
                    Program.TickManager.RemoveTick(npc);
                }
            }
            if (MMODamage > EICDamage && MMODamage > VRUDamage)
            {
                foreach (Character item in Spacemap.Characters.Values)
                {
                    if (item is Player playerWin && playerWin.FactionId == 1)
                    {

                        playerWin.SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                        playerWin.LoadData();
                        playerWin.ChangeData(DataType.URIDIUM, 152000);
                        playerWin.ChangeData(DataType.EXPERIENCE, 10350000);
                        playerWin.ChangeData(DataType.HONOR, 121050);
                        playerWin.ChangeData(DataType.CREDITS, 20000000);
                        playerWin.ChangeData(DataType.EC, 10);

                        factionWinner = "MMO";

                    }

                }
            }
            else if (EICDamage > MMODamage && EICDamage > VRUDamage)
            {
                foreach (Character item in Spacemap.Characters.Values)
                {
                    if (item is Player playerWin && playerWin.FactionId == 1)
                    {
                        playerWin.SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                        playerWin.LoadData();
                        playerWin.ChangeData(DataType.URIDIUM, 152000);
                        playerWin.ChangeData(DataType.EXPERIENCE, 10350000);
                        playerWin.ChangeData(DataType.HONOR, 121050);
                        playerWin.ChangeData(DataType.CREDITS, 20000000);
                        playerWin.ChangeData(DataType.EC, 10);

                        factionWinner = "EIC";


                    }

                }
            }
            else if (VRUDamage > MMODamage && VRUDamage > EICDamage)
            {
                foreach (Character item in Spacemap.Characters.Values)
                {
                    if (item is Player playerWin && playerWin.FactionId == 1)
                    {
                        playerWin.SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

                        playerWin.LoadData();
                        playerWin.ChangeData(DataType.URIDIUM, 152000);
                        playerWin.ChangeData(DataType.EXPERIENCE, 10350000);
                        playerWin.ChangeData(DataType.HONOR, 121050);
                        playerWin.ChangeData(DataType.CREDITS, 20000000);
                        playerWin.ChangeData(DataType.EC, 10);

                        factionWinner = "VRU";


                    }

                }
            }
            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Demaner'");
            GameManager.SendPacketToAll($"0|n|KSMSG|{factionWinner} WIN DemaNer-Event");

            tsUpdate.Cancel();

            //portal.Add(new Portal(Spacemap, positionPortal, Map16Position, 16, 1, 1, true, true, false));
            //foreach (Portal portal in portal)
            //{
            //    GameManager.SendCommandToMap(portal.Spacemap.Id, portal.GetAssetCreateCommand());
            //}
            //foreach (Character item in Spacemap.Characters.Values)
            //{
            //    if (item is Npc npc)
            //    {
            //        npc.Destroy(item, DestructionType.PLAYER);
            //        Spacemap.RemoveCharacter(npc);
            //        Program.TickManager.RemoveTick(npc);
            //    }
            //}
        }

        public static async Task FinishEvent()
        {
            try
            {
                int secondsleft = 60;
                int k = secondsleft - 10;

                for (int i = secondsleft; i > 0; i--)
                {
                    if (i == k)
                    {
                        string packet = $"0|A|STD| [DEMANER-EVENT] Map will be closed in {i} seconds";

                        GameManager.SendPacketToAll(packet);
                        k += -10;
                    }
                    if (i <= 1)
                    {
                        foreach (Character item in Spacemap.Characters.Values)
                        {
                            if (item is Player player)
                            {
                                player.SetPosition(player.GetBasePosition());
                                player.Jump(player.GetBaseMapId(), player.Position);
                            }
                        }
                        Spacemap.Characters.Clear();
                        Spacemap.Objects.Clear();
                        foreach (Portal portal in portal)
                        {
                            portal.Remove();
                        }
                    }
                    await Task.Delay(1000);
                    Console.WriteLine($"Seconds left: {i}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [DemanerEvent.cs] Main void exception: {ex}");
            }
        }
    }
}



using Darkorbit.Game.Objects.Collectables;
using Darkorbit.Game.Ticks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class BattleRoyal : Tick
    {

        public Spacemap Spacemap = GameManager.GetSpacemap(224);
        public bool Active = false;

        public ConcurrentDictionary<int, Player> Players = new ConcurrentDictionary<int, Player>();
        public List<int> Finalists = new List<int>();
        public List<Portal> portal = new List<Portal>();
        private readonly Position positionPortal = new Position(10300, 6300);

        public async void Start()
        {
            if (!Active)
            {

                Active = true;

                for (int i = 1; i <= 30; i++)
                {
                    new GreenBooty(Position.Random(Spacemap, 2500, 30000, 2500, 22000), Spacemap, false);
                }

         
                for (int i = 1; i <= 40; i++)
                {
                    new BonusBox(Position.Random(Spacemap, 2500, 30000, 2500, 22000), Spacemap, true);
                }

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

                GameManager.SendPacketToAll($"0|n|KSMSG|Battle Royal started round #{EventManager.countRoyalRound}");

                int seconds = 60;
                int k = seconds - 10;
                for (int i = seconds; i > 0; i--)
                {
                    if (i == k)
                    {
                        string packet = $"0|A|STD| [BATTLE ROYAL]The portal will disappear in {i} seconds, ENTER NOW...";

                        GameManager.SendPacketToAll(packet);
                        k += -10;
                    }
                    await Task.Delay(1000);
                    GameManager.SendPacketToMap(Spacemap.Id, $"0|A|STD|Waiting players ...");
                    if (i <= 1)
                    {
                        foreach (Portal portal in portal)
                        {
                            portal.Remove();
                        }

                        for (int j = 15; j > 0; j--)
                        {

                            GameManager.SendPacketToMap(Spacemap.Id, $"0|A|STD| Battle royal start in {j} seconds...");
                            await Task.Delay(1000);
                            if (j <= 1)
                            {
                                GameManager.SendPacketToMap(Spacemap.Id, $"0|A|STD|-=Start Figth=-");
                                foreach (Player item in Players.Values)
                                {

                                    item.Storage.noAttack = false;
                                }

                            }
                        }
                    }
                }

            }

            Program.TickManager.AddTick(this);
        }

        public bool Status()
        {
            return Active;
        }


        public void Tick()
        {

            if (Active)
            {

                CheckRadiation();
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
                if (Spacemap.Characters.Count == 2 && Finalists.Count < 2)
                {
                    foreach (Character player in Spacemap.Characters.Values)
                    {


                        Finalists.Add(player.Id);


                    }

                }

                if (Spacemap.Characters.Count == 1)
                {
                    SendRewardAndStop(Spacemap.Characters.FirstOrDefault().Value as Player);
                }
            }

        }

        public DateTime jpbTimer = new DateTime();
        public DateTime jpbStartTime = new DateTime();
        public int radiationX = 22800;
        public int radiationY = 10600;

        public int timerSecond = 1;

        public void CheckRadiation()
        {
            if (jpbTimer.AddSeconds(timerSecond) < DateTime.Now && jpbStartTime.AddSeconds(30) < DateTime.Now)
            {
                if ((radiationX - 100 < 12400 && timerSecond == 1))
                {
                    timerSecond = 4;
                    radiationX -= 200;
                }
                else if (radiationX - 100 >= 110)
                {
                    timerSecond = 1;
                    radiationX -= 200;
                }

                POI newPoi = new POI("jpb_poi", POITypes.RADIATION, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(20400, 12400), new Position(radiationX, radiationY) }, true, true);
                POI oldPoi = Spacemap.POIs.FirstOrDefault(x => x.Value.Id == newPoi.Id).Value;

                if (oldPoi != null)
                {
                    Spacemap.POIs.TryRemove(oldPoi.Id, out oldPoi);
                }

                Spacemap.POIs.TryAdd("jpb_poi", newPoi);
                GameManager.SendCommandToMap(Spacemap.Id, newPoi.GetPOICreateCommand());
                jpbTimer = DateTime.Now;
            }
        }

        public async void SendRewardAndStop(Player player)
        {

            Program.TickManager.RemoveTick(this);
            EventManager.countRoyalRound++;
            player.LoadData();
            player.ChangeData(DataType.URIDIUM, 100000);
            player.ChangeData(DataType.EXPERIENCE, 500000);
            player.ChangeData(DataType.HONOR, 10000);
            player.ChangeData(DataType.CREDITS, 10050000);
            player.ChangeData(DataType.EC, 15);

            GameManager.SendPacketToAll($"0|n|KSMSG|{player.Name} WIN BATTLE ROYAL");
            player.SendPacket("0|n|KSMSG|label_traininggrounds_results_victory");

            string title = "title_battle_royal_winner";



            player.SetTitle(title, true);

            await Task.Delay(5000);
            player.ChangeShip(player.Storage.oldShip);
            player.Storage.DamageBoost = player.Storage.oldDamage;
            player.SetPosition(player.GetBasePosition());
            player.Jump(player.GetBaseMapId(), player.Position);

            Active = false;
            radiationX = 22800;
            radiationY = 10600;
            timerSecond = 1;
            Players.Clear();
            Finalists.Clear();
            Spacemap.Characters.Clear();
            Spacemap.Objects.Clear();
            Spacemap.POIs.Clear();
            if (EventManager.countRoyalRound <= 3)
            {

                Start();
            }
            else {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'B.royal'");
                EventManager.countRoyalRound = 1;
            }

        }

        public bool InEvent(Player player)
        {

            return Players.ContainsKey(player.Id);
        }
    }
}
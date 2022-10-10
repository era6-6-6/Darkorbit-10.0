using Darkorbit.Game.Movements;
using Darkorbit.Game.Objects;
using Darkorbit.Game.Ticks;
using Darkorbit.Managers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Position = Darkorbit.Game.Movements.Position;


namespace Darkorbit.Game.Events
{
    internal class BattleCompany : Tick
    {

        public bool Active = false;
        public Spacemap Spacemap = GameManager.GetSpacemap(42);


        public ConcurrentDictionary<int, Player> Players = new ConcurrentDictionary<int, Player>();
        public List<Player> playersMMO = new List<Player>();
        public List<Player> playersEIC = new List<Player>();
        public List<Player> playersVRU = new List<Player>();
        public List<Portal> portal = new List<Portal>();
        public string factionWin = "";
        private readonly Position positionPortal = new Position(10300, 6300);
        public bool portalActive = false;

        public async void Start()
        {
            if (!Active)
            {
                Active = true;
                var mmoPosition = new Position(2000, 3500);
                var eicPosition = new Position(15000, 3500);
                var vruPosition = new Position(9500, 9500);
                var poi1 = new POI("uba_poi1", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> {mmoPosition, new Position(1000, 500) }, true, true);
                var poi2 = new POI("uba_poi2", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { eicPosition, new Position(1000, 500) }, true, true);
                var poi3 = new POI("uba_poi3", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { vruPosition, new Position(1000, 500) }, true, true);
                Spacemap.POIs.TryAdd("uba_poi1", poi1);
                Spacemap.POIs.TryAdd("uba_poi2", poi2);
                Spacemap.POIs.TryAdd("uba_poi3", poi3);
                foreach (Spacemap item in GameManager.Spacemaps.Values)
                {
                    if (item.Id != Spacemap.Id)
                    {
                        portal.Add(new Portal(item, positionPortal, Position.Random(Spacemap, 2000, 20000, 1500, 10000), Spacemap.Id, 4, 1, true, true, false));
                    }
                }
                GameManager.SendPacketToAll($"0|n|KSMSG|Battle Company started round #{EventManager.countCompanyRound}");

                foreach (Portal portal in portal)
                {
                    GameManager.SendCommandToMap(portal.Spacemap.Id, portal.GetAssetCreateCommand());
                }

                int seconds = 60;
                int k = seconds - 10;
                for (int i = seconds; i > 0; i--)
                {
                    if (i == k)
                    {
                        string packet = $"0|A|STD| [Center of map]The portal will disappear in {i} seconds...";

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

                            GameManager.SendPacketToMap(Spacemap.Id, $"0|A|STD|Battle comapny start in {j} seconds...");
                            await Task.Delay(1000);
                            if (j <= 1)
                            {
                                GameManager.SendPacketToMap(Spacemap.Id, $"0|A|STD|-=Start Figth=-");
                              
                            
                                foreach (Player item in Players.Values)
                                {
                                    item.SendCommand(MapRemovePOICommand.write("uba_poi1"));
                                    item.SendCommand(MapRemovePOICommand.write("uba_poi2"));
                                    item.SendCommand(MapRemovePOICommand.write("uba_poi3"));

                                    item.Storage.noAttack = false;
                                }

                            }
                        }
                    }
                }


                Program.TickManager.AddTick(this);
            }


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

                if (playersMMO.Count > 0 && playersEIC.Count == 0 && playersVRU.Count == 0)
                {
                    SendRewardAndStop(playersMMO);
                    factionWin = "MMO";
                }
                else if (playersMMO.Count == 0 && playersEIC.Count > 0 && playersVRU.Count == 0)
                {
                    SendRewardAndStop(playersEIC);
                    factionWin = "EIC";
                }
                else if (playersMMO.Count == 0 && playersEIC.Count == 0 && playersVRU.Count > 0)
                {
                    SendRewardAndStop(playersVRU);
                    factionWin = "VRU";
                }




            }
        }

        public DateTime jpbTimer = new DateTime();
        public DateTime jpbStartTime = new DateTime();
        public int radiationX = 12800;
        public int radiationY = 1600;

        public int timerSecond = 1;

        public void CheckRadiation()
        {

            if (jpbTimer.AddSeconds(timerSecond) < DateTime.Now && jpbStartTime.AddSeconds(30) < DateTime.Now)
            {
                if ((radiationX - 100 == 6400 && timerSecond == 1) || (radiationX - 100 == 3200 && timerSecond == 1) || (radiationX - 100 == 1600 && timerSecond == 1))
                {
                    timerSecond = 30;
                    radiationX -= 100;
                }
                else if (radiationX - 100 > 110)
                {
                    timerSecond = 1;
                    radiationX -= 100;
                }

                POI newPoi = new POI("jpb_poi", POITypes.RADIATION, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(10400, 6400), new Position(radiationX, radiationY) }, true, true);
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

        public void SendRewardAndStop(List<Player> players)
        {

            Program.TickManager.RemoveTick(this);

            EventManager.countCompanyRound++;
            GameManager.SendPacketToAll($"0|n|KSMSG|{factionWin} WIN BATTLE COMPANY");
            foreach (Player player in players)
            {
                player.LoadData();
                player.ChangeData(DataType.URIDIUM, 40000);
                player.ChangeData(DataType.EXPERIENCE, 2000000);
                player.ChangeData(DataType.HONOR, 15000);
                player.ChangeData(DataType.CREDITS, 15000000);
                // player.ChangeData(DataType.EC, 0);




                player.SetPosition(player.GetBasePosition());
                player.Jump(player.GetBaseMapId(), player.Position);
            }

            Active = false;
            radiationX = 12800;
            radiationY = 1600;
            timerSecond = 1;
            Players.Clear();
            playersEIC.Clear();
            playersMMO.Clear();
            playersVRU.Clear();
            Spacemap.Characters.Clear();
            Spacemap.Objects.Clear();
            Spacemap.POIs.Clear();
            portalActive = false;

        }


        public bool InEvent(Player player)
        {

            if (Active && player.Spacemap.Id == Spacemap.Id && Spacemap.Characters.ContainsKey(player.Id))
            {
                return true;
            }

            return false;

        }
    }
}
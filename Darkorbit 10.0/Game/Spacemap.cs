using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Darkorbit.Game.Objects;
using Darkorbit.Game.Ticks;
using Darkorbit.Game.Movements;
using Darkorbit.Game.Objects.Players.Stations;
using Darkorbit.Managers;
using Darkorbit.Game.Events;
using Position = Darkorbit.Game.Movements.Position;
using Darkorbit.Game.Objects.Collectables;
using Darkorbit.Net.netty.handlers;
using Darkorbit.Net.netty.handlers;
using Darkorbit.Game.Objects.Stations;

namespace Darkorbit.Game
{
    class OptionsBase
    {
        public bool StarterMap { get; set; }
        public bool PvpMap { get; set; }
        public bool RangeDisabled { get; set; }
        public bool CloakBlocked { get; set; }
        public bool LogoutBlocked { get; set; }
        public bool DeathLocationRepair { get; set; }
        public bool PvpDisabled { get; set; }
    }

    class NpcsBase
    {
        public int ShipId { get; set; }
        public int Amount { get; set; }
    }

    class Spacemap : Tick
    {
        public ConcurrentDictionary<int, Character> Characters = new ConcurrentDictionary<int, Character>();
        public ConcurrentDictionary<int, Activatable> Activatables = new ConcurrentDictionary<int, Activatable>();
        public ConcurrentDictionary<int, Object> Objects = new ConcurrentDictionary<int, Object>();
        public ConcurrentDictionary<string, POI> POIs = new ConcurrentDictionary<string, POI>();

        public int Id { get; set; }
        public string Name { get; set; }
        public int FactionId { get; set; }
        public string StationsJSON { get; set; }
        public Position[] Limits { get; private set; }
        public OptionsBase Options { get; set; }
        public static DateTime autorefresh = new DateTime();

        private List<NpcsBase> NpcsBase { get; set; }
        private List<PortalBase> PortalBase { get; set; }
        private List<StationBase> StationBase { get; set; }

        private List<CubiThread> CubikonThreadList = new List<CubiThread>();
        private List<BossCubiThread> BossCubikonThreadList = new List<BossCubiThread>();
        private List<CentaurThread> CentaurThreadList = new List<CentaurThread>();
        private List<BattlerayThread> BattlerayThreadList = new List<BattlerayThread>();
        private List<MeteoritThread> MeteoritThreadList = new List<MeteoritThread>();
        private List<KukuThread> KukuThreadList = new List<KukuThread>();
        public Task inactivityTask = null;
        public Task autoRefreshTask = null;
        public int inactivityTime = 300;
        public int inactivityTimeDefault = 300;

        public static bool BoxenEvent = false;
        public int UserId { get; set; }

        public List<CubiThread> GetCubikonThreadList()
        {
            return CubikonThreadList;
        }
        public List<CentaurThread> GetCentaurThreadList()
        {
            return CentaurThreadList;
        }
        public List<BossCubiThread> GetBossCubikonThreadList()
        {
            return BossCubikonThreadList;
        }
        public List<BattlerayThread> GetBattlerayThreadList()
        {
            return BattlerayThreadList;
        }
        public List<MeteoritThread> GetMeteoritThreadList()
        {
            return MeteoritThreadList;
        }
        public List<KukuThread> GetKukuThreadList()
        {
            return KukuThreadList;
        }

        public void AddCubikonThreadList(CubiThread ct)
        {
            lock (CubikonThreadList)
            {
                CubikonThreadList.Add(ct);
            }
        }
        public void AddBossCubikonThreadList(BossCubiThread ct)
        {
            BossCubikonThreadList.Add(ct);
        }
        public void AddCentaurThreadList(CentaurThread ct)
        {
            CentaurThreadList.Add(ct);
        }
        public void AddBattlerayThreadList(BattlerayThread ct)
        {
            BattlerayThreadList.Add(ct);
        }
        public void AddMeteoritThreadList(MeteoritThread ct)
        {
            MeteoritThreadList.Add(ct);
        }
        public void AddKukuThreadList(KukuThread ct)
        {
            KukuThreadList.Add(ct);
        }

        public Spacemap(int mapId, string name, int factionId, List<NpcsBase> npcs, List<PortalBase> portals, List<StationBase> stations, OptionsBase options, bool dynamic = false)
        {
            Id = mapId;
            Name = name;
            FactionId = factionId;
            NpcsBase = npcs;
            PortalBase = portals;
            StationBase = stations;
            Options = options;
            ParseLimits();
            LoadObjects();

            Program.TickManager.AddTickMap(this);

            if (dynamic)
            {
                inactivityTask = Task.Run(async () =>await InactivityTask(Id));
            }
        }

        public async Task InactivityTask(int mapId)
        {
            //remove temporary maps after an amount of time to save memory usage
            while(true)
            {
                if(Characters.Count(x => x.Value is Player) > 0)
                {
                    inactivityTime = inactivityTimeDefault;
                } else
                {
                    inactivityTime--;
                    if(inactivityTime < 0)
                    {
                        Spacemap tmp = null;
                        GameManager.Spacemaps.TryRemove(mapId, out tmp);
                        Console.WriteLine("map "+mapId+" was destroyed");
                        break;
                    }
                }
                await Task.Delay(1000);
            }
        }
       

        public void Tick()
        {
            CharacterTicker();
            //ObjectsTicker();
        }

        private void ParseLimits()
        {
            Limits = new Position[] { null, null };
            Limits[0] = new Position(0, 0);
            if ( Id == 224 || Id == 91 || Id == 92 || Id == 93)
                Limits[1] = new Position(41800, 26000);
            else Limits[1] = new Position(20800, 12800);
        }

        public class CubiThread
        {
            private int threadId;
            private Task thread;

            public CubiThread(int id, Task t)
            {
                threadId = id;
                thread = t;
            }

            public Task GetThread()
            {
                return thread;
            }
            public int GetId()
            {
                return threadId;
            }
        }
        public class BossCubiThread
        {
            private int threadId;
            private Task thread;

            public BossCubiThread(int id, Task t)
            {
                threadId = id;
                thread = t;
            }

            public Task GetThread()
            {
                return thread;
            }
            public int GetId()
            {
                return threadId;
            }
        }
        public class CentaurThread
        {
            private int threadId;
            private Task thread;

            public CentaurThread(int id, Task t)
            {
                threadId = id;
                thread = t;
            }

            public Task GetThread()
            {
                return thread;
            }
            public int GetId()
            {
                return threadId;
            }
        }
        public class BattlerayThread
        {
            private int threadId;
            private Task thread;

            public BattlerayThread(int id, Task t)
            {
                threadId = id;
                thread = t;
            }

            public Task GetThread()
            {
                return thread;
            }
            public int GetId()
            {
                return threadId;
            }
        }
        public class MeteoritThread
        {
            private int threadId;
            private Task thread;

            public MeteoritThread(int id, Task t)
            {
                threadId = id;
                thread = t;
            }

            public Task GetThread()
            {
                return thread;
            }
            public int GetId()
            {
                return threadId;
            }
        }
        public class KukuThread
        {
            private int threadId;
            private Task thread;

            public KukuThread(int id, Task t)
            {
                threadId = id;
                thread = t;
            }

            public Task GetThread()
            {
                return thread;
            }
            public int GetId()
            {
                return threadId;
            }
        }

        public void LoadObjects()
        {
            int CubiCount = 0;
            int BossCubiCount = 0;
            int CentaurCount = 0;
            int BattlerayCount = 0;
            int MeteoritCount = 0;
            int KukuCount = 0;

            if (StationBase != null && StationBase.Count >= 1)
            {
                foreach (var station in StationBase)
                {
                    var position = new Position(station.Position[0], station.Position[1]);

                    switch (station.TypeId)
                    {
                        case AssetTypeModule.BASE_COMPANY:
                            new HomeStation(this, station.FactionId, position, GameManager.GetClan(0));
                            break;

                        case AssetTypeModule.STATION_TURRET_SMALL:
                            new TURRETSMALL(this, station.FactionId, position, GameManager.GetClan(0));
                            break;

                        case AssetTypeModule.STATION_TURRET_LARGE:
                            new TURRETSGRANDE(this, station.FactionId, position, GameManager.GetClan(0));
                            break;
                    }
                }
            }

            if (NpcsBase != null && NpcsBase.Count >= 1)
            {
                foreach (var npc in NpcsBase)
                {
                    for (int i = 1; i <= npc.Amount; i++)
                    {
                        //if (Id == 3 || Id == 11 || Id == 55 || Id == 58 || Id == 2 || Id == 10 || Id == 16)
                       
                            if (npc.ShipId == 80 && Id != 42)
                            {
                                //Logger.Log("error_log", $"- [CubiconDebug] Cubicon spawned");
                                CubiThread tmp = Cubikon.SpawnCubicon(npc.ShipId, this, CubiCount);
                                AddCubikonThreadList(tmp);

                                CubiCount++;
                            }
                            else if (npc.ShipId == 119)
                            {
                                //Logger.Log("error_log", $"- [CubiconDebug] Cubicon spawned");
                                BossCubiThread tmp = BossCubikon.SpawnBossCubicon(npc.ShipId, this, BossCubiCount);
                                AddBossCubikonThreadList(tmp);

                                BossCubiCount++;
                            }
                            else if (npc.ShipId == 90)
                            {
                                //Logger.Log("error_log", $"- [CubiconDebug] Cubicon spawned");
                                CentaurThread tmp = Centaur.SpawnCentaur(npc.ShipId, this, CentaurCount);
                                AddCentaurThreadList(tmp);

                                CentaurCount++;
                            }
                            else if (npc.ShipId == 115)
                            {
                                //Logger.Log("error_log", $"- [CubiconDebug] Cubicon spawned");
                                BattlerayThread tmp = Battleray.SpawnBattleray(npc.ShipId, this, BattlerayCount);
                                AddBattlerayThreadList(tmp);

                                BattlerayCount++;
                            }
                            else if (npc.ShipId == 101)
                            {
                                //Logger.Log("error_log", $"- [CubiconDebug] Cubicon spawned");
                                MeteoritThread tmp = Meteorit.SpawnMeteorit(npc.ShipId, this, MeteoritCount);
                                AddMeteoritThreadList(tmp);

                                MeteoritCount++;
                            }
                            else if (npc.ShipId == 83)
                            {
                                //Logger.Log("error_log", $"- [CubiconDebug] Cubicon spawned");
                                KukuThread tmp = Kuku.SpawnKuku(npc.ShipId, this, KukuCount);
                                AddKukuThreadList(tmp);

                                KukuCount++;
                            }
                            else
                                new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npc.ShipId), this, Position.Random(this, 0, 20800, 0, 12800));
                        
                    }
                }

                /*foreach (var npcx2 in NpcsBase)
                {
                    for (int i = 1; i <= npcx2.Amount; i++)
                    {
                        if (Id == 29)
                        {
                            new Npcx2(Randoms.CreateRandomID(), GameManager.GetShip(npcx2.ShipId), this, Position.Random(this, 0, 41800, 0, 26000));
                        }
                    }

                }*/
            }

            //if (new int[] { 13,14,15,17,18,19,20,21,22,23,24,25,26,27,28 }.Contains(Id))
            //{
            //    for (int k = 0; k <= 10; k++)
            //    {
            //        int milShips = Randoms.random.Next(NPCFlagship.MilitaryShips.Count);
            //        new NPCFlagship(Randoms.CreateRandomID(), GameManager.GetShip(NPCFlagship.MilitaryShips[milShips]), this, Position.Random(this, 0, 20800, 0, 12800));
            //    }
            //}

            /*if (new int[] { 1, 2, 3, 4 }.Contains(Id))
            {
                for (int k = 0; k <= 10; k++)
                {
                    NPCFlagship.Add(this, 1);

                }
            }

            if (new int[] { 5, 6, 7, 8 }.Contains(Id))
            {
                for (int k = 0; k <= 10; k++)
                {
                    NPCFlagship.Add(this, 2);
                }
            }

            if (new int[] { 9, 10, 11, 12 }.Contains(Id))
            {
                for (int k = 0; k <= 10; k++)
                {
                    NPCFlagship.Add(this, 3);
                }
            }



            //if (new int[] { 150 }.Contains(Id))
            //{
            //    for (int k = 0; k <= 50; k++)
            //    {
            //        new NPCFlagship(Randoms.CreateRandomID(), GameManager.GetShip(56), this, Position.Random(this, 0, 20800, 0, 12800));
            //    }
            //}
            */
            if (PortalBase != null && PortalBase.Count >= 1)
            {
                foreach (var portal in PortalBase)
                {
                    var portalPosition = new Position(portal.Position[0], portal.Position[1]);
                    var targetPosition = new Position(portal.TargetPosition[0], portal.TargetPosition[1]);
                    new Portal(this, portalPosition, targetPosition, portal.TargetSpaceMapId, portal.GraphicId, portal.FactionId, portal.Visible, portal.Working, portal.PremiumMap);
                }
            }


            if (new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 }.Contains(Id))
            {
                for (int i = 1; i <= 125; i++)
                    new BonusBox(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 50; i++)
                //     new MiniPumpkin(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 50; i++)
                //     new GiantPumpkin(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 50; i++)
                //     new Flower(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 60; i++)
                //   new AlienEgg(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 60; i++)
                //     new FromShip(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 50; i++)
                //   new gifbox(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 50; i++)
                // new DemanerBox(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 50; i++)
                // new IceBox(Position.Random(this, 0, 20800, 0, 12800), this, true);

                //for (int i = 1; i <= 50; i++)
                //  new RedBooty(Position.Random(this, 0, 20800, 0, 12800), this, true);

                // for (int i = 1; i <= 50; i++)
                // new BlueBooty(Position.Random(this, 0, 20800, 0, 12800), this, true);
            }
             if (new int[] {16}.Contains(Id))
             {
                for (int i = 1; i <= 60; i++)
                    new GreenBooty(Position.Random(this, 0, 20800, 0, 12800), this, true);

                //for (int i = 1; i <= 260; i++)
                //    new BonusBox(Position.Random(this, 0, 20800, 0, 12800), this, true);
             }
            //4-5 box
            /*if (new int[] { 29 }.Contains(Id))
            {
                for (int i = 1; i <= 80; i++)
                    new gifbox(Position.Random(this, 0, 41500, 0, 25500), this, true);

                for (int i = 1; i <= 100; i++)
                    new BonusBox(Position.Random(this, 0, 41500, 0, 25500), this, true);
            }*/
            if(Id == 42)
            {
                for (int i = 0; i < 60; i++)
                {
                    new BonusBox(Position.Random(this, 0, 20800, 0, 12800), this, true);
                }
            }
            if (Id == 101)
            {
                var poi = new POI("jackpot_poi", POITypes.RADIATION, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(5000, 3200), new Position(2250, 950) }, true, true);
                POIs.TryAdd("jackpot_poi", poi);
            }

            //if (Id == 121)
            {
                //var poi1 = new POI("uba_poi1", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5000, 3000), new Position(4000, 4000), new Position(4000, 2000), new Position(6000, 2000) }, true, true);
                //var poi2 = new POI("uba_poi2", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(4400, 3600), new Position(200, 100) }, true, true);
                //var poi3 = new POI("uba_poi3", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(5600, 2400), new Position(200, 100) }, true, true);

                //POIs.TryAdd("uba_poi1", poi1);
                //POIs.TryAdd("uba_poi2", poi2);
                //POIs.TryAdd("uba_poi3", poi3);
            }
            /*if ( Id == 42 || Id == 224/* || Id == 29*/ //|| //Id == 91 || Id == 92 || Id == 93)
           /* {
                var poi199 = new POI("uba_poi199", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(-1110, -110), new Position(45000, -110) }, true, true);
                var poi2 = new POI("uba_poi2", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(4400, 3600), new Position(200, 100) }, true, true);
                var poi3 = new POI("uba_poi3", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(5600, 2400), new Position(200, 100) }, true, true);

                POIs.TryAdd("uba_poi1", poi199);
                //POIs.TryAdd("uba_poi2", poi2);
                //POIs.TryAdd("uba_poi3", poi3);
            }
            else
            {
                var poi199 = new POI("uba_poi199", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(-1110, -110), new Position(45000, -110) }, true, true);
                var poi2 = new POI("uba_poi2", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(4400, 3600), new Position(200, 100) }, true, true);
                var poi3 = new POI("uba_poi3", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.CIRCLE, new List<Position> { new Position(5600, 2400), new Position(200, 100) }, true, true);

                POIs.TryAdd("uba_poi199", poi199);
                //POIs.TryAdd("uba_poi2", poi2);
                //POIs.TryAdd("uba_poi3", poi3);
            }*/
            if (Id == 306)
            {
                var poi1 = new POI("uba_poi1", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(7000, -300), new Position(7000, 10000) }, true, true); //recta para abajo
                var poi2 = new POI("uba_poi2", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(-1200, 9900), new Position(5600, 9900) }, true, true); //horizontal para abajo

                var poi20 = new POI("uba_poi20", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(17200, 2000), new Position(17200, 9100) }, true, true); //recta para abajo
                var poi21 = new POI("uba_poi21", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(17200, 9100), new Position(22000, 9100) }, true, true); //horizontal para abajo

                var poi3 = new POI("uba_poi3", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6000, 700) }, true, true); //cuadrado
                var poi4 = new POI("uba_poi4", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5800, 2500) }, true, true); //cuadrado
                var poi5 = new POI("uba_poi5", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5600, 4000) }, true, true); //cuadrado
                var poi6 = new POI("uba_poi6", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5600, 5100) }, true, true); //cuadrado
                var poi9 = new POI("uba_poi9", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(4000, 5500) }, true, true); //cuadrado
                var poi13 = new POI("uba_poi13", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(4000, 7500) }, true, true); //cuadrado
                var poi12 = new POI("uba_poi12", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(4000, 8500) }, true, true); //cuadrado

                //cuadrados fuera de rectangulo
                var poi7 = new POI("uba_poi7", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(10100, 5100) }, true, true); //cuadrado
                var poi8 = new POI("uba_poi8", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(12100, 5100) }, true, true); //cuadrado
                var poi14 = new POI("uba_poi14", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(9300, 10700) }, true, true); //cuadrado
                var poi10 = new POI("uba_poi10", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(10100, 10200) }, true, true); //cuadrado
                var poi11 = new POI("uba_poi11", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(11700, 10300) }, true, true); //cuadrado

                POIs.TryAdd("uba_poi1", poi1);
                POIs.TryAdd("uba_poi2", poi2);
                POIs.TryAdd("uba_po20", poi20);
                POIs.TryAdd("uba_po21", poi21);

                POIs.TryAdd("uba_poi3", poi3);
                POIs.TryAdd("uba_poi4", poi4);
                POIs.TryAdd("uba_poi5", poi5);
                POIs.TryAdd("uba_poi6", poi6);
                POIs.TryAdd("uba_poi7", poi7);
                POIs.TryAdd("uba_poi8", poi8);
                POIs.TryAdd("uba_poi9", poi9);

                POIs.TryAdd("uba_po10", poi10);
                POIs.TryAdd("uba_po11", poi11);
                POIs.TryAdd("uba_poi2", poi12);
                POIs.TryAdd("uba_poi13", poi13);
                POIs.TryAdd("uba_poi14", poi14);

                var poi2493 = new POI("uba_poi2493", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(9800, 7400), new Position(11000, 7400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi2493", poi2493);

                var poi2494 = new POI("uba_poi2494", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(10700, 2900), new Position(11200, 2900) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi2494", poi2494);
            }

            if (Id == 307)
            {
                var poi300 = new POI("uba_poi300", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(500, 9400), new Position(9100, 9400) }, true, true); //horizontal para abajo linea 1
                var poi301 = new POI("uba_poi301", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(10300, 9400), new Position(16000, 9400) }, true, true); //horizontal para abajo linea 1
                var poi302 = new POI("uba_poi302", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(17200, 9400), new Position(20000, 9400) }, true, true); //horizontal para abajo linea 1

                var poi303 = new POI("uba_poi303", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(9700, -100), new Position(9700, 5400) }, true, true); //recta para abajo
                var poi304 = new POI("uba_poi304", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5300, 5400), new Position(9100, 5400) }, true, true); //horizontal para abajo linea 1
                var poi305 = new POI("uba_poi305", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(4700, 2300), new Position(4700, 5400) }, true, true); //recta para abajo
                var poi06 = new POI("uba_poi064", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(1400, 1700), new Position(6400, 1700) }, true, true); //horizontal para abajo linea 1

                var poi07 = new POI("uba_poi077", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(-500, 3800), new Position(3000, 3800) }, true, true); //recta para abajo

                POIs.TryAdd("uba_poi300", poi300);
                POIs.TryAdd("uba_poi301", poi301);
                POIs.TryAdd("uba_poi302", poi302);

                POIs.TryAdd("uba_poi303", poi303);
                POIs.TryAdd("uba_poi304", poi304);
                POIs.TryAdd("uba_poi305", poi305);
                POIs.TryAdd("uba_poi064", poi06);
                POIs.TryAdd("uba_poi077", poi07);

                var poi5112 = new POI("uba_poi5112", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(13600, 6300), new Position(13600, 6400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5112", poi5112);

                var poi5113 = new POI("uba_poi5113", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(14400, 5300), new Position(14400, 5300) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5113", poi5113);

                var poi5114 = new POI("uba_poi5114", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(14700, 4300), new Position(14700, 4300) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5114", poi5114);

                var poi5115 = new POI("uba_poi5115", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(16500, 3400), new Position(16500, 3400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5115", poi5115);

                var poi5116 = new POI("uba_poi5116", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(17500, 3100), new Position(17500, 3100) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5116", poi5116);

                var poi5117 = new POI("uba_poi5117", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(16500, 4900), new Position(16500, 4900) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5117", poi5117);

                var poi5118 = new POI("uba_poi5118", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(18300, 4400), new Position(18300, 4400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5118", poi5118);

                var poi5119 = new POI("uba_poi5119", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(17300, 5500), new Position(17300, 5500) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5119", poi5119);

                var poi1119 = new POI("uba_poi1119", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(19200, 2100), new Position(19400, 2100) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi1119", poi1119);

                var poi1113 = new POI("uba_poi1113", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(1000, 10800), new Position(1600, 10800) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi1113", poi1113);

                var poi1133 = new POI("uba_poi1133", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(1800, 11500), new Position(1800, 11500) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi1133", poi1133);

                var poi1233 = new POI("uba_poi1233", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(3600, 11500), new Position(3600, 11500) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi1233", poi1233);

                var poi0233 = new POI("uba_poi0233", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(3600, 10400), new Position(3600, 10400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi0233", poi0233);

                var poi0433 = new POI("uba_poi0433", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(18100, 10400), new Position(18100, 10800) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi0433", poi0433);

                var poi2433 = new POI("uba_poi2433", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(17400, 11300), new Position(17400, 11500) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi2433", poi2433);

                var poi2493 = new POI("uba_poi2493", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(18900, 11200), new Position(18900, 11200) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi2493", poi2493);

            }

            if (Id == 308)
            {
                var poi3003 = new POI("uba_poi3003", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(14400, 1900), new Position(14400, 9700) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3003", poi3003);

                var poi3004 = new POI("uba_poi3004", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5400, 2000), new Position(5400, 10400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3004", poi3004);

                var poi3005 = new POI("uba_poi3005", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(1200, 2000), new Position(12800, 2000) }, true, true); //horizontal para abajo linea
                POIs.TryAdd("uba_poi3005", poi3005);

                var poi3007 = new POI("uba_poi3007", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(16600, 9100), new Position(22000, 9100) }, true, true); //horizontal para abajo linea
                POIs.TryAdd("uba_poi3007", poi3007);

                var poi3009 = new POI("uba_poi3009", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(1500, 11800), new Position(1800, 11700) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3009", poi3009);

                var poi3099 = new POI("uba_poi3099", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(2500, 12400), new Position(2800, 12400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3099", poi3099);

                var poi3096 = new POI("uba_poi3096", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(1100, 9100), new Position(1300, 9100) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3096", poi3096);

                var poi3044 = new POI("uba_poi3044", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(2100, 10000), new Position(2200, 10000) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3044", poi3044);

                var poi3041 = new POI("uba_poi3041", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(7200, 9800), new Position(7300, 9800) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3041", poi3041);

                var poi3021 = new POI("uba_poi3021", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(7100, 11400), new Position(7200, 11400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3021", poi3021);

                var poi3331 = new POI("uba_poi3331", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(5400, 12700), new Position(5200, 12700) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3331", poi3331);

                var poi2331 = new POI("uba_poi3331", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(15400, 700), new Position(15400, 700) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3331", poi2331);

                var poi2332 = new POI("uba_poi3332", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(15900, 1400), new Position(15900, 1400) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi3332", poi2332);

                var poi2132 = new POI("uba_poi2132", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(16100, 6600), new Position(16100, 6600) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi2132", poi2132);

                var poi1132 = new POI("uba_poi1132", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(16800, 7300), new Position(16800, 7300) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi1132", poi1132);

                var poi5132 = new POI("uba_poi5132", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(16900, 8100), new Position(16900, 8100) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5132", poi5132);

                var poi5111 = new POI("uba_poi5111", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(4200, 11500), new Position(4400, 11500) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5111", poi5111);

                var poi5112 = new POI("uba_poi5112", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(16100, 9600), new Position(16100, 9600) }, true, true); //horizontal para abajo linea 1
                POIs.TryAdd("uba_poi5112", poi5112);

            }

            if (Id == 91)
            {

                var Labyrinth1 = new POI("Labyrinth1", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24786, 5040), new Position(25353, 7360) }, true, false, "");
                var Labyrinth2 = new POI("Labyrinth2", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18387, 5440), new Position(19035, 7360) }, true, false, "");
                var Labyrinth3 = new POI("Labyrinth3", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17091, 5600), new Position(17820, 7520) }, true, false, "");
                var Labyrinth4 = new POI("Labyrinth4", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19602, 5840), new Position(22113, 6560) }, true, false, "");
                var Labyrinth5 = new POI("Labyrinth5", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23571, 6080), new Position(24300, 7920) }, true, false, "");
                var Labyrinth6 = new POI("Labyrinth6", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(15876, 6320), new Position(16524, 8320) }, true, false, "");
                var Labyrinth7 = new POI("Labyrinth7", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26406, 6400), new Position(27216, 8320) }, true, false, "");
                var Labyrinth8 = new POI("Labyrinth8", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20817, 6960), new Position(23328, 7680) }, true, false, "");
                var Labyrinth9 = new POI("Labyrinth9", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27621, 7600), new Position(29565, 8320) }, true, false, "");
                var Labyrinth10 = new POI("Labyrinth10", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(14742, 7680), new Position(15309, 10720) }, true, false, "");
                var Labyrinth11 = new POI("Labyrinth11", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17091, 7680), new Position(19683, 8320) }, true, false, "");
                var Labyrinth12 = new POI("Labyrinth12", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24624, 7840), new Position(26001, 8320) }, true, false, "");
                var Labyrinth13 = new POI("Labyrinth13", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(22437, 8160), new Position(23085, 9840) }, true, false, "");
                var Labyrinth14 = new POI("Labyrinth14", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20898, 8320), new Position(22032, 8960) }, true, false, "");
                var Labyrinth15 = new POI("Labyrinth15", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19764, 8640), new Position(20412, 10240) }, true, false, "");
                var Labyrinth16 = new POI("Labyrinth16", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(15633, 8720), new Position(18954, 9440) }, true, false, "");
                var Labyrinth17 = new POI("Labyrinth17", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24786, 8800), new Position(28755, 9440) }, true, false, "");
                var Labyrinth18 = new POI("Labyrinth18", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23490, 9280), new Position(24300, 12560) }, true, false, "");
                var Labyrinth19 = new POI("Labyrinth19", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17982, 9760), new Position(18954, 12160) }, true, false, "");
                var Labyrinth20 = new POI("Labyrinth20", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24786, 9840), new Position(25515, 11280) }, true, false, "");
                var Labyrinth21 = new POI("Labyrinth21", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(15795, 9920), new Position(17496, 10480) }, true, false, "");
                var Labyrinth22 = new POI("Labyrinth22", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26082, 9920), new Position(26730, 12960) }, true, false, "");
                var Labyrinth23 = new POI("Labyrinth23", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27216, 9920), new Position(29565, 10880) }, true, false, "");
                var Labyrinth24 = new POI("Labyrinth24", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19440, 10560), new Position(22032, 11200) }, true, false, "");
                var Labyrinth25 = new POI("Labyrinth25", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(16929, 10880), new Position(17496, 13200) }, true, false, "");
                var Labyrinth26 = new POI("Labyrinth26", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(13527, 11040), new Position(16362, 11680) }, true, false, "");
                var Labyrinth27 = new POI("Labyrinth27", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27621, 11520), new Position(28188, 13040) }, true, false, "");
                var Labyrinth28 = new POI("Labyrinth28", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24867, 11760), new Position(25515, 15200) }, true, false, "");
                var Labyrinth29 = new POI("Labyrinth29", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(15066, 12160), new Position(15876, 14080) }, true, false, "");
                var Labyrinth30 = new POI("Labyrinth30", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(13770, 12800), new Position(14337, 15040) }, true, false, "");
                var Labyrinth31 = new POI("Labyrinth31", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23328, 12960), new Position(24300, 14400) }, true, false, "");
                var Labyrinth32 = new POI("Labyrinth32", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26244, 13440), new Position(28512, 14000) }, true, false, "");
                var Labyrinth33 = new POI("Labyrinth33", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(16362, 13600), new Position(18792, 14240) }, true, false, "");
                var Labyrinth34 = new POI("Labyrinth34", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19116, 14080), new Position(19764, 16640) }, true, false, "");
                var Labyrinth35 = new POI("Labyrinth35", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27783, 14480), new Position(28512, 16880) }, true, false, "");
                var Labyrinth36 = new POI("Labyrinth36", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(15147, 14560), new Position(18630, 15120) }, true, false, "");
                var Labyrinth37 = new POI("Labyrinth37", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26082, 14640), new Position(27216, 16080) }, true, false, "");
                var Labyrinth38 = new POI("Labyrinth38", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(22275, 14800), new Position(24300, 15520) }, true, false, "");
                var Labyrinth39 = new POI("Labyrinth39", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20331, 15040), new Position(21951, 15920) }, true, false, "");
                var Labyrinth40 = new POI("Labyrinth40", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17172, 15360), new Position(17820, 17920) }, true, false, "");
                var Labyrinth41 = new POI("Labyrinth41", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18225, 15440), new Position(18792, 16800) }, true, false, "");
                var Labyrinth42 = new POI("Labyrinth42", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(13203, 15600), new Position(16281, 16080) }, true, false, "");
                var Labyrinth43 = new POI("Labyrinth43", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23328, 15840), new Position(25272, 16560) }, true, false, "");
                var Labyrinth44 = new POI("Labyrinth44", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(21465, 16240), new Position(22275, 18480) }, true, false, "");
                var Labyrinth45 = new POI("Labyrinth45", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(14823, 16640), new Position(15471, 19200) }, true, false, "");
                var Labyrinth46 = new POI("Labyrinth46", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(15876, 16640), new Position(16686, 17600) }, true, false, "");
                var Labyrinth47 = new POI("Labyrinth47", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(25677, 16640), new Position(27216, 17280) }, true, false, "");
                var Labyrinth48 = new POI("Labyrinth48", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(22761, 16960), new Position(23814, 18560) }, true, false, "");
                var Labyrinth49 = new POI("Labyrinth49", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24300, 16960), new Position(25029, 19840) }, true, false, "");
                var Labyrinth50 = new POI("Labyrinth50", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19035, 17040), new Position(21141, 17600) }, true, false, "");
                var Labyrinth51 = new POI("Labyrinth51", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28107, 17360), new Position(28755, 19680) }, true, false, "");
                var Labyrinth52 = new POI("Labyrinth52", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(25434, 17680), new Position(27621, 18320) }, true, false, "");
                var Labyrinth53 = new POI("Labyrinth53", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(15876, 17920), new Position(16605, 20480) }, true, false, "");
                var Labyrinth54 = new POI("Labyrinth54", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20007, 17920), new Position(20898, 20800) }, true, false, "");
                var Labyrinth55 = new POI("Labyrinth55", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17091, 18320), new Position(19602, 18960) }, true, false, "");
                var Labyrinth56 = new POI("Labyrinth56", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(25839, 18880), new Position(26568, 20560) }, true, false, "");
                var Labyrinth57 = new POI("Labyrinth57", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(21303, 18960), new Position(23895, 19600) }, true, false, "");
                var Labyrinth58 = new POI("Labyrinth58", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17091, 19440), new Position(17820, 21120) }, true, false, "");
                var Labyrinth59 = new POI("Labyrinth59", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(22194, 20080), new Position(25272, 20560) }, true, false, "");

                POIs.TryAdd("Labyrinth1", Labyrinth1);
                POIs.TryAdd("Labyrinth2", Labyrinth2);
                POIs.TryAdd("Labyrinth3", Labyrinth3);
                POIs.TryAdd("Labyrinth4", Labyrinth4);
                POIs.TryAdd("Labyrinth5", Labyrinth5);
                POIs.TryAdd("Labyrinth6", Labyrinth6);
                POIs.TryAdd("Labyrinth7", Labyrinth7);
                POIs.TryAdd("Labyrinth8", Labyrinth8);
                POIs.TryAdd("Labyrinth9", Labyrinth9);
                POIs.TryAdd("Labyrinth10", Labyrinth10);
                POIs.TryAdd("Labyrinth11", Labyrinth11);
                POIs.TryAdd("Labyrinth12", Labyrinth12);
                POIs.TryAdd("Labyrinth13", Labyrinth13);
                POIs.TryAdd("Labyrinth14", Labyrinth14);
                POIs.TryAdd("Labyrinth15", Labyrinth15);
                POIs.TryAdd("Labyrinth16", Labyrinth16);
                POIs.TryAdd("Labyrinth17", Labyrinth17);
                POIs.TryAdd("Labyrinth18", Labyrinth18);
                POIs.TryAdd("Labyrinth19", Labyrinth19);
                POIs.TryAdd("Labyrinth20", Labyrinth20);
                POIs.TryAdd("Labyrinth21", Labyrinth21);
                POIs.TryAdd("Labyrinth22", Labyrinth22);
                POIs.TryAdd("Labyrinth23", Labyrinth23);
                POIs.TryAdd("Labyrinth24", Labyrinth24);
                POIs.TryAdd("Labyrinth25", Labyrinth25);
                POIs.TryAdd("Labyrinth26", Labyrinth26);
                POIs.TryAdd("Labyrinth27", Labyrinth27);
                POIs.TryAdd("Labyrinth28", Labyrinth28);
                POIs.TryAdd("Labyrinth29", Labyrinth29);
                POIs.TryAdd("Labyrinth30", Labyrinth30);
                POIs.TryAdd("Labyrinth31", Labyrinth31);
                POIs.TryAdd("Labyrinth32", Labyrinth32);
                POIs.TryAdd("Labyrinth33", Labyrinth33);
                POIs.TryAdd("Labyrinth34", Labyrinth34);
                POIs.TryAdd("Labyrinth35", Labyrinth35);
                POIs.TryAdd("Labyrinth36", Labyrinth36);
                POIs.TryAdd("Labyrinth37", Labyrinth37);
                POIs.TryAdd("Labyrinth38", Labyrinth38);
                POIs.TryAdd("Labyrinth39", Labyrinth39);
                POIs.TryAdd("Labyrinth40", Labyrinth40);
                POIs.TryAdd("Labyrinth41", Labyrinth41);
                POIs.TryAdd("Labyrinth42", Labyrinth42);
                POIs.TryAdd("Labyrinth43", Labyrinth43);
                POIs.TryAdd("Labyrinth44", Labyrinth44);
                POIs.TryAdd("Labyrinth45", Labyrinth45);
                POIs.TryAdd("Labyrinth46", Labyrinth46);
                POIs.TryAdd("Labyrinth47", Labyrinth47);
                POIs.TryAdd("Labyrinth48", Labyrinth48);
                POIs.TryAdd("Labyrinth49", Labyrinth49);
                POIs.TryAdd("Labyrinth50", Labyrinth50);
                POIs.TryAdd("Labyrinth51", Labyrinth51);
                POIs.TryAdd("Labyrinth52", Labyrinth52);
                POIs.TryAdd("Labyrinth53", Labyrinth53);
                POIs.TryAdd("Labyrinth54", Labyrinth54);
                POIs.TryAdd("Labyrinth55", Labyrinth55);
                POIs.TryAdd("Labyrinth56", Labyrinth56);
                POIs.TryAdd("Labyrinth57", Labyrinth57);
                POIs.TryAdd("Labyrinth58", Labyrinth58);
                POIs.TryAdd("Labyrinth59", Labyrinth59);

            }

            if (Id == 92)
            {

                var Labyrinth60 = new POI("Middle11", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(6400, 7168), new Position(6912, 7168), new Position(6912, 7424), new Position(6400, 7424) }, true, false, "");
                var Labyrinth61 = new POI("Middle10", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(5632, 6656), new Position(6400, 6656), new Position(6400, 6912), new Position(5632, 6912) }, true, false, "");
                var Labyrinth62 = new POI("Middle13", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(7168, 8448), new Position(7424, 8448), new Position(7424, 8960), new Position(7168, 8960) }, true, false, "");
                var Labyrinth63 = new POI("Middle12", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(6144, 8192), new Position(6400, 8192), new Position(6400, 8960), new Position(6144, 8960) }, true, false, "");
                var Labyrinth64 = new POI("Middle15", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(8960, 8960), new Position(9216, 8960), new Position(9216, 9472), new Position(8960, 9472) }, true, false, "");
                var Labyrinth65 = new POI("Middle14", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(7680, 9216), new Position(8192, 9216), new Position(8192, 9728), new Position(7680, 9728) }, true, false, "");
                var Labyrinth66 = new POI("Middle17", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(5376, 9216), new Position(5888, 9216), new Position(5888, 9728), new Position(5376, 9728) }, true, false, "");
                var Labyrinth67 = new POI("Middle16", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(9472, 9472), new Position(10240, 9472), new Position(10240, 9728), new Position(9472, 9728) }, true, false, "");
                var Labyrinth68 = new POI("Middle19", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(11776, 9472), new Position(12288, 9472), new Position(12288, 9984), new Position(11776, 9984) }, true, false, "");
                var Labyrinth69 = new POI("Middle18", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(10752, 9728), new Position(11264, 9728), new Position(11264, 10496), new Position(10752, 10496) }, true, false, "");
                var Labyrinth70 = new POI("Middle20", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(12800, 9728), new Position(13312, 9728), new Position(13312, 9984), new Position(12800, 9984) }, true, false, "");
                var Labyrinth71 = new POI("Middle22", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(14080, 8448), new Position(14336, 8448), new Position(14336, 8960), new Position(14080, 8960) }, true, false, "");
                var Labyrinth73 = new POI("Middle21", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(13056, 8704), new Position(13568, 8704), new Position(13568, 9216), new Position(13056, 9216) }, true, false, "");
                var Labyrinth75 = new POI("Middle24", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(14336, 7168), new Position(14848, 7168), new Position(14848, 7680), new Position(14336, 7680) }, true, false, "");
                var Labyrinth77 = new POI("Middle23", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(13568, 7680), new Position(14080, 7680), new Position(14080, 7936), new Position(13568, 7936) }, true, false, "");
                var Labyrinth79 = new POI("Middle9", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(6144, 5632), new Position(6912, 5632), new Position(6912, 6144), new Position(6144, 6144) }, true, false, "");
                var Labyrinth80 = new POI("Middle26", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(14592, 5376), new Position(15104, 5376), new Position(15104, 5632), new Position(14592, 5632) }, true, false, "");
                var Labyrinth82 = new POI("Middle25", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(14080, 6400), new Position(14592, 6400), new Position(14592, 6656), new Position(14080, 6656) }, true, false, "");
                //var Labyrinth83 = new POI("mapAssetRangeZone-150000144", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.CIRCLE, new List<Position> { new Position(9990,6622),  }, 580, 580, true, false, "");
                var Labyrinth85 = new POI("Middle28", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(14080, 4096), new Position(14336, 4096), new Position(14336, 4608), new Position(14080, 4608) }, true, false, "");
                var Labyrinth87 = new POI("Middle27", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(13824, 4864), new Position(14336, 4864), new Position(14336, 5376), new Position(13824, 5376) }, true, false, "");
                var Labyrinth88 = new POI("Middle29", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(13056, 3584), new Position(13568, 3584), new Position(13568, 4096), new Position(13056, 4096) }, true, false, "");
                var Labyrinth96 = new POI("Middle31", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(11264, 3072), new Position(11776, 3072), new Position(11776, 3584), new Position(11264, 3584) }, true, false, "");
                var Labyrinth97 = new POI("Middle30", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(12544, 2816), new Position(12800, 2816), new Position(12800, 3328), new Position(12544, 3328) }, true, false, "");
                var Labyrinth98 = new POI("Middle33", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(9216, 3328), new Position(9472, 3328), new Position(9472, 4096), new Position(9216, 4096) }, true, false, "");
                var Labyrinth100 = new POI("Middle32", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(10240, 2816), new Position(10496, 2816), new Position(10496, 3584), new Position(10240, 3584) }, true, false, "");
                var Labyrinth102 = new POI("Middle35", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(7936, 3584), new Position(8192, 3584), new Position(8192, 4096), new Position(7936, 4096) }, true, false, "");
                var Labyrinth104 = new POI("Middle34", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(8448, 2816), new Position(8960, 2816), new Position(8960, 3328), new Position(8448, 3328) }, true, false, "");
                var Labyrinth105 = new POI("Middle37", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(6656, 4352), new Position(7168, 4352), new Position(7168, 4864), new Position(6656, 4864) }, true, false, "");
                var Labyrinth106 = new POI("Middle36", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(6912, 3584), new Position(7424, 3584), new Position(7424, 3840), new Position(6912, 3840) }, true, false, "");
                var Labyrinth107 = new POI("Middle38", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(5888, 4608), new Position(6400, 4608), new Position(6400, 4864), new Position(5888, 4864) }, true, false, "");
                //var Labyrinth108 = new POI("Equippable Zone", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(10500,6750),  }, 1500, 1500, true, false, "");
                var Labyrinth109 = new POI("SpaceStation20", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(10496, 5888), new Position(10752, 5888), new Position(10752, 6315), new Position(10496, 6315) }, true, false, "");
                var Labyrinth110 = new POI("SpaceStation21", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(10752, 5888), new Position(11008, 5888), new Position(11008, 6315), new Position(10752, 6315) }, true, false, "");
                var Labyrinth111 = new POI("SpaceStation22", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(11264, 6144), new Position(11424, 6144), new Position(11424, 6315), new Position(11264, 6315) }, true, false, "");
                var Labyrinth112 = new POI("SpaceStation23", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(11008, 6144), new Position(11264, 6144), new Position(11264, 6315), new Position(11008, 6315) }, true, false, "");
                var Labyrinth89 = new POI("SpaceStation13", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(9728, 7083), new Position(11264, 7083), new Position(11264, 7168), new Position(9728, 7168) }, true, false, "");
                var Labyrinth90 = new POI("SpaceStation14", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(9728, 7168), new Position(9984, 7168), new Position(9984, 7324), new Position(9728, 7324) }, true, false, "");
                var Labyrinth91 = new POI("SpaceStation15", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(9728, 5888), new Position(10240, 5888), new Position(10240, 6315), new Position(9728, 6315) }, true, false, "");
                var Labyrinth92 = new POI("SpaceStation16", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9728, 5888), new Position(9728, 5888), new Position(9728, 6144), new Position(9728, 6144) }, true, false, "");
                var Labyrinth93 = new POI("SpaceStation17", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9472, 6144), new Position(9728, 6144), new Position(9728, 6144), new Position(9472, 6144) }, true, false, "");
                var Labyrinth94 = new POI("SpaceStation18", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(10240, 5888), new Position(10496, 5888), new Position(10496, 6315), new Position(10240, 6315) }, true, false, "");
                var Labyrinth95 = new POI("SpaceStation19", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9984, 5888), new Position(10240, 5888), new Position(10240, 5888), new Position(9984, 5888) }, true, false, "");
                var Labyrinth103 = new POI("SpaceStation12", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9216, 6656), new Position(9216, 6656), new Position(9216, 6656), new Position(9216, 6656) }, true, false, "");
                var Labyrinth99 = new POI("SpaceStation10", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9472, 6144), new Position(9472, 6144), new Position(9472, 6400), new Position(9472, 6400) }, true, false, "");
                var Labyrinth101 = new POI("SpaceStation11", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9216, 6400), new Position(9216, 6400), new Position(9216, 6912), new Position(9216, 6912) }, true, false, "");
                var Labyrinth84 = new POI("SpaceStation8", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(9472, 6144), new Position(9728, 6144), new Position(9728, 7324), new Position(9472, 7324) }, true, false, "");
                var Labyrinth86 = new POI("SpaceStation9", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9216, 6400), new Position(9472, 6400), new Position(9472, 7068), new Position(9216, 7068) }, true, false, "");
                var Labyrinth81 = new POI("SpaceStation7", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(9984, 7424), new Position(10240, 7424), new Position(10240, 7552), new Position(9984, 7552) }, true, false, "");
                var Labyrinth78 = new POI("SpaceStation6", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(10752, 7168), new Position(11008, 7168), new Position(11008, 7168), new Position(10752, 7168) }, true, false, "");
                var Labyrinth72 = new POI("SpaceStation3", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(9984, 7168), new Position(10752, 7168), new Position(10752, 7424), new Position(9984, 7424) }, true, false, "");
                var Labyrinth76 = new POI("SpaceStation5", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(10752, 7168), new Position(11008, 7168), new Position(11008, 7168), new Position(10752, 7168) }, true, false, "");
                var Labyrinth74 = new POI("SpaceStation4", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.POLYGON, new List<Position> { new Position(10752, 7168), new Position(11264, 7168), new Position(11264, 7168), new Position(10752, 7168) }, true, false, "");

                POIs.TryAdd("Labyrinth60", Labyrinth60);
                POIs.TryAdd("Labyrinth61", Labyrinth61);
                POIs.TryAdd("Labyrinth62", Labyrinth62);
                POIs.TryAdd("Labyrinth63", Labyrinth63);
                POIs.TryAdd("Labyrinth64", Labyrinth64);
                POIs.TryAdd("Labyrinth65", Labyrinth65);
                POIs.TryAdd("Labyrinth66", Labyrinth66);
                POIs.TryAdd("Labyrinth67", Labyrinth67);
                POIs.TryAdd("Labyrinth68", Labyrinth68);
                POIs.TryAdd("Labyrinth69", Labyrinth69);
                POIs.TryAdd("Labyrinth70", Labyrinth70);
                POIs.TryAdd("Labyrinth71", Labyrinth71);
                POIs.TryAdd("Labyrinth72", Labyrinth72);
                POIs.TryAdd("Labyrinth73", Labyrinth73);
                POIs.TryAdd("Labyrinth74", Labyrinth74);
                POIs.TryAdd("Labyrinth75", Labyrinth75);
                POIs.TryAdd("Labyrinth76", Labyrinth76);
                POIs.TryAdd("Labyrinth77", Labyrinth77);
                POIs.TryAdd("Labyrinth78", Labyrinth78);
                POIs.TryAdd("Labyrinth79", Labyrinth79);
                POIs.TryAdd("Labyrinth80", Labyrinth80);
                POIs.TryAdd("Labyrinth81", Labyrinth81);
                POIs.TryAdd("Labyrinth82", Labyrinth82);
                //POIs.TryAdd("Labyrinth83", Labyrinth83);
                POIs.TryAdd("Labyrinth84", Labyrinth84);
                POIs.TryAdd("Labyrinth85", Labyrinth85);
                POIs.TryAdd("Labyrinth86", Labyrinth86);
                POIs.TryAdd("Labyrinth87", Labyrinth87);
                POIs.TryAdd("Labyrinth88", Labyrinth88);
                POIs.TryAdd("Labyrinth89", Labyrinth89);
                POIs.TryAdd("Labyrinth90", Labyrinth90);
                POIs.TryAdd("Labyrinth91", Labyrinth91);
                POIs.TryAdd("Labyrinth92", Labyrinth92);
                POIs.TryAdd("Labyrinth93", Labyrinth93);
                POIs.TryAdd("Labyrinth94", Labyrinth94);
                POIs.TryAdd("Labyrinth95", Labyrinth95);
                POIs.TryAdd("Labyrinth96", Labyrinth96);
                POIs.TryAdd("Labyrinth97", Labyrinth97);
                POIs.TryAdd("Labyrinth98", Labyrinth98);
                POIs.TryAdd("Labyrinth99", Labyrinth99);
                POIs.TryAdd("Labyrinth100", Labyrinth100);
                POIs.TryAdd("Labyrinth101", Labyrinth101);
                POIs.TryAdd("Labyrinth102", Labyrinth102);
                POIs.TryAdd("Labyrinth103", Labyrinth103);
                POIs.TryAdd("Labyrinth104", Labyrinth104);
                POIs.TryAdd("Labyrinth105", Labyrinth105);
                POIs.TryAdd("Labyrinth106", Labyrinth106);
                POIs.TryAdd("Labyrinth107", Labyrinth107);
                //POIs.TryAdd("Labyrinth108", Labyrinth108);
                POIs.TryAdd("Labyrinth109", Labyrinth109);
                POIs.TryAdd("Labyrinth110", Labyrinth110);
                POIs.TryAdd("Labyrinth111", Labyrinth111);
                POIs.TryAdd("Labyrinth112", Labyrinth112);

            }

            if (Id == 93)
            {

                var Labyrinth1 = new POI("Labyrinth1", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(16284, 177), new Position(16815, 2537) }, true, false, "");
                var Labyrinth2 = new POI("Labyrinth2", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17169, 177), new Position(19116, 649) }, true, false, "");
                var Labyrinth3 = new POI("Labyrinth3", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19883, 177), new Position(20414, 2301) }, true, false, "");
                var Labyrinth4 = new POI("Labyrinth4", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20886, 177), new Position(23128, 708) }, true, false, "");
                var Labyrinth5 = new POI("Labyrinth5", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23659, 177), new Position(24190, 1711) }, true, false, "");
                var Labyrinth6 = new POI("Labyrinth6", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27907, 177), new Position(28497, 1652) }, true, false, "");
                var Labyrinth7 = new POI("Labyrinth7", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(29146, 236), new Position(31329, 590) }, true, false, "");
                var Labyrinth8 = new POI("Labyrinth8", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24485, 472), new Position(27494, 885) }, true, false, "");
                var Labyrinth9 = new POI("Labyrinth9", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(30031, 944), new Position(30621, 3481) }, true, false, "");
                var Labyrinth10 = new POI("Labyrinth10", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28969, 1121), new Position(29441, 3835) }, true, false, "");
                var Labyrinth11 = new POI("Labyrinth11", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17287, 1180), new Position(19529, 1593) }, true, false, "");
                var Labyrinth12 = new POI("Labyrinth12", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(25252, 1180), new Position(25901, 3835) }, true, false, "");
                var Labyrinth13 = new POI("Labyrinth13", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26137, 1947), new Position(28556, 2537) }, true, false, "");
                var Labyrinth14 = new POI("Labyrinth14", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17169, 2065), new Position(17818, 4543) }, true, false, "");
                var Labyrinth15 = new POI("Labyrinth15", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18113, 2065), new Position(19470, 2773) }, true, false, "");
                var Labyrinth16 = new POI("Labyrinth16", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20001, 2655), new Position(20591, 3953) }, true, false, "");
                var Labyrinth17 = new POI("Labyrinth17", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19057, 3127), new Position(19529, 4248) }, true, false, "");
                var Labyrinth18 = new POI("Labyrinth18", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27730, 3127), new Position(28202, 4366) }, true, false, "");
                var Labyrinth19 = new POI("Labyrinth19", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18231, 3245), new Position(18644, 5959) }, true, false, "");
                var Labyrinth20 = new POI("Labyrinth20", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26491, 3422), new Position(27140, 6254) }, true, false, "");
                var Labyrinth21 = new POI("Labyrinth21", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(31093, 3953), new Position(31565, 6903) }, true, false, "");
                var Labyrinth22 = new POI("Labyrinth22", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24544, 4130), new Position(26196, 4661) }, true, false, "");
                var Labyrinth23 = new POI("Labyrinth23", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28556, 4130), new Position(30562, 4602) }, true, false, "");
                var Labyrinth24 = new POI("Labyrinth24", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18939, 4484), new Position(21240, 4897) }, true, false, "");
                var Labyrinth25 = new POI("Labyrinth25", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17287, 5015), new Position(17818, 7257) }, true, false, "");
                var Labyrinth26 = new POI("Labyrinth26", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27376, 5015), new Position(30798, 5369) }, true, false, "");
                var Labyrinth27 = new POI("Labyrinth27", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(25370, 5074), new Position(26137, 5900) }, true, false, "");
                var Labyrinth28 = new POI("Labyrinth28", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19057, 5192), new Position(19470, 6313) }, true, false, "");
                var Labyrinth29 = new POI("Labyrinth29", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19942, 5192), new Position(22302, 5841) }, true, false, "");
                var Labyrinth30 = new POI("Labyrinth30", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23010, 5251), new Position(24957, 5723) }, true, false, "");
                var Labyrinth31 = new POI("Labyrinth31", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(1652, 5664), new Position(2655, 6726) }, true, false, "");
                var Labyrinth32 = new POI("Labyrinth32", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(30385, 5723), new Position(30857, 7080) }, true, false, "");
                var Labyrinth33 = new POI("Labyrinth33", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28025, 5841), new Position(30031, 6431) }, true, false, "");
                var Labyrinth34 = new POI("Labyrinth34", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(295, 5900), new Position(1180, 7139) }, true, false, "");
                var Labyrinth35 = new POI("Labyrinth35", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3245, 6195), new Position(4130, 7493) }, true, false, "");
                var Labyrinth36 = new POI("Labyrinth36", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20886, 6254), new Position(23482, 6785) }, true, false, "");
                var Labyrinth37 = new POI("Labyrinth37", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24013, 6254), new Position(26137, 6785) }, true, false, "");
                var Labyrinth38 = new POI("Labyrinth38", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18290, 6549), new Position(20296, 7021) }, true, false, "");
                var Labyrinth39 = new POI("Labyrinth39", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27081, 6549), new Position(27553, 9263) }, true, false, "");
                var Labyrinth40 = new POI("Labyrinth40", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20591, 7257), new Position(21004, 9853) }, true, false, "");
                var Labyrinth41 = new POI("Labyrinth41", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27848, 7375), new Position(31565, 8024) }, true, false, "");
                var Labyrinth42 = new POI("Labyrinth42", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17287, 7611), new Position(20178, 8260) }, true, false, "");
                var Labyrinth43 = new POI("Labyrinth43", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3894, 7906), new Position(5015, 9263) }, true, false, "");
                var Labyrinth44 = new POI("Labyrinth44", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(30090, 8319), new Position(30562, 10856) }, true, false, "");
                var Labyrinth45 = new POI("Labyrinth45", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18231, 8555), new Position(18762, 11269) }, true, false, "");
                var Labyrinth46 = new POI("Labyrinth46", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28320, 8555), new Position(28851, 10207) }, true, false, "");
                var Labyrinth47 = new POI("Labyrinth47", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17169, 8614), new Position(17818, 9971) }, true, false, "");
                var Labyrinth48 = new POI("Labyrinth48", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18998, 8614), new Position(20296, 9204) }, true, false, "");
                var Labyrinth49 = new POI("Labyrinth49", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(30916, 9263), new Position(31506, 12154) }, true, false, "");
                var Labyrinth50 = new POI("Labyrinth50", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19824, 9499), new Position(20237, 11505) }, true, false, "");
                var Labyrinth51 = new POI("Labyrinth51", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3186, 9676), new Position(3953, 10738) }, true, false, "");
                var Labyrinth52 = new POI("Labyrinth52", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24367, 9794), new Position(26609, 10325) }, true, false, "");
                var Labyrinth53 = new POI("Labyrinth53", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20709, 10148), new Position(21122, 11564) }, true, false, "");
                var Labyrinth54 = new POI("Labyrinth54", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17523, 10384), new Position(17995, 12390) }, true, false, "");
                var Labyrinth55 = new POI("Labyrinth55", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27199, 10443), new Position(29736, 11210) }, true, false, "");
                var Labyrinth56 = new POI("Labyrinth56", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19116, 10620), new Position(19529, 13393) }, true, false, "");
                var Labyrinth57 = new POI("Labyrinth57", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26078, 10679), new Position(26609, 13098) }, true, false, "");
                var Labyrinth58 = new POI("Labyrinth58", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3953, 10974), new Position(4661, 12036) }, true, false, "");
                var Labyrinth59 = new POI("Labyrinth59", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23718, 10974), new Position(25724, 11682) }, true, false, "");
                var Labyrinth60 = new POI("Labyrinth60", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28733, 11505), new Position(30562, 12213) }, true, false, "");
                var Labyrinth61 = new POI("Labyrinth61", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18349, 11800), new Position(18821, 13511) }, true, false, "");
                var Labyrinth62 = new POI("Labyrinth62", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(27435, 11800), new Position(28143, 14986) }, true, false, "");
                var Labyrinth63 = new POI("Labyrinth63", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19883, 11859), new Position(22125, 12390) }, true, false, "");
                var Labyrinth64 = new POI("Labyrinth64", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(22597, 11859), new Position(23010, 13688) }, true, false, "");
                var Labyrinth65 = new POI("Labyrinth65", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24839, 12154), new Position(25606, 14455) }, true, false, "");
                var Labyrinth66 = new POI("Labyrinth66", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3481, 12272), new Position(4602, 13216) }, true, false, "");
                var Labyrinth67 = new POI("Labyrinth67", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28674, 12626), new Position(32214, 13216) }, true, false, "");
                var Labyrinth68 = new POI("Labyrinth68", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(19942, 12685), new Position(20296, 14809) }, true, false, "");
                var Labyrinth69 = new POI("Labyrinth69", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(21476, 12980), new Position(21889, 15753) }, true, false, "");
                var Labyrinth70 = new POI("Labyrinth70", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26019, 13452), new Position(27081, 13924) }, true, false, "");
                var Labyrinth71 = new POI("Labyrinth71", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(4071, 13629), new Position(4897, 14750) }, true, false, "");
                var Labyrinth72 = new POI("Labyrinth72", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(17641, 13747), new Position(19588, 14278) }, true, false, "");
                var Labyrinth73 = new POI("Labyrinth73", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(28497, 13865), new Position(41064, 15753) }, true, false, "");
                var Labyrinth74 = new POI("Labyrinth74", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(22066, 14042), new Position(24426, 14632) }, true, false, "");
                var Labyrinth75 = new POI("Labyrinth75", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(26668, 14278), new Position(27140, 16166) }, true, false, "");
                var Labyrinth76 = new POI("Labyrinth76", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(23718, 14986), new Position(26314, 15576) }, true, false, "");
                var Labyrinth77 = new POI("Labyrinth77", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(22479, 15045), new Position(23423, 15635) }, true, false, "");
                var Labyrinth78 = new POI("Labyrinth78", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3186, 15104), new Position(4071, 16166) }, true, false, "");
                var Labyrinth79 = new POI("Labyrinth79", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(18880, 15340), new Position(21063, 15753) }, true, false, "");
                var Labyrinth80 = new POI("Labyrinth80", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(24839, 15930), new Position(25429, 17405) }, true, false, "");
                var Labyrinth81 = new POI("Labyrinth81", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(20650, 16048), new Position(24367, 16756) }, true, false, "");
                var Labyrinth82 = new POI("Labyrinth82", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3894, 16461), new Position(4425, 17700) }, true, false, "");
                var Labyrinth83 = new POI("Labyrinth83", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(25724, 16461), new Position(29264, 17169) }, true, false, "");
                var Labyrinth84 = new POI("Labyrinth84", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(3658, 18113), new Position(4838, 19293) }, true, false, "");
                var Labyrinth85 = new POI("Labyrinth85", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(59, 18939), new Position(826, 20001) }, true, false, "");
                var Labyrinth86 = new POI("Labyrinth86", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(1180, 19234), new Position(2242, 20119) }, true, false, "");
                var Labyrinth87 = new POI("Labyrinth87", POITypes.NO_ACCESS, POIDesigns.ASTEROIDS_MIXED_WITH_SCRAP, POIShapes.RECTANGLE, new List<Position> { new Position(2537, 19588), new Position(3599, 20709) }, true, false, "");

                POIs.TryAdd("Labyrinth1", Labyrinth1);
                POIs.TryAdd("Labyrinth2", Labyrinth2);
                POIs.TryAdd("Labyrinth3", Labyrinth3);
                POIs.TryAdd("Labyrinth4", Labyrinth4);
                POIs.TryAdd("Labyrinth5", Labyrinth5);
                POIs.TryAdd("Labyrinth6", Labyrinth6);
                POIs.TryAdd("Labyrinth7", Labyrinth7);
                POIs.TryAdd("Labyrinth8", Labyrinth8);
                POIs.TryAdd("Labyrinth9", Labyrinth9);
                POIs.TryAdd("Labyrinth10", Labyrinth10);
                POIs.TryAdd("Labyrinth11", Labyrinth11);
                POIs.TryAdd("Labyrinth12", Labyrinth12);
                POIs.TryAdd("Labyrinth13", Labyrinth13);
                POIs.TryAdd("Labyrinth14", Labyrinth14);
                POIs.TryAdd("Labyrinth15", Labyrinth15);
                POIs.TryAdd("Labyrinth16", Labyrinth16);
                POIs.TryAdd("Labyrinth17", Labyrinth17);
                POIs.TryAdd("Labyrinth18", Labyrinth18);
                POIs.TryAdd("Labyrinth19", Labyrinth19);
                POIs.TryAdd("Labyrinth20", Labyrinth20);
                POIs.TryAdd("Labyrinth21", Labyrinth21);
                POIs.TryAdd("Labyrinth22", Labyrinth22);
                POIs.TryAdd("Labyrinth23", Labyrinth23);
                POIs.TryAdd("Labyrinth24", Labyrinth24);
                POIs.TryAdd("Labyrinth25", Labyrinth25);
                POIs.TryAdd("Labyrinth26", Labyrinth26);
                POIs.TryAdd("Labyrinth27", Labyrinth27);
                POIs.TryAdd("Labyrinth28", Labyrinth28);
                POIs.TryAdd("Labyrinth29", Labyrinth29);
                POIs.TryAdd("Labyrinth30", Labyrinth30);
                POIs.TryAdd("Labyrinth31", Labyrinth31);
                POIs.TryAdd("Labyrinth32", Labyrinth32);
                POIs.TryAdd("Labyrinth33", Labyrinth33);
                POIs.TryAdd("Labyrinth34", Labyrinth34);
                POIs.TryAdd("Labyrinth35", Labyrinth35);
                POIs.TryAdd("Labyrinth36", Labyrinth36);
                POIs.TryAdd("Labyrinth37", Labyrinth37);
                POIs.TryAdd("Labyrinth38", Labyrinth38);
                POIs.TryAdd("Labyrinth39", Labyrinth39);
                POIs.TryAdd("Labyrinth30", Labyrinth30);
                POIs.TryAdd("Labyrinth40", Labyrinth40);
                POIs.TryAdd("Labyrinth41", Labyrinth41);
                POIs.TryAdd("Labyrinth42", Labyrinth42);
                POIs.TryAdd("Labyrinth43", Labyrinth43);
                POIs.TryAdd("Labyrinth44", Labyrinth44);
                POIs.TryAdd("Labyrinth45", Labyrinth45);
                POIs.TryAdd("Labyrinth46", Labyrinth46);
                POIs.TryAdd("Labyrinth47", Labyrinth47);
                POIs.TryAdd("Labyrinth48", Labyrinth48);
                POIs.TryAdd("Labyrinth49", Labyrinth49);
                POIs.TryAdd("Labyrinth50", Labyrinth50);
                POIs.TryAdd("Labyrinth51", Labyrinth51);
                POIs.TryAdd("Labyrinth52", Labyrinth52);
                POIs.TryAdd("Labyrinth53", Labyrinth53);
                POIs.TryAdd("Labyrinth54", Labyrinth54);
                POIs.TryAdd("Labyrinth55", Labyrinth55);
                POIs.TryAdd("Labyrinth56", Labyrinth56);
                POIs.TryAdd("Labyrinth57", Labyrinth57);
                POIs.TryAdd("Labyrinth58", Labyrinth58);
                POIs.TryAdd("Labyrinth59", Labyrinth59);
                POIs.TryAdd("Labyrinth50", Labyrinth50);
                POIs.TryAdd("Labyrinth61", Labyrinth61);
                POIs.TryAdd("Labyrinth62", Labyrinth62);
                POIs.TryAdd("Labyrinth63", Labyrinth63);
                POIs.TryAdd("Labyrinth64", Labyrinth64);
                POIs.TryAdd("Labyrinth65", Labyrinth65);
                POIs.TryAdd("Labyrinth66", Labyrinth66);
                POIs.TryAdd("Labyrinth67", Labyrinth67);
                POIs.TryAdd("Labyrinth68", Labyrinth68);
                POIs.TryAdd("Labyrinth69", Labyrinth69);
                POIs.TryAdd("Labyrinth60", Labyrinth60);
                POIs.TryAdd("Labyrinth71", Labyrinth71);
                POIs.TryAdd("Labyrinth72", Labyrinth72);
                POIs.TryAdd("Labyrinth73", Labyrinth73);
                POIs.TryAdd("Labyrinth74", Labyrinth74);
                POIs.TryAdd("Labyrinth75", Labyrinth75);
                POIs.TryAdd("Labyrinth76", Labyrinth76);
                POIs.TryAdd("Labyrinth77", Labyrinth77);
                POIs.TryAdd("Labyrinth78", Labyrinth78);
                POIs.TryAdd("Labyrinth79", Labyrinth79);
                POIs.TryAdd("Labyrinth70", Labyrinth70);
                POIs.TryAdd("Labyrinth81", Labyrinth81);
                POIs.TryAdd("Labyrinth82", Labyrinth82);
                POIs.TryAdd("Labyrinth83", Labyrinth83);
                POIs.TryAdd("Labyrinth84", Labyrinth84);
                POIs.TryAdd("Labyrinth85", Labyrinth85);
                POIs.TryAdd("Labyrinth86", Labyrinth86);
                POIs.TryAdd("Labyrinth87", Labyrinth87);
                POIs.TryAdd("Labyrinth80", Labyrinth80);
            }

        }

        //TODO: rewrite this into better code
        public void CharacterTicker()
        {
            try
            {
                foreach (var character in Characters.Values)
                {
                    foreach (var otherCharacter in Characters.Values.Where(x => x.Id != character.Id && !x.Equals(character)))
                    {
                        if (character.InRange(otherCharacter, otherCharacter.RenderRange))
                        {
                            character.AddInRangeCharacter(otherCharacter);
                        }                        

                        else if (character.SelectedCharacter == null || (character.SelectedCharacter != null && !character.SelectedCharacter.Equals(otherCharacter)))
                            character.RemoveInRangeCharacter(otherCharacter);
                    }

                    if (character is Player player)
                    {
                        foreach (var obj in Objects.Values)
                        {
                            if (obj is Collectable collectable)
                            {
                                if (!(collectable.ToPlayer == null || (collectable.ToPlayer != null && player == collectable.ToPlayer))) continue;
                            }
                            else if (obj is Mine mine)
                            {
                                if (!(player == mine.Player || !Duel.InDuel(player) || (Duel.InDuel(player) && player.Storage.Duel?.GetOpponent(player) == mine.Player))) continue;
                            }

                            if (player.Position.DistanceTo(obj.Position) <= 1250)
                            {
                                if (!player.Storage.InRangeObjects.ContainsKey(obj.Id))
                                {
                                    player.Storage.InRangeObjects.TryAdd(obj.Id, obj);

                                    if (obj is Collectable)
                                        player.SendCommand((obj as Collectable).GetCollectableCreateCommand());
                                    else if (obj is Mine)
                                        player.SendCommand((obj as Mine).GetMineCreateCommand());
                                    else if (obj is Asset)
                                        player.SendCommand((obj as Asset).GetAssetCreateCommand());
                                }
                                else
                                {
                                    if (obj is Mine)
                                    {
                                        var mine = obj as Mine;
                                        if (mine.Active && mine.activationTime.AddMilliseconds(Mine.ACTIVATION_TIME) < DateTime.Now)
                                        {
                                            if (player.Position.DistanceTo(mine.Position) < Mine.RANGE)
                                            {
                                                mine.Remove();
                                                mine.Explode();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (player.Storage.InRangeObjects.ContainsKey(obj.Id))
                                {
                                    player.Storage.InRangeObjects.TryRemove(obj.Id, out var outObj);

                                    if (obj is Collectable)
                                        player.SendCommand(DisposeBoxCommand.write((obj as Collectable).Hash, true));
                                    else if (obj is Mine)
                                        player.SendPacket($"0|{Net.netty.ServerCommands.REMOVE_ORE}|{(obj as Mine).Hash}");
                                    else if (obj is Asset)
                                        player.SendCommand(AssetRemoveCommand.write(new AssetTypeModule((obj as Asset).AssetTypeId), obj.Id));
                                }
                            }
                        }


                        if (CheckRadiation(player) || CheckActivatables(player))
                            player.SendCommand(player.GetBeaconCommand());





                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Spacemap.cs] Execute void exception: {ex}");
            }
        }

        private async void ObjectsTicker()
            {
                try
                {
                    foreach (var character in Characters.Values)
                    {
                        foreach (var otherCharacter in Characters.Values.Where(x => x.Id != character.Id && !x.Equals(character)))
                        if (Id == 16)
                        {
                            if ((character.InRange(otherCharacter, otherCharacter.RenderRange2)))
                            {
                                if (otherCharacter is Npc && otherCharacter.Destroyed == false)
                                {
                                    character.AddInRangeCharacter(otherCharacter);
                                    await Task.Delay(15000);
                                }
                            }
                        }
                    await Task.Delay(15000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Spacemap.cs] Execute void exception: {ex}");
            }
        }

        public bool CheckActivatables(Player Player)
        {
            bool isInSecureZone = false;
            bool inEquipZone = false;
            bool onBlockedMinePosition = false;

            foreach (var entity in Activatables.Values)
            {
                bool inRange = Player.Position.DistanceTo(entity.Position) <= (entity is HomeStation ? HomeStation.SECURE_ZONE_RANGE : 700);



                short status = inRange ? MapAssetActionAvailableCommand.ON : MapAssetActionAvailableCommand.OFF;

                if (inRange)
                {
                    if (!(entity is BattleStation) && !(entity is Satellite))
                        onBlockedMinePosition = true;

                    if (entity is HomeStation homeStation)
                    {
                        if (homeStation.FactionId == Player.FactionId)
                        {
                            if (!Player.LastAttackTime(5))
                            {
                                isInSecureZone = true;
                                //inEquipZone = true;

                            }
                        }
                    }

                    else if (entity is RepairStation)
                    {
                        if (Player.CurrentHitPoints == Player.MaxHitPoints || Player.FactionId != entity.FactionId)
                            inRange = false;
                    }
                    else if (entity is Portal)
                    {
                        onBlockedMinePosition = true;
                        if (FactionId != 0 && FactionId == Player.FactionId)
                        {
                            if (Player.AttackingOrUnderAttack(5))
                            {
                                isInSecureZone = false;

                            }
                            else
                            {
                                isInSecureZone = true;

                            }
                        }
                    }
                }


                bool activateButton = Player.UpdateActivatable(entity, inRange);

                if (activateButton)
                {
                    if (entity is Portal portal && !portal.Working)
                        status = MapAssetActionAvailableCommand.OFF;

                    //TODO check old = if (entity is BattleStation battleStation && battleStation.Clan.Id != 0 && !battleStation.InBuildingState && battleStation.Clan.Id != Player.Clan.Id)

                    if (entity is BattleStation battleStation && battleStation.Clan.Id != 0 && battleStation.Clan.Id != Player.Clan.Id)
                        status = MapAssetActionAvailableCommand.OFF;



                    var portalTooltip = new List<ClientUITooltipModule>();
                    portalTooltip.Add(new ClientUITooltipModule(new ClientUITooltipTextFormatModule(ClientUITooltipTextFormatModule.LOCALIZED), ClientUITooltipModule.STANDARD, "q2_condition_JUMP", new List<ClientUITextReplacementModule>()));

                    var assetAction =
                            MapAssetActionAvailableCommand.write(entity.Id,
                                                               status,
                                                               inRange,
                                                               new ClientUITooltipsCommand(entity is Portal ? portalTooltip : new List<ClientUITooltipModule>()),
                                                               new class_h45()
                            );

                    Player.SendCommand(assetAction);
                }
            }

            if (Player.Storage.OnBlockedMinePosition != onBlockedMinePosition)
                Player.Storage.OnBlockedMinePosition = onBlockedMinePosition;

            if (Player.Storage.IsInEquipZone != inEquipZone)
            {
                Player.Storage.IsInEquipZone = inEquipZone;

                Player.SendCommand(EquipReadyCommand.write(Player.Storage.IsInEquipZone));

                var packet = Player.Storage.IsInEquipZone ? "0|A|STM|msg_equip_ready" : "0|A|STM|msg_equip_not_ready";
                Player.SendPacket(packet);
            }

            if (Player.Storage.IsInDemilitarizedZone != isInSecureZone)
            {
                Player.Storage.IsInDemilitarizedZone = isInSecureZone;
                return true;
            }

            return false;

        }

        public bool CheckRadiation(Player Player)
        {
            int positionX = Player.Position.X;
            int positionY = Player.Position.Y;

            bool inRadiationZone = false;

            if (Id == 42 || Id == 224/* || Id == 29*/ || Id == 91 || Id == 92 || Id == 93)
                inRadiationZone = positionX < 0 || positionX > 41800 || positionY < 0 || positionY > 26000;
            else if (Id == 121 || Id.ToString().StartsWith("88") || Id == 81)
                inRadiationZone = positionX < 0 || positionX > 10450 || positionY < 0 || positionY > 6500;
            else
                inRadiationZone = positionX < 0 || positionX > 20900 || positionY < 0 || positionY > 13000;

            foreach (var poi in POIs.Values)
            {
                if (poi.Type == POITypes.RADIATION)
                {
                    if (Player.Position.DistanceTo(poi.ShapeCords[0]) > poi.ShapeCords[1].X)
                        inRadiationZone = true;
                }
            }

            if (Player.Storage.IsInRadiationZone != inRadiationZone)
            {
                Player.Storage.IsInRadiationZone = inRadiationZone;
                return true;
            }
            return false;
        }

        public void SendObjects(Player player)
        {
            foreach (var activatable in Activatables.Values)
            {
                if(activatable is Portal p)
                {
                    if(p.GraphicsId == 2 || p.GraphicsId == 3 || p.GraphicsId == 4)
                    {
                        //send gate portals only to own faction start map
                        if(p.FactionId == player.FactionId)
                        {
                            short relationType = player.Clan.Id != 0 && activatable.Clan.Id != 0 ? activatable.Clan.GetRelation(player.Clan) : (short)0;
                            player.SendCommand(activatable.GetAssetCreateCommand(relationType, player));
                        }
                    } else
                    {
                        short relationType = player.Clan.Id != 0 && activatable.Clan.Id != 0 ? activatable.Clan.GetRelation(player.Clan) : (short)0;
                        player.SendCommand(activatable.GetAssetCreateCommand(relationType, player));
                    }
                } else
                {
                    short relationType = player.Clan.Id != 0 && activatable.Clan.Id != 0 ? activatable.Clan.GetRelation(player.Clan) : (short)0;
                    player.SendCommand(activatable.GetAssetCreateCommand(relationType, player));
                }
            }

            foreach (var poi in POIs.Values)
                player.SendCommand(poi.GetPOICreateCommand());

            foreach (var obj in Objects.Values)
                if (obj is Asset asset)
                    player.SendCommand(asset.GetAssetCreateCommand());
        }

        public void AddAndInitPlayer(Player player, bool sendSettings = false)
        {
            var petActivated = player.Pet != null && player.Pet.Activated;
            var flagshipActive = player.Flagschip != null && player.Flagschip.Activated;

            if (petActivated)
                player.Pet.Deactivate(true);
            if (flagshipActive)
            {
                player.Flagschip.Deactivate();
            }

            LoginRequestHandler.SendPlayer(player);
            AddCharacter(player);

            if (petActivated)
                player.Pet.Activate();

            if (sendSettings)
                LoginRequestHandler.SendSettings(player);
            if (player.RankId == 22)
            {
                //player.SendPacket($"0|n|fx|start|RAGE|{player.Id}");
            }
            if (flagshipActive)
            {
                player.Flagschip.Activate();
            }

            player.UpdateStatus();
        }

        public Activatable GetActivatableMapEntity(int pAssetId)
        {
            return !Activatables.ContainsKey(pAssetId) ? null : Activatables[pAssetId];
        }

        public class CharacterArgs : EventArgs
        {
            public Character Character { get; }
            public CharacterArgs(Character character)
            {
                Character = character;
            }
        }

        public event EventHandler<CharacterArgs> CharacterRemoved;
        public event EventHandler<CharacterArgs> CharacterAdded;

        public bool AddCharacter(Character character)
        {
            var success = Characters.TryAdd(character.Id, character);
            if (success)
            {
                CharacterAdded?.Invoke(this, new CharacterArgs(character));
            }
            return success;
        }

        public bool RemoveCharacter(Character character)
        {
            if (character == null) return true;
            var success = Characters.TryRemove(character.Id, out character);
            if (success)
            {
                CharacterRemoved?.Invoke(this, new CharacterArgs(character));

                foreach (var otherCharacter in Characters.Values.Where(x => x.InRangeCharacters.ContainsKey(character.Id)))
                    otherCharacter.RemoveInRangeCharacter(character);
            }
            else
            {
                lock(Characters)
                {
                    Characters.TryRemove(character.Id, out character);
                }
            }
            
            return success;
        }

        public void SendToAllOnMap(byte[] command)
        {
            Dictionary<int, Character> tmp = this.Characters.ToDictionary(kvp => kvp.Key,
                                                              kvp => kvp.Value);

            foreach (KeyValuePair<int, Character> entry in tmp)
            {
                if (entry.Value is Player)
                {
                    Player tmp1 = (Player)entry.Value;
                    tmp1.SendCommand(command);
                }
            }
        }
    }
}

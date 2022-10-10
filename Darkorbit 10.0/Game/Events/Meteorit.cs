using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Meteorit
    {
        private static MeteoritContainer meteorit;
        public static int icyCount = 25;
        public static int MeteoritDelay = 60;
        public static ConcurrentDictionary<int, Player> playersDamageCubikon = new ConcurrentDictionary<int, Player>();
        public static ConcurrentDictionary<int, int> cubikonDamagingPlayer = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, dynamic> cubikonPlayerDamage = new ConcurrentDictionary<int, dynamic>();
        public static ConcurrentDictionary<int, Tuple<int, int>> pDamage = new ConcurrentDictionary<int, Tuple<int, int>>();
        private static Position c1 = new Position(12800, 6000);
        private static Position c2 = new Position(13700, 3800);
        private static Position c3 = new Position(6800, 8200);
        private static Position c4 = new Position(14500, 8300);
        class MeteoritContainer
        {
            List<MeteoritMap> meteoritMaps = new List<MeteoritMap>();
            public MeteoritContainer()
            {
                //1-6, 2-6, 3-6
                meteoritMaps.Add(new MeteoritMap());
                meteoritMaps.Add(new MeteoritMap());
                //cubiMaps.Add(new CubiMap());
            }
            public MeteoritMap GetMeteoritMap(int i)
            {
                return meteoritMaps[i];
            }
        }

        class MeteoritMap
        {
            List<MeteoritObject> meteoritObjects = new List<MeteoritObject>();
            public MeteoritMap()
            {
                //Cubi 1 - 4
                meteoritObjects.Add(new MeteoritObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
            }
            public MeteoritObject GetMeteoritObject(int i)
            {
                return meteoritObjects[i];
            }
        }

        class MeteoritObject
        {
            List<Npc> npcs1 = new List<Npc>();
            public void AddIcy(Npc npc)
            {
                npcs1.Add(npc);
            }
            public List<Npc> GetNpcs1()
            {
                return npcs1;
            }
            public void SetNpcs1(List<Npc> t)
            {
                npcs1 = t;
            }
        }
        public static void Init()
        {
            meteorit = new MeteoritContainer();
        }

        public static void SpawnIcyLoop(int mapConvertIndex, Npc tmpNpc)
        {
            try
            {
                while (!tmpNpc.Destroyed)
                {
                    //generate protegits
                    meteorit.GetMeteoritMap(mapConvertIndex).GetMeteoritObject(tmpNpc.tmpId).SetNpcs1(Meteorit.createIcy(meteorit.GetMeteoritMap(mapConvertIndex).GetMeteoritObject(tmpNpc.tmpId).GetNpcs1(), tmpNpc));

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static Spacemap.MeteoritThread SpawnMeteorit(int shipId, Spacemap map, int MeteoritCount)
        {
            Position tmp = null;

            switch (MeteoritCount)
            {
                case 0:
                    tmp = c1;
                    break;
                    // case 1:
                    //   tmp = c2;
                    //   break;
                    //    case 2:
                    //   tmp = c3;
                    //   break;
                    //  case 3:
                    //   tmp = c4;
                    //  break;
            }

            Npc tmpNpc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(shipId), map, tmp);
            tmpNpc.InitialPosition = tmp;
            tmpNpc.NpcAI.AIOption = NpcAIOption.CUBIKON_POSITION_MOVE;
            tmpNpc.tmpId = MeteoritCount;

            int mapConvertIndex = 0;

            Meteorit.Init();
            if (map.Id == 2) mapConvertIndex = 0;
            else if (map.Id == 10) mapConvertIndex = 1;
            //  else if (map.Id == 26) mapConvertIndex = 2;

            Task meteoritThread = Task.Factory.StartNew(() => Meteorit.SpawnIcyLoop(mapConvertIndex, tmpNpc));

            Spacemap.MeteoritThread ct = new Spacemap.MeteoritThread(tmpNpc.Id, meteoritThread);
            return ct;
        }

        public static void RespawnMeteorit(Npc meteorit)
        {
            try
            {
                Thread.Sleep(MeteoritDelay * 1000);

                foreach (Spacemap s in GameManager.Spacemaps.Values)
                {
                    if (s.Id == meteorit.Spacemap.Id)
                    {
                        Spacemap.MeteoritThread tmp = Meteorit.SpawnMeteorit(meteorit.Ship.Id, s, meteorit.tmpId);
                        s.AddMeteoritThreadList(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static List<Npc> createIcy(List<Npc> listIcy, Npc meteorit)
        {
            if (meteorit.MainAttacker != null && !meteorit.Destroyed)
            {
                if (meteorit.childs.Count < icyCount)
                {
                    for (int i = meteorit.childs.Count; i < icyCount; i++)
                    {
                        Npc tmp = createNPC(103, 1, meteorit.Spacemap.Id, Position.GetPosOnCircle(new Position(meteorit.Position.X, meteorit.Position.Y), 0));
                        meteorit.childs.Add(tmp);
                        listIcy.Add(tmp);
                        tmp.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                        tmp.mother = meteorit;
                    }
                }
            }
            else
            {
                for (int j = 0; j < listIcy.Count; j++)
                {
                    listIcy[j].NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                }
            }
            return listIcy;
        }
    public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;

            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                if (npcID == 103)
                    npc.CR = true;
            }
            return npc;
        }
    }
}



using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class BossCubikon
    {
        private static BossCubiContainer bosscubs;
        public static int progeticsCount = 30;
        public static int bosscubiconDelay = 120;
        public static ConcurrentDictionary<int, Player> playersDamageCubikon = new ConcurrentDictionary<int, Player>();
        public static ConcurrentDictionary<int, int> cubikonDamagingPlayer = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, dynamic> cubikonPlayerDamage = new ConcurrentDictionary<int, dynamic>();
        public static ConcurrentDictionary<int, Tuple<int, int>> pDamage = new ConcurrentDictionary<int, Tuple<int, int>>();
        private static Position c1 = new Position(7100, 4100);
        private static Position c2 = new Position(13700, 3800);
        private static Position c3 = new Position(6800, 8200);
        private static Position c4 = new Position(14500, 8300);
        class BossCubiContainer
        {
            List<BossCubiMap> bosscubiMaps = new List<BossCubiMap>();
            public BossCubiContainer()
            {
                //1-6, 2-6, 3-6
                bosscubiMaps.Add(new BossCubiMap());
                //bosscubiMaps.Add(new BossCubiMap());
               // cubiMaps.Add(new CubiMap());
            }
            public BossCubiMap GetBossCubiMap(int i)
            {
                return bosscubiMaps[i];
            }
        }

        class BossCubiMap
        {
            List<BossCubiObject> bosscubiObjects = new List<BossCubiObject>();
            public BossCubiMap()
            {
                //Cubi 1 - 4
                bosscubiObjects.Add(new BossCubiObject());
                bosscubiObjects.Add(new BossCubiObject());
                bosscubiObjects.Add(new BossCubiObject());
                bosscubiObjects.Add(new BossCubiObject());
            }
            public BossCubiObject GetBossCubiObject(int i)
            {
                return bosscubiObjects[i];
            }
        }

        class BossCubiObject
        {
            List<Npc> npcs1 = new List<Npc>();
            public void AddProti(Npc npc)
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
            bosscubs = new BossCubiContainer();
        }

        public static void SpawnProtiLoop(int mapConvertIndex, Npc tmpNpc)
        {
            try
            {
                while (!tmpNpc.Destroyed)
                {
                    //generate protegits
                    bosscubs.GetBossCubiMap(mapConvertIndex).GetBossCubiObject(tmpNpc.tmpId).SetNpcs1(BossCubikon.createProteg(bosscubs.GetBossCubiMap(mapConvertIndex).GetBossCubiObject(tmpNpc.tmpId).GetNpcs1(), tmpNpc));

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static Spacemap.BossCubiThread SpawnBossCubicon(int shipId, Spacemap map, int BossCubiCount)
        {
            Position tmp = null;

            switch (BossCubiCount)
            {
                case 0:
                    tmp = c1;
                    break;
                case 1:
                    tmp = c2;
                    break;
               case 2:
                    tmp = c3;
                    break;
                case 3:
                    tmp = c4;
                    break;
            }

            Npc tmpNpc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(shipId), map, tmp);
            tmpNpc.InitialPosition = tmp;
            tmpNpc.NpcAI.AIOption = NpcAIOption.CUBIKON_POSITION_MOVE;
            tmpNpc.tmpId = BossCubiCount;
            tmpNpc.respawnable = false;

            int mapConvertIndex = 0;

            BossCubikon.Init();
            if (map.Id == 58) mapConvertIndex = 0;
            else if (map.Id == 2) mapConvertIndex = 1;
            else if (map.Id == 26) mapConvertIndex = 2;

            Task bosscubiThread = Task.Factory.StartNew(() => BossCubikon.SpawnProtiLoop(mapConvertIndex, tmpNpc));

            Spacemap.BossCubiThread ct = new Spacemap.BossCubiThread(tmpNpc.Id, bosscubiThread);
            return ct;
        }

        public static void RespawnBossCubicon(Npc bosscubi)
        {
            try
            {
                Thread.Sleep(bosscubiconDelay * 1000);

                foreach (Spacemap s in GameManager.Spacemaps.Values)
                {
                    if (s.Id == bosscubi.Spacemap.Id)
                    {
                        Spacemap.BossCubiThread tmp = BossCubikon.SpawnBossCubicon(bosscubi.Ship.Id, s, bosscubi.tmpId);
                        s.AddBossCubikonThreadList(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static List<Npc> createProteg(List<Npc> listPro, Npc bosscubi)
        {
            if (bosscubi.MainAttacker != null && !bosscubi.Destroyed)
            {
                if (bosscubi.childs.Count < progeticsCount)
                {
                    for (int i = bosscubi.childs.Count; i < progeticsCount; i++)
                    {
                        Npc tmp = createNPC(81, 1, bosscubi.Spacemap.Id, Position.GetPosOnCircle(new Position(bosscubi.Position.X, bosscubi.Position.Y), 0));
                        bosscubi.childs.Add(tmp);
                        listPro.Add(tmp);
                        tmp.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                        tmp.mother = bosscubi;
                    }
                }
            }
            else
            {
                for (int j = 0; j < listPro.Count; j++)
                {
                    listPro[j].NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                }
            }
            return listPro;
        }

        public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;

            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                if (npcID == 81)
                    npc.CR = true;
            }
            return npc;
        }
    }
}


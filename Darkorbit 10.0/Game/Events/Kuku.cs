using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Kuku
    {
        private static KukuContainer kuku;
        public static int kucuCount = 25;
        public static int KukuDelay = 60;
        public static ConcurrentDictionary<int, Player> playersDamageCubikon = new ConcurrentDictionary<int, Player>();
        public static ConcurrentDictionary<int, int> cubikonDamagingPlayer = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, dynamic> cubikonPlayerDamage = new ConcurrentDictionary<int, dynamic>();
        public static ConcurrentDictionary<int, Tuple<int, int>> pDamage = new ConcurrentDictionary<int, Tuple<int, int>>();
        private static Position c1 = new Position(12800, 6000);
        private static Position c2 = new Position(13700, 3800);
        private static Position c3 = new Position(6800, 8200);
        private static Position c4 = new Position(14500, 8300);
        class KukuContainer
        {
            List<KukuMap> kukuMaps = new List<KukuMap>();
            public KukuContainer()
            {
                //1-6, 2-6, 3-6
                kukuMaps.Add(new KukuMap());
                kukuMaps.Add(new KukuMap());
                //cubiMaps.Add(new CubiMap());
            }
            public KukuMap GetKukuMap(int i)
            {
                return kukuMaps[i];
            }
        }

        class KukuMap
        {
            List<KukuObject> kukuObjects = new List<KukuObject>();
            public KukuMap()
            {
                //Cubi 1 - 4
                kukuObjects.Add(new KukuObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
            }
            public KukuObject GetKukuObject(int i)
            {
                return kukuObjects[i];
            }
        }

        class KukuObject
        {
            List<Npc> npcs1 = new List<Npc>();
            public void AddKucu(Npc npc)
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
            kuku = new KukuContainer();
        }

        public static void SpawnKucuLoop(int mapConvertIndex, Npc tmpNpc)
        {
            try
            {
                while (!tmpNpc.Destroyed)
                {
                    //generate protegits
                    kuku.GetKukuMap(mapConvertIndex).GetKukuObject(tmpNpc.tmpId).SetNpcs1(Kuku.createKucu(kuku.GetKukuMap(mapConvertIndex).GetKukuObject(tmpNpc.tmpId).GetNpcs1(), tmpNpc));

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static Spacemap.KukuThread SpawnKuku(int shipId, Spacemap map, int KukuCount)
        {
            Position tmp = null;

            switch (KukuCount)
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
            tmpNpc.tmpId = KukuCount;

            int mapConvertIndex = 0;

            Kuku.Init();
            if (map.Id == 16) mapConvertIndex = 0;
            //else if (map.Id == 11) mapConvertIndex = 1;
            //  else if (map.Id == 26) mapConvertIndex = 2;

            Task KukuThread = Task.Factory.StartNew(() => Kuku.SpawnKucuLoop(mapConvertIndex, tmpNpc));

            Spacemap.KukuThread ct = new Spacemap.KukuThread(tmpNpc.Id, KukuThread);
            return ct;
        }

        public static void RespawnKuku(Npc kuku)
        {
            try
            {
                Thread.Sleep(KukuDelay * 1000);

                foreach (Spacemap s in GameManager.Spacemaps.Values)
                {
                    if (s.Id == kuku.Spacemap.Id)
                    {
                        Spacemap.KukuThread tmp = Kuku.SpawnKuku(kuku.Ship.Id, s, kuku.tmpId);
                        s.AddKukuThreadList(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static List<Npc> createKucu(List<Npc> listKucu, Npc kuku)
        {
            if (kuku.MainAttacker != null && !kuku.Destroyed)
            {
                if (kuku.childs.Count < kucuCount)
                {
                    for (int i = kuku.childs.Count; i < kucuCount; i++)
                    {
                        Npc tmp = createNPC(82, 1, kuku.Spacemap.Id, Position.GetPosOnCircle(new Position(kuku.Position.X, kuku.Position.Y), 0));
                        kuku.childs.Add(tmp);
                        listKucu.Add(tmp);
                        tmp.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                        tmp.mother = kuku;
                    }
                }
            }
            else
            {
                for (int j = 0; j < listKucu.Count; j++)
                {
                    listKucu[j].NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                }
            }
            return listKucu;
        }
    public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;

            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                if (npcID == 82)
                    npc.CR = true;
            }
            return npc;
        }
    }
}


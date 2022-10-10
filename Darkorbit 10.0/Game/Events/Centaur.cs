
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Centaur
    {
        private static CentaurContainer centaur;
        public static int vagaCount = 25;
        public static int CentaurDelay = 60;
        public static ConcurrentDictionary<int, Player> playersDamageCubikon = new ConcurrentDictionary<int, Player>();
        public static ConcurrentDictionary<int, int> cubikonDamagingPlayer = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, dynamic> cubikonPlayerDamage = new ConcurrentDictionary<int, dynamic>();
        public static ConcurrentDictionary<int, Tuple<int, int>> pDamage = new ConcurrentDictionary<int, Tuple<int, int>>();
        private static Position c1 = new Position(12800, 6000);
        private static Position c2 = new Position(13700, 3800);
        private static Position c3 = new Position(6800, 8200);
        private static Position c4 = new Position(14500, 8300);
        class CentaurContainer
        {
            List<CentaurMap> centaurMaps = new List<CentaurMap>();
            public CentaurContainer()
            {
                //1-6, 2-6, 3-6
                centaurMaps.Add(new CentaurMap());
                centaurMaps.Add(new CentaurMap());
                //cubiMaps.Add(new CubiMap());
            }
            public CentaurMap GetCentaurMap(int i)
            {
                return centaurMaps[i];
            }
        }

        class CentaurMap
        {
            List<CentaurObject> centaurObjects = new List<CentaurObject>();
            public CentaurMap()
            {
                //Cubi 1 - 4
                centaurObjects.Add(new CentaurObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
            }
            public CentaurObject GetCentaurObject(int i)
            {
                return centaurObjects[i];
            }
        }

        class CentaurObject
        {
            List<Npc> npcs1 = new List<Npc>();
            public void AddVaga(Npc npc)
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
            centaur = new CentaurContainer();
        }

        public static void SpawnVagaLoop(int mapConvertIndex, Npc tmpNpc)
        {
            try
            {
                while (!tmpNpc.Destroyed)
                {
                    //generate protegits
                    centaur.GetCentaurMap(mapConvertIndex).GetCentaurObject(tmpNpc.tmpId).SetNpcs1(Centaur.createVaga(centaur.GetCentaurMap(mapConvertIndex).GetCentaurObject(tmpNpc.tmpId).GetNpcs1(), tmpNpc));
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static Spacemap.CentaurThread SpawnCentaur(int shipId, Spacemap map, int CentaurCount)
        {
            Position tmp = null;

            switch (CentaurCount)
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
            tmpNpc.tmpId = CentaurCount;

            int mapConvertIndex = 0;

            Centaur.Init();
            if (map.Id == 3) mapConvertIndex = 0;
            else if (map.Id == 11) mapConvertIndex = 1;
            //  else if (map.Id == 26) mapConvertIndex = 2;

            Task centaurThread = Task.Factory.StartNew(() => Centaur.SpawnVagaLoop(mapConvertIndex, tmpNpc));

            Spacemap.CentaurThread ct = new Spacemap.CentaurThread(tmpNpc.Id, centaurThread);
            return ct;
        }

        public static void RespawnCentaur(Npc centaur)
        {
            try
            {
                Thread.Sleep(CentaurDelay * 1000);

                foreach (Spacemap s in GameManager.Spacemaps.Values)
                {
                    if (s.Id == centaur.Spacemap.Id)
                    {
                        Spacemap.CentaurThread tmp = Centaur.SpawnCentaur(centaur.Ship.Id, s, centaur.tmpId);
                        s.AddCentaurThreadList(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static List<Npc> createVaga(List<Npc> listVaga, Npc centaur)
        {
            if (centaur.MainAttacker != null && !centaur.Destroyed)
            {
                if (centaur.childs.Count < vagaCount)
                {
                    for (int i = centaur.childs.Count; i < vagaCount; i++)
                    {
                        Npc tmp = createNPC(94, 1, centaur.Spacemap.Id, Position.GetPosOnCircle(new Position(centaur.Position.X, centaur.Position.Y), 0));
                        centaur.childs.Add(tmp);
                        listVaga.Add(tmp);
                        tmp.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                        tmp.mother = centaur;
                    }
                }
            }
            else
            {
                for (int j = 0; j < listVaga.Count; j++)
                {
                    listVaga[j].NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                }
            }
            return listVaga;
        }
    public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;

            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                if (npcID == 94)
                    npc.CR = true;
            }
            return npc;
        }
    }
}


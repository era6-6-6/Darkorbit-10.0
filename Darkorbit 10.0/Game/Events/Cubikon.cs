
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Cubikon
    {
        private static CubiContainer cubs;
        public static int progeticsCount = 15;
        public static int cubiconDelay = 120;
        public static ConcurrentDictionary<int, Player> playersDamageCubikon = new ConcurrentDictionary<int, Player>();
        public static ConcurrentDictionary<int, int> cubikonDamagingPlayer = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, dynamic> cubikonPlayerDamage = new ConcurrentDictionary<int, dynamic>();
        public static ConcurrentDictionary<int, Tuple<int, int>> pDamage = new ConcurrentDictionary<int, Tuple<int, int>>();
        private static readonly Position c1 = new Position(7100, 4100);
        private static readonly Position c2 = new Position(13700, 3800);
        private static readonly Position c3 = new Position(6800, 8200);
        private static readonly Position c4 = new Position(14500, 8300);

        class CubiContainer
        {
            List<CubiMap> cubiMaps = new List<CubiMap>();
            public CubiContainer()
            {
                //1-6, 2-6, 3-6
                lock (cubiMaps)
                {
                    cubiMaps.Add(new CubiMap());
                    cubiMaps.Add(new CubiMap());
                }
               // cubiMaps.Add(new CubiMap());
            }
            public CubiMap GetCubiMap(int i)
            {
                return cubiMaps[i];
            }
        }

        class CubiMap
        {
            List<CubiObject> cubiObjects = new List<CubiObject>();
            public CubiMap()
            {
                //Cubi 1 - 4
                cubiObjects.Add(new CubiObject());
                cubiObjects.Add(new CubiObject());
                cubiObjects.Add(new CubiObject());
                cubiObjects.Add(new CubiObject());
            }
            public CubiObject GetCubiObject(int i)
            {
                return cubiObjects[i];
            }
        }

        class CubiObject
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
            cubs = new CubiContainer();
        }

        public static async Task SpawnProtiLoop(int mapConvertIndex, Npc tmpNpc)
        {
            try
            {
                while (!tmpNpc.Destroyed)
                {
                    //generate protegits
                    cubs.GetCubiMap(mapConvertIndex).GetCubiObject(tmpNpc.tmpId).SetNpcs1(createProteg(cubs.GetCubiMap(mapConvertIndex).GetCubiObject(tmpNpc.tmpId).GetNpcs1(), tmpNpc));
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
                await Task.CompletedTask;
            }
            await Task.CompletedTask;
        }

        public static Spacemap.CubiThread SpawnCubicon(int shipId, Spacemap map, int CubiCount)
        {
            Position tmp = null;

            switch (CubiCount)
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
            tmpNpc.tmpId = CubiCount;

            int mapConvertIndex = 0;

            Init();
            if (map.Id == 10) mapConvertIndex = 0;
            else if (map.Id == 2) mapConvertIndex = 1;
           // else if (map.Id == 26) mapConvertIndex = 2;

            Task cubiThread = Task.Run(async () => await SpawnProtiLoop(mapConvertIndex, tmpNpc));

            Spacemap.CubiThread ct = new Spacemap.CubiThread(tmpNpc.Id, cubiThread);
            return ct;
        }

        public static async Task RespawnCubicon(Npc cubi)
        {
            try
            {
                await Task.Delay(cubiconDelay * 1000);

                foreach (Spacemap s in GameManager.Spacemaps.Values)
                {
                    if (s.Id == cubi.Spacemap.Id)
                    {
                        Spacemap.CubiThread tmp = Cubikon.SpawnCubicon(cubi.Ship.Id, s, cubi.tmpId);
                        s.AddCubikonThreadList(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static List<Npc> createProteg(List<Npc> listPro, Npc cubi)
        {
            if (cubi.MainAttacker != null && !cubi.Destroyed)
            {
                if (cubi.childs.Count < progeticsCount)
                {
                    for (int i = cubi.childs.Count; i < progeticsCount; i++)
                    {
                        Npc tmp = createNPC(81, 1, cubi.Spacemap.Id, Position.GetPosOnCircle(new Position(cubi.Position.X, cubi.Position.Y), 0));
                        cubi.childs.Add(tmp);
                        listPro.Add(tmp);
                        tmp.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                        tmp.mother = cubi;
                    }
                }
            }
            else
            {
                listPro.ForEach(x => x.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE); 
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


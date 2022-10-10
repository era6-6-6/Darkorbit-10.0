
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Battleray
    {
        private static BattlerayContainer battleray;
        public static int interCount = 25;
        public static int BattlerayDelay = 60;
        public static ConcurrentDictionary<int, Player> playersDamageCubikon = new ConcurrentDictionary<int, Player>();
        public static ConcurrentDictionary<int, int> cubikonDamagingPlayer = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, dynamic> cubikonPlayerDamage = new ConcurrentDictionary<int, dynamic>();
        public static ConcurrentDictionary<int, Tuple<int, int>> pDamage = new ConcurrentDictionary<int, Tuple<int, int>>();
        private static Position c1 = new Position(12800, 6000);
        private static Position c2 = new Position(13700, 3800);
        private static Position c3 = new Position(6800, 8200);
        private static Position c4 = new Position(14500, 8300);
        class BattlerayContainer
        {
            List<BattlerayMap> battlerayMaps = new List<BattlerayMap>();
            public BattlerayContainer()
            {
                //1-6, 2-6, 3-6
                battlerayMaps.Add(new BattlerayMap());
                //battlerayMaps.Add(new BattlerayMap());
                //cubiMaps.Add(new CubiMap());
            }
            public BattlerayMap GetBattlerayMap(int i)
            {
                return battlerayMaps[i];
            }
        }

        class BattlerayMap
        {
            List<BattlerayObject> battlerayObjects = new List<BattlerayObject>();
            public BattlerayMap()
            {
                //Cubi 1 - 4
                battlerayObjects.Add(new BattlerayObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
                //cubiObjects.Add(new CubiObject());
            }
            public BattlerayObject GetBattlerayObject(int i)
            {
                return battlerayObjects[i];
            }
        }

        class BattlerayObject
        {
            List<Npc> npcs1 = new List<Npc>();
            public void AddInter(Npc npc)
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
            battleray = new BattlerayContainer();
        }

        public static void SpawnInterLoop(int mapConvertIndex, Npc tmpNpc)
        {
            try
            {
                while (!tmpNpc.Destroyed)
                {
                    //generate protegits
                    battleray.GetBattlerayMap(mapConvertIndex).GetBattlerayObject(tmpNpc.tmpId).SetNpcs1(Battleray.createInter(battleray.GetBattlerayMap(mapConvertIndex).GetBattlerayObject(tmpNpc.tmpId).GetNpcs1(), tmpNpc));

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static Spacemap.BattlerayThread SpawnBattleray(int shipId, Spacemap map, int BattlerayCount)
        {
            Position tmp = null;

            switch (BattlerayCount)
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
            tmpNpc.tmpId = BattlerayCount;

            int mapConvertIndex = 0;

            Battleray.Init();
            if (map.Id == 55) mapConvertIndex = 0;
            //else if (map.Id == 11) mapConvertIndex = 1;
            //  else if (map.Id == 26) mapConvertIndex = 2;

            Task battlerayThread = Task.Factory.StartNew(() => Battleray.SpawnInterLoop(mapConvertIndex, tmpNpc));

            Spacemap.BattlerayThread ct = new Spacemap.BattlerayThread(tmpNpc.Id, battlerayThread);
            return ct;
        }

        public static void RespawnBattleray(Npc battleray )
        {
            try
            {
                Thread.Sleep(BattlerayDelay * 1000);

                foreach (Spacemap s in GameManager.Spacemaps.Values)
                {
                    if (s.Id == battleray.Spacemap.Id)
                    {
                        Spacemap.BattlerayThread tmp = Battleray.SpawnBattleray(battleray.Ship.Id, s, battleray.tmpId);
                        s.AddBattlerayThreadList(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Cubikon.cs] Main void exception: {ex}");
            }
        }

        public static List<Npc> createInter(List<Npc> listInter, Npc battleray)
        {
            if (battleray.MainAttacker != null && !battleray.Destroyed)
            {
                if (battleray.childs.Count < interCount)
                {
                    for (int i = battleray.childs.Count; i < interCount; i++)
                    {
                        Npc tmp = createNPC(111, 1, battleray.Spacemap.Id, Position.GetPosOnCircle(new Position(battleray.Position.X, battleray.Position.Y), 0));
                        battleray.childs.Add(tmp);
                        listInter.Add(tmp);
                        tmp.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE;
                        tmp.mother = battleray;
                    }
                }
            }
            else
            {
                
                listInter.ForEach(x => x.NpcAI.AIOption = NpcAIOption.PROTI_POSITION_MOVE);
                
            }
            return listInter;
        }
    public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;

            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                if (npcID == 111)
                    npc.CR = true;
            }
            return npc;
        }
    }
}


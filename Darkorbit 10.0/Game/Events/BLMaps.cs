using System.Collections.Concurrent;

namespace Darkorbit.Game.Events
{
    internal class BLMaps
    {
        public static bool Active = false;

        public static Position startPosition1BLMap1 = new Position(1800, 7500); // Invoke XVI BLMap 1.
        public static Position startPosition2BLMap1 = new Position(18300, 6800); // BeheMoth Mindfire BLMap 1.
        public static Position startPosition3BLMap1 = new Position(4600, 1200); // Invoke XVI BLMap 1.
        public static Position startPosition4BLMap1 = new Position(2200, 3900); // Invoke XVI BLMap 1.

        public static Position startPosition1BLMap2 = new Position(10700, 11400); // Invoke XVI BLMap 2.
        public static Position startPosition2BLMap2 = new Position(7800, 4100); // BeheMoth Mindfire BLMap 2.
        public static Position startPosition3BLMap2 = new Position(6600, 11600); // Invoke XVI BLMap 2.
        public static Position startPosition4BLMap2 = new Position(15800, 11400); // Invoke XVI BLMap 2.

        public static Position startPosition1BLMap3 = new Position(1200, 5500); // Invoke XVI BLMap 3.
        public static Position startPosition2BLMap3 = new Position(19100, 2800); // BeheMoth Mindfire BLMap 3.
        public static Position startPosition3BLMap3 = new Position(4300, 3000); // Invoke XVI BLMap 3.
        public static Position startPosition4BLMap3 = new Position(3500, 9200); // Invoke XVI BLMap 3.

        public static int NpcSafeCount = 8;
        public static int cooldownBLMap = 120;
        private static Task processLoop;
        private static Npc BLMapMMO1;
        private static Npc BLMapMMO2;
        private static Npc BLMapMMO3;
        private static Npc BLMapMMO4;
        private static Npc BLMapEIC1;
        private static Npc BLMapEIC2;
        private static Npc BLMapEIC3;
        private static Npc BLMapEIC4;
        private static Npc BLMapVRU1;
        private static Npc BLMapVRU2;
        private static Npc BLMapVRU3;
        private static Npc BLMapVRU4;
        private static bool coolBLMapMMO1 = false;
        private static bool coolBLMapMMO2 = false;
        private static bool coolBLMapMMO3 = false;
        private static bool coolBLMapMMO4 = false;
        private static bool coolBLMapEIC1 = false;
        private static bool coolBLMapEIC2 = false;
        private static bool coolBLMapEIC3 = false;
        private static bool coolBLMapEIC4 = false;
        private static bool coolBLMapVRU1 = false;
        private static bool coolBLMapVRU2 = false;
        private static bool coolBLMapVRU3 = false;
        private static bool coolBLMapVRU4 = false;
        private static bool BLMapTiming1 = false;
        private static bool BLMapTiming2 = false;
        private static bool BLMapTiming3 = false;
        private static bool BLMapTiming4 = false;
        private static bool BLMapTiming5 = false;
        private static bool BLMapTiming6 = false;
        private static bool BLMapTiming7 = false;
        private static bool BLMapTiming8 = false;
        private static bool BLMapTiming9 = false;
        private static bool BLMapTiming10 = false;
        private static bool BLMapTiming11 = false;
        private static bool BLMapTiming12 = false;
        private static readonly List<Npc> SafeNpcBLMapMMO1 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapMMO2 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapMMO3 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapMMO4 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapEIC1 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapEIC2 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapEIC3 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapEIC4 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapVRU1 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapVRU2 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapVRU3 = new List<Npc>();
        private static readonly List<Npc> SafeNpcBLMapVRU4 = new List<Npc>();
        public static ConcurrentDictionary<int, Player> playersDamageBLMaps = new ConcurrentDictionary<int, Player>();


        public static void Start()
        {

            if (Active)
            {
                return;
            }

            Active = true;
            BLMapMMO1 = createNPC(215, 1, 306, startPosition1BLMap1);
            BLMapMMO2 = createNPC(216, 1, 306, startPosition2BLMap1);
            BLMapMMO3 = createNPC(215, 1, 306, startPosition3BLMap1);
            BLMapMMO4 = createNPC(215, 1, 306, startPosition4BLMap1);

            BLMapEIC1 = createNPC(215, 1, 307, startPosition1BLMap2);
            BLMapEIC2 = createNPC(216, 1, 307, startPosition2BLMap2);
            BLMapEIC3 = createNPC(215, 1, 307, startPosition3BLMap2);
            BLMapEIC4 = createNPC(215, 1, 307, startPosition4BLMap2);

            BLMapVRU1 = createNPC(215, 1, 308, startPosition1BLMap3);
            BLMapVRU2 = createNPC(216, 1, 308, startPosition2BLMap3);
            BLMapVRU3 = createNPC(215, 1, 308, startPosition3BLMap3);
            BLMapVRU4 = createNPC(215, 1, 308, startPosition4BLMap3);

            processLoop = Task.Factory.StartNew(() => loop());

        }

        public static bool Status()
        {
            return Active;
        }

        public static void loop()
        {
            try
            {
                while (true)
                {

                    createProteg(SafeNpcBLMapMMO1, BLMapMMO1);
                    movementProte(SafeNpcBLMapMMO1, BLMapMMO1);
                    checkAlive(SafeNpcBLMapMMO1, BLMapMMO1, "BLMapMMO1");

                    createProteg(SafeNpcBLMapMMO2, BLMapMMO2);
                    movementProte(SafeNpcBLMapMMO2, BLMapMMO2);
                    checkAlive(SafeNpcBLMapMMO2, BLMapMMO2, "BLMapMMO2");

                    createProteg(SafeNpcBLMapMMO3, BLMapMMO3);
                    movementProte(SafeNpcBLMapMMO3, BLMapMMO3);
                    checkAlive(SafeNpcBLMapMMO3, BLMapMMO3, "BLMapMMO3");

                    createProteg(SafeNpcBLMapMMO4, BLMapMMO4);
                    movementProte(SafeNpcBLMapMMO4, BLMapMMO4);
                    checkAlive(SafeNpcBLMapMMO4, BLMapMMO4, "BLMapMMO4");

                    createProteg(SafeNpcBLMapEIC1, BLMapEIC1);
                    movementProte(SafeNpcBLMapEIC1, BLMapEIC1);
                    checkAlive(SafeNpcBLMapEIC1, BLMapEIC1, "BLMapEIC1");

                    createProteg(SafeNpcBLMapEIC2, BLMapEIC2);
                    movementProte(SafeNpcBLMapEIC2, BLMapEIC2);
                    checkAlive(SafeNpcBLMapEIC2, BLMapEIC2, "BLMapEIC2");

                    createProteg(SafeNpcBLMapEIC3, BLMapEIC3);
                    movementProte(SafeNpcBLMapEIC3, BLMapEIC3);
                    checkAlive(SafeNpcBLMapEIC3, BLMapEIC3, "BLMapEIC3");

                    createProteg(SafeNpcBLMapEIC4, BLMapEIC4);
                    movementProte(SafeNpcBLMapEIC4, BLMapEIC4);
                    checkAlive(SafeNpcBLMapEIC4, BLMapEIC4, "BLMapEIC4");

                    createProteg(SafeNpcBLMapVRU1, BLMapVRU1);
                    movementProte(SafeNpcBLMapVRU1, BLMapVRU1);
                    checkAlive(SafeNpcBLMapVRU1, BLMapVRU1, "BLMapVRU1");

                    createProteg(SafeNpcBLMapVRU2, BLMapVRU2);
                    movementProte(SafeNpcBLMapVRU2, BLMapVRU2);
                    checkAlive(SafeNpcBLMapVRU2, BLMapVRU2, "BLMapVRU2");

                    createProteg(SafeNpcBLMapVRU3, BLMapVRU3);
                    movementProte(SafeNpcBLMapVRU3, BLMapVRU3);
                    checkAlive(SafeNpcBLMapVRU3, BLMapVRU3, "BLMapVRU3");

                    createProteg(SafeNpcBLMapVRU4, BLMapVRU4);
                    movementProte(SafeNpcBLMapVRU4, BLMapVRU4);
                    checkAlive(SafeNpcBLMapVRU4, BLMapVRU4, "BLMapVRU4");

                    Thread.Sleep(550);
                }
            } catch(Exception ex)
            {
                Logger.Log("error_log", $"- [BLMaps.cs] Main void exception: {ex}");
            }
        }

        public static void createProteg(List<Npc> listPro, Npc cubi)
        {
            if (cubi.MainAttacker != null && !cubi.Destroyed)
            {

                if (listPro.Count <= NpcSafeCount)
                {
                    for (int i = listPro.Count; i <= NpcSafeCount; i++)
                    {
                        listPro.Add(createNPC(213, 1, cubi.Spacemap.Id, Position.GetPosOnCircle(new Position(cubi.Position.X, cubi.Position.Y), 1000)));
                    }
                }
            }
            else
            {
                for (int j = 0; j < listPro.Count; j++)
                {
                    //listPro[j].Ship.Rewards.Credits = 1000000;
                    //listPro[j].Ship.Rewards.Uridium = 30000;
                    //listPro[j].Ship.Rewards.Experience = 1250000;
                    //listPro[j].Ship.Rewards.Honor = 1200;
                    //listPro[j].Destroy(listPro[j], DestructionType.PLAYER);
                    listPro[j].NpcAI.AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                }
                //listPro.Clear();
            }
        }

        public static void movementProte(List<Npc> listProt, Npc cubi)
        {
            if (listProt.Count > 0)
            {
                for (int j = 0; j < listProt.Count; j++)
                {
                    if (!listProt[j].Destroyed)
                    {
                        Movement.Move(listProt[j], Position.GetPosOnCircle(new Position(cubi.Position.X, cubi.Position.Y), 1200));
                    }
                }
            }
        }

        public static void checkAlive(List<Npc> listProt, Npc cubi, string nameCubi)
        {
            if (cubi.Destroyed)
            {
                if (nameCubi == "BLMapMMO1")
                {
                    if (!BLMapTiming1)
                    {
                        deleteNpcs(listProt);
                        coolCUBIM1();
                        BLMapTiming1 = true;
                    }

                    if (coolBLMapMMO1)
                    {
                        BLMapMMO1 = createNPC(215, 1, 306, startPosition1BLMap1);
                        coolBLMapMMO1 = false;
                        BLMapTiming1 = false;
                    }
                }

                if (nameCubi == "BLMapMMO2")
                {
                    if (!BLMapTiming2)
                    {
                        deleteNpcs(listProt);
                        coolCUBIM2();
                        BLMapTiming2 = true;
                    }
                    if (coolBLMapMMO2)
                    {
                        BLMapMMO2 = createNPC(216, 1, 306, startPosition2BLMap1);
                        coolBLMapMMO2 = false;
                        BLMapTiming2 = false;
                    }
                }
                if (nameCubi == "BLMapMMO3")
                {
                    if (!BLMapTiming7)
                    {
                        deleteNpcs(listProt);
                        coolCUBIM3();
                        BLMapTiming7 = true;
                    }
                    if (coolBLMapMMO3)
                    {
                        BLMapMMO3 = createNPC(215, 1, 306, startPosition3BLMap1);
                        coolBLMapMMO3 = false;
                        BLMapTiming7 = false;
                    }
                }
                if (nameCubi == "BLMapMMO4")
                {
                    if (!BLMapTiming10)
                    {
                        deleteNpcs(listProt);
                        coolCUBIM4();
                        BLMapTiming10 = true;
                    }
                    if (coolBLMapMMO4)
                    {
                        BLMapMMO4 = createNPC(215, 1, 306, startPosition4BLMap1);
                        coolBLMapMMO4 = false;
                        BLMapTiming10 = false;
                    }
                }
                if (nameCubi == "BLMapEIC1")
                {
                    if (!BLMapTiming3)
                    {
                        deleteNpcs(listProt);
                        coolCUBIE1();
                        BLMapTiming3 = true;
                    }
                    if (coolBLMapEIC1)
                    {
                        BLMapEIC1 = createNPC(215, 1, 307, startPosition1BLMap2);
                        coolBLMapEIC1 = false;
                        BLMapTiming3 = false;
                    }
                }

                if (nameCubi == "BLMapEIC2")
                {

                    if (!BLMapTiming4)
                    {
                        deleteNpcs(listProt);
                        coolCUBIE2();
                        BLMapTiming4 = true;
                    }
                    if (coolBLMapEIC2)
                    {
                        BLMapEIC2 = createNPC(216, 1, 307, startPosition2BLMap2);
                        coolBLMapEIC2 = false;
                        BLMapTiming4 = false;
                    }
                }

                if (nameCubi == "BLMapEIC3")
                {

                    if (!BLMapTiming8)
                    {
                        deleteNpcs(listProt);
                        coolCUBIE3();
                        BLMapTiming8 = true;
                    }
                    if (coolBLMapEIC3)
                    {
                        BLMapEIC3 = createNPC(215, 1, 307, startPosition3BLMap2);
                        coolBLMapEIC3 = false;
                        BLMapTiming8 = false;
                    }
                }

                if (nameCubi == "BLMapEIC4")
                {

                    if (!BLMapTiming11)
                    {
                        deleteNpcs(listProt);
                        coolCUBIE4();
                        BLMapTiming11 = true;
                    }
                    if (coolBLMapEIC4)
                    {
                        BLMapEIC4 = createNPC(215, 1, 307, startPosition4BLMap2);
                        coolBLMapEIC4 = false;
                        BLMapTiming11 = false;
                    }
                }

                if (nameCubi == "BLMapVRU1")
                {
                    if (!BLMapTiming5)
                    {
                        deleteNpcs(listProt);
                        coolCUBIV1();
                        BLMapTiming5 = true;
                    }
                    if (coolBLMapVRU1)
                    {
                        BLMapVRU1 = createNPC(215, 1, 308, startPosition1BLMap3);
                        coolBLMapVRU1 = false;
                        BLMapTiming5 = false;
                    }
                }

                if (nameCubi == "BLMapVRU2")
                {
                    if (!BLMapTiming6)
                    {
                        deleteNpcs(listProt);
                        coolCUBIV2();
                        BLMapTiming6 = true;
                    }
                    if (coolBLMapVRU2)
                    {
                        BLMapVRU2 = createNPC(216, 1, 308, startPosition2BLMap3);
                        coolBLMapVRU2 = false;
                        BLMapTiming6 = false;
                    }
                }
                if (nameCubi == "BLMapVRU3")
                {
                    if (!BLMapTiming9)
                    {
                        deleteNpcs(listProt);
                        coolCUBIV3();
                        BLMapTiming9 = true;
                    }
                    if (coolBLMapVRU3)
                    {
                        BLMapVRU3 = createNPC(215, 1, 308, startPosition3BLMap3);
                        coolBLMapVRU3 = false;
                        BLMapTiming9 = false;
                    }
                }

                if (nameCubi == "BLMapVRU4")
                {
                    if (!BLMapTiming12)
                    {
                        deleteNpcs(listProt);
                        coolCUBIV4();
                        BLMapTiming12 = true;
                    }
                    if (coolBLMapVRU4)
                    {
                        BLMapVRU4 = createNPC(215, 1, 308, startPosition4BLMap3);
                        coolBLMapVRU4 = false;
                        BLMapTiming12 = false;
                    }
                }
            }
        }

        public static Npc createNPC(int npcID, int amount, int SpacemapID, Position position)
        {
            Npc npc = null;

            for (int i = 1; i <= amount; i++)
            {
                npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), position);
                if (npcID == 213)
                    npc.CR = true;
            }
            return npc;
        }

        private static async void coolCUBIM1()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {

                    coolBLMapMMO1 = true;
                    //BLMapTiming1 = false;

                }

            }
        }

        private static async void coolCUBIM2()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapMMO2 = true;
                    //BLMapTiming2 = false;
                }

            }
        }

        private static async void coolCUBIM3()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapMMO3 = true;
                    //BLMapTiming7 = false;
                }

            }
        }

        private static async void coolCUBIM4()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapMMO4 = true;
                    //BLMapTiming10 = false;
                }

            }
        }

        private static async void coolCUBIE1()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapEIC1 = true;
                    //BLMapTiming3 = false;
                }

            }
        }

        private static async void coolCUBIE2()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapEIC2 = true;
                    //BLMapTiming4 = false;
                }

            }
        }

        private static async void coolCUBIE3()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapEIC3 = true;
                    //BLMapTiming8 = false;
                }

            }
        }

        private static async void coolCUBIE4()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapEIC4 = true;
                    //BLMapTiming11 = false;
                }

            }
        }

        private static async void coolCUBIV1()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapVRU1 = true;
                    //BLMapTiming5 = false;
                }

            }
        }

        private static async void coolCUBIV2()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {
                    coolBLMapVRU2 = true;
                    //BLMapTiming6 = false;
                }

            }
        }

        private static async void coolCUBIV3()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {

                    coolBLMapVRU3 = true;
                    //BLMapTiming9 = false;
                }

            }
        }

        private static async void coolCUBIV4()
        {
            for (int i = cooldownBLMap; i > 0; i--)
            {
                await Task.Delay(1000);
                if (i <= 1)
                {

                    coolBLMapVRU4 = true;
                    //BLMapTiming12 = false;
                }

            }
        }

        private static void deleteNpcs(List<Npc> listProt)
        {
            for (int j = 0; j < listProt.Count; j++)
            {
                listProt[j].Destroy(listProt[j], DestructionType.NPC);
                listProt[j].Attacking = false;
                listProt[j].Spacemap.RemoveCharacter(listProt[j]);
                Program.TickManager.RemoveTick(listProt[j]);
            }
            listProt.Clear();
        }
    }
}


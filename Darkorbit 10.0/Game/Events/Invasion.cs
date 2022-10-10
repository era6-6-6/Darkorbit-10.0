using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Darkorbit.Game.Events
{
    internal class Invasion
    {

        public static bool Active = false;
        public static bool ActiveMMO = false;
        public static bool ActiveEIC = false;
        public static bool ActiveVRU = false;
        public bool Started = false;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public static List<Portal> portal = new List<Portal>();
        public int seconds = 160;
        private static readonly Spacemap SpacemaPortalMMO = GameManager.GetSpacemap(61); //61
        private static readonly Spacemap SpacemaPortalEIC = GameManager.GetSpacemap(62); //62
        private static readonly Spacemap SpacemaPortalVRU = GameManager.GetSpacemap(63); //63
        private static readonly Position positionPortal = new Position(10300, 6300);

        private static Portal finishPortalMMO;
        private static Portal finishPortalEIC;
        private static Portal finishPortalVRU;

        public static List<Npc> NpcsMMO = new List<Npc>();
        public static List<Npc> NpcsEIC = new List<Npc>();
        public static List<Npc> NpcsVRU = new List<Npc>();
        public int waveMMO = 1;
        public int waveVRU = 1;
        public int waveEIC = 1;

        private Task thread;
        private Task notify;
        private Task notifyConsole;

        private static CancellationTokenSource tsThread = new CancellationTokenSource();
        private static CancellationToken ctThread;
        private static CancellationTokenSource tsNotify = new CancellationTokenSource();
        private static CancellationToken ctNotify;
        private static CancellationTokenSource tsNotifyConsole = new CancellationTokenSource();
        private static CancellationToken ctNotifyConsole;

        public int mmoKills = 0;

        public async void Start()
        {
            if (!Active)
            {
                mmoKills = 0;

                Active = true;
                ActiveMMO = true;
                ActiveEIC = true;
                ActiveVRU = true;

                waveMMO = 1;
                waveEIC = 1;
                waveVRU = 1;

                NpcsMMO = new List<Npc>();
                NpcsEIC = new List<Npc>();
                NpcsVRU = new List<Npc>();

                ctThread = tsThread.Token;
                ctNotify = tsNotify.Token;
                ctNotifyConsole = tsNotifyConsole.Token;

                if (finishPortalMMO != null) finishPortalMMO.Remove();
                if (finishPortalEIC != null) finishPortalEIC.Remove();
                if (finishPortalVRU != null) finishPortalVRU.Remove();

                int seconds = 10;
                int k = seconds;
                for (int i = seconds; i > 0; i--)
                {
                    if (i == k)
                    {
                        string packet = $"0|A|STD|[INVASION] The portal will appears in {i} seconds in center of map...";

                        GameManager.SendPacketToAll(packet);
                        k += -10;
                    }

                    await Task.Delay(1000);
                }

                foreach (Spacemap item in GameManager.Spacemaps.Values)
                {
                    if (item.Id >= 1 && item.Id <= 28 && item.Id != 16)
                    {
                        if (item.FactionId == 1)
                        {
                            portal.Add(new Portal(item, positionPortal, Position.Random(SpacemaPortalMMO, 2000, 20000, 1500, 10000), SpacemaPortalMMO.Id, 43, 1, true, true, false));
                        }

                        if (item.FactionId == 2)
                        {
                            portal.Add(new Portal(item, positionPortal, Position.Random(SpacemaPortalMMO, 2000, 20000, 1500, 10000), SpacemaPortalMMO.Id, 43, 2, true, true, false));
                        }

                        if (item.FactionId == 3)
                        {
                            portal.Add(new Portal(item, positionPortal, Position.Random(SpacemaPortalMMO, 2000, 20000, 1500, 10000), SpacemaPortalMMO.Id, 43, 3, true, true, false));
                        }
                    }

                    foreach (Portal portal in portal)
                    {
                        GameManager.SendCommandToMap(portal.Spacemap.Id, portal.GetAssetCreateCommand());
                    }
                }
                GameManager.SendPacketToAll($"0|n|KSMSG|INVASION STARTED");

                Started = true;

                foreach (GameSession gameSession in GameManager.GameSessions.Values)
                {
                    Player player = gameSession.Player;
                    player.SettingsManager.SendMenuBarsCommand();
                }

                GameManager.SendPacketToAll($"0|n|isi|{mmoKills}|{mmoKills}|{mmoKills}|{waveMMO}");

                if (thread != null)
                {
                    tsThread.Cancel();
                    tsThread = new CancellationTokenSource();
                    ctThread = tsThread.Token;
                }
                if (notify != null)
                {
                    tsNotify.Cancel();
                    tsNotify = new CancellationTokenSource();
                    ctNotify = tsNotify.Token;
                }
                if (notifyConsole != null)
                {
                    tsNotifyConsole.Cancel();
                    tsNotifyConsole = new CancellationTokenSource();
                    ctNotifyConsole = tsNotifyConsole.Token;
                }

                thread = Task.Factory.StartNew(() => loop());
                notify = Task.Factory.StartNew(() => notifyLoop());
                notifyConsole = Task.Factory.StartNew(() => notifyLoopConsole());
            }

        }

        public bool Status()
        {
            return Active;
        }

        public void notifyLoop()
        {
            try
            {
                while (Active)
                {
                    if (ctNotify.IsCancellationRequested) break;
                    GameManager.SendPacketToMap(SpacemaPortalMMO.Id, $"0|A|STD|[INVASION] Wave: {waveMMO} - Npcs left: {NpcsMMO.Count}");
                    Thread.Sleep(30000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Invasion.cs] Main void exception: {ex}");
            }
        }
        public void notifyLoopConsole()
        {
            try
            {
                while (Active)
                {
                    if (ctNotifyConsole.IsCancellationRequested) break;
                    Console.WriteLine($"[INVASION] Wave: {waveMMO} - Npcs left: {NpcsMMO.Count}");
                    Thread.Sleep(60000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Invasion.cs] Main void exception: {ex}");
            }
        }

        public void loop()
        {
            try
            {
                while (Active)
                {
                    if (ctThread.IsCancellationRequested) break;
                    if (ActiveMMO)
                    {
                        checkWave(NpcsMMO.Count, "MMO", npcWave(waveMMO, "MMO"));
                        checkNPClive("MMO");
                    }
                    if (ActiveEIC)
                    {
                        checkWave(NpcsEIC.Count, "EIC", npcWave(waveEIC, "EIC"));
                        checkNPClive("EIC");

                    }
                    if (ActiveVRU)
                    {
                        checkWave(NpcsVRU.Count, "VRU", npcWave(waveVRU, "VRU"));
                        checkNPClive("VRU");

                    }
                    GameManager.SendPacketToAll($"0|n|isc|1|{mmoKills}");
                    GameManager.SendPacketToAll($"0|n|isc|2|{mmoKills}");
                    GameManager.SendPacketToAll($"0|n|isc|3|{mmoKills}");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Invasion.cs] Main void exception: {ex}");
            }
        }
        public int npcWave(int wave, string company = "")
        {
            int npc = 0;
            if (wave == 1)
            {
                npc = 111;
            }

            if (wave == 2)
            {
                npc = 112;
            }

            if (wave == 3)
            {
                npc = 113;
            }

            if (wave == 4)
            {
                npc = 114;
            }

            if (wave == 5)
            {
                npc = 115;
            }

            if (wave == 6)
            {
                npc = 116;
            }

            if (wave == 7)
            {
                npc = 118;
            }

            if (wave == 8)
            {
                npc = 127;
            }

            if (wave == 9)
            {
                npc = 83;
            }

            if (npc == 0 && company == "MMO")
            {
                npc = 90;
                sendRewardMMO();

            }
            if (npc == 0 && company == "EIC")
            {
                npc = 90;
                sendRewardEIC();
            }
            if (npc == 0 && company == "VRU")
            {
                npc = 90;
                sendRewardVRU();
            }

            return npc;
        }
        public void checkWave(int countNPC, string company, int wave)
        {


            if (company == "MMO" && countNPC == 0)
            {

                createNPC(npcWave(waveMMO), 25, SpacemaPortalMMO.Id, 1);

            }
            if (company == "EIC" && countNPC == 0)
            {

                createNPC(npcWave(waveEIC), 25, SpacemaPortalEIC.Id, 2);

            }
            if (company == "VRU" && countNPC == 0)
            {

                createNPC(npcWave(waveVRU), 25, SpacemaPortalVRU.Id, 3);

            }
        }

        public void checkNPClive(string company)
        {

            if (company == "MMO")
            {
                for (int i = 0; i < NpcsMMO.Count; i++)
                {
                    if (NpcsMMO[i].Destroyed)
                    {
                        Program.TickManager.RemoveTick(NpcsMMO[i]);
                        NpcsMMO.Remove(NpcsMMO[i]);
                        mmoKills++;
                    }
                }
                if (NpcsMMO.Count == 0 && waveMMO > 0)
                {
                    waveMMO++;
                    GameManager.SendPacketToAll($"0|n|isw|{waveMMO}");
                }
            }
            if (company == "EIC")
            {
                for (int i = 0; i < NpcsEIC.Count; i++)
                {
                    if (NpcsEIC[i].Destroyed)
                    {
                        Program.TickManager.RemoveTick(NpcsEIC[i]);
                        NpcsEIC.Remove(NpcsEIC[i]);
                    }
                }
                if (NpcsEIC.Count == 0 && waveEIC > 0)
                {
                    waveEIC++;
                }
            }
            if (company == "VRU")
            {
                for (int i = 0; i < NpcsVRU.Count; i++)
                {
                    if (NpcsVRU[i].Destroyed)
                    {
                        Program.TickManager.RemoveTick(NpcsVRU[i]);
                        NpcsVRU.Remove(NpcsVRU[i]);
                    }
                }
                if (NpcsVRU.Count == 0 && waveMMO > 0)
                {
                    waveVRU++;
                }
            }
        }
        public static Npc createNPC(int npcID, int amount, int SpacemapID, int Companny)
        {
            Npc npc = null;
            for (int i = 1; i <= amount; i++)
            {
                if (npcID != 0)
                {
                    npc = new Npc(Randoms.CreateRandomID(), GameManager.GetShip(npcID), GameManager.GetSpacemap(SpacemapID), Position.GetPosOnCircle(new Position(10500, 6500), 5000));
                    //npc.Ship.Respawnable = false;
                    //npc.Ship.Aggressive = true;
                    npc.respawnable = false;
                    npc.aggressive = true;

                    if (Companny == 1)
                    {
                        NpcsMMO.Add(npc);
                    }
                    if (Companny == 2)
                    {
                        NpcsEIC.Add(npc);
                    }
                    if (Companny == 3)
                    {
                        NpcsVRU.Add(npc);
                    }
                }
            }
            return npc;
        }

        public void sendRewardMMO()
        {
            finishPortalMMO = new Portal(SpacemaPortalMMO, positionPortal, Position.Random(SpacemaPortalMMO, 2000, 20000, 1500, 10000), -1, 1, 1, true, true, false);

            foreach (Character item in SpacemaPortalMMO.Characters.Values)
            {
                if (item is Player)
                {
                    Player player = item as Player;
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                        mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Invasion'");
                    player.SendPacket("0|n|KSMSG|INVASION FINISHED");
                    GameManager.SendCommandToMap(finishPortalMMO.Spacemap.Id, finishPortalMMO.GetAssetCreateCommand());
                }
            }

            for (int i = 0; i < portal.Count; i++)
            {
                portal[i].Remove();
            }

            foreach (Character item in SpacemaPortalMMO.Characters.Values)
            {
                if (item is Player player)
                {
                    player.LoadData();
                    player.ChangeData(DataType.CREDITS, 10000000);
                    player.ChangeData(DataType.URIDIUM, 10000);
                    player.ChangeData(DataType.EXPERIENCE, 1500000);
                    player.ChangeData(DataType.HONOR, 10000);
                    player.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(5000, 5000));
                    player.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.SAB_50, Randoms.random.Next(5000, 5000));
                    player.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(2000, 2000));
                    player.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(1000, 1000));
                    player.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.ISH_01, Randoms.random.Next(5, 5));
                    player.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.SMB_01, Randoms.random.Next(5, 5));
                    player.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.EMP_01, Randoms.random.Next(5, 5));
                }

            }

            Started = false;

            ActiveMMO = false;
            Active = false;

            tsThread.Cancel();
            tsNotify.Cancel();
            tsNotifyConsole.Cancel();
        }
        public void sendRewardEIC()
        {
            finishPortalEIC = new Portal(SpacemaPortalEIC, positionPortal, Position.Random(SpacemaPortalEIC, 2000, 20000, 1500, 10000), -1, 1, 2, true, true, false);

            foreach (Character item in SpacemaPortalEIC.Characters.Values)
            {
                if (item is Player)
                {
                    Player player = item as Player;
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                        mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Invasion'");
                    player.SendPacket("0|n|KSMSG|INVASION FINISH");
                    GameManager.SendCommandToMap(finishPortalEIC.Spacemap.Id, finishPortalEIC.GetAssetCreateCommand());
                }
            }
            for (int i = 0; i < portal.Count; i++)
            {

                if (portal[i].FactionId == 2)
                {
                    portal[i].Remove();
                }
            }


            foreach (Character item in SpacemaPortalEIC.Characters.Values)
            {
                if (item is Player player)
                {
                    player.LoadData();
                    player.ChangeData(DataType.CREDITS, 1);
                }

            }

            ActiveEIC = false;
            Active = false;

            Started = false;

            tsThread.Cancel();
            tsNotify.Cancel();
            tsNotifyConsole.Cancel();
        }
        public void sendRewardVRU()
        {
            finishPortalVRU = new Portal(SpacemaPortalVRU, positionPortal, Position.Random(SpacemaPortalVRU, 2000, 20000, 1500, 10000), -1, 1, 3, true, true, false);

            foreach (Character item in SpacemaPortalVRU.Characters.Values)
            {
                if (item is Player)
                {
                    Player player = item as Player;
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                        mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Invasion'");
                    player.SendPacket("0|n|KSMSG|INVASION FINISH");
                    GameManager.SendCommandToMap(finishPortalVRU.Spacemap.Id, finishPortalVRU.GetAssetCreateCommand());
                }

            }
            for (int i = 0; i < portal.Count; i++)
            {

                if (portal[i].FactionId == 3)
                {
                    portal[i].Remove();
                }
            }


            foreach (Character item in SpacemaPortalVRU.Characters.Values)
            {
                if (item is Player player)
                {
                    player.LoadData();
                    player.ChangeData(DataType.CREDITS, 1);

                }

            }

            ActiveVRU = false;
            Active = false;

            Started = false;

            tsThread.Cancel();
            tsNotify.Cancel();
            tsNotifyConsole.Cancel();
        }

    }

}
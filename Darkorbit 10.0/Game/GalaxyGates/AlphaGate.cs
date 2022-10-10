
using Darkorbit.Game.Movements;
using Darkorbit.Game.Objects;
using Darkorbit.Managers;
using Darkorbit.Managers.MySQLManager;
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darkorbit.Game.GalaxyGates
{
    internal class AlphaGate
    {
        public int wave;
        public int lives;
        private int enemysCount = 0;
        private readonly int gateMapId;
        private int enemy;
        private readonly Player currentPlayer;
        private readonly GameSession currentGameSession;
        private readonly bool active = false;
        public Spacemap Spacemap { get; set; }

        private Portal alphaGateNextWavePortal;
        private Portal alphaGateBackPortal;

        public AlphaGate(int map, GameSession gameSession, int gateWave, int gateLives)
        {

            gateMapId = map;
            wave = gateWave;
            lives = gateLives;
            currentPlayer = gameSession.Player;
            currentGameSession = gameSession;

            //Console.WriteLine(NpcGG.Npcs.Count);

            foreach (var n in NpcGG.Npcs)
            {
                foreach (var e in n.Value)
                {
                    if (e.Session.Id == currentPlayer.Id)
                    {
                        e.Destroy(e, DestructionType.NPC);
                    }
                }
            }

            Start();

            // Remove player cloak
            currentPlayer.CpuManager.DisableCloak();
        }

        public async void Start()
        {
            // Wave countdown
            for (int i = 20; i > 0; i--)
            {
                if (currentPlayer.Destroyed) break;
                currentPlayer.SendPacket($"0|A|STD|-=- {i} -=-");
                await Task.Delay(1000);
            }

            if (!currentPlayer.Destroyed)
            {
                // Final message
                currentPlayer.SendPacket($"0|A|STD|{{{lives}}} lives left");
                //currentPlayer.SendPacket($"0|A|STD|To leave the gate typ /leave in the chat");
                // Create wave
                CreateNewWave();
                // Update database to wave
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET wave = '{wave}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '1'");
                }
            }
        }

        //creates the real waves and spawns the npcs
        public void CreateNewWaveSub(int npc, int count, int health, int wave, bool lastSubwave)
        {
            CreateNPC(npc, count, health, wave);
            currentPlayer.SendPacket($"0|A|STD|Map Alpha - Wave {wave}");
            if (!lastSubwave)
            {
                if (currentPlayer.positionInitializacion.mapID != 51)
                {
                    return;
                }
            }
        }

        //create the wave subs which are creating the real waves
        public async void CreateNewWaveSubIterate(int npc, int[] count, int health, int wave)
        {
            for(int i = 0; i < count.Length; i++)
            {
                if (currentPlayer.Destroyed) break;
                if (i == count.Length)
                {
                    CreateNewWaveSub(npc, count[i], health, wave + i, true);
                } else
                {
                    CreateNewWaveSub(npc, count[i], health, wave + i, false);
                    await Task.Delay(15000);
                }
            }
        }

        // Create new wave of Npcs
        public void CreateNewWave()
        {
            if (wave == 1) // 4*10=40 Streuners
            {
                enemysCount = 40;
                CreateNewWaveSubIterate(84, new int[] { 10,10,10,10 }, 100, 1);
            }
            else if (wave == 2) // 4*10=40 Lordakias
            {
                enemysCount = 40;
                CreateNewWaveSubIterate(71, new int[] { 10, 10, 10, 10 }, 2000, 5);

            }
            else if (wave == 3) // 4*10=40 Mordons
            {
                enemysCount = 40;
                CreateNewWaveSubIterate(73, new int[] { 10, 10, 10, 10 }, 20000, 9);
            }
            else if (wave == 4) // 4*20=80 Saimons
            {
                enemysCount = 80;
                CreateNewWaveSubIterate(75, new int[] { 20, 20, 20, 20 }, 6300, 13);
            }
            else if (wave == 5) // 4*5=20 Devolariums
            {
                enemysCount = 20;
                CreateNewWaveSubIterate(72, new int[] { 5, 5, 5, 5 }, 100000, 17);

            }
            else if (wave == 6) // 4*20=80 Kristallins
            {
                enemysCount = 80;
                CreateNewWaveSubIterate(78, new int[] { 20, 20, 20, 20 }, 50000, 21);
            }
            else if (wave == 7) // 4*5=20 Sibelons
            {
                enemysCount = 20;
                CreateNewWaveSubIterate(74, new int[] { 5,5,5,5 }, 200000, 25);
            }
            else if (wave == 8) // 4*20=80 Sibelonits
            {
                enemysCount = 80;
                CreateNewWaveSubIterate(76, new int[] { 20, 20, 20, 20 }, 40000, 29);
            }
            else if (wave == 9) // 4*4=16 Kristallons
            {
                enemysCount = 16;
                CreateNewWaveSubIterate(79, new int[] { 4, 4, 4, 4 }, 400000, 33);
            }
            else if (wave == 10) // 5+10+10+5=30 Protegits
            {
                enemysCount = 30;
                CreateNewWaveSubIterate(103, new int[] { 5, 10, 10, 5 }, 50000, 37);
            }

        }

        public void CreateNPC(int npc, int count, int health, int wave)
        {
            for (int i = 0; i < count; i++)
            {
                if (currentPlayer.Destroyed || GameManager.GetSpacemap(gateMapId) == null) break;
                int npcNumber = i + 1;
                new NpcGG(Randoms.CreateRandomID(), GameManager.GetShip(npc), GameManager.GetSpacemap(gateMapId), Position.GetPosOnCircle(new Position(10500, 6500), 5000), "α", wave, currentPlayer, 51, gateMapId); // Create NPC
            }
        }
   
        public async void WaveCheck(Player player)
        {
            // Killed Npc
            enemysCount--;

            if (enemysCount == 0)
            {
                if (wave != 10)
                {
                    wave++;
                    using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET wave = '{wave}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '1'");
                    }
                    CreateNextGates(player);
                }
                else // Gate Done
                {
                    // Reward
                    int uridium = 15000;
                    int experience = 4000000;
                    int honor = 80000;
                    //int ec = 5;
                    int logfiles = 50;
                    currentPlayer.LoadData();
                    currentPlayer.ChangeData(DataType.EXPERIENCE, experience);
                    currentPlayer.ChangeData(DataType.HONOR, honor);
                    currentPlayer.ChangeData(DataType.URIDIUM, uridium);
                    currentPlayer.SendPacket($"0|LM|ST|LOG|{logfiles}");
                    currentPlayer.AddLogfiles(logfiles);
                    //currentPlayer.ChangeData(DataType.EC, ec);
                    currentPlayer.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(20000, 20000));
                    currentPlayer.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(2000, 3000));

                    // update database
                    using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{3}', parts = '[]', wave = '{1}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '1'");
                    }

                    await Task.Delay(10000);

                    // Return to home map
                    currentGameSession.Player.Jump(currentGameSession.Player.GetBaseMapId(true), currentGameSession.Player.GetBasePosition(true));

                    Spacemap tmp = null;
                    GameManager.Spacemaps.TryRemove(gateMapId, out tmp);
                }
            }
        }

        // Remove Life
        public void RemoveLife()
        {
            if (lives <= 1)
            {
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{0}', wave = '{1}', parts = '[]' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '1'");
                }
            }
            else
            {
                int newLives = lives - 1;
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET lives = '{newLives}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '1'");
                }
            }
        }

        // Create new wave gates
        public void CreateNextGates(Player player)
        {
            alphaGateNextWavePortal = new Portal(GameManager.GetSpacemap(player.GetPlayerActiveMap()), new Position(9500, 6500), new Position(10500, 6500), 51, 2, 3, true, true, false); // Next wave
            alphaGateBackPortal = new Portal(GameManager.GetSpacemap(player.GetPlayerActiveMap()), new Position(11500, 6500), player.GetBasePosition(true), player.GetBaseMapId(true), 1, 3, true, true, false); // Back to homebase

            player.SendCommand(alphaGateNextWavePortal.GetAssetCreateCommand());
            player.SendCommand(alphaGateBackPortal.GetAssetCreateCommand());
        }

        public int GetGateMapId()
        {
            return gateMapId;
        }
    }
}

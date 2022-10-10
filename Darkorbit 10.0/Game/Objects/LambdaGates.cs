using Darkorbit.Game.Movements;
using Darkorbit.Game.Objects;
using Darkorbit.Managers;
using Darkorbit.Managers.MySQLManager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Darkorbit.Game.GalaxyGates
{
    internal class LambdaGates
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
        public Portal lambdaGateNextWavePortal { get; private set; }
        public Portal lambdaGateBackPortal { get; private set; }

        private Portal alphaGateNextWavePortal;
        private Portal alphaGateBackPortal;

        public LambdaGates(int map, GameSession gameSession, int gateWave, int gateLives)
        {

            gateMapId = map;
            wave = gateWave;
            lives = gateLives;
            currentPlayer = gameSession.Player;
            currentGameSession = gameSession;

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
                currentPlayer.SendPacket($"0|A|STD|-=- {i} -=-");
                await Task.Delay(1000);
            }

            // Final message
            currentPlayer.SendPacket($"0|A|STD|{{{lives}}} lives left");
            currentPlayer.SendPacket($"0|A|STD|To leave the gate typ /leave in the chat");
            // Create wave
            CreateNewWave();
            // Update database to wave
            using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
            {
                mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET wave = '{wave}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '8'");
            }
        }


        // Create new wave of Npcs
        public async void CreateNewWave()
        {

            if (wave == 1)
            {

                enemysCount = 30;

                CreateNPC(23, 10, 900000); // Wave 1
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {1}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(24, 10, 900000); // Wave 2
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {2}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(24, 10, 900000); // Wave 3
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {3}");

            }
            else if (wave == 2)
            {

                enemysCount = 20;

                CreateNPC(31, 6, 1500); // Wave 5
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {4}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(25, 14, 1500); // Wave 6
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {5}");

            }
            else if (wave == 3)
            {

                enemysCount = 25;

                CreateNPC(26, 10, 15000); // Wave 9
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {6}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(27, 10, 15000); // Wave 10
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {7}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(26, 5, 15000); // Wave 11
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {8}");

            }
            else if (wave == 4)
            {

                enemysCount = 15;

                CreateNPC(27, 10, 5300); // Wave 13
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {9}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(46, 2, 5300); // Wave 14
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {10}");

            }
            else if (wave == 5)
            {

                enemysCount = 13;

                CreateNPC(28, 1, 50000); // Wave 17
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {11}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(24, 10, 50000); // Wave 18
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {12}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(28, 2, 50000); // Wave 19
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {13}");

            }
            else if (wave == 6)
            {

                enemysCount = 12;

                CreateNPC(29, 6, 140000); // Wave 17
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {14}");
                await Task.Delay(15000); // 60000

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(29, 6, 140000); // Wave 18
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {15}");

            }
            else if (wave == 7)
            {

                enemysCount = 13;

                CreateNPC(35, 1, 450000); // Wave 25
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {16}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(29, 7, 200000); // Wave 26
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {17}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(27, 5, 200000); // Wave 27
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {18}");

            }
            else if (wave == 8)
            {

                enemysCount = 20;

                CreateNPC(31, 10, 120000); // Wave 29
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {19}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(23, 10, 120000); // Wave 30
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {20}");

            }
            else if (wave == 9)
            {

                enemysCount = 12;

                CreateNPC(29, 6, 800000); // Wave 33
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {21}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 75)
                {
                    return;
                }

                CreateNPC(29, 6, 800000); // Wave 34
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {22}");

            }
            else if (wave == 10)
            {

                enemysCount = 2;

                CreateNPC(35, 2, 300000); // Wave 37
                currentPlayer.SendPacket($"0|A|STD|Map Lambda - wave {23}");

            }
        }

        public void CreateNPC(int npc, int count, int health)
        {
            for (int i = 0; i < count; i++)
            {
                int npcNumber = i + 1;
                var npclambda = new NpcGG(Randoms.CreateRandomID(), GameManager.GetShip(npc), GameManager.GetSpacemap(gateMapId), Position.GetPosOnCircle(new Position(10500, 6500), 5000), "λ", npcNumber, currentPlayer, 75);
            }
        }

        public void WaveCheck(Player player)
        {
            // Killed Npc
            enemysCount = enemysCount - 1;

            if (enemysCount == 0)
            {
                if (wave != 10)
                {
                    wave = wave + 1;
                    Start();
                }
                else // Gate Done
                {
                    // Reward
                    int uridium = 20000;
                    int honor = 100000;
                    int experience = 2750000;
                    int ec = 1500;
                    currentPlayer.LoadData();
                    currentPlayer.ChangeData(DataType.HONOR, honor);
                    currentPlayer.ChangeData(DataType.EXPERIENCE, experience);
                    currentPlayer.ChangeData(DataType.EC, ec);                    
                    currentPlayer.ChangeData(DataType.URIDIUM, uridium);
                    currentPlayer.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(10000, 10000));
                    // Return to home map
                    currentGameSession.Player.Jump(currentGameSession.Player.GetBaseMapId(), currentGameSession.Player.GetBasePosition());

                    // update database
                    using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{0}', parts = '[]', wave = '{1}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '8'");
                    }
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
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{0}', wave = '{1}', parts = '[]' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '8'");
                }
            }
            else
            {
                int newLives = lives - 1;
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET lives = '{newLives}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '8'");
                }
            }
        }

        // Create new wave gates
        public void CreateNextGates(Player player)
        {
            lambdaGateNextWavePortal = new Portal(GameManager.GetSpacemap(player.GetPlayerActiveMap()), new Position(9500, 6500), new Position(10500, 6500), player.GetPlayerActiveMap(), 2, 3, true, true, false); // Next wave
            lambdaGateBackPortal = new Portal(GameManager.GetSpacemap(9), new Position(11500, 6500), new Position(19500, 11600), 9, 1, 3, true, true, false); // Back to homebase

            GameManager.SendCommandToMap(player.GetPlayerActiveMap(), lambdaGateNextWavePortal.GetAssetCreateCommand());
            GameManager.SendCommandToMap(player.GetPlayerActiveMap(), lambdaGateBackPortal.GetAssetCreateCommand());
        }
    }
}
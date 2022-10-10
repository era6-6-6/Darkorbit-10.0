
using Darkorbit.Game.Movements;
using Darkorbit.Game.Objects;
using Darkorbit.Managers;
using Darkorbit.Managers.MySQLManager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Darkorbit.Game.GalaxyGates
{
    internal class KronosGates
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
        public Portal kronosGateNextWavePortal { get; private set; }
        public Portal kronosGateBackPortal { get; private set; }

        private Portal alphaGateNextWavePortal;
        private Portal alphaGateBackPortal;

        public KronosGates(int map, GameSession gameSession, int gateWave, int gateLives)
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
                mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET wave = '{wave}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '9'");
            }
        }


        // Create new wave of Npcs
        public async void CreateNewWave()
        {

            if (wave == 1)
            {

                enemysCount = 30;

                CreateNPC(71, 10, 900000); // Wave 1
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {1}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(73, 10, 900000); // Wave 2
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {2}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(75, 10, 900000); // Wave 3
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {3}");

            }
            else if (wave == 2)
            {

                enemysCount = 12;

                CreateNPC(84, 11, 1500); // Wave 5
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {4}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(85, 1, 1500); // Wave 6
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {5}");

            }
            else if (wave == 3)
            {

                enemysCount = 30;

                CreateNPC(73, 5, 15000); // Wave 9
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {6}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(75, 10, 15000); // Wave 10
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {7}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(78, 15, 15000); // Wave 11
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {8}");

            }
            else if (wave == 4)
            {

                enemysCount = 13;

                CreateNPC(71, 12, 5300); // Wave 13
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {9}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(77, 1, 5300); // Wave 14
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {10}");

            }
            else if (wave == 5)
            {

                enemysCount = 24;

                CreateNPC(24, 10, 50000); // Wave 17
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {11}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(25, 6, 50000); // Wave 18
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {12}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(31, 8, 50000); // Wave 19
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {13}");

            }
            else if (wave == 6)
            {

                enemysCount = 16;

                CreateNPC(76, 15, 140000); // Wave 17
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {14}");
                await Task.Delay(15000); // 60000

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(76, 1, 140000); // Wave 18
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {15}");

            }
            else if (wave == 7)
            {

                enemysCount = 20;

                CreateNPC(76, 5, 450000); // Wave 25
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {16}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(78, 10, 200000); // Wave 26
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {17}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(23, 5, 200000); // Wave 27
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {18}");

            }
            else if (wave == 8)
            {

                enemysCount = 11;

                CreateNPC(78, 10, 120000); // Wave 29
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {19}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(79, 1, 120000); // Wave 30
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {20}");

            }
            else if (wave == 9)
            {

                enemysCount = 18;

                CreateNPC(81, 15, 800000); // Wave 33
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {21}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 76)
                {
                    return;
                }

                CreateNPC(28, 3, 800000); // Wave 34
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {22}");

            }
            else if (wave == 10)
            {

                enemysCount = 3;

                CreateNPC(11, 3, 300000); // Wave 37
                currentPlayer.SendPacket($"0|A|STD|Map Kronos - wave {23}");

            }
        }

        public void CreateNPC(int npc, int count, int health)
        {
            for (int i = 0; i < count; i++)
            {
                int npcNumber = i + 1;
                var npckronos = new NpcGG(Randoms.CreateRandomID(), GameManager.GetShip(npc), GameManager.GetSpacemap(gateMapId), Position.GetPosOnCircle(new Position(10500, 6500), 5000), "ϰronos", npcNumber, currentPlayer, 76);
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
                    int uridium = 30000;
                    int honor = 450000;
                    int experience = 12000000;
                    int ec = 3000;
                    currentPlayer.LoadData();
                    currentPlayer.ChangeData(DataType.HONOR, honor);
                    currentPlayer.ChangeData(DataType.EXPERIENCE, experience);
                    currentPlayer.ChangeData(DataType.EC, ec);                  
                    currentPlayer.ChangeData(DataType.URIDIUM, uridium);
                    currentPlayer.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(25000, 25000));
                    // Return to home map
                    currentGameSession.Player.Jump(currentGameSession.Player.GetBaseMapId(), currentGameSession.Player.GetBasePosition());

                    // update database
                    using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{0}', parts = '[]', wave = '{1}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '9'");
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
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{0}', wave = '{1}', parts = '[]' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '9'");
                }
            }
            else
            {
                int newLives = lives - 1;
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET lives = '{newLives}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '9'");
                }
            }
        }

        // Create new wave gates
        public void CreateNextGates(Player player)
        {
            kronosGateNextWavePortal = new Portal(GameManager.GetSpacemap(player.GetPlayerActiveMap()), new Position(9500, 6500), new Position(10500, 6500), player.GetPlayerActiveMap(), 2, 3, true, true, false); // Next wave
            kronosGateBackPortal = new Portal(GameManager.GetSpacemap(9), new Position(11500, 6500), new Position(19500, 11600), 9, 1, 3, true, true, false); // Back to homebase

            GameManager.SendCommandToMap(player.GetPlayerActiveMap(), kronosGateNextWavePortal.GetAssetCreateCommand());
            GameManager.SendCommandToMap(player.GetPlayerActiveMap(), kronosGateBackPortal.GetAssetCreateCommand());
        }
    }
}
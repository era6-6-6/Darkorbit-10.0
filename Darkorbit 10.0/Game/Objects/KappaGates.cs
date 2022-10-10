using Darkorbit.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using Darkorbit.Game.Objects;
using Darkorbit.Managers.MySQLManager;
using Darkorbit.Managers;
using Darkorbit.Game.Movements;

namespace Darkorbit.Game.GalaxyGates
{
    internal class KappaGates
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
        public Portal kappaGateNextWavePortal { get; private set; }
        public Portal kappaGateBackPortal { get; private set; }

        private Portal alphaGateNextWavePortal;
        private Portal alphaGateBackPortal;

        public KappaGates(int map, GameSession gameSession, int gateWave, int gateLives)
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
                mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET wave = '{wave}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '7'");
            }
        }


        // Create new wave of Npcs
        public async void CreateNewWave()
        {

            if (wave == 1)
            {

                enemysCount = 30;

                CreateNPC(92, 10, 150000); // Wave 3
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {1}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(94, 10, 800000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(84, 10, 100); // Wave 30
            }
            else if (wave == 2)
            {

                enemysCount = 21;

                CreateNPC(93, 6, 380000); // Wave 6
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {2}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(97, 7, 370000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(31, 8, 80000); // Wave 30

            }
            else if (wave == 3)
            {

                enemysCount = 15;

                CreateNPC(92, 6, 150000); // Wave 11
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {3}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(72, 6, 100000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(103, 3, 50000); // Wave 30

            }
            else if (wave == 4)
            {

                enemysCount = 15;

                CreateNPC(91, 5, 200000); // Wave 14
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {4}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(27, 5, 160000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(97, 5, 370000); // Wave 30

            }
            else if (wave == 5)
            {

                enemysCount = 9;


                CreateNPC(96, 4, 300000); // Wave 19
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {5}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(78, 5, 50000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(95, 4, 400000); // Wave 30

            }
            else if (wave == 6)
            {

                enemysCount = 17;

                CreateNPC(111, 8, 150000); // Wave 18
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {6}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(112, 6, 180000); // Wave 30
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {20}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(114, 3, 800000); // Wave 30
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {20}");

            }
            else if (wave == 7)
            {

                enemysCount = 16;


                CreateNPC(28, 6, 1200000); // Wave 27
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {7}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(46, 2, 800000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(81, 8, 50000); // Wave 30

            }
            else if (wave == 8)
            {

                enemysCount = 14;

                CreateNPC(82, 3, 2000000); // Wave 30
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {8}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(82, 3, 2000000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(38, 8, 45000); // Wave 30

            }
            else if (wave == 9)
            {

                enemysCount = 11;


                CreateNPC(95, 5, 370000); // Wave 34
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {9}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(29, 5, 200000); // Wave 30
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(11, 1, 1500000); // Wave 30

            }
            else if (wave == 10)
            {

                enemysCount = 2;

                CreateNPC(118, 1, 3000000); // Wave 37
                currentPlayer.SendPacket($"0|A|STD|Map Kappa - wave {10}");
                await Task.Delay(15000);

                if (currentPlayer.positionInitializacion.mapID != 74)
                {
                    return;
                }

                CreateNPC(90, 1, 3600000); // Wave 30

            }
        }

        public void CreateNPC(int npc, int count, int health)
        {
            for (int i = 0; i < count; i++)
            {
                int npcNumber = i + 1;
                new NpcGG(Randoms.CreateRandomID(), GameManager.GetShip(npc), GameManager.GetSpacemap(gateMapId), Position.GetPosOnCircle(new Position(10500, 6500), 5000), "ϰ", npcNumber, currentPlayer, 74);
            }
        }

        public void WaveCheck(Player player)
        {
            // Killed Npc
            enemysCount = enemysCount - 1;

            //foreach (var character in player.Spacemap.Characters.Values)
            //{
            //    if (character is NpcGG)
            //    {
            //        if (character.Destroyed)
            //        {
            //            character.MainAttacker = null;
            //            (character as NpcGG).Attacking = false;
            //            Spacemap.RemoveCharacter(character);
            //        }
            //    }
            //}

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
                    int honor = 30000;
                    int experience = 9000000;
                    int ec = 2500;
                    int ran = Randoms.random.Next(1, 100);
                    int droneDesignChance = Randoms.random.Next(1, 100);
                    currentPlayer.LoadData();
                    currentPlayer.ChangeData(DataType.HONOR, honor);
                    currentPlayer.ChangeData(DataType.EXPERIENCE, experience);
                    currentPlayer.ChangeData(DataType.EC, ec);
                    currentPlayer.ChangeData(DataType.URIDIUM, uridium);
                    currentPlayer.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(30000, 30000));
                    // Return to home map
                    currentGameSession.Player.Jump(currentGameSession.Player.GetBaseMapId(), currentGameSession.Player.GetBasePosition());

                    // update database
                    using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{3}', parts = '[]', wave = '{1}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '7'");

                        if (ran >= 1 &&  ran <= 65)
                        {
                            if (droneDesignChance >= 1 && droneDesignChance <= 50)
                            {
                                var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                                foreach (DataRow row in equipment.Rows)
                                {
                                    var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                                    int logdisks = (int)items.logdisks;
                                    if (logdisks == 10)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        logdisks++;
                                        items.logdisks = logdisks;
                                        mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                                        player.SendPacket("0|A|STD|You received 1 Havoc Drone Design");
                                    }
                                }
                            }
                            else
                            {
                                var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                                foreach (DataRow row in equipment.Rows)
                                {
                                    var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                                    int hercules = (int)items.herculesCount;
                                    if (hercules == 10)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        hercules++;
                                        items.herculesCount = hercules;
                                        mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                                        player.SendPacket("0|A|STD|You received 1 Hercules Drone Design");
                                    }
                                }
                            }
                        }
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
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET prepared = '{0}', lives = '{0}', wave = '{1}', parts = '[]' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '7'");
                }
            }
            else
            {
                int newLives = lives - 1;
                using (SqlDatabaseClient mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_galaxygates SET lives = '{newLives}' WHERE userId = {currentPlayer.GetPlayerId()} AND gateId = '7'");
                }
            }
        }

        // Create new wave gates
        public void CreateNextGates(Player player)
        {
            kappaGateNextWavePortal = new Portal(GameManager.GetSpacemap(player.GetPlayerActiveMap()), new Position(9500, 6500), new Position(10500, 6500), player.GetPlayerActiveMap(), 2, 3, true, true, false); // Next wave
            kappaGateBackPortal = new Portal(GameManager.GetSpacemap(9), new Position(11500, 6500), new Position(19500, 11600), 9, 1, 3, true, true, false); // Back to homebase

            GameManager.SendCommandToMap(player.GetPlayerActiveMap(), kappaGateNextWavePortal.GetAssetCreateCommand());
            GameManager.SendCommandToMap(player.GetPlayerActiveMap(), kappaGateBackPortal.GetAssetCreateCommand());
        }
    }
}
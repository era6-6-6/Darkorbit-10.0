using Darkorbit.Game.Movements;
using Darkorbit.Managers.MySQLManager;
using Darkorbit.Net.netty.commands;
using Darkorbit.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Collectables
{
    class GreenBooty : Collectable
    {
        public GreenBooty(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
          

            /* DESIGN SECTION */

            int ranDesign = Randoms.random.Next(1, 5);

            var uridium = Randoms.random.Next(100, 1000);
            int ran = Randoms.random.Next(1, 100);
            player.LoadData();
            if (ran <= 1 && ran >= 0) //jackpot
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(300, 500));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(200, 200));
                
            }
            else if (ran <= 11 && ran > 1)// ucb100
            {
                
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(200, 5000));
            }
            else if (ran <= 26 && ran > 11)// rsb75
            {
                
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(1700, 6100));
            }
            else if (ran <= 41 && ran > 26)// ucb100
            {

                player.AmmunitionManager.AddAmmo(AmmunitionManager.ACM_01, Randoms.random.Next(1, 9));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(100, 300));
            }
            else if (ran <= 51 && ran > 41)// rsb75
            {

                player.AmmunitionManager.AddAmmo(AmmunitionManager.EMP_01, Randoms.random.Next(1, 3));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(250, 350));
            }
            else if (ran <= 61 && ran > 51)// rsb75
            {
                
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 1350));
            }
            else if (ran <= 70 && ran > 61)// rsb75
            {
                
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 550));
            }
            else if (ran <= 79 && ran > 70)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(200, 500));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(100, 250));
                
            }
            else if (ran <= 96 && ran > 79)// rsb75
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(250, 550));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 550));
            }
            else if (ran <= 100 && ran > 98)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    if (ranDesign == 1)
                    {
                        // DESIGN SOLANCE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_goliath_design_solace'");
                        if (result.Rows.Count >= 1)
                        {
                            player.SendPacket($"0|A|STD|You got 20.000 uridium");
                            player.ChangeData(DataType.URIDIUM, 20_000);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_goliath_design_solace', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Goliath Design SOLACE");
                        }
                    }
                    else if (ranDesign == 2)
                    {
                        // DESIGN SOLANCE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_goliath_design_diminisher'");
                        if (result.Rows.Count >= 1)
                        {
                            player.SendPacket($"0|A|STD|You got 20.000 uridium");

                            player.ChangeData(DataType.URIDIUM, 20_000);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_goliath_design_diminisher', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Goliath Design DIMINISHER");
                        }
                    }
                    else if (ranDesign == 3)
                    {
                        // DESIGN SOLANCE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_goliath_design_sentinel'");
                        if (result.Rows.Count >= 1)
                        {
                            player.SendPacket($"0|A|STD|You got 20.000 uridium");

                            player.ChangeData(DataType.URIDIUM, 20_000);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_goliath_design_sentinel', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Goliath Design SENTINEL");
                        }
                    }
                    else if (ranDesign == 4)
                    {
                        // DESIGN SOLANCE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_goliath_design_spectrum'");
                        if (result.Rows.Count >= 1)
                        {
                            player.SendPacket($"0|A|STD|You got 20.000 uridium");

                            player.ChangeData(DataType.URIDIUM, 20_000);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_goliath_design_spectrum', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Goliath Design SPECTRUM");
                        }
                    }
                    else if (ranDesign == 5)
                    {
                        // DESIGN SOLANCE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_goliath_design_venom'");
                        if (result.Rows.Count >= 1)
                        {
                            player.SendPacket($"0|A|STD|You got 20000 uridium");
                            player.ChangeData(DataType.URIDIUM, 20_000);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_goliath_design_venom', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Goliath Design VENOM");
                        }
                    }
            }
            player.bootyKeys.greenKeys--;
            player.SendPacket($"0|A|BK|{player.bootyKeys.greenKeys}");

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("PIRATE_BOOTY", Hash, Position.Y, Position.X);
        }
    }
}
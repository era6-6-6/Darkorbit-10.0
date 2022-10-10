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
            var randomDesign = new Random();
            var designlist_goliath = new List<String>
            {
                ""
            };

            /* DESIGN SECTION */

            int ranDesign = Randoms.random.Next(1, 5);

            var uridium = Randoms.random.Next(100, 200);
            var credits = Randoms.random.Next(40000, 150000);
            int ran = Randoms.random.Next(1, 100);
            player.LoadData();
            if (ran <= 1 && ran >= 0) //jackpot
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(200, 5000));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(200, 5000));
                player.ChangeData(DataType.URIDIUM, uridium);
            }
            else if (ran <= 11 && ran > 1)// ucb100
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(200, 5000));
            }
            else if (ran <= 26 && ran > 11)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(1700, 6100));
            }
            else if (ran <= 41 && ran > 26)// ucb100
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(100, 3000));
            }
            else if (ran <= 51 && ran > 41)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(250, 850));
            }
            else if (ran <= 61 && ran > 51)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 2550));
            }
            else if (ran <= 70 && ran > 61)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 550));
            }
            else if (ran <= 79 && ran > 70)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(200, 500));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(100, 250));
                player.ChangeData(DataType.URIDIUM, uridium);
            }
            else if (ran <= 96 && ran > 79)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 550));
            }
            else if (ran <= 100 && ran > 96)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    if (ranDesign == 1)
                    {
                        // DESIGN SOLANCE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_goliath_design_solace'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
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
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
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
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
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
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
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
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_goliath_design_venom', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Goliath Design VENOM");
                        }
                    }
            }

            //player.Equipment.Items.greenKeys--;
            player.bootyKeys.greenKeys--;

            player.SendPacket($"0|A|BK|{player.bootyKeys.greenKeys}");

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("PIRATE_BOOTY", Hash, Position.Y, Position.X);
        }
    }
}
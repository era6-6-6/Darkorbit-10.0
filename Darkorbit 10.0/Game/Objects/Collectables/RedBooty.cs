using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Collectables
{
    class RedBooty : Collectable
    {
        public RedBooty(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

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
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(200, 500));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(200, 500));
                player.ChangeData(DataType.URIDIUM, uridium);
            }
            else if (ran <= 11 && ran > 1)// ucb100
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(200, 500));
            }
            else if (ran <= 26 && ran > 11)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(1700, 6100));
            }
            else if (ran <= 41 && ran > 26)// ucb100
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(100, 700));
            }
            else if (ran <= 51 && ran > 41)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(250, 550));
            }
            else if (ran <= 61 && ran > 51)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 550));
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
            else if (ran <= 91 && ran > 79)// rsb75
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(250, 550));
            }
            else if (ran <= 100 && ran > 91)
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
                    else if (ranDesign == 6)
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
                    else if (ranDesign == 7)
                    {   //SPEARHEAD ELITE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spearhead_design_spearhead-superelite'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spearhead_design_spearhead-superelite', 70, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design SPEARHEAD ELITE");
                        }
                    }
                    else if (ranDesign == 8)
                    {   //SOLACE ASIIMOV
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-asimov'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-asimov', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solace-Asimov");
                        }
                    }
                    else if (ranDesign == 9)
                    {   //SOLACE AGON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-argon'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-argon', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solace-Argon");
                        }
                    }
                    else if (ranDesign == 10)
                    {   //SOLACE BLAZE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-blaze'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-blaze', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solace-Blaze");
                        }
                    }
                    else if (ranDesign == 11)
                    {// SOLACE BOREALIS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-borealis'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-borealis', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solace-Borealis");
                        }
                    }
                    else if (ranDesign == 12)
                    {   // SOLACE OCEAN
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-ocean'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-ocean', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solace-Ocean");
                        }
                    }
                    else if (ranDesign == 13)
                    {   //SOLACE POISON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-poison'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-poison', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solace-Poison");
                        }
                    }
                    else if (ranDesign == 14)
                    {   //SOLACE Tyrannos
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-tyrannos'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-tyrannos', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solace-Tyrannos");
                        }
                    }
                    else if (ranDesign == 15)
                    {   //SPECTRUM DUSKLIGHT
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-dusklight'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-dusklight', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-Dusklight");
                        }
                    }
                    else if (ranDesign == 16)
                    {   //SPECTRUM Legend
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-legend'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-legend', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-Legend");
                        }
                    }
                    else if (ranDesign == 17)
                    {   //SENTINEL ARGON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-argon'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-argon', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel-Argon");
                        }
                    }
                    else if (ranDesign == 18)
                    {   //SENTINEL LEGEND
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-legend'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-legend', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel-Legend");
                        }
                    }
                    else if (ranDesign == 19)
                    {   //DIMINISHER ARGON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_diminisher_design_diminisher-argon'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_diminisher_design_diminisher-argon', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Diminisher-Argon");
                        }
                    }
                    else if (ranDesign == 20)
                    {
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_diminisher_design_diminisher-legend'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_diminisher_design_diminisher-legend', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Diminisher-Legend");
                        }
                    }
                    else if (ranDesign == 21)
                    {   //SENTINEL EXPO 16
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-expo2016'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-expo2016', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel-Expo");
                        }
                    }
                    else if (ranDesign == 22)
                    {   //SENTINEL FROST
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-frost'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-frost', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel-FROST");
                        }
                    }
                    else if (ranDesign == 23)
                    {   // SPECTRUM INFERNO
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-inferno'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-inferno', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-INFERNO");
                        }
                    }
                    else if (ranDesign == 24)
                    {   // SPECTRUM POISON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-poison'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-poison', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-POISON");
                        }
                    }
                    else if (ranDesign == 25)
                    {   //SPEC LAVA
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-lava'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-lava', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-LAVA");
                        }
                    }
                    else if (ranDesign == 26)
                    {   //SPEC SANDSTORM
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-sandstorm'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-sandstorm', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-SANDSTORM");
                        }
                    }
                    else if (ranDesign == 27)
                    {   //SPEC BLAZE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'sship_spectrum_design_spectrum-blaze'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-blaze', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-BLAZE");
                        }
                    }
                    else if (ranDesign == 28)
                    {   //SPEC OCEAN
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-ocean'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-ocean', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-OCEAN");
                        }
                    }
                    else if (ranDesign == 29)
                    {   //DEMINISHER EXPO
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_diminisher_design_diminisher-expo2016'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_diminisher_design_diminisher-expo2016', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Diminisher-Expo");
                        }
                    }
                    else if (ranDesign == 30)
                    {   //DEMINISHER expo
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_diminisher_design_diminisher-lava'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_diminisher_design_diminisher-lava', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Diminisher-LAVA");
                        }
                    }
                    else if (ranDesign == 31)
                    {   //HAMMERCLAW LAVA
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-lava'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-lava', 246, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Hammerclaw-LAVA");
                        }
                    }
                    else if (ranDesign == 32)
                    {   //HAMMERCLAW CARBONITE
                        //var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-carbonite'");
                        //if (result.Rows.Count >= 1)
                        //{
                        //    player.ChangeData(DataType.CREDITS, credits);
                        //    player.ChangeData(DataType.URIDIUM, uridium);
                        //}
                        //else
                        //{
                        //    mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-carbonite', 246, {player.Id})");
                        //    player.SendPacket($"0|A|STD|You got Design Hammerclaw-CARBONITE");
                        //}
                        player.ChangeData(DataType.CREDITS, credits);
                        player.ChangeData(DataType.URIDIUM, uridium);
                    }
                    else if (ranDesign == 33)
                    {   //HAMMERCLAW BANE
                        //var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-bane'");
                        //if (result.Rows.Count >= 1)
                        //{
                        //    player.ChangeData(DataType.CREDITS, credits);
                        //    player.ChangeData(DataType.URIDIUM, uridium);
                        //}
                        //else
                        //{
                        //    mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-bane', 264, {player.Id})");
                        //    player.SendPacket($"0|A|STD|You got Design Hammerclaw-BANE");
                        //}
                        player.ChangeData(DataType.CREDITS, credits);
                        player.ChangeData(DataType.URIDIUM, uridium);
                    }
                    else if (ranDesign == 34)
                    {   //HAMMERCLAW FROZEN
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-frozen'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-frozen', 246, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Hammerclaw-FROZEN");
                        }
                    }
                    else if (ranDesign == 35)
                    {   //HAMMERCLAW NOBILIS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-nobilis'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-nobilis', 246, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Hammerclaw-NOBILIS");
                        }
                    }
                    else if (ranDesign == 36)
                    {   //SENTINEL ASIMOV
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-asimov'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-asimov', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel-ASIMOV");
                        }
                    }
                    else if (ranDesign == 37)
                    {   //SOLACE FROST
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-frost'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-frost', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design SOLACE FROST");
                        }
                    }
                    else if (ranDesign == 38)
                    {   //SENTINEL ARIOS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-arios'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-arios', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel ARIOS");
                        }
                    }
                    else if (ranDesign == 39)
                    {//SENTINEL LAVA
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-lava'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-lava', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel LAVA");
                        }
                    }
                    else if (ranDesign == 40)
                    {   //SENTINEL TYRANNOS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-tyrannos'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-tyrannos', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel TYRANNOS");
                        }
                    }
                    else if (ranDesign == 41)
                    {   //SPEC TYRANNOS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-tyrannos'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-tyrannos', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Spectrum-TYRANNOS");
                        }
                    }
                    else if (ranDesign == 42)
                    {   //VENOM ARGON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_venom_design_venom-argon'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_venom_design_venom-argon', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Venom-ARGON");
                        }
                    }
                    else if (ranDesign == 43)
                    {   //VENOM BLAZE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_venom_design_venom-blaze'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_venom_design_venom-blaze', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Venom-BLAZE");
                        }
                    }
                    else if (ranDesign == 44)
                    {   //VENOM BOREALIS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_venom_design_venom-borealis'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_venom_design_venom-borealis', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Venom-BOREALIS");
                        }
                    }
                    else if (ranDesign == 45)
                    {   //VENOM OCEAN
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_venom_design_venom-ocean'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_venom_design_venom-ocean', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Venom-OCEAN");
                        }
                    }
                    else if (ranDesign == 46)
                    {   //VENOM POISON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_venom_design_venom-poison'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_venom_design_venom-poison', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Venom-POISON");
                        }
                    }
                    else if (ranDesign == 47)
                    {   //VENOM FROST
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_diminisher_design_diminisher-frost'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_diminisher_design_diminisher-frost', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Diminisher-FROST");
                        }
                    }
                    else if (ranDesign == 48)
                    {   //HAMMERCLAW TYRANNOS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-tyrannos'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-tyrannos', 246, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Hammerclaw-TYRANNOS");
                        }
                    }
                    else if (ranDesign == 49)
                    { // HAMEMRCLAW PROMETHEUS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-prometheus'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-prometheus', 246, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Hammerclaw-PROMETHEUS");
                        }
                    }
                    else if (ranDesign == 50)
                    {   //HAMMERCLAW PRMETHEUS
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_hammerclaw_design_hammerclaw-tyrannos'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_hammerclaw_design_hammerclaw-tyrannos', 246, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Hammerclaw-TYRANNOS");
                        }
                    }
                    else if (ranDesign == 51)
                    {   //PUSAT BLAZE
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_pusat_design_pusat-blaze'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_pusat_design_pusat-blaze', 130, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Pusat-BLAZE");
                        }
                    }
                    else if (ranDesign == 52)
                    {   //PUSAT EXPO
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_pusat_design_pusat-expo16'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_pusat_design_pusat-expo16', 130, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Pusat-EXPO");
                        }
                    }
                    else if (ranDesign == 53)
                    {   //PUSAT LAVA
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_pusat_design_pusat-lava'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_pusat_design_pusat-lava', 130, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Pusat-LAVA");
                        }
                    }
                    else if (ranDesign == 54)
                    {   //PUSAT LEGEND
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_pusat_design_pusat-legend'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_pusat_design_pusat-legend', 130, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Pusat-LEGEND");
                        }
                    }
                    else if (ranDesign == 55)
                    {   //PUSAT OCEAN
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_pusat_design_pusat-ocean'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_pusat_design_pusat-ocean', 130, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Pusat-OCEAN");
                        }
                    }
                    else if (ranDesign == 56)
                    {   //PUSAT POISON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_pusat_design_pusat-poison'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_pusat_design_pusat-poison', 130, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Pusat-POISON");
                        }
                    }
                    else if (ranDesign == 57)
                    {   //PUSAT SANDSTORM
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_pusat_design_pusat-sandstorm'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_pusat_design_pusat-sandstorm', 130, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Pusat-SANDSTORM");
                        }
                    }
                    else if (ranDesign == 58)
                    {   //SOLACE CONTAGION
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_solace_design_solace-contagion'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_solace_design_solace-contagion', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Solance-CONTAGION");
                        }
                    }
                    else if (ranDesign == 59)
                    {   //SENTINEL CONTAGION
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_sentinel_design_sentinel-contagion'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_sentinel_design_sentinel-contagion', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel-CONTAGION");
                        }
                    }
                    else if (ranDesign == 60)
                    {   //SEPCTRUM ARGON
                        var result = mySqlClient.ExecuteQueryTable($"SELECT name FROM player_designs WHERE userId = {player.Id} AND name = 'ship_spectrum_design_spectrum-argon'");
                        if (result.Rows.Count >= 1)
                        {
                            player.ChangeData(DataType.CREDITS, credits);
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO player_designs (name, baseShipId, userId) VALUES ('ship_spectrum_design_spectrum-argon', 10, {player.Id})");
                            player.SendPacket($"0|A|STD|You got Design Sentinel-CONTAGION");
                        }
                    }
            }
           
                //player.Equipment.Items.greenKeys--;
                player.bootyKeys.redKeys--;

            player.SendPacket($"0|A|BKR|{player.bootyKeys.redKeys}");

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("PIRATE_BOOTY_RED", Hash, Position.Y, Position.X);
        }
    }
}
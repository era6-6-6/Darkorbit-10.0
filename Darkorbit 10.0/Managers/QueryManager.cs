using System.Data;
using Darkorbit.Game.Objects.Players.Stations;
using Pet = Darkorbit.Game.Objects.Pet;

namespace Darkorbit.Managers
{
    class QueryManager
    {
        public class SavePlayer
        {
            public static void SaveData(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET data = '{JsonConvert.SerializeObject(player.Data)}', nanohull = {player.CurrentNanoHull}, destructions = '{JsonConvert.SerializeObject(player.Destructions)}', warPoints = {player.UbaPoints}, warBattel = {player.Ubabattel}, bootyKeys = '{JsonConvert.SerializeObject(player.bootyKeys)}', droneExp = {player.droneExp}, position = '{JsonConvert.SerializeObject(player.positionInitializacion)}'  WHERE userId = {player.Id}");
                    mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET boosters = '{JsonConvert.SerializeObject(player.BoosterManager.Boosters)}' WHERE userId = {player.Id}");
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET droneExp = {player.droneExp} WHERE userId = {player.Id}");
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET ammo = '{{\"mcb25\":{player.AmmunitionManager.mcb25},\"lcb10\":{player.AmmunitionManager.lcb10},\"mcb50\":{player.AmmunitionManager.mcb50},\"mcb100\":{player.AmmunitionManager.mcb100},\"mcb250\":{player.AmmunitionManager.mcb250},\"mcb500\":{player.AmmunitionManager.mcb500},\"hstrm\":{player.AmmunitionManager.hstrm},\"sar2\":{player.AmmunitionManager.sar2},\"ucb\":{player.AmmunitionManager.ucb},\"rsb\":{player.AmmunitionManager.rsb},\"sab\":{player.AmmunitionManager.sab},\"pib\":{player.AmmunitionManager.pib},\"ish\":{player.AmmunitionManager.ish},\"emp\":{player.AmmunitionManager.emp},\"smb\":{player.AmmunitionManager.smb},\"plt3030\":{player.AmmunitionManager.plt3030},\"ice\":{player.AmmunitionManager.ice},\"dcr\":{player.AmmunitionManager.dcr},\"wiz\":{player.AmmunitionManager.wiz},\"pld\":{player.AmmunitionManager.pld},\"slm\":{player.AmmunitionManager.slm},\"ddm\":{player.AmmunitionManager.ddm},\"empm\":{player.AmmunitionManager.empm},\"sabm\":{player.AmmunitionManager.sabm},\"cloacks\":{player.AmmunitionManager.cloacks},\"r310\":{player.AmmunitionManager.r310},\"plt26\":{player.AmmunitionManager.plt26},\"cbo100\":{player.AmmunitionManager.cbo100},\"job100\":{player.AmmunitionManager.job100},\"rb214\":{player.AmmunitionManager.rb214},\"plt21\":{player.AmmunitionManager.plt21},\"eco\":{player.AmmunitionManager.eco}}}' WHERE userId = " + player.Id);
                }
            }

            public static void Settings(Player player, string target, object settings)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_settings SET {target} = '{JsonConvert.SerializeObject(settings)}' WHERE userId = {player.Id}");
            }

            public static void Information(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET data = '{JsonConvert.SerializeObject(player.Data)}', nanohull = {player.CurrentNanoHull}, destructions = '{JsonConvert.SerializeObject(player.Destructions)}', warPoints = {player.UbaPoints}, warBattel = {player.Ubabattel}, bootyKeys = '{JsonConvert.SerializeObject(player.bootyKeys)}', droneExp = {player.droneExp}, position = '{JsonConvert.SerializeObject(player.positionInitializacion)}'  WHERE userId = {player.Id}");
            }

            public static void changePremium(Player player, int value)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET premium={value} WHERE userId = {player.Id}");

            }
            public static void Boosters(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET boosters = '{JsonConvert.SerializeObject(player.BoosterManager.Boosters)}' WHERE userId = {player.Id}");
            }

            public static void SaveDronesEXP(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET droneExp = {player.droneExp} WHERE userId = {player.Id}");
            }

            public static void Modules(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET modules = '{JsonConvert.SerializeObject(player.Storage.BattleStationModules)}' WHERE userId = {player.Id}");
            }
            public static void BoostersBUY(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET boostersList = '' WHERE userId = {player.Id}");

            }

            public static void Ammunition(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_accounts SET ammo = '{{\"mcb25\":{player.AmmunitionManager.mcb25},\"lcb10\":{player.AmmunitionManager.lcb10},\"mcb50\":{player.AmmunitionManager.mcb50},\"mcb100\":{player.AmmunitionManager.mcb100},\"mcb250\":{player.AmmunitionManager.mcb250},\"mcb500\":{player.AmmunitionManager.mcb500},\"hstrm\":{player.AmmunitionManager.hstrm},\"sar2\":{player.AmmunitionManager.sar2},\"ucb\":{player.AmmunitionManager.ucb},\"rsb\":{player.AmmunitionManager.rsb},\"sab\":{player.AmmunitionManager.sab},\"pib\":{player.AmmunitionManager.pib},\"ish\":{player.AmmunitionManager.ish},\"emp\":{player.AmmunitionManager.emp},\"smb\":{player.AmmunitionManager.smb},\"plt3030\":{player.AmmunitionManager.plt3030},\"ice\":{player.AmmunitionManager.ice},\"dcr\":{player.AmmunitionManager.dcr},\"wiz\":{player.AmmunitionManager.wiz},\"pld\":{player.AmmunitionManager.pld},\"slm\":{player.AmmunitionManager.slm},\"ddm\":{player.AmmunitionManager.ddm},\"empm\":{player.AmmunitionManager.empm},\"sabm\":{player.AmmunitionManager.sabm},\"cloacks\":{player.AmmunitionManager.cloacks},\"r310\":{player.AmmunitionManager.r310},\"plt26\":{player.AmmunitionManager.plt26},\"cbo100\":{player.AmmunitionManager.cbo100},\"job100\":{player.AmmunitionManager.job100},\"rb214\":{player.AmmunitionManager.rb214},\"plt21\":{player.AmmunitionManager.plt21},\"eco\":{player.AmmunitionManager.eco}}}' WHERE userId = " + player.Id);
            }


            public static void ModulesBUY(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET modulesList = '' WHERE userId = {player.Id}");
            }

            public static void Items(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var data = mySqlClient.ExecuteQueryTable($"SELECT * FROM player_equipment WHERE userId = {player.Id}");
                    foreach (DataRow row in data.Rows)
                    {
                        dynamic items = JsonConvert.DeserializeObject(row["items"].ToString());
                        if (items["pet"] == "true")
                        {
                            items["fuel"] = player.Pet.Fuel;
                            items["GUARD"] = player.Pet.GUARD;
                            items["KAMIKAZE"] = player.Pet.KAMIKAZE;
                            items["COMBO_SHIP_REPAIR"] = player.Pet.COMBO_SHIP_REPAIR;
                            items["REPAIR_PET"] = player.Pet.REPAIR_PET;
                            items["AUTO_LOOT"] = player.Pet.AUTO_LOOT;
                        }

                        mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = " + player.Id);
                    }
                }
            }

            public static void saveEC(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE event_coins SET coins = '{player.ec}' WHERE userId = " + player.Id);
            }

            public static void formations(Player player)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var data = mySqlClient.ExecuteQueryTable($"SELECT * FROM player_equipment WHERE userId = {player.Id}");
                    foreach (DataRow row in data.Rows)
                    {

                        dynamic formations;

                        if (row["formationsSaved"] == "")
                        {
                            formations = JsonConvert.DeserializeObject("{}".ToString());
                        }
                        /*else
                        {
                            formations = JsonConvert.DeserializeObject(row["formationsSaved"].ToString());
                        }

                        //formations[DroneManager.DEFAULT_FORMATION] = player.DEFAULT_FORMATION;
                        formations[DroneManager.ARROW_FORMATION] = player.ARROW_FORMATION;
                        formations[DroneManager.BARRAGE_FORMATION] = player.BARRAGE_FORMATION;
                        formations[DroneManager.BAT_FORMATION] = player.BAT_FORMATION;
                        formations[DroneManager.CHEVRON_FORMATION] = player.CHEVRON_FORMATION;
                        formations[DroneManager.CRAB_FORMATION] = player.CRAB_FORMATION;
                        formations[DroneManager.DIAMOND_FORMATION] = player.DIAMOND_FORMATION;
                        formations[DroneManager.DOME_FORMATION] = player.DOME_FORMATION;
                        formations[DroneManager.DOUBLE_ARROW_FORMATION] = player.DOUBLE_ARROW_FORMATION;
                        formations[DroneManager.DRILL_FORMATION] = player.DRILL_FORMATION;
                        formations[DroneManager.HEART_FORMATION] = player.HEART_FORMATION;
                        formations[DroneManager.LANCE_FORMATION] = player.LANCE_FORMATION;
                        formations[DroneManager.MOTH_FORMATION] = player.MOTH_FORMATION;
                        formations[DroneManager.PINCER_FORMATION] = player.PINCER_FORMATION;
                        formations[DroneManager.RING_FORMATION] = player.RING_FORMATION;
                        formations[DroneManager.STAR_FORMATION] = player.STAR_FORMATION;
                        formations[DroneManager.TURTLE_FORMATION] = player.TURTLE_FORMATION;
                        formations[DroneManager.VETERAN_FORMATION] = player.VETERAN_FORMATION;
                        formations[DroneManager.WAVE_FORMATION] = player.WAVE_FORMATION;
                        formations[DroneManager.WHEEL_FORMATION] = player.WHEEL_FORMATION;
                        formations[DroneManager.X_FORMATION] = player.X_FORMATION;
                        */
                        //mySqlClient.ExecuteNonQuery($"UPDATE player_equipment SET formationsSaved = '{JsonConvert.SerializeObject(formations)}' WHERE userId = " + player.Id);
                    }
                }
            }

        }

        public class ChatFunctions
        {
            public static void AddBan(int bannedId, int modId, string reason, int typeId, string endDate)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var result = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT userId FROM player_accounts WHERE userId = {bannedId}");
                    if (result.Rows.Count >= 1)
                    {
                        mySqlClient.ExecuteNonQuery($"INSERT INTO server_bans (userId, modId, reason, typeId, end_date) VALUES ({bannedId}, {modId}, '{reason}', {typeId}, '{endDate}')");

                        GameManager.SendChatSystemMessage($"{QueryManager.GetUserPilotName(bannedId)} has banned.");
                    }
                }
            }

            public static void UnBan(int bannedId, int modId, int typeId)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var result = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT * FROM server_bans WHERE userId = {bannedId} AND typeId = {typeId}");
                    if (result.Rows.Count >= 1)
                    {
                        //mySqlClient.ExecuteNonQuery($"UPDATE server_bans SET ended = 1 WHERE userId = {bannedId} AND typeId = {typeId}");
                        mySqlClient.ExecuteNonQuery($"DELETE FROM server_bans WHERE userId = {bannedId} AND typeId = {typeId}");

                        var client = GameManager.ChatClients[modId];

                        if (client != null)
                            client.Send($"{QueryManager.GetUserPilotName(bannedId)} has unbanned.");
                    }
                }
            }

            public static bool Banned(int userId)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var result = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT id FROM server_bans WHERE userId = {userId} AND typeId = 0 AND ended = 0");
                    return result.Rows.Count >= 1 ? true : false;
                }
            }
        }

        public static string GetUserPilotName(int userId)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var result = mySqlClient.ExecuteQueryRow($"SELECT pilotName FROM player_accounts WHERE userId = {userId}");
                return result["pilotName"].ToString();
            }
        }

        public static bool CheckSessionId(int userId, string sessionId)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var query = $"SELECT sessionId FROM player_accounts WHERE userId = {userId}";
                var table = (DataTable)mySqlClient.ExecuteQueryTable(query);

                if (table.Rows.Count >= 1)
                {
                    var result = mySqlClient.ExecuteQueryRow(query);
                    return sessionId == result["sessionId"].ToString();
                }
                else return false;
            }
        }

        public static bool Banned(int userId)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var result = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT id FROM server_bans WHERE userId = {userId} AND typeId = 1 AND ended = 0");
                return result.Rows.Count >= 1 ? true : false;
            }
        }

        public static Player GetPlayer(int playerId)
        {
            Player player = null;
            try
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var data = mySqlClient.ExecuteQueryTable($"SELECT * FROM player_accounts WHERE userId = {playerId}");
                    foreach (DataRow row in data.Rows)
                    {
                        var name = Convert.ToString(row["pilotName"]);
                        var ship = GameManager.GetShip(Convert.ToInt32(row["shipId"]));
                        var factionId = Convert.ToInt32(row["factionId"]);
                        var rankId = Convert.ToInt32(row["rankID"]);
                        var rank = Convert.ToInt32(row["rank"]);
                        var clan = GameManager.GetClan(Convert.ToInt32(row["clanID"]));
                        var petDesign = Convert.ToInt16(row["petDesign"]);


                        player = new Player(playerId, name, clan, factionId, rankId, rank, ship, petDesign);
                        player.Premium = Convert.ToBoolean(row["premium"]);
                        player.Title = Convert.ToString(row["title"]);
                        player.Data = JsonConvert.DeserializeObject<DataBase>(row["data"].ToString());
                        player.Destructions = JsonConvert.DeserializeObject<DestructionsBase>(row["destructions"].ToString());
                        player.CurrentNanoHull = Convert.ToInt32(row["nanohull"]);
                        player.PetName = Convert.ToString(row["petName"]);
                        player.UbaPoints = Convert.ToInt32(row["warPoints"]);
                        player.Ubabattel = Convert.ToInt32(row["warBattel"]);
                        player.WarRank = Convert.ToInt32(row["warRank"]);

                        if (row["bootyKeys"] == "")
                        {
                            player.bootyKeys = JsonConvert.DeserializeObject<bootyKeys>("{}".ToString());
                        }
                        else
                        {
                            player.bootyKeys = JsonConvert.DeserializeObject<bootyKeys>(row["bootyKeys"].ToString());
                        }

                        player.droneExp = Convert.ToInt32(row["droneExp"]);
                        player.positionInitializacion = JsonConvert.DeserializeObject<positionInitializacion>(row["position"].ToString());
                        player.currentMapName = player.GetMapName(player.positionInitializacion.mapID.ToString());
                    }

                    var settings = mySqlClient.ExecuteQueryTable($"SELECT * FROM player_settings WHERE userId = {playerId}");
                    foreach (DataRow row in settings.Rows)
                    {
                        if (row["audio"].ToString() != "")
                            player.Settings.Audio = JsonConvert.DeserializeObject<AudioBase>(row["audio"].ToString());
                        if (row["quality"].ToString() != "")
                            player.Settings.Quality = JsonConvert.DeserializeObject<QualityBase>(row["quality"].ToString());
                        if (row["classY2T"].ToString() != "")
                            player.Settings.ClassY2T = JsonConvert.DeserializeObject<ClassY2TBase>(row["classY2T"].ToString());
                        if (row["display"].ToString() != "")
                            player.Settings.Display = JsonConvert.DeserializeObject<DisplayBase>(row["display"].ToString());
                        if (row["gameplay"].ToString() != "")
                            player.Settings.Gameplay = JsonConvert.DeserializeObject<GameplayBase>(row["gameplay"].ToString());
                        if (row["window"].ToString() != "")
                        {
                            player.Settings.Window = JsonConvert.DeserializeObject<WindowBase>(row["window"].ToString());
                            WindowBase wb = new WindowBase();
                            foreach(KeyValuePair<string, Window> w in wb.windows)
                            {
                                bool windowExist = false;

                                foreach(KeyValuePair<string, Window> w1 in player.Settings.Window.windows)
                                {
                                    if (w.Key == w1.Key) windowExist = true;
                                }

                                if(!windowExist)
                                {
                                    player.Settings.Window.windows.Add(w.Key, w.Value);
                                }
                            }
                        }
                        if (row["inGameSettings"].ToString() != "")
                            player.Settings.InGameSettings = JsonConvert.DeserializeObject<InGameSettingsBase>(row["inGameSettings"].ToString());
                        if (row["cooldowns"].ToString() != "")
                            player.Settings.Cooldowns = JsonConvert.DeserializeObject<Dictionary<string, string>>(row["cooldowns"].ToString());
                        if (row["boundKeys"].ToString() != "")
                            player.Settings.BoundKeys = JsonConvert.DeserializeObject<List<BoundKeysBase>>(row["boundKeys"].ToString());
                        if (row["slotbarItems"].ToString() != "")
                            player.Settings.SlotBarItems = JsonConvert.DeserializeObject<Dictionary<short, string>>(row["slotbarItems"].ToString());
                        if (row["premiumSlotbarItems"].ToString() != "")
                            player.Settings.PremiumSlotBarItems = JsonConvert.DeserializeObject<Dictionary<short, string>>(row["premiumSlotbarItems"].ToString());
                        if (row["proActionBarItems"].ToString() != "")
                            player.Settings.ProActionBarItems = JsonConvert.DeserializeObject<Dictionary<short, string>>(row["proActionBarItems"].ToString());
                    }


                    var equipment = mySqlClient.ExecuteQueryTable($"SELECT * FROM player_equipment WHERE userId = {playerId}");
                    Dictionary<short, List<BoosterBase>> BoostersList = new Dictionary<short, List<BoosterBase>>();
                    List<ModuleBase> BattleStationModules = new List<ModuleBase>();
                    foreach (DataRow row in equipment.Rows)
                    {
                        string booster = row["boosters"].ToString();

                        BoostersList = JsonConvert.DeserializeObject<Dictionary<short, List<BoosterBase>>>(booster);
                        List<BoosterBase> lHON = new List<BoosterBase>();
                        List<BoosterBase> lDMG = new List<BoosterBase>();
                        List<BoosterBase> lSHD = new List<BoosterBase>();
                        List<BoosterBase> lHP = new List<BoosterBase>();
                        List<BoosterBase> lEP = new List<BoosterBase>();
                        //short  DICTIONARY = EP[0],HONOR[1],DAMAGE[2],SHIELD[3],REPAIR[4],HP[7]
                        //SHORT BOOSTER IMPORTANT = DMG[1] HON[5] SHD[15] HP[8]
                        string boos = row["boostersList"].ToString();

                        short HON = 1;
                        short DMG = 2;
                        short SHD = 3;
                        short HP = 7;
                        short EP = 0;
                        int seconds = 20000;
                        BoosterBase bostHON = new BoosterBase(5, seconds);
                        BoosterBase bostDMG = new BoosterBase(1, seconds);
                        BoosterBase bostSHD = new BoosterBase(15, seconds);
                        BoosterBase bostHP = new BoosterBase(8, seconds);
                        BoosterBase bostEP = new BoosterBase(2, seconds);

                        if (boos.Contains(HON.ToString()))  //SHORT DICTIONARY
                        {
                            if (!BoostersList.ContainsKey(HON))
                            {
                                lHON.Add(bostHON); //SHORT BOOSTER AND TIME
                                BoostersList.Add(HON, lHON); //SHORT DICTIONARY   

                            }
                            else
                            {
                                int pos = 0;
                                bool agregado = false;
                                bool boosPremium = false;
                                foreach (var item in BoostersList[HON])
                                {
                                    if (BoostersList[HON][pos].Type == 6)
                                    {
                                        boosPremium = true;
                                        break;
                                    }
                                    if (BoostersList[HON][pos].Type == bostHON.Type)
                                    {
                                        BoostersList[HON].ElementAt(pos).Seconds += seconds;
                                        agregado = true;
                                    }
                                    else pos++;
                                }

                                if (!agregado && !boosPremium) BoostersList[HON].Add(bostHON);

                            }
                        }
                        if (boos.Contains(DMG.ToString()))  //SHORT DICTIONARY
                        {
                            if (!BoostersList.ContainsKey(DMG))
                            {
                                lDMG.Add(bostDMG); //SHORT BOOSTER AND TIME
                                BoostersList.Add(DMG, lDMG); //SHORT DICTIONARY    

                            }
                            else
                            {
                                int pos = 0;
                                bool agregado = false;
                                bool boosPremium = false;
                                foreach (var item in BoostersList[DMG])
                                {
                                    if (BoostersList[DMG][pos].Type == 0)
                                    {
                                        boosPremium = true;
                                        break;
                                    }
                                    if (BoostersList[DMG][pos].Type == bostDMG.Type)
                                    {
                                        BoostersList[DMG].ElementAt(pos).Seconds += seconds;
                                        agregado = true;
                                    }
                                    else pos++;
                                }
                                if (!agregado && !boosPremium) BoostersList[DMG].Add(bostDMG);

                            }
                        }
                        if (boos.Contains(SHD.ToString()))  //SHORT DICTIONARY
                        {
                            if (!BoostersList.ContainsKey(SHD))
                            {
                                lSHD.Add(bostSHD); //SHORT BOOSTER AND TIME
                                BoostersList.Add(SHD, lSHD); //SHORT DICTIONARY  

                            }
                            else
                            {
                                int pos = 0;
                                bool agregado = false;
                                bool boosPremium = false;
                                foreach (var item in BoostersList[SHD])
                                {
                                    if (BoostersList[SHD][pos].Type == 16)
                                    {
                                        boosPremium = true;
                                        break;
                                    }
                                    if (BoostersList[SHD][pos].Type == bostSHD.Type)
                                    {
                                        BoostersList[SHD].ElementAt(pos).Seconds += seconds;
                                        agregado = true;
                                    }
                                    else pos++;
                                }
                                if (!agregado && !boosPremium) BoostersList[SHD].Add(bostSHD);

                            }
                        }

                        if (boos.Contains(EP.ToString()))  //SHORT DICTIONARY
                        {
                            if (!BoostersList.ContainsKey(EP))
                            {
                                lEP.Add(bostEP); //SHORT BOOSTER AND TIME
                                BoostersList.Add(EP, lEP); //SHORT DICTIONARY

                            }
                            else
                            {
                                int pos = 0;
                                bool agregado = false;
                                bool boosPremium = false;
                                foreach (var item in BoostersList[EP])
                                {
                                    if (BoostersList[EP][pos].Type == 3)
                                    {
                                        boosPremium = true;
                                        break;
                                    }
                                    if (BoostersList[EP][pos].Type == bostEP.Type)
                                    {
                                        BoostersList[EP].ElementAt(pos).Seconds += seconds;
                                        agregado = true;
                                    }
                                    else pos++;
                                }

                                if (!agregado && !boosPremium) BoostersList[EP].Add(bostEP);

                            }
                        }

                        if (boos.Contains(HP.ToString()))  //SHORT DICTIONARY
                        {
                            if (!BoostersList.ContainsKey(HP))
                            {
                                lHP.Add(bostHP); //SHORT BOOSTER AND TIME
                                BoostersList.Add(HP, lHP); //SHORT DICTIONARY   

                            }
                            else
                            {
                                int pos = 0;
                                bool agregado = false;
                                bool boosPremium = false;
                                //VERIFICA SI YA TIENE EL BOOSTER Y SUMA LOS SEGUNDOS
                                foreach (var item in BoostersList[HP])
                                {
                                    if (BoostersList[HP][pos].Type == 9)
                                        boosPremium = true;
                                    break;
                                }
                                if (BoostersList[HP][pos].Type == bostHP.Type)
                                {
                                    BoostersList[HP].ElementAt(pos).Seconds += seconds;
                                    agregado = true;
                                }

                                else pos++;

                                if (!agregado && !boosPremium) BoostersList[HP].Add(bostHP); // SI NO TIENE EL BOOSTER LO AGREGA

                            }
                        }


                        string mod = row["modulesList"].ToString();


                        const short HULL = 2;
                        const short DEFLECTOR = 3;
                        const short REPAIR = 4;
                        const short LASER_HIGH_RANGE = 5;
                        const short LASER_MID_RANGE = 6;
                        const short LASER_LOW_RANGE = 7;
                        const short ROCKET_MID_ACCURACY = 8;
                        const short ROCKET_LOW_ACCURACY = 9;
                        const short HONOR_BOOSTER = 10;
                        const short DAMAGE_BOOSTER = 11;
                        const short EXPERIENCE_BOOSTER = 12;
                        BattleStationModules = JsonConvert.DeserializeObject<List<ModuleBase>>(row["modules"].ToString());

                        for (int i = 0; i < mod.Length; i++)
                        {
                            if (mod.Contains(HULL.ToString()))  //SHORT DICTIONARY
                                BattleStationModules.Add(new ModuleBase(HULL, HULL, false));
                            if (mod.Contains(DEFLECTOR.ToString()))  //SHORT DICTIONARY
                                BattleStationModules.Add(new ModuleBase(DEFLECTOR, DEFLECTOR, false));
                            if (mod.Contains(LASER_HIGH_RANGE.ToString()))  //SHORT DICTIONARY
                                BattleStationModules.Add(new ModuleBase(LASER_HIGH_RANGE, LASER_HIGH_RANGE, false));
                            if (mod.Contains(ROCKET_MID_ACCURACY.ToString()))  //SHORT DICTIONARY
                                BattleStationModules.Add(new ModuleBase(ROCKET_MID_ACCURACY, ROCKET_MID_ACCURACY, false));

                        }

                        player.SkillTree = JsonConvert.DeserializeObject<SkillTreeBase>(row["skill_points"].ToString());
                        if (player.SkillTree.bountyhunter2 == 3)
                        {
                            player.bountyHunterAquired = true;
                        }
                        if(player.SkillTree.shieldmechanics == 5)
                        {
                            player.shieldmechanics = true;
                        }

                        dynamic items = JsonConvert.DeserializeObject(row["items"].ToString());

                        if (items["pet"] == "true")
                        {
                            player.Pet = new Pet(player);
                            player.Pet.Fuel = (items["fuel"] != null) ? items["fuel"] : 5000;
                            player.Pet.GUARD = (items["GUARD"] != null) ? items["GUARD"] : false;
                            player.Pet.KAMIKAZE = (items["KAMIKAZE"] != null) ? items["KAMIKAZE"] : false;
                            player.Pet.COMBO_SHIP_REPAIR = (items["COMBO_SHIP_REPAIR"] != null) ? items["COMBO_SHIP_REPAIR"] : false;
                            player.Pet.REPAIR_PET = (items["REPAIR_PET"] != null) ? items["REPAIR_PET"] : false;
                            player.Pet.AUTO_LOOT = (items["AUTO_LOOT"] != null) ? items["AUTO_LOOT"] : false;
                        }

                        // Formaciones Drones.

                        dynamic formations;

                        if (row["formationsSaved"] == "")
                        {
                            formations = JsonConvert.DeserializeObject("{}".ToString());
                        }
                        /*else
                        {
                            formations = JsonConvert.DeserializeObject(row["formationsSaved"].ToString());
                        }

                       // player.DEFAULT_FORMATION = DroneManager.DEFAULT_FORMATION;
                        player.ARROW_FORMATION = (formations[DroneManager.ARROW_FORMATION] != null && formations[DroneManager.ARROW_FORMATION] != "") ? DroneManager.ARROW_FORMATION : "";
                        player.BARRAGE_FORMATION = (formations[DroneManager.BARRAGE_FORMATION] != null && formations[DroneManager.BARRAGE_FORMATION] != "") ? DroneManager.BARRAGE_FORMATION : "";
                        player.BAT_FORMATION = (formations[DroneManager.BAT_FORMATION] != null && formations[DroneManager.BAT_FORMATION] != "") ? DroneManager.BAT_FORMATION : "";
                        player.CHEVRON_FORMATION = (formations[DroneManager.CHEVRON_FORMATION] != null && formations[DroneManager.CHEVRON_FORMATION] != "") ? DroneManager.CHEVRON_FORMATION : "";
                        player.CRAB_FORMATION = (formations[DroneManager.CRAB_FORMATION] != null && formations[DroneManager.CRAB_FORMATION] != "") ? DroneManager.CRAB_FORMATION : "";
                        player.DIAMOND_FORMATION = (formations[DroneManager.DIAMOND_FORMATION] != null && formations[DroneManager.DIAMOND_FORMATION] != "") ? DroneManager.DIAMOND_FORMATION : "";
                        player.DOME_FORMATION = (formations[DroneManager.DOME_FORMATION] != null && formations[DroneManager.DOME_FORMATION] != "") ? DroneManager.DOME_FORMATION : "";
                        player.DOUBLE_ARROW_FORMATION = (formations[DroneManager.DOUBLE_ARROW_FORMATION] != null && formations[DroneManager.DOUBLE_ARROW_FORMATION] != "") ? DroneManager.DOUBLE_ARROW_FORMATION : "";
                        player.DRILL_FORMATION = (formations[DroneManager.DRILL_FORMATION] != null && formations[DroneManager.DRILL_FORMATION] != "") ? DroneManager.DRILL_FORMATION : "";
                        player.HEART_FORMATION = (formations[DroneManager.HEART_FORMATION] != null && formations[DroneManager.HEART_FORMATION] != "") ? DroneManager.HEART_FORMATION : "";
                        player.LANCE_FORMATION = (formations[DroneManager.LANCE_FORMATION] != null && formations[DroneManager.LANCE_FORMATION] != "") ? DroneManager.LANCE_FORMATION : "";
                        player.MOTH_FORMATION = (formations[DroneManager.MOTH_FORMATION] != null && formations[DroneManager.MOTH_FORMATION] != "") ? DroneManager.MOTH_FORMATION : "";
                        player.PINCER_FORMATION = (formations[DroneManager.PINCER_FORMATION] != null && formations[DroneManager.PINCER_FORMATION] != "") ? DroneManager.PINCER_FORMATION : "";
                        player.RING_FORMATION = (formations[DroneManager.RING_FORMATION] != null && formations[DroneManager.RING_FORMATION] != "") ? DroneManager.RING_FORMATION : "";
                        player.STAR_FORMATION = (formations[DroneManager.STAR_FORMATION] != null && formations[DroneManager.STAR_FORMATION] != "") ? DroneManager.STAR_FORMATION : "";
                        player.TURTLE_FORMATION = (formations[DroneManager.TURTLE_FORMATION] != null && formations[DroneManager.TURTLE_FORMATION] != "") ? DroneManager.TURTLE_FORMATION : "";
                        player.VETERAN_FORMATION = (formations[DroneManager.VETERAN_FORMATION] != null && formations[DroneManager.VETERAN_FORMATION] != "") ? DroneManager.VETERAN_FORMATION : "";
                        player.WAVE_FORMATION = (formations[DroneManager.WAVE_FORMATION] != null && formations[DroneManager.WAVE_FORMATION] != "") ? DroneManager.WAVE_FORMATION : "";
                        player.WHEEL_FORMATION = (formations[DroneManager.WHEEL_FORMATION] != null && formations[DroneManager.WHEEL_FORMATION] != "") ? DroneManager.WHEEL_FORMATION : "";
                        player.X_FORMATION = (formations[DroneManager.X_FORMATION] != null && formations[DroneManager.X_FORMATION] != "") ? DroneManager.X_FORMATION : "";
                        */
                        // Final Formaciones Drones.

                        var event_coins = mySqlClient.ExecuteQueryTable($"SELECT * FROM event_coins WHERE userId = {playerId}");

                        if (event_coins.Rows.Count > 0)
                        {
                            player.ec = Convert.ToInt32(event_coins.Rows[0]["coins"]);
                        }
                        else
                        {
                            mySqlClient.ExecuteNonQuery($"INSERT INTO event_coins (coins, userId) VALUES ({0}, {playerId})");
                            player.ec = 0;
                        }

                    }
                    player.BoosterManager.Boosters = BoostersList;
                    player.Storage.BattleStationModules = BattleStationModules;
                    SavePlayer.Boosters(player);
                    SavePlayer.Modules(player);
                    SavePlayer.BoostersBUY(player);
                    SavePlayer.ModulesBUY(player);
                    SavePlayer.Items(player);
                    SavePlayer.saveEC(player);
                    SavePlayer.formations(player);

                    player.StartSync();
                }

                SetEquipment(player);

                return player;
            }
            catch (Exception e)
            {
                Logger.Log("error_log", $"- [QueryManager.cs] GetPlayer({playerId}) exception: {e}");
                return null;
            }
        }

        public static void SetEquipment(Player player)
        {
            try
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var r = mySqlClient.ExecuteQueryRow($"SELECT * FROM player_equipment WHERE userId = {player.Id}");

                    var bo3Shield = 6950 + Maths.GetPercentage(30000, (int)r["B03lvl"]);
                    var bo2Shield = 12000; //+ Maths.GetPercentage(30000, (int)r["B02lvl"]);
                    var B01Shield = 11400;// + Maths.GetPercentage(30000, (int)r["B01lvl"]);
                    var A03Shield = 6000;// + Maths.GetPercentage(30000, (int)r["A03lvl"]);
                    var A02Shield = 2400;// + Maths.GetPercentage(30000, (int)r["A02lvl"]);
                    var A01Shield = 1200;// + Maths.GetPercentage(30000, (int)r["A01lvl"]);

                    var g3n1010Speed = 2;
                    var g3n2010Speed = 3;
                    var g3n3210Speed = 4;
                    var g3n3310Speed = 5;
                    var g3n6900Speed = 7;
                    var g3n7900Speed = 13;

                    player.lf5lvl = (int)r["lf5lvl"];


                    var lf1Damage = 80;// + Maths.GetPercentage(100, (int)r["lf1lvl"]);
                    var lf2Damage = 155;// + Maths.GetPercentage(100, (int)r["lf2lvl"]);
                    var lf3Damage = 190;// + Maths.GetPercentage(500, (int)r["lf3lvl"]);
                    var lf4Damage = 90;// + Maths.GetPercentage(500, (int)r["lf4lvl"]);
                    var lf5Damage = 400;// + Maths.GetPercentage(600, (int)r["lf5lvl"]);



                    var hitpoints = new int[] { player.Ship.BaseHitpoints, player.Ship.BaseHitpoints };
                    var speed = new int[] { player.Ship.BaseSpeed, player.Ship.BaseSpeed };
                    var damage = new int[] { 0, 0 };
                    var shield = new int[] { 0, 0 };

                    var equipment = mySqlClient.ExecuteQueryTable($"SELECT * FROM player_equipment WHERE userId = {player.Id}");
                    int lasers = 0;
                    int lf5 = 0;
                    int lf5lasers = 0;

                    foreach (DataRow row in equipment.Rows)
                    {
                        dynamic items = JsonConvert.DeserializeObject(row["items"].ToString());

                        for (var i = 1; i <= 2; i++)
                        {
                            foreach (int itemId in (dynamic)JsonConvert.DeserializeObject(row[$"config{i}_lasers"].ToString()))
                            {
                                if (itemId >= 0 && itemId < 40) //lf3
                                {

                                    damage[i - 1] += lf3Damage;
                                    player.lf4lasers++;
                                    player.lasers++;
                                    lasers++;
                                }

                                else if (itemId >= 140 && itemId < 180) //lf4
                                {
                                    damage[i - 1] += lf4Damage;
                                    player.lf4lasers++;
                                    player.lasers++;
                                    lasers++;

                                }
                                else if (itemId >= 180 && itemId < 220) //lf2
                                {
                                    damage[i - 1] += lf2Damage;
                                    player.lf4lasers++;
                                    player.lasers++;
                                    lasers++;

                                }
                                else if (itemId >= 220 && itemId < 260) //lf1
                                {
                                    damage[i - 1] += lf1Damage;
                                    player.lf4lasers++;
                                    player.lasers++;
                                    lasers++;

                                }
                                else if (itemId >= 260 && itemId < 300) //lf5
                                {
                                    damage[i - 1] += lf5Damage;
                                    lasers++;
                                    lf5++;
                                    lf5lasers++;
                                    player.lasers++;
                                }
                            }

                            foreach (int itemId in (dynamic)JsonConvert.DeserializeObject(row[$"config{i}_generators"].ToString()))
                            {
                                if (itemId >= 40 && itemId < 100)
                                {
                                    shield[i - 1] += bo2Shield;
                                }
                                else if (itemId >= 460 && itemId < 500)
                                {
                                    shield[i - 1] += bo3Shield;
                                }
                                else if (itemId >= 420 && itemId < 460)
                                {
                                    shield[i - 1] += B01Shield;
                                }
                                else if (itemId >= 380 && itemId < 420)
                                {
                                    shield[i - 1] += A03Shield;
                                }
                                else if (itemId >= 340 && itemId < 380)
                                {
                                    shield[i - 1] += A02Shield;
                                }
                                else if (itemId >= 300 && itemId < 340)
                                {
                                    shield[i - 1] += A01Shield;
                                }
                                else if (itemId >= 100 && itemId < 120)
                                {
                                    speed[i - 1] += g3n7900Speed;
                                }
                                else if (itemId >= 500 && itemId < 520)
                                {
                                    speed[i - 1] += g3n1010Speed;
                                }
                                else if (itemId >= 520 && itemId < 540)
                                {
                                    speed[i - 1] += g3n2010Speed;
                                }
                                else if (itemId >= 540 && itemId < 560)
                                {
                                    speed[i - 1] += g3n3210Speed;
                                }
                                else if (itemId >= 560 && itemId < 580)
                                {
                                    speed[i - 1] += g3n3310Speed;
                                }
                                else if (itemId >= 580 && itemId < 600)
                                {
                                    speed[i - 1] += g3n6900Speed;
                                }
                            }

                            var havocCount = 0;
                            var herculesCount = 0;
                            var spartanCount = 0;
                            var drones = (dynamic)JsonConvert.DeserializeObject(row[$"config{i}_drones"].ToString());

                            foreach (var drone in drones)
                            {
                                var herculesEquipped = false;

                                foreach (int design in drone["designs"])
                                {
                                    if (design >= 120 && design < 130)
                                        havocCount++;
                                    else if (design >= 130 && design < 140)
                                    {
                                        herculesEquipped = true;
                                        herculesCount++;
                                    }
                                    else if (design >= 600 && design < 610)
                                    {
                                        //spartanEquipped = true;
                                        spartanCount++;
                                    }
                                }

                                var droneShield3 = bo3Shield ;
                                var droneShield2 = bo2Shield ;
                                var droneShield1 = B01Shield ;

                                var droneShield4 = A01Shield ;
                                var droneShield5 = A02Shield ;
                                var droneShield6 = A03Shield ;

                                

                                foreach (int item in drone["items"])
                                {
                                    if (item >= 220 && item < 260)
                                    {
                                        damage[i - 1] += lf1Damage + 5;
                                        lasers++;
                                        player.lasers++;
                                    }
                                    else if (item >= 180 && item < 220)
                                    {
                                        if(player.droneExp >= 500 && player.droneExp < 800)
                                        {
                                            damage[i - 1] += lf2Damage + Maths.GetPercentage(lf2Damage, 2);
                                            lasers++;
                                            player.lasers++;
                                        }else if (player.droneExp >= 800 && player.droneExp < 1200)
                                        {
                                            damage[i - 1] += lf2Damage + Maths.GetPercentage(lf2Damage, 4);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 1200 && player.droneExp < 1700)
                                        {
                                            damage[i - 1] += lf2Damage + Maths.GetPercentage(lf2Damage, 6);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 1700 && player.droneExp < 2500)
                                        {
                                            damage[i - 1] += lf2Damage + Maths.GetPercentage(lf2Damage, 8);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 2500)
                                        {
                                            damage[i - 1] += lf2Damage + Maths.GetPercentage(lf2Damage, 10);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else
                                        {
                                            damage[i - 1] += lf2Damage;
                                            lasers++;
                                            player.lasers++;
                                        }
                                    }
                                    else if (item >= 0 && item < 40)
                                    {
                                        if (player.droneExp >= 500 && player.droneExp < 800)
                                        {
                                            damage[i - 1] += lf3Damage + Maths.GetPercentage(lf3Damage, 2);
                                            lasers++;
                                            player.lasers++;
                                        }else if(player.droneExp >= 800 && player.droneExp < 1200)
                                        {
                                            damage[i - 1] += lf3Damage + Maths.GetPercentage(lf3Damage, 4);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 1200 && player.droneExp < 1700)
                                        {
                                            damage[i - 1] += lf3Damage + Maths.GetPercentage(lf3Damage, 6);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 1700 && player.droneExp < 2500)
                                        {
                                            damage[i - 1] += lf3Damage + Maths.GetPercentage(lf3Damage, 8);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 2500)
                                        {
                                            damage[i - 1] += lf3Damage + Maths.GetPercentage(lf3Damage, 10);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else
                                        {
                                            damage[i - 1] += lf3Damage;
                                            lasers++;
                                            player.lasers++;
                                        }
                                    }
                                    else if (item >= 140 && item < 180)
                                    {
                                        if (player.droneExp >= 500 && player.droneExp < 800)
                                        {
                                            damage[i - 1] += lf4Damage + Maths.GetPercentage(lf4Damage, 2);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 800 && player.droneExp < 1200) {
                                            damage[i - 1] += lf4Damage + Maths.GetPercentage(lf4Damage, 4);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 1200 && player.droneExp < 1700)
                                        {
                                            damage[i - 1] += lf4Damage + Maths.GetPercentage(lf4Damage, 6);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 1700 && player.droneExp < 2500)
                                        {
                                            damage[i - 1] += lf4Damage + Maths.GetPercentage(lf4Damage, 8);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else if (player.droneExp >= 2500)
                                        {
                                            damage[i - 1] += lf4Damage + Maths.GetPercentage(lf4Damage, 10);
                                            lasers++;
                                            player.lasers++;
                                        }
                                        else
                                        {
                                            damage[i - 1] += lf4Damage;
                                            lasers++;
                                            player.lasers++;
                                        }
                                    }
                                    else if (item >= 260 && item < 300)
                                    {
                                        damage[i - 1] += lf5Damage + Maths.GetPercentage(lf5Damage, 10);
                                        lf5++;
                                        lasers++;
                                        player.lasers++;
                                    }


                                    else if (item >= 300 && item < 340)
                                        shield[i - 1] += droneShield4 + (herculesEquipped ? +Maths.GetPercentage(droneShield4, 15) : +Maths.GetPercentage(droneShield4, 20));
                                    else if (item >= 340 && item < 380)
                                        shield[i - 1] += droneShield5 + (herculesEquipped ? +Maths.GetPercentage(droneShield5, 15) : +Maths.GetPercentage(droneShield5, 20));
                                    else if (item >= 380 && item < 420)
                                        shield[i - 1] += droneShield6 + (herculesEquipped ? +Maths.GetPercentage(droneShield6, 15) : +Maths.GetPercentage(droneShield6, 20));
                                    else if (item >= 420 && item < 460)
                                        shield[i - 1] += droneShield1 + (herculesEquipped ? +Maths.GetPercentage(droneShield1, 15) : +Maths.GetPercentage(droneShield1, 20));
                                    else if (item >= 40 && item < 100)
                                        shield[i - 1] += droneShield2 + (herculesEquipped ? +Maths.GetPercentage(droneShield2, 15) : +Maths.GetPercentage(droneShield2, 20));
                                    else if (item >= 460 && item < 500)
                                        shield[i - 1] += droneShield3 + (herculesEquipped ? +Maths.GetPercentage(droneShield3, 15) : +Maths.GetPercentage(droneShield3, 20));

                                }
                            }

                            if (havocCount == drones.Count)
                            {
                                damage[i - 1] += Maths.GetPercentage(damage[i - 1], 10);
                                hitpoints[i - 1] += Maths.GetPercentage(hitpoints[i - 1], 15);
                            }
                            else if (herculesCount == 10)
                            {
                                hitpoints[i - 1] += Maths.GetPercentage(hitpoints[i - 1], 20);
                                shield[i - 1] += Maths.GetPercentage(shield[i - 1], 15);
                            }
                            else if (spartanCount == 10)
                            {
                                hitpoints[i - 1] += Maths.GetPercentage(hitpoints[i - 1], 35);
                                damage[i - 1] += Maths.GetPercentage(damage[i - 1], 20);
                                shield[i - 1] += Maths.GetPercentage(shield[i - 1], 15);
                            }
                        }

                        //speed[0] += Maths.GetPercentage(speed[0], 20);
                        //speed[1] += Maths.GetPercentage(speed[1], 20);

                        var configsBase = new ConfigsBase(hitpoints[0], damage[0], shield[0], speed[0], hitpoints[1], damage[1], shield[1], speed[1]);
                        var itemsBase = new bootyKeys();

                        player.Equipment = new EquipmentBase(configsBase, itemsBase);
                        player.lf5Damage = lf5Damage;
                        player.lf5lasers = lf5lasers;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("error_log", $"- [QueryManager.cs] SetEquipment({player.Id}) exception: {e}");
            }
        }

        public static void LoadMaps()
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable("SELECT * FROM server_maps");
                foreach (DataRow row in data.Rows)
                {
                    int mapId = Convert.ToInt32(row["mapID"]);
                    string name = Convert.ToString(row["name"]);
                    int factionId = Convert.ToInt32(row["factionID"]);
                    var npcs = JsonConvert.DeserializeObject<List<NpcsBase>>(row["npcs"].ToString());
                    var portals = JsonConvert.DeserializeObject<List<PortalBase>>(row["portals"].ToString());
                    var stations = JsonConvert.DeserializeObject<List<StationBase>>(row["stations"].ToString());
                    var options = JsonConvert.DeserializeObject<OptionsBase>(row["options"].ToString());
                    var spacemap = new Spacemap(mapId, name, factionId, npcs, portals, stations, options);
                    GameManager.Spacemaps.TryAdd(spacemap.Id, spacemap);
                }
            }

            LoadBattleStations();
        }

        public static void LoadPremiumMaps()
        {

            var sessionMapId18 = Int32.Parse("620" + 18);
            var sessionMapId22 = Int32.Parse("620" + 22);
            var sessionMapId26 = Int32.Parse("620" + 26);

            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable("SELECT * FROM server_maps WHERE mapID = 18");
                foreach (DataRow row in data.Rows)
                {
                    int mapId18 = Convert.ToInt32(row["mapID"]);
                    string name18 = Convert.ToString("1-6 Premium Map");
                    int factionId18 = Convert.ToInt32(row["factionID"]);
                    var npcs18 = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                    var portals18 = JsonConvert.DeserializeObject<List<PortalBase>>(row["portals"].ToString());
                    var stations18 = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                    var options18 = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                    var spacemap18 = new Spacemap(mapId18, name18, factionId18, npcs18, portals18, stations18, options18);
                    GameManager.Spacemaps.TryAdd(sessionMapId18, spacemap18);
                }
            }

            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable("SELECT * FROM server_maps WHERE mapID = 22");
                foreach (DataRow row in data.Rows)
                {
                    int mapId22 = Convert.ToInt32(row["mapID"]);
                    string name22 = Convert.ToString("2-6 Premium Map");
                    int factionId22 = Convert.ToInt32(row["factionID"]);
                    var npcs22 = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                    var portals22 = JsonConvert.DeserializeObject<List<PortalBase>>(row["portals"].ToString());
                    var stations22 = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                    var options22 = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                    var spacemap22 = new Spacemap(mapId22, name22, factionId22, npcs22, portals22, stations22, options22);
                    GameManager.Spacemaps.TryAdd(sessionMapId22, spacemap22);
                }
            }

            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable("SELECT * FROM server_maps WHERE mapID = 26");
                foreach (DataRow row in data.Rows)
                {
                    int mapId26 = Convert.ToInt32(row["mapID"]);
                    string name26 = Convert.ToString("3-6 Premium Map");
                    int factionId26 = Convert.ToInt32(row["factionID"]);
                    var npcs26 = JsonConvert.DeserializeObject<List<NpcsBase>>("".ToString());
                    var portals26 = JsonConvert.DeserializeObject<List<PortalBase>>(row["portals"].ToString());
                    var stations26 = JsonConvert.DeserializeObject<List<StationBase>>("".ToString());
                    var options26 = JsonConvert.DeserializeObject<OptionsBase>("{'StarterMap':false,'PvpMap':false,'RangeDisabled':true,'CloakBlocked':true,'LogoutBlocked':true,'DeathLocationRepair':false}".ToString());
                    var spacemap26 = new Spacemap(mapId26, name26, factionId26, npcs26, portals26, stations26, options26);
                    GameManager.Spacemaps.TryAdd(sessionMapId26, spacemap26);
                }
            }

        }

        public class BattleStations
        {
            public static void BattleStation(BattleStation battleStation)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var visualModifiers = new List<int>();

                    foreach (var modifier in battleStation.VisualModifiers.Keys)
                        visualModifiers.Add(modifier);

                    var buildTime = battleStation.AssetTypeId != AssetTypeModule.BATTLESTATION && battleStation.InBuildingState ? $"buildTime = '{battleStation.buildTime.ToString("yyyy-MM-dd HH:mm:ss")}'," : "";
                    //var deflectorTime = !battleStation.DeflectorActive ? $"deflectorTime = '{battleStation.deflectorTime.ToString("yyyy-MM-dd HH:mm:ss")}'," : "";
                    var deflectorTime = $"deflectorTime = '{battleStation.deflectorTime.ToString("yyyy-MM-dd HH:mm:ss")}',";

                    mySqlClient.ExecuteNonQuery($"UPDATE server_battlestations SET clanId = {battleStation.Clan.Id}," +
                    $"inBuildingState = {battleStation.InBuildingState}, buildTimeInMinutes = {battleStation.BuildTimeInMinutes}, {buildTime}" +
                    $"deflectorActive = {battleStation.DeflectorActive}, deflectorSecondsLeft = {battleStation.DeflectorSecondsLeft}, {deflectorTime} visualModifiers = '{JsonConvert.SerializeObject(visualModifiers)}', protectedClanId = '{battleStation.Clan.Id}' WHERE id = '{battleStation.idBSS}'");

                }
            }

            public static void Modules(BattleStation battleStation)
            {
                var modules = new List<EquippedModuleBase>();

                foreach (var equipped in battleStation.EquippedStationModule)
                {
                    var module = new List<SatelliteBase>();

                    foreach (var equippedModule in battleStation.EquippedStationModule[equipped.Key])
                    {
                        module.Add(new SatelliteBase(equippedModule.OwnerId, equippedModule.ItemId, equippedModule.SlotId, equippedModule.DesignId, equippedModule.Type, equippedModule.CurrentHitPoints,
                            equippedModule.MaxHitPoints, equippedModule.CurrentShieldPoints, equippedModule.MaxShieldPoints, equippedModule.InstallationSecondsLeft, equippedModule.Installed));
                    }

                    modules.Add(new EquippedModuleBase(equipped.Key, module));
                }

                using (var mySqlClient = SqlDatabaseManager.GetClient())
                    mySqlClient.ExecuteNonQuery($"UPDATE server_battlestations SET modules = '{JsonConvert.SerializeObject(modules)}' WHERE id = '{battleStation.idBSS}'");
            }
        }

        public static void LoadBattleStations()
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable("SELECT * FROM server_battlestations");
                foreach (DataRow row in data.Rows)
                {
                    bool active = Convert.ToBoolean(row["active"]);

                    if (active)
                    {
                        int idBS = Convert.ToInt32(row["id"]);
                        string name = Convert.ToString(row["name"]);
                        int mapId = Convert.ToInt32(row["mapId"]);
                        int clanId = Convert.ToInt32(row["clanId"]);
                        int positionX = Convert.ToInt32(row["positionX"]);
                        int positionY = Convert.ToInt32(row["positionY"]);
                        var modules = JsonConvert.DeserializeObject<List<EquippedModuleBase>>(row["modules"].ToString());
                        var inBuildingState = Convert.ToBoolean(Convert.ToInt32(row["inBuildingState"]));
                        var buildTimeInMinutes = Convert.ToInt32(row["buildTimeInMinutes"]);
                        var buildTime = DateTime.Parse(row["buildTime"].ToString());
                        var deflectorActive = Convert.ToBoolean(Convert.ToInt32(row["deflectorActive"]));
                        var deflectorSecondsLeft = Convert.ToInt32(row["deflectorSecondsLeft"]);
                        var deflectorTime = DateTime.Parse(row["deflectorTime"].ToString());
                        var visualModifiers = JsonConvert.DeserializeObject<List<int>>(row["visualModifiers"].ToString());
                        int protectedClanId = Convert.ToInt32(row["protectedClanId"]);

                        var battleStation = new BattleStation(name, GameManager.GetSpacemap(mapId), new  Darkorbit.Game.Movements.Position(positionX, positionY), GameManager.GetClan(clanId), modules, inBuildingState, buildTimeInMinutes, buildTime, deflectorActive, deflectorSecondsLeft, deflectorTime, visualModifiers, protectedClanId, idBS);
                        GameManager.BattleStations.TryAdd(battleStation.Name, battleStation);
                    }
                }
            }
        }

        public static void LoadShips()
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable("SELECT * FROM server_ships");
                foreach (DataRow row in data.Rows)
                {
                    string name = Convert.ToString(row["name"]);
                    int shipID = Convert.ToInt32(row["shipID"]);
                    int damage = Convert.ToInt32(row["damage"]);
                    int shields = Convert.ToInt32(row["shield"]);
                    int hitpoints = Convert.ToInt32(row["health"]);
                    int speed = Convert.ToInt32(row["speed"]);
                    int lasers = Convert.ToInt32(row["lasers"]);
                    string lootID = Convert.ToString(row["lootID"]);
                    bool aggressive = Convert.ToBoolean(row["aggressive"]);
                    bool respawnable = Convert.ToBoolean(row["respawnable"]);
                    var rewards = JsonConvert.DeserializeObject<ShipRewards>(row["reward"].ToString());
                    var cargo = Convert.ToInt32(row["cargo"]);

                    var ship = new Ship(name, shipID, hitpoints, shields, speed, lootID, damage, aggressive, respawnable, rewards, lasers);
                    GameManager.Ships.TryAdd(ship.Id, ship);
                }
            }
        }

        public static void LoadClans()
        {
            GameManager.Clans.TryAdd(0, new Clan(0, "", "", 0));
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable("SELECT * FROM server_clans");
                foreach (DataRow row in data.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    string name = Convert.ToString(row["name"]);
                    string tag = Convert.ToString(row["tag"]);
                    int factionId = Convert.ToInt32(row["factionId"]);

                    var clan = new Clan(id, name, tag, factionId);
                    GameManager.Clans.TryAdd(clan.Id, clan);
                    LoadClanDiplomacy(clan);
                }
            }
        }

        private static void LoadClanDiplomacy(Clan clan)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT * FROM server_clan_diplomacy WHERE senderClanId = {clan.Id}");
                foreach (DataRow row in data.Rows)
                {
                    int id = Convert.ToInt32(row["toClanId"]);
                    Diplomacy relation = (Diplomacy)Convert.ToInt32(row["diplomacyType"]);
                    clan.DiplomaciesSender.Add(id, relation);
                }

                var data2 = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT * FROM server_clan_diplomacy WHERE toClanId = {clan.Id}");
                foreach (DataRow row in data2.Rows)
                {
                    int id = Convert.ToInt32(row["senderClanId"]);
                    Diplomacy relation = (Diplomacy)Convert.ToInt32(row["diplomacyType"]);
                    clan.DiplomaciesTo.Add(id, relation);
                }
            }
        }

        public static int GetChatPermission(int userId)
        {
            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var data = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT * FROM chat_permissions WHERE userId = {userId}");
                foreach (DataRow row in data.Rows)
                {
                    return Convert.ToInt32(row["type"]);
                }
                return 0;
            }
        }

       

    }
}

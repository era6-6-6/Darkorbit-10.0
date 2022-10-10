global using Darkorbit.Game.Movements;
global using Darkorbit.Managers.MySQLManager;
global using Darkorbit.Net.netty.commands;
global using Darkorbit.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Collectables
{
    class GreenBooty1 : Collectable
    {
        public GreenBooty1(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {

            var uridium = Randoms.random.Next(100, 2500);
            var credits = Randoms.random.Next(55000, 80000);
           // uridium += Maths.GetPercentage(uridium, player.GetSkillPercentage("Luck"));
            int ran = Randoms.random.Next(1, 100);
            int ran2 = Randoms.random.Next(1, 100);
            player.LoadData();
            if (ran <= 30 && ran >= 0)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SMB_01, Randoms.random.Next(1, 3));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_500, Randoms.random.Next(900, 1500));

            }
            else if (ran <= 50 && ran > 30)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ISH_01, Randoms.random.Next(1, 3));
            }
            else if (ran <= 65 && ran > 50)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.EMP_01, Randoms.random.Next(1, 3));
            }
            else if (ran <= 70 && ran > 66)
            {
                player.ChangeData(DataType.URIDIUM, uridium);
            }
            else if (ran <= 72 && ran > 70)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                    foreach (DataRow row in equipment.Rows)
                    {
                        var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                        int LF4 = (int)items.lf4Count;
                        if (LF4 == 40)
                        {
                            player.ChangeData(DataType.URIDIUM, uridium);
                            player.ChangeData(DataType.CREDITS, credits);
                        }
                        else
                        {
                            LF4++;
                            items.lf4Count = LF4;
                            mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                            player.SendPacket("0|A|STD|You received 1 LF4 Laser Cannon");
                        }
                    }
                }
            }
            else if (ran <= 88 && ran > 72)
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.ChangeData(DataType.CREDITS, credits);
            }
            else if (ran <= 100 && ran > 89)
            {

                if (ran2 >= 2 && ran2 <= 50)
                {
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                        foreach (DataRow row in equipment.Rows)
                        {
                            var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                            int ZeusParts = (int)items.droneZeusParts;
                            ZeusParts++;
                            items.droneZeusParts = ZeusParts;
                            mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                            player.SendPacket("0|A|STD|You received 1 Zeus Part");
                        }

                    }
                }
                else if (ran2 <= 100 && ran2 >= 51)
                {
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                        foreach (DataRow row in equipment.Rows)
                        {
                            var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                            int ApisParts = (int)items.droneApisParts;
                            ApisParts++;
                            items.droneApisParts = ApisParts;
                            mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                            player.SendPacket("0|A|STD|You received 1 Apis Part");
                        }

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
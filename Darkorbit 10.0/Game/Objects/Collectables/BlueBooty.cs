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
    class BlueBooty : Collectable
    {
        public BlueBooty(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {

            var uridium = Randoms.random.Next(100, 500);
            var credits = Randoms.random.Next(550000, 8000000);
            // uridium += Maths.GetPercentage(uridium, player.GetSkillPercentage("Luck"));
            int ran = Randoms.random.Next(1, 100);
            int ran2 = Randoms.random.Next(1, 100);

            player.LoadData();

            if (ran <= 30 && ran >= 0)
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SMB_01, Randoms.random.Next(1, 3));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_500, Randoms.random.Next(900, 1500));

            }
            else if (ran <= 50 && ran > 30)
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ISH_01, Randoms.random.Next(1, 3));
            }
            else if (ran <= 65 && ran > 50)
            {
                player.ChangeData(DataType.URIDIUM, uridium);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.EMP_01, Randoms.random.Next(1, 3));
            }
            else if (ran <= 94 && ran > 65)
            {
                player.ChangeData(DataType.URIDIUM, uridium);

            }
            else if (ran <= 100 && ran > 94)
            {

                if (ran2 >= 2 && ran2 <= 50)
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
                else if (ran2 <= 100 && ran2 >= 51)
                {
                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                        foreach (DataRow row in equipment.Rows)
                        {
                            var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                            int B03 = (int)items.bo3Count;
                            B03++;
                            items.bo3Count = B03;
                            mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                            player.SendPacket("0|A|STD|You received 1 B03 Shield Generator");
                        }

                    }
                }
            }
                //player.Equipment.Items.greenKeys--;
                player.bootyKeys.blueKeys--;

            player.SendPacket($"0|A|BKB|{player.bootyKeys.blueKeys}");

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("PIRATE_BOOTY_BLUE", Hash, Position.Y, Position.X);
        }
    }
}
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
    class GoldBooty : Collectable
    {
        public GoldBooty(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
            var uridium = Randoms.random.Next(25, 110);
            var credits = Randoms.random.Next(100, 1000);
            var uridium1 = Randoms.random.Next(185, 360);
            var credits1 = Randoms.random.Next(10000, 100000);

            var ran = Randoms.random.Next(1, 100);
            // QueryManager.SavePlayer.Information(player);
            player.LoadData();
            if (ran >= 0 && ran < 1)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {

                    var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                    foreach (DataRow row in equipment.Rows)
                    {
                        var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                        int LF2 = (int)items.lf2Count;
                        LF2++;
                        items.lf2Count = LF2;
                        mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                        player.SendPacket("0|A|STD|You received 1 LF-2 Laser Cannon");
                    }

                }
            }
            else if (ran > 1 && ran < 35)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.LCB_10, Randoms.random.Next(1100, 5400));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(150, 600));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(130, 220));
            }
            else if (ran > 35 && ran <= 44)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAB_50, Randoms.random.Next(140, 240));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(60, 100));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(40, 120));
            }
            else if (ran > 44 && ran <= 61)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(140, 200));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2026, Randoms.random.Next(120, 200));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2021, Randoms.random.Next(20, 80));
            }
            else if (ran > 61 && ran <= 93)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_3030, Randoms.random.Next(20, 60));
                player.ChangeData(DataType.URIDIUM, uridium);
                player.ChangeData(DataType.CREDITS, credits);
            }
            else if (ran > 93 && ran <= 100)
            {
                player.ChangeData(DataType.URIDIUM, uridium1);
                player.ChangeData(DataType.CREDITS, credits1);
            }
        }
        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("PIRATE_BOOTY_GOLD", Hash, Position.Y, Position.X);
        }
    }
}

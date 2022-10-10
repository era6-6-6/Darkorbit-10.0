using Newtonsoft.Json;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Collectables
{
    class SilverBooty : Collectable
    {
        public SilverBooty(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
            var uridium = Randoms.random.Next(25, 100);
            var credits = Randoms.random.Next(100, 10000);
            var uridium1 = Randoms.random.Next(85, 250);
            var credits1 = Randoms.random.Next(10000, 81000);

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
                        items.lf5Count = LF2;
                        mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                        player.SendPacket("0|A|STD|You received 1 LF-2 Laser Cannon");
                    }

                }
            }
            else if (ran > 1 && ran < 33)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.LCB_10, Randoms.random.Next(1100, 4400));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(50, 400));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(30, 180));
            }
            else if (ran > 33 && ran <= 41)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAB_50, Randoms.random.Next(40, 190));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(40, 120));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(20, 80));
            }
            else if (ran > 41 && ran <= 49)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_3030, Randoms.random.Next(20, 40));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(40, 100));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2026, Randoms.random.Next(60, 100));
            }
            else if (ran > 49 && ran <= 81)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2021, Randoms.random.Next(20, 40));
                player.ChangeData(DataType.URIDIUM, uridium);
                player.ChangeData(DataType.CREDITS, credits);
            }
            else if (ran > 81 && ran <= 95)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(50, 140));
                player.ChangeData(DataType.URIDIUM, uridium1);
                player.ChangeData(DataType.CREDITS, credits1);
            }
            else if (ran > 95 && ran <= 100)
            {
                var hours = Randoms.random.NextDouble() <= 0.1 ? 1 : 1;
                var boosterTypes = new int[] { 15, 0, 2, 5, 10, 8 };
                var boosterType = boosterTypes[Randoms.random.Next(boosterTypes.Length)];

                player.BoosterManager.Add((BoosterType)boosterType, hours);
            }
        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("PIRATE_BOOTY_SILVER", Hash, Position.Y, Position.X);
        }
    }
}

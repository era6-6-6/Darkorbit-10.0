
using Newtonsoft.Json;

using System.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Collectables
{
    class SuperIceBox : Collectable
    {
        public SuperIceBox(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
            int experience = 0;
            int honor = 0;
            int uridium = 0;
            int credits = 0;
            int uridium1 = 0;
            int credits1 = 0;
            player.LoadData();
            //experience = player.Ship.GetExperienceBoost(Randoms.random.Next(2500, 12800));
            //honor = player.Ship.GetHonorBoost(Randoms.random.Next(100, 520));
            uridium = Randoms.random.Next(50, 300);
            credits = Randoms.random.Next(200, 2000);
            uridium1 = Randoms.random.Next(600, 1600);
            credits1 = Randoms.random.Next(40000, 150000);

            //player.ChangeData(DataType.EXPERIENCE, experience);
            //player.ChangeData(DataType.HONOR, honor);
        
            player.ChangeData(DataType.URIDIUM, uridium);
            player.ChangeData(DataType.CREDITS, credits);

            var ran = Randoms.random.Next(1, 100);
            // QueryManager.SavePlayer.Information(player);

            if (ran >= 0 && ran < 1)
            {
                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    var equipment = mySqlClient.ExecuteQueryTable($"SELECT items FROM player_equipment WHERE userId = {player.Id}");

                    foreach (DataRow row in equipment.Rows)
                    {
                        var items = JsonConvert.DeserializeObject<dynamic>(row["items"].ToString());
                        int LF3 = (int)items.lf3Count;
                        if (LF3 >= 35)
                        {
                            player.ChangeData(DataType.URIDIUM, uridium);
                        }
                        else
                        {
                            LF3++;
                            items.lf5Count = LF3;
                            mySqlClient.ExecuteQueryTable($"UPDATE player_equipment SET items = '{JsonConvert.SerializeObject(items)}' WHERE userId = {player.Id}");
                            player.SendPacket("0|A|STD|You received 1 LF-3 Laser Cannon");
                        }
                    }

                }
            }
            else if (ran > 1 && ran < 35)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.LCB_10, Randoms.random.Next(2200, 10800));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(300, 1000));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(660, 1040));
            }
            else if (ran > 35 && ran <= 43)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAB_50, Randoms.random.Next(480, 800));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(200, 400));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(180, 300));
            }
            else if (ran > 43 && ran <= 51)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(280, 500));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2026, Randoms.random.Next(80, 200));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2021, Randoms.random.Next(240, 600));
            }
            else if (ran > 51 && ran <= 83)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_3030, Randoms.random.Next(80, 260));
                player.ChangeData(DataType.URIDIUM, uridium);
                player.ChangeData(DataType.CREDITS, credits);
            }
            else if (ran > 83 && ran <= 100)
            {
                player.ChangeData(DataType.URIDIUM, uridium1);
                player.ChangeData(DataType.CREDITS, credits1);
            }

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("TREASURE", Hash, Position.Y, Position.X);
        }
    }
}

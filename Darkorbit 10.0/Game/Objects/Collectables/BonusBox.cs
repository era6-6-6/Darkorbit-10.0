
using Darkorbit.Game.Movements;
using Darkorbit.Net.netty.commands;
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Collectables
{
    class BonusBox : Collectable
    {
        public BonusBox(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_BONUS_BOX, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        
        {
            player.LoadData();
            if (player.Spacemap.Id == 42)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, 1000);
               
            }
            else
            {
                var uridium = Randoms.random.Next(20, 200);
                var credits = Randoms.random.Next(100, 1500);
                var uridium1 = Randoms.random.Next(125, 720);
                uridium += Maths.GetPercentage(uridium, player.GetSkillPercentage("Luck"));
                int ran = Randoms.random.Next(1, 100);

                if (ran <= 22 && ran >= 0)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.LCB_10, (int)(Randoms.random.Next(20, 150) * 1.5));
                    }
                    else
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.LCB_10, Randoms.random.Next(20, 150));

                }
                else if (ran <= 34 && ran > 22)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, (int)(Randoms.random.Next(20, 150) * 1.5));
                    }
                    else
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(20, 150));

                }
                else if (ran <= 42 && ran > 34)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, (int)(Randoms.random.Next(20, 150) * 1.5));
                    }
                    else
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(20, 150));

                }
                else if (ran <= 50 && ran > 42)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAB_50, (int)(Randoms.random.Next(20, 150) * 1.5));
                    }
                    else
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAB_50, Randoms.random.Next(20, 150));
                }
                else if (ran <= 65 && ran > 50)
                {

                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2026, (int)(Randoms.random.Next(2, 15) * 1.5));
                    }
                    else
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2026, Randoms.random.Next(2, 15));

                }
                else if (ran <= 67 && ran > 65)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2021, (int)(Randoms.random.Next(2, 15) * 1.5));
                    }
                    else
                        player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2021, Randoms.random.Next(2, 15));

                }
                else if (ran <= 77 && ran > 67)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.ChangeData(DataType.URIDIUM, (int)(uridium * 1.5));
                    }
                    else
                        player.ChangeData(DataType.URIDIUM, uridium);

                }
                else if (ran <= 97 && ran > 77)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.ChangeData(DataType.URIDIUM, (int)(uridium * 1.5));
                    }
                    else
                        player.ChangeData(DataType.URIDIUM, uridium);

                }
                else if (ran <= 100 && ran > 97)
                {
                    if (player.Spacemap.Id == 13 || Spacemap.Id == 14 || Spacemap.Id == 15)
                    {
                        player.ChangeData(DataType.URIDIUM, (int)(uridium1 * 1.5));
                    }
                    else
                        player.ChangeData(DataType.URIDIUM, uridium1);

                }

                
            }
            Dispose();

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("BONUS_BOX", Hash, Position.Y, Position.X);
        }
    }
}

using global::Darkorbit.Game.Movements;
using global::Darkorbit.Net.netty.commands;
using global::Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Collectables
{
    class gifbox : Collectable
    {
        public gifbox(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_GIFT_BOX, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
            var uridium = Randoms.random.Next(35, 125);
            var credits = Randoms.random.Next(300, 2000);
            var uridium1 = Randoms.random.Next(155, 330);
            //uridium += Maths.GetPercentage(uridium, player.GetSkillPercentage("Luck"));
            int ran = Randoms.random.Next(1, 100);
            player.LoadData();
            if (ran <= 10 && ran >= 0)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(50, 150));
            }
            else if (ran <= 20 && ran > 10)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(120, 180));
            }
            else if (ran <= 34 && ran > 20)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.HSTRM_01, Randoms.random.Next(4, 12));
            }
            else if (ran <= 37 && ran > 34)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(60, 150));
            }
            else if (ran <= 43 && ran > 37)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAB_50, Randoms.random.Next(70, 150));
            }
            else if (ran <= 48 && ran > 43)
            {
                player.ChangeData(DataType.URIDIUM, uridium);
            }
            else if (ran <= 50 && ran > 48)
            {
                player.ChangeData(DataType.URIDIUM, uridium1);
            }
            else if (ran <= 51 && ran > 50)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(70, 115));
            }
            else if (ran <= 70 && ran > 51)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(15, 25));
            }
            else if (ran <= 80 && ran > 70)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAR_02, Randoms.random.Next(5, 15));
            }
            else if (ran <= 90 && ran > 80)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(30, 50));
            }
            else if (ran <= 95 && ran > 90)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_3030, Randoms.random.Next(15, 25));
            }
  
            else if (ran <= 96 && ran > 95)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLD_8, Randoms.random.Next(1, 1));
            }
            else if (ran <= 97 && ran > 96)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.DCR_250, Randoms.random.Next(1, 1));
            }
            else if (ran <= 98 && ran > 97)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ISH_01, Randoms.random.Next(1, 1));
            }
            else if (ran <= 99 && ran > 98)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SMB_01, Randoms.random.Next(1, 1));
            }
            else if (ran <= 100 && ran > 99)
            {
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.EMP_01, Randoms.random.Next(1, 1));
            }

            Dispose();

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("GIFT_BOXES", Hash, Position.Y, Position.X);
        }
    }
}
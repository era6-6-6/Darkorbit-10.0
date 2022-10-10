
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game
{
    class AlienEgg : Collectable
    {
        public AlienEgg(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_ALIEN_EGG, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
            var uridium = Randoms.random.Next(25, 160);
            var credits = Randoms.random.Next(2500, 15200);
            //uridium += Maths.GetPercentage(uridium, player.GetSkillPercentage("Luck"));
            
            int ran = Randoms.random.Next(1, 100);

            if (ran <= 10 && ran >= 0)
            {
                player.AmmunitionManager.AddAmmo(AmmunitionManager.UCB_100, Randoms.random.Next(2, 12));
            }
            else if (ran <= 20 && ran > 10)
            {
                player.AmmunitionManager.AddAmmo(AmmunitionManager.SAB_50, Randoms.random.Next(1, 17));
            }
            else if (ran <= 30 && ran > 20)
            {
                player.AmmunitionManager.AddAmmo(AmmunitionManager.RSB_75, Randoms.random.Next(5, 18));
            }
            else if (ran <= 40 && ran > 30)
            {
                player.SendPacket($"0|A|STD|This box is empty.");
            }
            else if (ran <= 50 && ran > 40)
            {
                player.LoadData();
                player.ChangeData(DataType.CREDITS, credits);
            }
            else if (ran <= 60 && ran > 50)
            {
                player.AmmunitionManager.AddAmmo(AmmunitionManager.MCB_25, Randoms.random.Next(53, 218));
            }
            else if (ran <= 70 && ran > 60)
            {
                player.AmmunitionManager.AddAmmo(AmmunitionManager.LCB_10, Randoms.random.Next(100, 350));
            }
            else if (ran <= 80 && ran > 70)
            {
                player.AmmunitionManager.AddAmmo(AmmunitionManager.MCB_50, Randoms.random.Next(51, 124));
            }
            else if (ran <= 90 && ran > 80)
            {
                player.AmmunitionManager.AddAmmo(AmmunitionManager.LCB_10, Randoms.random.Next(100, 350));
            }
            else if (ran <= 100 && ran > 90)
            {
                player.LoadData();
                player.ChangeData(DataType.URIDIUM, uridium);
            }

            Dispose();

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("ALIEN_EGG", Hash, Position.Y, Position.X);
        }
    }
}

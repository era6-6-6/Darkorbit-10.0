
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game
{
    class Flower : Collectable
    {
        public Flower(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_FLOWER, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
            var credits = Randoms.random.Next(2500, 15200);
            //credits += Maths.GetPercentage(credits, player.GetSkillPercentage("Luck"));
            player.AmmunitionManager.AddAmmo(AmmunitionManager.LCB_10, Randoms.random.Next(100, 350));

            {
                player.LoadData();
                player.ChangeData(DataType.CREDITS, credits);
            }

            Dispose();

        }


        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("FLOWER", Hash, Position.Y, Position.X);
        }
    }
}

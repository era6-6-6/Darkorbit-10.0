
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game
{
    class FromShip : Collectable
    {
        public FromShip(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_FROM_SHIP, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {


            var credits = Randoms.random.Next(2500, 15200);
           // credits += Maths.GetPercentage(credits, player.GetSkillPercentage("Luck"));

         
            player.LoadData();
            player.ChangeData(DataType.CREDITS, credits);
         

            Dispose();

        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("FROM_SHIP", Hash, Position.Y, Position.X);
        }
    }
}

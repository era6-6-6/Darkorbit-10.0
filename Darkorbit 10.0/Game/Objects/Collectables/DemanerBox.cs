using Darkorbit.Game.Movements;
using Darkorbit.Net.netty.commands;
using Darkorbit.Utils;
using Newtonsoft.Json;
using System;

namespace Darkorbit.Game.Objects.Collectables
{
    class DemanerBox : Collectable
    {
        public DemanerBox(Position position, Spacemap spacemap, bool respawnable, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_PIRATE_BOOTY, position, spacemap, respawnable, toPlayer) { }

        public override void Reward(Player player)
        {
            int experience = 0;
            int honor = 0;
            int uridium = 0;
            int credits = 0;

            experience = player.Ship.GetExperienceBoost(Randoms.random.Next(2500, 100000));
            honor = player.Ship.GetHonorBoost(Randoms.random.Next(100, 15000));
            uridium = Randoms.random.Next(10000, 80500);
            credits = Randoms.random.Next(100, 25000);
            player.LoadData();
            player.ChangeData(DataType.EXPERIENCE, experience);
            player.ChangeData(DataType.HONOR, honor);
            player.ChangeData(DataType.URIDIUM, uridium);
            player.ChangeData(DataType.CREDITS, credits);
        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("DEMANER_BOX", Hash, Position.Y, Position.X);
        }
    }
}

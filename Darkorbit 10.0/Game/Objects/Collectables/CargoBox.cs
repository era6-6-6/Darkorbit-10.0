

using Darkorbit.Game.Movements;
using Darkorbit.Net.netty.commands;
using Darkorbit.Utils;

namespace Darkorbit.Game.Objects.Collectables
{
    class CargoBox : Collectable
    {
        public CargoBox(Position position, Spacemap spacemap, bool respawnable, bool spaceball, bool demaner, Player toPlayer = null) : base(AssetTypeModule.BOXTYPE_FROM_SHIP, position, spacemap, respawnable, toPlayer) { Spaceball = spaceball; Demander = demaner; }

        private bool Spaceball { get; set; }
        private bool Demander { get; set; }
        public override void Reward(Player player)
        {
            int experience = 0;
            int honor = 0;
            int uridium = 0;
            int credits = 0;
            int ec = 0;

            if (Spaceball)
            {
                experience = player.Ship.GetExperienceBoost(Randoms.random.Next(25000, 50000));
                honor = player.Ship.GetHonorBoost(Randoms.random.Next(500, 1000));
                uridium = Randoms.random.Next(1000, 1500);
                ec = Randoms.random.Next(0, 1);
                //credits = Randoms.random.Next(5000, 12000);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(1000, 3000));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(0, 3000));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(0, 700));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(0, 600));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ISH_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SMB_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.EMP_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.DCR_250, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLD_8, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.HSTRM_01, Randoms.random.Next(0, 20));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SAR_02, Randoms.random.Next(0, 20));
            }
            else if (Demander)
            {
                experience = player.Ship.GetExperienceBoost(Randoms.random.Next(100, 500));
                honor = player.Ship.GetHonorBoost(Randoms.random.Next(10, 80));
                uridium = Randoms.random.Next(100, 400);
                credits = Randoms.random.Next(5000, 12000);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.LCB_10, Randoms.random.Next(0, 5400));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(0, 300));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(0, 300));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(0, 250));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(0, 200));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ISH_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SMB_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.EMP_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2021, Randoms.random.Next(0, 60));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ECO_10, Randoms.random.Next(0, 60));
            }
            else
            {
                experience = player.Ship.GetExperienceBoost(Randoms.random.Next(100, 500));
                honor = player.Ship.GetHonorBoost(Randoms.random.Next(10, 80));
                uridium = Randoms.random.Next(100, 400);
                credits = Randoms.random.Next(5000, 12000);
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.LCB_10, Randoms.random.Next(0, 5400));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_25, Randoms.random.Next(0, 300));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.MCB_50, Randoms.random.Next(0, 300));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.UCB_100, Randoms.random.Next(0, 250));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.RSB_75, Randoms.random.Next(0, 200));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ISH_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.SMB_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.EMP_01, Randoms.random.Next(0, 2));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.PLT_2021, Randoms.random.Next(0, 60));
                player.AmmunitionManager.AddAmmo(Players.Managers.AmmunitionManager.ECO_10, Randoms.random.Next(0, 60));
            }
            player.LoadData();
            player.ChangeData(DataType.EXPERIENCE, experience);
            player.ChangeData(DataType.HONOR, honor);
            player.ChangeData(DataType.URIDIUM, uridium);
            player.ChangeData(DataType.CREDITS, credits);
            player.ChangeData(DataType.EC, ec);
        }

        public override byte[] GetCollectableCreateCommand()
        {
            return CreateBoxCommand.write("FROM_SHIP", Hash, Position.Y, Position.X);
        }
    }
}

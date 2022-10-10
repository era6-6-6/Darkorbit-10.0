using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class KillsceenRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new KillscreenRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            player.LoadData();
            switch (read.selection.typeValue)
            {
                case KillScreenOptionTypeModule.BASIC_REPAIR:
                    player.Respawn(true);
                    break;
                case KillScreenOptionTypeModule.BASIC_FULL_REPAIR:
                    player.Respawn(true, false, false, true);
                    break;
                case KillScreenOptionTypeModule.AT_DEATHLOCATION_REPAIR:
                    if (player.Data.uridium >= 2500 && player.Spacemap.Options.DeathLocationRepair)
                    {
                        player.ChangeData(DataType.URIDIUM, 2500, ChangeType.DECREASE);
                        player.Storage.KillscreenDeathLocationRepairTime = DateTime.Now;
                        player.Respawn(false, true, false,false);
                    }
                    break;
                case KillScreenOptionTypeModule.AT_JUMPGATE_REPAIR:
                    if (player.Data.uridium >= 1000)
                    {
                        player.ChangeData(DataType.URIDIUM, 1000, ChangeType.DECREASE);
                        player.Storage.KillscreenPortalRepairTime = DateTime.Now;
                        player.Respawn(false, false, true,false);
                    }
                    break;
            }
        }
    }
}

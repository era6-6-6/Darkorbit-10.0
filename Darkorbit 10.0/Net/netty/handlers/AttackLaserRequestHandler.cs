namespace Darkorbit.Net.netty.handlers
{
    class AttackLaserRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;

            if (player.Selected != null)
                player.EnableAttack(player.Settings.InGameSettings.selectedLaser);
        }
    }
}

namespace Darkorbit.Net.netty.handlers
{
    class LogoutCancelRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;
            player.AbortLogout();
        }
    }
}

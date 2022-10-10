namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupLeaveRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;
            player.Group?.Leave(gameSession.Player);
        }
    }
}

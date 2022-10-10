using Darkorbit.Net.netty.requests.GroupRequests;


namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupFollowPlayerRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupFollowPlayerRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var lockedPlayer = GameManager.GetPlayerById(read.userId);

            if (lockedPlayer == null) return;
            player.Group?.Follow(player, lockedPlayer);
        }
    }
}

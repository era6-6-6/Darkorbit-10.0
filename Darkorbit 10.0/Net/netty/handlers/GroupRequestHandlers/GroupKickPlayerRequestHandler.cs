using Darkorbit.Net.netty.requests.GroupRequests;

namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupKickPlayerRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupKickPlayerRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var kickedPlayer = GameManager.GetPlayerById(read.userId);

            if (kickedPlayer == null) return;
            player.Group?.Kick(kickedPlayer);
        }
    }
}



using Darkorbit.Net.netty.requests.GroupRequests;

namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupPingPositionRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupPingPositionRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;

            player.Group?.Ping(new Position(read.x, read.y));
        }
    }
}

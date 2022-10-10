using Darkorbit.Net.netty.requests.GroupRequests;

namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupPingPlayerRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupPingPlayerRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var pingedPlayer = GameManager.GetPlayerById(read.userId);

            if (pingedPlayer == null) return;
            if (pingedPlayer.Spacemap != player.Spacemap) return;
            player.Group?.Ping(pingedPlayer.Position);
        }
    }
}

using Darkorbit.Net.netty.requests.GroupRequests;

namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupChangeLeaderRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupChangeLeaderRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            player.Group.ChangeLeader(GameManager.GetPlayerById(read.userId));
        }
    }
}

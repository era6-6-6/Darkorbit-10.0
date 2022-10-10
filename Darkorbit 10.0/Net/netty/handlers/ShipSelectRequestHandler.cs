
using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class ShipSelectRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new ShipSelectRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            player.SelectEntity(read.targetID);
        }
    }
}

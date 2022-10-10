

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class MoveRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new MoveRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var position = new Position(read.targetX, read.targetY);
            Movement.Move(player, position);
        }
    }
}

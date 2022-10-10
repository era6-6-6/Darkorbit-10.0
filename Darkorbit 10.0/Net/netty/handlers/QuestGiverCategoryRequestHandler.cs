

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class QuestGiverCategoryRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new QuestGiverCategoryRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;

            //TODO
        }
    }
}

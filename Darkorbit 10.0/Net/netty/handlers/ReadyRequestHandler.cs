

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class ReadyRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new ReadyRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;

            switch (read.readyType)
            {
                case ReadyRequest.MAP_LOADED:
                    
                    break;
                case ReadyRequest.HERO_LOADED:
                    
                    break;
            }
        }
    }
}

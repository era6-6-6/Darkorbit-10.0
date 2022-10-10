

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class UIOpenRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new UIOpenRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            switch (read.itemId)
            {
                case UIOpenRequest.ACTION_LOGOUT:
                    player.Logout(true);
                    break;
                case UIOpenRequest.ACTION_SHIP_WARP:
                    //gemi değiştirme ekranı gönder
                    break;
            }
        }
    }
}

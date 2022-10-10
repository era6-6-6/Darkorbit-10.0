using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class ProActionBarRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new ProActionBarRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var displaySettings = player.Settings.Display;

            displaySettings.proActionBarOpened = read.opened;

            QueryManager.SavePlayer.Settings(player, "display", player.Settings.Display);
        }
    }
}

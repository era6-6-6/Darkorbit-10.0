namespace Darkorbit.Net.netty.handlers
{
    class SendWindowUpdateRequestHandler : IHandler
    {
        private static List<string> alwaysNotMaximized = new List<string>()
        {
            "logout",
            "settings"
        };

        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new requests.SendWindowUpdateRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var windowSettings = player.Settings.Window;

            if (windowSettings.windows.ContainsKey(read.itemId))
            {
                windowSettings.windows[read.itemId].height = read.height;
                windowSettings.windows[read.itemId].width = read.width;
                windowSettings.windows[read.itemId].x = read.x;
                windowSettings.windows[read.itemId].y = read.y;

                if (!alwaysNotMaximized.Contains(read.itemId))
                    windowSettings.windows[read.itemId].maximixed = read.maximized;

                QueryManager.SavePlayer.Settings(player, "window", windowSettings);
            }
        }
    }
}

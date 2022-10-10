using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class WindowSettingsRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new WindowSettingsRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var windowSettings = player.Settings.Window;

            windowSettings.barState = read.barStatesAsString;
            windowSettings.hideAllWindows = read.hideAllWindows;
            windowSettings.scale = read.scaleFactor;
            windowSettings.gameFeatureBarLayoutType = read.gameFeatureBarLayoutType;
            windowSettings.gameFeatureBarPosition = read.gameFeatureBarPosition;
            windowSettings.genericFeatureBarLayoutType = read.genericFeatureBarLayoutType;
            windowSettings.genericFeatureBarPosition = read.genericFeatureBarPosition;
            windowSettings.categoryBarPosition = read.categoryBarPosition;
            windowSettings.standartSlotBarLayoutType = read.standartSlotBarLayoutType;
            windowSettings.standartSlotBarPosition = read.standartSlotBarPosition;
            if (read.proActionBarLayoutType != "")
            {
                windowSettings.proActionBarLayoutType = read.proActionBarLayoutType;
            }
            if (read.proActionBarPosition != "")
            {
                windowSettings.proActionBarPosition = read.proActionBarPosition;
            }
            if (read.premiumSlotBarLayoutType != "")
            {
                windowSettings.premiumSlotBarLayoutType = read.premiumSlotBarLayoutType;
            }
            if (read.premiumSlotBarPosition != "")
            {
                windowSettings.premiumSlotBarPosition = read.premiumSlotBarPosition;
            }

            QueryManager.SavePlayer.Settings(player, "window", windowSettings);
        }
    }
}

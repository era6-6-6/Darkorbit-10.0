using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class SelectMenuBarItemHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new SelectMenuBarItemRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            player.SettingsManager.UseSlotBarItem(read.itemId);
        }
    }
}

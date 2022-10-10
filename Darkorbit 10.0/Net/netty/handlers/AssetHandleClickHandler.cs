

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class AssetHandleClickHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new AssetHandleClickRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var activatable = player.Spacemap.GetActivatableMapEntity(read.AssetId);

            if (activatable != null && !(activatable is Portal))
                activatable.Click(gameSession);
        }
    }
}

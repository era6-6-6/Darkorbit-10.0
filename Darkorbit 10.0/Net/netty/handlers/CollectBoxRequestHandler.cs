using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class CollectBoxRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new CollectBoxRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;

            var obj = player.Spacemap.Objects.Values.Where(x => x is Collectable collectable && collectable.Hash == read.hash).FirstOrDefault();
            if (obj != null)
                (obj as Collectable).Collect(player);
        }
    }
}

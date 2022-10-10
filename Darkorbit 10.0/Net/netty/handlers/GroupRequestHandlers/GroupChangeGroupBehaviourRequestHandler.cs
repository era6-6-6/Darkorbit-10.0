namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupChangeGroupBehaviourRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;
            player.Group?.ChangeBehavior(player);
        }
    }
}

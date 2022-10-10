namespace Darkorbit.Net.netty.handlers
{
    class AttackRocketRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;
            player.AttackManager.RocketAttack();
        }
    }
}

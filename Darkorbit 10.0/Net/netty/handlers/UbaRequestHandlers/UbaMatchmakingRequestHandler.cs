using Darkorbit.Game.Events;

namespace Darkorbit.Net.netty.handlers.UbaRequestHandlers
{
    class UbaMatchmakingRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;
            if (EventManager.JackpotBattle.InEvent(player) || Duel.InDuel(player)) return;
            if(player.Storage.ubal != null)
            {
                player.SendPacket("0|A|STD|You are already in an UBA lobby.");
                return;
            }
            EventManager.UltimateBattleArena.AddWaitingPlayer(player);
        }
    }
}

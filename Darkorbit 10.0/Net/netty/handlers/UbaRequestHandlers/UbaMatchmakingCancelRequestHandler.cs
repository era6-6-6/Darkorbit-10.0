using Darkorbit.Game.Events;

namespace Darkorbit.Net.netty.handlers.UbaRequestHandlers
{
    class UbaMatchmakingCancelRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;
            if (EventManager.JackpotBattle.InEvent(player) || Duel.InDuel(player)) return;
            if(player.Storage.ubal != null)
            {
                UltimateBattleArenaLobby tmp = player.Storage.ubal;
                foreach(Player p in tmp.players)
                {
                    EventManager.UltimateBattleArena.RemoveWaitingPlayer(p);
                }
                EventManager.UltimateBattleArena.DeleteUbaLobby(player);
            } else
            {
                EventManager.UltimateBattleArena.RemoveWaitingPlayer(player);
            }

        }
    }
}

namespace Darkorbit.Net.netty.handlers
{
    class AttackAbortLaserRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;

            if(player.Selected != null)
            {
                player.DisableAttack(player.Settings.InGameSettings.selectedLaser);
                //player.SendPacket("0|A|STM|attstop|%!|" + ((player.Selected is Player && EventManager.JackpotBattle.Active && EventManager.JackpotBattle.InEvent(player)) ? EventManager.JackpotBattle.Name : player.Selected.Name));
            }
        }
    }
}

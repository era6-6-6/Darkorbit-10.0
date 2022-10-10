namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupUpdateBlockInvitationStateRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;

            player.Settings.InGameSettings.blockedGroupInvites = player.Settings.InGameSettings.blockedGroupInvites ? false : true;
            player.SendCommand(GroupUpdateBlockInvitationState.write(player.Settings.InGameSettings.blockedGroupInvites));
            QueryManager.SavePlayer.Settings(player, "inGameSettings", player.Settings.InGameSettings);
        }
    }
}

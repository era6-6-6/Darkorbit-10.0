using Darkorbit.Net.netty.requests.GroupRequests;

namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupRevokeInvitationRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupRevokeInvitationRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var invitedSession = GameManager.GetGameSession(read.userId);

            if (invitedSession == null) return;
            if (invitedSession.Player.Storage.GroupInvites.ContainsKey(player.Id))
            {
                invitedSession.Player.Storage.GroupInvites.Remove(player.Id);
                invitedSession.Player.SendCommand(GroupRemoveInvitationCommand.write(player.Id, invitedSession.Player.Id, GroupRemoveInvitationCommand.REVOKE));
            }
        }
    }
}

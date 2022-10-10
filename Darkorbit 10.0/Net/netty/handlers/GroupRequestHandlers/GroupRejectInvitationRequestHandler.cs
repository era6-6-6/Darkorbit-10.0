using Darkorbit.Net.netty.requests.GroupRequests;


namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupRejectInvitationRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupRejectInvitationRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var inviterSession = GameManager.GetGameSession(read.userId);

            if (inviterSession == null) return;
            if (player.Storage.GroupInvites.ContainsKey(inviterSession.Player.Id))
            {
                player.Storage.GroupInvites.Remove(inviterSession.Player.Id);
                inviterSession.Player.SendCommand(GroupRemoveInvitationCommand.write(inviterSession.Player.Id, player.Id, GroupRemoveInvitationCommand.REJECT));
            }
        }
    }
}

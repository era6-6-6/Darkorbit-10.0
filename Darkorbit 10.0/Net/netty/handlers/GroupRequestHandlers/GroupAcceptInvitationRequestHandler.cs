using Darkorbit.Net.netty.handlers;
using Darkorbit.Net.netty.requests.GroupRequests;


namespace Darkorbit.Net.netty.handlers.GroupRequestHandlers
{
    class GroupAcceptInvitationRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GroupAcceptInvitationRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var inviterPlayer = GameManager.GetPlayerById(read.userId);

            AssembleAcceptedInvitation(player, inviterPlayer);
        }

        public void AssembleAcceptedInvitation(Player player, Player inviterPlayer)
        {
            if (inviterPlayer == null || !player.Storage.GroupInvites.ContainsKey(inviterPlayer.Id))
            {
                player.SendPacket("0|A|STM|msg_grp_inv_err_inviter_nonexistant");
                return;
            }
            if (inviterPlayer.Group == null)
            {
                new Group(inviterPlayer, player);
            }
            else if (inviterPlayer.Group.Members.Count < Group.DEFAULT_MAX_GROUP_SIZE)
            {
                inviterPlayer.Group.Accept(inviterPlayer, player);
            }
            player.Storage.GroupInvites.Clear();
        }
    }
}

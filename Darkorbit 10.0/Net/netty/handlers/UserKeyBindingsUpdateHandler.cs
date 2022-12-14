using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class UserKeyBindingsUpdateHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {           
            try
            {
                var read = new UserKeyBindingsUpdateRequest();
                read.readCommand(bytes);

                var player = gameSession.Player;
                var keys = player.Settings.BoundKeys;
                keys.Clear();

                foreach (var action in read.changedKeyBindings)
                    keys.Add(new BoundKeysBase(action.actionType, (short)action.charCode, action.parameter, action.keyCodes));

                QueryManager.SavePlayer.Settings(player, "boundKeys", keys);
            }
            catch (Exception e)
            {
                Logger.Log("error_log", $"- [UserKeyBindingsUpdateHandler.cs] execute void exception: {e}");
            }
        }
    }
}

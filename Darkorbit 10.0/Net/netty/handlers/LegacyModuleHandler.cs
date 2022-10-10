

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class LegacyModuleHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new LegacyModuleRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            string[] packet = read.message.Split('|');
            switch (packet[0])
            {
                case ServerCommands.SET_STATUS:
                    switch (packet[1])
                    {
                        case ClientCommands.CONFIGURATION:
                            player.ChangeConfiguration(packet[2]);
                            break;
                    }
                    break;
            }
        }
    }
}

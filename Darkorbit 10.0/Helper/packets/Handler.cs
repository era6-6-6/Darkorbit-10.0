
using Darkorbit.Helper.packets.handlers;
using Darkorbit.Helper.packets.requests;
using Discord;
using Darkorbit.Net.netty.handlers;

namespace Darkorbit.Helper.packets
{
    class Handler
    {
        private readonly Dictionary<string, Handler> HandledCommands = new Dictionary<string, Handler>();

        public Handler()
        {
            LoadCommands();
        }

        public void LoadCommands()
        {
            
        }

        public void Handle(IDiscordClient client, string packet)
        {
          
        }
    }
}

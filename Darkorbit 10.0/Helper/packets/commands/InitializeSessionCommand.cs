using Darkorbit.Helper.objects;
using System.Diagnostics;

namespace Darkorbit.Helper.packets.commands
{
    class InitializeSessionCommand : ICommand
    {
        public override string Prefix => "init";

        public InitializeSessionCommand(List<ConnectedPlayer> connectedPlayers)
        {
            AddParam(Process.GetCurrentProcess().Id.ToString());
            AddParam(JsonConvert.SerializeObject(connectedPlayers));
        }
    }
}

namespace Darkorbit.Helper.packets.commands
{
    class PongCommand : ICommand
    {
        public override string Prefix => "pong";

        public PongCommand()
        {
            AddParam("Pong!");
        }
    }
}

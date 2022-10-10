
using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers.PetRequestHandlers
{
    class PetGearActivationRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new PetGearActivationRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            if (player.Pet == null) return;

            player.Pet.SwitchGear(read.gearId);
        }
    }
}

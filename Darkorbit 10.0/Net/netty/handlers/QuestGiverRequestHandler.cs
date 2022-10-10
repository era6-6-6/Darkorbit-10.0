

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class QuestGiverRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new QuestGiverRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;

            Console.WriteLine("init_sending");

            //player.SendCommand(InitQuestList.write(new List<InitQuestItem>(), true, 0, 0));

            Console.WriteLine("sended");
        }
    }
}

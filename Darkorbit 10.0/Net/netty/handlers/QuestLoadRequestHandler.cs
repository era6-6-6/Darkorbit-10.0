using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class QuestLoadRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new QuestLoadRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;

            Console.WriteLine("test: "+read.bS);

            List<InitQuestIcon> p2 = new List<InitQuestIcon>();
            InitQuestLoadItem_Dx p3 = new InitQuestLoadItem_Dx(0, true, true, true, 0, new List<InitQuestLoadItem_a2R>());
            List<InitQuestLoadItem_Y39> p4 = new List<InitQuestLoadItem_Y39>();
            List<InitQuestType> p5 = new List<InitQuestType>();

            InitQuestLoadItem_uZ tmp = new InitQuestLoadItem_uZ(871, p2, p3, p4, p5, "", "");
            //quest accept
            //InitQuestLoadItem_D2s tmp1 = new InitQuestLoadItem_D2s(tmp);
            List<InitQuestLoadItem_Z1M> tmp2 = new List<InitQuestLoadItem_Z1M>();
            tmp2.Add(new InitQuestLoadItem_Z1M("", "", 0, 0, 0));
            InitQuestLoadItem_TJ tmp1 = new InitQuestLoadItem_TJ(tmp, tmp2, new InitQuestLoadItem_Z1M("", "", 0, 0, 0));

            player.SendCommand(tmp1.write());
        }
    }
}

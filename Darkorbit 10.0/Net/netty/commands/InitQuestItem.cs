using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestItem
    {
        public const short ID = 31291;

        public byte[] write(int paramx1, int param2, int param3, int param4, List<InitQuestIcon> param5, InitQuestType param6, InitQuestCategory param7, List<InitQuestLevel> param8, string param9, string param10)
        {
            var param1 = new ByteArray(ID);
            //questicon
            param1.write(param6.write(0));
            //param10
            param1.writeUTF("");
            //param9
            param1.writeUTF("");
            //param5 quest category
            param5 = new List<InitQuestIcon>();
            param5.Add(new InitQuestIcon());
            param1.writeInt(param5.Count);
            foreach (var _loc2_ in param5)
            {
                param1.write(_loc2_.write(0));
            }
            //param2 rootCaseId
            param1.writeInt(0 >> 13 | 0 << 19);
            //param4 priority
            param1.writeInt(0 << 16 | 0 >> 16);
            param1.writeShort(5563);
            //paramx1 questid
            param1.writeInt(871 >> 10 | 871 << 22);
            param8 = new List<InitQuestLevel>();
            param8.Add(new InitQuestLevel());
            param1.writeInt(param8.Count);
            foreach (var _loc2_ in param8)
            {
                param1.write(_loc2_.write(3, 0, 0, new List<InitQuestLevelMinMax>(), new List<InitQuestLevelMinMax>()));
            }
            //queststate
            param1.write(param7.write(0));
            //param3 minLevel
            param1.writeInt(0 >> 7 | 0 << 25);
            return param1.Message.ToArray();
        }
    }
}

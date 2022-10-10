using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestList
    {
        public const short ID = 16203;

        public static byte[] write(List<InitQuestItem> paramx1, bool param2, int param3, int param4)
        {
            var param1 = new ByteArray(ID);
            //param4 ???
            param1.writeInt(0 >> 10 | 0 << 22);
            //param3 quest slot count
            param1.writeInt(1 << 6 | 1 >> 26);
            //param2 show only starterquest tab or not
            param1.writeBoolean(false);
            paramx1.Add(new InitQuestItem());
            param1.writeInt(paramx1.Count);
            //questlist
            foreach (var _loc2_ in paramx1)
            {
                param1.write(_loc2_.write(0, 0, 0, 0, new List<InitQuestIcon>(), new InitQuestType(), new InitQuestCategory(), new List<InitQuestLevel>(), "", ""));
            }
            return param1.ToByteArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLevel
    {
        public const short ID = 29001;

        public byte[] write(short paramx1, int param2, int param3, List<InitQuestLevelMinMax> param4, List<InitQuestLevelMinMax> param5)
        {
            var param1 = new ByteArray(ID);
            param1.writeShort(0);
            param1.writeDouble(0);
            param1.writeShort(17018);
            param1.writeInt(param4.Count);
            foreach (var _loc2_ in param4)
            {
                param1.write(_loc2_.write(0, ""));
            }
            param1.writeDouble(0);
            param1.writeInt(param5.Count);
            foreach (var _loc2_ in param5)
            {
                param1.write(_loc2_.write(0, ""));
            }
            return param1.Message.ToArray();
        }
    }
}

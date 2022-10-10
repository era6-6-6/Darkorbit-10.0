using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLevelMinMax
    {
        public const short ID = 25570;

        public byte[] write(int paramx1, string param2)
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(0 >> 4 | 0 << 28);
            param1.writeUTF("");
            return param1.Message.ToArray();
        }
    }
}

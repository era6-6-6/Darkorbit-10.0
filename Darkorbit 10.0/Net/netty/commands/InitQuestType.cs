using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestType
    {
        public const short ID = 5610;

        public byte[] write(int paramx1)
        {
            var param1 = new ByteArray(ID);
            param1.writeShort(0);
            return param1.Message.ToArray();
        }
    }
}

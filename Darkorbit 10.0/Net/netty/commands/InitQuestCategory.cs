using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestCategory
    {
        public const short ID = 24033;

        public byte[] write(int paramx1)
        {
            var param1 = new ByteArray(ID);
            param1.writeShort(11654);
            param1.writeShort(-1488);
            param1.writeShort(0);
            return param1.Message.ToArray();
        }
    }
}

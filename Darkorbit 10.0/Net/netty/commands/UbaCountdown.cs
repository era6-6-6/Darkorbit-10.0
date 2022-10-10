using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class UbaCountdown
    {
        public const short ID = 30831;

        private int type = 0;

        public UbaCountdown(int type)
        {
            this.type = type;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeShort(16110);
            param1.writeShort(-8668);
            param1.writeShort((short)type);
            return param1.Message.ToArray();
        }
    }
}

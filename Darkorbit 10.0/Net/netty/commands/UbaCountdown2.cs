using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class UbaCountdown2
    {
        public const short ID = 23083;

        private int number = 0;
        private int type = 0;

        public UbaCountdown2(int number, int type)
        {
            this.number = number;
            this.type = type;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(number >> 14 | number << 18);
            param1.writeShort((short)type);
            return param1.Message.ToArray();
        }
    }
}

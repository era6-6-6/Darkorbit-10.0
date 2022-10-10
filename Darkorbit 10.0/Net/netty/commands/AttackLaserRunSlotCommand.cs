using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class AttackLaserRunSlotCommand
    {
        public const short ID = 5862;

        public static byte[] write(int userId)
        {
            ByteArray param1 = new ByteArray(ID);
            param1.writeShort(-13671);
            param1.writeInt(userId >> 2 | userId << 30);
            return param1.ToByteArray();
        }
    }
}

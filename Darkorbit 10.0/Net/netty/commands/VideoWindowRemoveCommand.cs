using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class VideoWindowRemoveCommand
    {
        public const short ID = 23422;

        public static byte[] write(int windowID)
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(windowID << 16 | windowID >> 16);
            param1.writeShort(-27458);
            return param1.ToByteArray();
        }
    }
}

using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.requests
{
    class QuestGiverRequest
    {
        public const short ID = 13518;

        public int a3D = 0;

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            parser.readShort();
            a3D = parser.readInt();
            a3D = a3D >> 15 | a3D << 17;
        }
    }
}

using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.requests
{
    class QuestLoadRequest
    {
        public const short ID = 21421;

        public int bS = 0;

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            bS = parser.readInt();
            bS = bS >> 16 | bS << 16;
        }
    }
}

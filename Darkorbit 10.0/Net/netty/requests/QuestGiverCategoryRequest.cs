using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.requests
{
    class QuestGiverCategoryRequest
    {
        public const short ID = 5503;

        public bool paramx1 = false;
        public bool param2 = false;
        public bool param3 = false;
        public bool param4 = false;
        public bool param5 = false;
        public bool param6 = false;

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            param6 = parser.readBoolean();
            parser.readShort();
            param4 = parser.readBoolean();
            param5 = parser.readBoolean();
            param3 = parser.readBoolean();
            param2 = parser.readBoolean();
            paramx1 = parser.readBoolean();
        }
    }
}

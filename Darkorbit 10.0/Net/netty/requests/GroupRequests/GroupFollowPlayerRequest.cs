using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.requests.GroupRequests
{
    class GroupFollowPlayerRequest
    {
        public const short ID = 758;

        public int userId = 0;

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            parser.readShort();
            userId = parser.readInt();
            userId = userId << 7 | userId >> 25;
        }
    }
}

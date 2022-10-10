using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_61W
    {
        public const short ID = 23281;

        public int paramx1 = 0;
        public bool param2 = false;
        public bool param3 = false;

        public InitQuestLoadItem_61W(int paramx1, bool param2, bool param3)
        {
            this.paramx1 = paramx1;
            this.param2 = param2;
            this.param3 = param3;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeBoolean(this.param2);
            param1.writeShort(-2605);
            param1.writeBoolean(this.param3);
            param1.writeDouble(this.paramx1);
            return param1.Message.ToArray();
        }
    }
}

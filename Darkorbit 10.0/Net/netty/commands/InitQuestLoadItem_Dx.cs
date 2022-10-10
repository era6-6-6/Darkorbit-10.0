using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_Dx
    {
        public const short ID = 15763;

        public int paramx1 = 0;
        public bool param2 = false;
        public bool param3 = false;
        public bool param4 = false;
        public int param5 = 0;
        public List<InitQuestLoadItem_a2R> param6 = new List<InitQuestLoadItem_a2R>();

        public InitQuestLoadItem_Dx(int paramx1, bool param2, bool param3, bool param4, int param5, List<InitQuestLoadItem_a2R> param6)
        {
            this.paramx1 = paramx1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.param6 = param6;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(this.param6.Count);
            foreach (var _loc2_ in this.param6)
            {
                param1.write(_loc2_.write());
            }
            param1.writeBoolean(this.param2);
            param1.writeInt(this.paramx1 << 7 | this.paramx1 >> 25);
            param1.writeBoolean(this.param4);
            param1.writeBoolean(this.param3);
            param1.writeInt(this.param5 << 6 | this.param5 >> 26);
            return param1.Message.ToArray();
        }
    }
}

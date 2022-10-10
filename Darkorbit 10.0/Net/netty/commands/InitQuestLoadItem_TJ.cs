using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_TJ
    {
        public const short ID = 26745;

        public InitQuestLoadItem_uZ paramx1 = null;
        public List<InitQuestLoadItem_Z1M> param2 = new List<InitQuestLoadItem_Z1M>();
        public InitQuestLoadItem_Z1M param3 = null;

        public InitQuestLoadItem_TJ(InitQuestLoadItem_uZ paramx1, List<InitQuestLoadItem_Z1M> param2, InitQuestLoadItem_Z1M param3)
        {
            this.paramx1 = paramx1;
            this.param2 = param2;
            this.param3 = param3;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.write(paramx1.write());
            param1.writeInt(this.param2.Count);
            foreach (var _loc2_ in this.param2)
            {
                param1.write(_loc2_.write());
            }
            param1.writeShort(32712);
            param1.write(param3.write());
            return param1.ToByteArray();
        }
    }
}

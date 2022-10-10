using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_D2s
    {
        public const short ID = 8537;

        public InitQuestLoadItem_uZ paramx1 = null;

        public InitQuestLoadItem_D2s(InitQuestLoadItem_uZ paramx1)
        {
            this.paramx1 = paramx1;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.write(this.paramx1.write());
            param1.writeShort(-4395);
            return param1.ToByteArray();
        }
    }
}

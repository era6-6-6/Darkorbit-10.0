using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_a2R
    {
        public const short ID = 12207;

        public InitQuestLoadItem_Dx paramx1 = null;
        public InitQuestLoadItem_G4F param2 = null;

        public InitQuestLoadItem_a2R(InitQuestLoadItem_Dx paramx1, InitQuestLoadItem_G4F param2)
        {
            this.paramx1 = paramx1;
            this.param2 = param2;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.write(this.param2.write());
            param1.write(this.paramx1.write());
            return param1.Message.ToArray();
        }
    }
}

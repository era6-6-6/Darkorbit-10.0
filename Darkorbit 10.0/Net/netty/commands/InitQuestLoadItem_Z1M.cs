using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_Z1M
    {
        public const short ID = 16508;

        public string paramx1 = "";
        public string param2 = "";
        public int param3 = 0;
        public int param4 = 0;
        public int param5 = 0;

        public InitQuestLoadItem_Z1M(string paramx1, string param2, int param3, int param4, int param5)
        {
            this.paramx1 = paramx1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(this.param4 << 5 | this.param4 >> 27);
            param1.writeInt(this.param3 << 5 | this.param3 >> 27);
            param1.writeInt(this.param5 >> 6 | this.param5 << 26);
            param1.writeUTF(this.paramx1);
            param1.writeUTF(this.param2);
            param1.writeShort(-10417);
            return param1.Message.ToArray();
        }
    }
}

using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.commands
{
    class Ubal4bModule
    {
        public const short ID = 11744;

        public string vard1r = "";
        public int WarPoints = 0;

        public Ubal4bModule(string param1, int param2)
        {
            this.vard1r = param1;
            this.WarPoints = param2;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeUTF(this.vard1r);
            param1.writeInt(this.WarPoints << 11 | this.WarPoints >> 21);
            param1.writeShort(-7796);
            return param1.Message.ToArray();
        }
    }
}

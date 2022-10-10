using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class Ubaq2HModule : command_NQ
    {
        public const short ID = 16157;

        public int timer = 0;
        public UbaM1tModule var44b { get; set; }

        public Ubaq2HModule(int timer, UbaM1tModule param2)
        {
            this.timer = timer;
            this.var44b = param2;
        }

        public override byte[] write()
        {
            var param1 = new ByteArray(ID);
            super(param1);
            param1.writeInt(this.timer << 6 | this.timer >> 26);
            param1.write(var44b.write());
            param1.writeShort(26792);
            return param1.Message.ToArray();
        }
    }
}

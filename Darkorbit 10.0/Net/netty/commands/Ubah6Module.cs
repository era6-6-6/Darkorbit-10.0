using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.commands
{
    class Ubah6Module
    {
        public const short ID = 7919;

        public int _12R = 0;

        public Ubah6Module(int _12R)
        {
            this._12R = _12R;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(this._12R << 8 | this._12R >> 24);
            return param1.ToByteArray();
        }
    }
}

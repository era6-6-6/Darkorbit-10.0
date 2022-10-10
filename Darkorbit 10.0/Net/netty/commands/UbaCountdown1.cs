using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class UbaCountdown1
    {
        public const short ID = 9191;

        private object type = null;
        private int duration = 0;

        public UbaCountdown1(object type, int duration)
        {
            this.type = type;
            this.duration = duration;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            if(type is UbaCountdown2 uba2) param1.write(uba2.write());
            if(type is UbaCountdown uba) param1.write(uba.write());
            param1.writeShort(-24891);
            param1.writeInt(duration >> 4 | duration << 28);
            return param1.ToByteArray();
        }
    }
}

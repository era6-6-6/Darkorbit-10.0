using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class PremiumStateCommand
    {
        public const short ID = 5107;

        public static byte[] write(bool premium)
        {
            var param1 = new ByteArray(ID);
            param1.writeBoolean(premium);
            return param1.ToByteArray();
        }
    }
}

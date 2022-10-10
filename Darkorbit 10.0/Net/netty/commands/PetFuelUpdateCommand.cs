using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class PetFuelUpdateCommand
    {
        public const short ID = 22079;

        public static byte[] write(int petFuelNow, int petFuelMax)
        {
            var cmd = new ByteArray(ID);
            cmd.writeInt(petFuelMax << 5 | petFuelMax >> 27);
            cmd.writeInt(petFuelNow >> 11 | petFuelNow << 21);
            return cmd.ToByteArray();
        }
    }
}
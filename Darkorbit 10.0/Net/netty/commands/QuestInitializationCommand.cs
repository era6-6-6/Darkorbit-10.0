using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class QuestInitializationCommand
    {
        public const short ID = 16927;

        public static byte[] write(bool var1)
        {
            var param1 = new ByteArray(ID);
            param1.writeBoolean(var1);
            return param1.ToByteArray();
        }
    }
}

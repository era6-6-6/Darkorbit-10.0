using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class Ubas3wModule : command_NQ
    {
        public const short ID = 14585;

        public UbaG3FModule WarPoints { get; set; }
        public Uba64iModule varA3j { get; set; }
        public UbahsModule vare3G { get; set; }

        public Ubas3wModule(UbaG3FModule WarPoints, Uba64iModule varA3j, UbahsModule vare3G)
        {
            this.WarPoints = WarPoints;
            this.varA3j = varA3j;
            this.vare3G = vare3G;
        }

        public override byte[] write()
        {
            var param1 = new ByteArray(ID);
            super(param1);
            param1.write(WarPoints.write());
            param1.writeShort(-9335);
            param1.write(varA3j.write());
            param1.write(vare3G.write());
            return param1.Message.ToArray();
        }
    }
}

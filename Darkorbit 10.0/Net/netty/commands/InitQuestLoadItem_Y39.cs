using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_Y39
    {
        public const short ID = 19105;

        public int amount = 0;
        public string lootId = "";

        public InitQuestLoadItem_Y39(string param1, int param2)
        {
            this.lootId = param1;
            this.amount = param2;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(this.amount >> 7 | this.amount << 25);
            param1.writeUTF(this.lootId);
            return param1.Message.ToArray();
        }
    }
}

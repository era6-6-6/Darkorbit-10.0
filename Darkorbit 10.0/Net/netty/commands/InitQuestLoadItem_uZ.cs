using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_uZ
    {
        public const short ID = 10794;

        public int paramx1 = 0;
        public List<InitQuestIcon> param2 = new List<InitQuestIcon>();
        public InitQuestLoadItem_Dx param3 = null;
        public List<InitQuestLoadItem_Y39> param4 = new List<InitQuestLoadItem_Y39>();
        public List<InitQuestType> param5 = new List<InitQuestType>();
        public string param6 = "";
        public string param7 = "";

        public InitQuestLoadItem_uZ(int paramx1, List<InitQuestIcon> param2, InitQuestLoadItem_Dx param3, List<InitQuestLoadItem_Y39> param4, List<InitQuestType> param5, string param6, string param7)
        {
            this.paramx1 = paramx1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.param6 = param6;
            this.param7 = param7;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeInt(this.paramx1 >> 15 | this.paramx1 << 17);
            param1.writeUTF(this.param7);
            param1.write(this.param3.write());
            param1.writeInt(this.param5.Count);
            foreach (var _loc2_ in this.param5)
            {
                param1.write(_loc2_.write(0));
            }
            param1.writeUTF(this.param6);
            param1.writeInt(this.param4.Count);
            foreach (var _loc2_ in this.param4)
            {
                param1.write(_loc2_.write());
            }
            param1.writeInt(this.param2.Count);
            foreach (var _loc2_ in this.param2)
            {
                param1.write(_loc2_.write(0));
            }
            return param1.Message.ToArray();
        }
    }
}

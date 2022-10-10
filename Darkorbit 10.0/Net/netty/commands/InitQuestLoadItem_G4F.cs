using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Utils;

namespace Darkorbit.Net.netty.commands
{
    class InitQuestLoadItem_G4F
    {
        public const short ID = 26664;

        public int paramx1 = 0;
        public List<string> param2 = new List<string>();
        public int param3 = 0;
        public int param4 = 0;
        public int param5 = 0;
        public bool param6 = false;
        public InitQuestLoadItem_61W param7 = null;
        public List<InitQuestLoadItem_G4F> param8 = null;

        public InitQuestLoadItem_G4F(int paramx1, List<string> param2, int param3, int param4, int param5, bool param6, InitQuestLoadItem_61W param7, List<InitQuestLoadItem_G4F> param8)
        {
            this.paramx1 = paramx1;
            this.param2 = param2;
            this.param3 = param3;
            this.param4 = param4;
            this.param5 = param5;
            this.param6 = param6;
            this.param7 = param7;
            this.param8 = param8;
        }

        public byte[] write()
        {
            var param1 = new ByteArray(ID);
            param1.writeShort((short)this.param4);
            param1.write(this.param7.write());
            param1.writeShort((short)this.param3);
            param1.writeInt(this.param8.Count);
            foreach (var _loc2_ in this.param8)
            {
                param1.write(_loc2_.write());
            }
            param1.writeInt(this.paramx1 << 10 | this.paramx1 >> 22);
            param1.writeInt(this.param2.Count);
            foreach (var _loc2_ in this.param2)
            {
                param1.writeUTF(_loc2_);
            }
            param1.writeBoolean(this.param6);
            param1.writeDouble(this.param5);
            return param1.Message.ToArray();
        }
    }
}

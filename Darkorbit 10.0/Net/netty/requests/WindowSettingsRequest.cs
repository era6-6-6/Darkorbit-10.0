using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.requests
{
    class WindowSettingsRequest
    {
        public const short ID = 30907;

        public String proActionBarPosition = "";     
        public String categoryBarPosition = "";     
        public String standartSlotBarPosition = "";     
        public String genericFeatureBarLayoutType = "";      
        public String premiumSlotBarLayoutType = "";      
        public String premiumSlotBarPosition = "";      
        public int scaleFactor = 0;      
        public String gameFeatureBarLayoutType = "";      
        public String gameFeatureBarPosition = "";      
        public String proActionBarLayoutType = "";      
        public String genericFeatureBarPosition = "";      
        public Boolean hideAllWindows = false;      
        public String standartSlotBarLayoutType = "";      
        public String unknown = "";      
        public string barStatesAsString = "";

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            var proActionBarPosition = parser.readUTF();
            if (proActionBarPosition != "")
            {
                this.proActionBarPosition = proActionBarPosition;
            }
            this.categoryBarPosition = parser.readUTF();
            this.standartSlotBarPosition = parser.readUTF();
            this.genericFeatureBarLayoutType = parser.readUTF();
            parser.readShort();
            var premiumSlotBarLayoutType = parser.readUTF();
            if (premiumSlotBarLayoutType != "")
            {
                this.premiumSlotBarLayoutType = premiumSlotBarLayoutType;
            }
            var premiumSlotBarPosition = parser.readUTF();
            if (premiumSlotBarPosition != "")
            {
                this.premiumSlotBarPosition = premiumSlotBarPosition;
            }
            parser.readShort();
            this.scaleFactor = parser.readInt();
            this.scaleFactor = (int)(((uint)this.scaleFactor) << 1 | ((uint)this.scaleFactor >> 31));
            this.gameFeatureBarLayoutType = parser.readUTF();
            this.gameFeatureBarPosition = parser.readUTF();
            var proActionBarLayoutType = parser.readUTF();
            if (proActionBarLayoutType != "")
            {
                this.proActionBarLayoutType = proActionBarLayoutType;
            }
            this.genericFeatureBarPosition = parser.readUTF();
            this.hideAllWindows = parser.readBoolean();
            this.standartSlotBarLayoutType = parser.readUTF();
            this.unknown = parser.readUTF();
            this.barStatesAsString = parser.readUTF();
        }
    }
}

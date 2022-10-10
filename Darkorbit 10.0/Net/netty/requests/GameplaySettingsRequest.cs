using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Net.netty.requests
{
    class GameplaySettingsRequest
    {
        public const short ID = 25300;

        public bool DoubleClickAttackEnabled = false;
        public bool AutoChangeAmmo = false;
        public string ExtraSettings = "false,false,false";
        public bool AutoRefinement = false;
        public bool AutoBoost = false;
        public bool AutoBuyBootyKeys = false;
        public bool QuickSlotStopAttack = false;
        public bool ShowBattlerayNotifications = false;
        public bool varE3N = false;

        public void readCommand(byte[] bytes)
        {
            var parser = new ByteParser(bytes);
            ExtraSettings = parser.readUTF();
            DoubleClickAttackEnabled = parser.readBoolean();
            varE3N = parser.readBoolean();
            AutoBuyBootyKeys = parser.readBoolean();
            parser.readShort();
            AutoBoost = parser.readBoolean();
            QuickSlotStopAttack = parser.readBoolean();
            AutoChangeAmmo = parser.readBoolean();
            ShowBattlerayNotifications = parser.readBoolean();
            AutoRefinement = parser.readBoolean();
        }
    }
}

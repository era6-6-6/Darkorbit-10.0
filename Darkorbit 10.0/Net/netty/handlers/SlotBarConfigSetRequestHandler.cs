

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class SlotBarConfigSetRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new SlotBarConfigSetRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var settings = player.Settings;

            if (read.FromIndex != 0)
            {
                switch (read.FromSlotBarId)
                {
                    case SettingsManager.STANDARD_SLOT_BAR:
                        settings.SlotBarItems.Remove((short)read.FromIndex);
                        QueryManager.SavePlayer.Settings(player, "slotbarItems", settings.SlotBarItems);
                        break;
                    case SettingsManager.PREMIUM_SLOT_BAR:
                        settings.PremiumSlotBarItems.Remove((short)read.FromIndex);
                        QueryManager.SavePlayer.Settings(player, "premiumSlotbarItems", settings.PremiumSlotBarItems);
                        break;
                    case SettingsManager.PRO_ACTION_BAR:
                        settings.ProActionBarItems.Remove((short)read.FromIndex);
                        QueryManager.SavePlayer.Settings(player, "proActionBarItems", settings.ProActionBarItems);
                        break;
                }
            }

            if (read.ToIndex != 0)
            {
                switch (read.ToSlotBarId)
                {
                    case SettingsManager.STANDARD_SLOT_BAR:
                        settings.SlotBarItems.Remove((short)read.ToIndex);
                        settings.SlotBarItems.Add((short)read.ToIndex, read.ItemId);
                        QueryManager.SavePlayer.Settings(player, "slotbarItems", settings.SlotBarItems);
                        break;
                    case SettingsManager.PREMIUM_SLOT_BAR:
                        settings.PremiumSlotBarItems.Remove((short)read.ToIndex);
                        settings.PremiumSlotBarItems.Add((short)read.ToIndex, read.ItemId);
                        QueryManager.SavePlayer.Settings(player, "premiumSlotbarItems", settings.PremiumSlotBarItems);
                        break;
                    case SettingsManager.PRO_ACTION_BAR:
                        settings.ProActionBarItems.Remove((short)read.ToIndex);
                        settings.ProActionBarItems.Add((short)read.ToIndex, read.ItemId);
                        QueryManager.SavePlayer.Settings(player, "proActionBarItems", settings.ProActionBarItems);
                        break;
                }
            }

            player.SettingsManager.SendSlotBarCommand();
        }
    }
}

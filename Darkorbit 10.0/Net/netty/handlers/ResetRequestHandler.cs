using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class ResetRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new ReadyRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;

            var settings = new SettingsBase();

            player.Settings = settings;

            string audio = JsonConvert.SerializeObject(settings.Audio);
            string quality = JsonConvert.SerializeObject(settings.Quality);
            string classY2T = JsonConvert.SerializeObject(settings.ClassY2T);
            string display = JsonConvert.SerializeObject(settings.Display);
            string gameplay = JsonConvert.SerializeObject(settings.Gameplay);
            string window = JsonConvert.SerializeObject(settings.Window);
            string boundKeys = JsonConvert.SerializeObject(settings.BoundKeys);
            string inGameSettings = JsonConvert.SerializeObject(settings.InGameSettings);
            string slotbarItems = JsonConvert.SerializeObject(settings.SlotBarItems);
            string premiumSlotbarItems = JsonConvert.SerializeObject(settings.PremiumSlotBarItems);
            string proActionBarItems = JsonConvert.SerializeObject(settings.ProActionBarItems);

            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                mySqlClient.ExecuteNonQuery($"UPDATE player_settings SET audio = '{audio}', quality = '{quality}', classY2T = '{classY2T}', display = '{display}', gameplay = '{gameplay}',"+
                    $"window = '{window}', boundKeys = '{boundKeys}', inGameSettings = '{inGameSettings}', slotbarItems = '{slotbarItems}', premiumSlotbarItems = '{premiumSlotbarItems}', proActionBarItems = '{proActionBarItems}' WHERE userId = {player.Id}");
            }
        }
    }
}

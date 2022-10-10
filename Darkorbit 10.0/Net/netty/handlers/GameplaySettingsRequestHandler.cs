

using Darkorbit.Net.netty.requests;

namespace Darkorbit.Net.netty.handlers
{
    class GameplaySettingsRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new GameplaySettingsRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var gameplaySettings = player.Settings.Gameplay;

            gameplaySettings.notSet = false;
            gameplaySettings.autoBoost = read.AutoBoost;
            gameplaySettings.autoBuyBootyKeys = read.AutoBuyBootyKeys;
            gameplaySettings.autochangeAmmo = read.AutoChangeAmmo;
            gameplaySettings.autoRefinement = read.AutoRefinement;
            gameplaySettings.extraSettings = read.ExtraSettings;
            gameplaySettings.doubleclickAttackEnabled = read.DoubleClickAttackEnabled;
            gameplaySettings.quickSlotStopAttack = read.QuickSlotStopAttack;
            gameplaySettings.varE3N = read.varE3N;

            QueryManager.SavePlayer.Settings(player, "gameplay", gameplaySettings);
        }
    }
}

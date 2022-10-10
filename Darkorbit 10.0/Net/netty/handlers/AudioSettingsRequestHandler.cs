using Darkorbit.Net.netty.requests;


namespace Darkorbit.Net.netty.handlers
{
    class AudioSettingsRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new AudioSettingsRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var audioSettings = player.Settings.Audio;

            audioSettings.notSet = false;
            audioSettings.music = read.music;
            audioSettings.sound = read.sound;
            audioSettings.voice = read.voice;
            audioSettings.playCombatMusic = read.playCombatMusic;

            QueryManager.SavePlayer.Settings(player, "audio", audioSettings);
        }
    }
}

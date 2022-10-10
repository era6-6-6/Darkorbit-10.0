using Darkorbit.Game.Objects.Players.Stations;

namespace Darkorbit.Net.netty.handlers
{
    class RepairStationRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var player = gameSession.Player;

            foreach (var station in player.Spacemap.Activatables.Values)
            {
                var inRangeStations = player.Storage.InRangeAssets;
                if (inRangeStations.ContainsKey(station.Id)) continue;

                if (station is RepairStation)
                    station.Click(gameSession);
            }
        }
    }
}

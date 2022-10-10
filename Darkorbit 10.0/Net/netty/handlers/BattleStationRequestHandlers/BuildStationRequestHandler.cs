using Darkorbit.Game.Objects.Players.Stations;
using Darkorbit.Net.netty.requests.BattleStationRequests;

namespace Darkorbit.Net.netty.handlers.BattleStationRequestHandlers
{
    class BuildStationRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new BuildStationRequest();
            read.readCommand(bytes);

            var player = gameSession.Player;
            var battleStation = player.Spacemap.GetActivatableMapEntity(read.battleStationId) as BattleStation;

            if (battleStation != null)
            {
                if (player.Position.DistanceTo(battleStation.Position) > 700)
                {
                    player.SendCommand(BattleStationErrorCommand.write(BattleStationErrorCommand.OUT_OF_RANGE));
                    return;
                }

                if (battleStation.EquippedStationModule[player.Clan.Id].Count != 10 || battleStation.InBuildingState || battleStation.Clan.Id != 0) return;

                if (battleStation.Destroyed)
                    battleStation.Destroyed = false;

                battleStation.InBuildingState = true;
                battleStation.Clan = player.Clan;

                foreach (var module in battleStation.EquippedStationModule[player.Clan.Id])
                {
                    module.Clan = player.Clan;
                    module.FactionId =player.FactionId;
                }

                battleStation.FactionId = player.FactionId;
                battleStation.BuildTimeInMinutes = read.buildTimeInMinutes;
                battleStation.buildTime = DateTime.Now;

                Program.TickManager.AddTick(battleStation);

                battleStation.AddVisualModifier(VisualModifierCommand.BATTLESTATION_CONSTRUCTING, 0, "", 0, true);

                player.SendCommand(BattleStationBuildingStateCommand.write(battleStation.Id, battleStation.Id, battleStation.Name, (int)TimeSpan.FromMinutes(battleStation.BuildTimeInMinutes).TotalSeconds, 3600, battleStation.Clan.Name, new FactionModule((short)battleStation.FactionId)));

                QueryManager.BattleStations.BattleStation(battleStation);
                QueryManager.BattleStations.Modules(battleStation);
            }
        }
    }
}

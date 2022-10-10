using Darkorbit.Game.Objects.Players.Stations;
using Darkorbit.Net.netty.requests.BattleStationRequests;

namespace Darkorbit.Net.netty.handlers.BattleStationRequestHandlers
{
    class UnEquipModuleRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            var read = new UnEquipModuleRequest();
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

                var module = battleStation.EquippedStationModule[player.Clan.Id].Where(x => x.ItemId == read.itemId).FirstOrDefault();

                if (module != null)
                {
                    /*
                    if (module.OwnerId != player.Id)
                    {
                        player.SendCommand(BattleStationErrorCommand.write(BattleStationErrorCommand.ITEM_NOT_OWNED));
                        return;
                    }*/

                    module.Remove();

                    battleStation.Click(gameSession);
                }
            }
        }
    }
}

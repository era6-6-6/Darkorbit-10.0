namespace Darkorbit.Net.netty.handlers.UbaRequestHandlers
{
    class UbaMatchmakingAcceptRequestHandler : IHandler
    {
        public void execute(GameSession gameSession, byte[] bytes)
        {
            //check if lobby to wait for other players is already initiated
            if (gameSession.Player.Storage.ubal.lobbyWaitForPlayerId == 0)
            {
                gameSession.Player.Storage.ubal.lobbyWaitForPlayer = Task.Run(() => LobbyWaitForPlayer(gameSession.Player));
                gameSession.Player.Storage.ubal.lobbyWaitForPlayerId = gameSession.Player.Id;

                foreach(Player p in gameSession.Player.Storage.ubal.players)
                {
                    if(p.Id != gameSession.Player.Id)
                    {
                        p.Storage.ubal.lobbyAcceptTime = 30;
                    }
                }
            } else
            {
                //if lobby exists already, then kill this task and initiate the battle
                gameSession.Player.Storage.ubal.initiateBattle = true;

                foreach(Player p in gameSession.Player.Storage.ubal.players)
                {
                    p.SendCommand(UbaWindowInitializationCommand.write(new Ubaq2HModule(Portal.JUMP_DELAY, new UbaM1tModule(false)), 4));
                }

                EventManager.UltimateBattleArena.Uba(gameSession.Player.Storage.ubal.players[0], gameSession.Player.Storage.ubal.players[1], gameSession.Player.Storage.ubal.mapId, gameSession.Player.Storage.ubal);
            }
        }

        public async void LobbyWaitForPlayer(Player player1)
        {
            for (int i = 30; i > 0; i--)
            {
                if (player1.Storage.ubal == null || player1.Storage.ubal.initiateBattle) break;

                player1.SendCommand(UbaWindowInitializationCommand.write(new Ubaq2HModule(i * 1000, new UbaM1tModule(false)), 4));

                await Task.Delay(1000);
            }
        }
    }
}

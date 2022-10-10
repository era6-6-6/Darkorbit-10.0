namespace Darkorbit.Game.Objects.Players.Managers
{
    abstract class AbstractManager
    {
        public Player Player { get; }

        public AbstractManager(Player player)
        {
            Player = player;
        }
    }
}

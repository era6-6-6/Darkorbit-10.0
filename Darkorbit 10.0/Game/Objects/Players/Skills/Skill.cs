namespace Darkorbit.Game.Objects.Players.Skills
{
    abstract class Skill : Tick
    {
        public abstract string LootId { get; }

        public abstract int Duration { get; }
        public abstract int Cooldown { get; }

        public Player Player { get; set; }

        public bool Active = false;
        public DateTime cooldown = new DateTime();

        public Skill(Player player)
        {
            Player = player;

            Program.TickManager.AddTickPlayer(this);
        }

        public abstract void Tick();
        public abstract void Send();
        public abstract void Disable();
    }
}

namespace Darkorbit.Game.Objects.Players.Techs
{
    class PrecisionTargeter
    {
        public Player Player { get; set; }

        public bool Active = false;

        public PrecisionTargeter(Player player) { Player = player; }

        public void Tick()
        {
            if (Active)
                if (cooldown.AddMilliseconds(TimeManager.PRECISION_TARGETER_DURATION) < DateTime.Now)
                    Disable();
        }

        public DateTime cooldown = new DateTime();
        public void Send()
        {
            if (cooldown.AddMilliseconds(TimeManager.PRECISION_TARGETER_DURATION + TimeManager.PRECISION_TARGETER_COOLDOWN) < DateTime.Now || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.Storage.PrecisionTargeter = true;

                Player.SendCooldown(TechManager.TECH_PRECISION_TARGETER, TimeManager.PRECISION_TARGETER_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
        }

        public void Disable()
        {
            Active = false;
            Player.Storage.PrecisionTargeter = false;
            Player.SendCooldown(TechManager.TECH_PRECISION_TARGETER, TimeManager.PRECISION_TARGETER_COOLDOWN);
        }
    }
}

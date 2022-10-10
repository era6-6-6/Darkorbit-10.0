namespace Darkorbit.Game.Objects.Players.Skills
{
    class Sentinel : Skill
    {
        public override string LootId { get => SkillManager.SENTINEL; }

        public override int Duration { get => TimeManager.SENTINEL_DURATION; }
        public override int Cooldown { get => TimeManager.SENTINEL_COOLDOWN; }

        public Sentinel(Player player) : base(player) { }

        public override void Tick()
        {
            if (Active)
                if (cooldown.AddMilliseconds(Duration) < DateTime.Now)
                    Disable();
        }

        public override void Send()
        {
            if (Ship.SENTINELS.Contains(Player.Ship.Id) && cooldown.AddMilliseconds(Duration + Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SENTINEL_NEIKOS && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SENTINEL_ARIOS && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SENTINEL_CONTAGION && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SENTINEL_HARBINGER && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SENTINEL_ULLRIN && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode)
            {
                Player.SkillManager.DisableAllSkills();

                Player.Storage.Sentinel = true;

                Player.AddVisualModifier(VisualModifierCommand.FORTRESS, 0, "", 0, true);

                Player.SendCooldown(LootId, Duration, true);
                Active = true;
                cooldown = DateTime.Now;
            }
        }

        public override void Disable()
        {
            Player.Storage.Sentinel = false;

            Player.RemoveVisualModifier(VisualModifierCommand.FORTRESS);

            Player.SendCooldown(LootId, Cooldown);
            Active = false;
        }
    }
}
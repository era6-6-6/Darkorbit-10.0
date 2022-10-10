namespace Darkorbit.Game.Objects.Players.Skills
{
    class Solace : Skill
    {
        public override string LootId { get => SkillManager.SOLACE; }

        public override int Duration { get => -1; }
        public override int Cooldown { get => TimeManager.SOLACE_COOLDOWN; }

        public Solace(Player player) : base(player) { }

        public override void Tick() { }

        public override void Send()
        {
            if (Ship.SOLACES.Contains(Player.Ship.Id) && cooldown.AddMilliseconds(Duration + Cooldown) < DateTime.Now || Player.Ship.Id == Ship.SOLACE_FROST && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_ASIMOV && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_ARGON && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_BLAZE && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_BOREALIS && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_OCEAN && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_POISON && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_TYRANNOS && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode || Player.Ship.Id == Ship.SOLACE_CONTAGION && cooldown.AddMilliseconds(Cooldown) < DateTime.Now || Player.Storage.GodMode)
            {
                Player.SkillManager.DisableAllSkills();

                ExecuteHeal();

                string packet = "0|SD|A|R|1|" + Player.Id + "";
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.SendCooldown(LootId, Cooldown);
                cooldown = DateTime.Now;
            }
        }

        public override void Disable() { }

        public void ExecuteHeal()
        {
            int heal = Maths.GetPercentage(Player.MaxHitPoints, 35);
            if (Player.Group != null)
            {
                foreach (var player in Player.Group.Members.Values)
                {
                    if (player.Spacemap != Player.Spacemap) continue;

                    if (player != Player)
                        heal = Maths.GetPercentage(Player.MaxHitPoints, 15);

                    player.Heal(heal);
                    if (player == Player) continue;

                    string packet = "0|SD|A|R|1|" + player.Id + "";
                    player.SendPacket(packet);
                    player.SendPacketToInRangePlayers(packet);
                }
            }
            else Player.Heal(heal);
        }
    }
}

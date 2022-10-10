namespace Darkorbit.Game.Objects.Players.Techs
{
    class EnergyLeech
    {
        public Player Player { get; set; }

        public bool Active = false;

        public EnergyLeech(Player player) { Player = player; }

        public void Tick()
        {
            if (Active)
                if (cooldown.AddMilliseconds(TimeManager.ENERGY_LEECH_DURATION) < DateTime.Now)
                    Disable();
        }

        public void ExecuteHeal(int damage)
        {
            try
            {
                if (Player.Settings.InGameSettings.selectedLaser != AmmunitionManager.MCB_500 && (Attackable.BLACKLIGHT != null && Player.SelectedCharacter != null && !Attackable.BLACKLIGHT.Contains(Player.SelectedCharacter.Ship.Id)))
                {
                    int heal = Maths.GetPercentage(damage, 10);
                    Player.Heal(heal);
                }
                else if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.MCB_500 && (Attackable.BLACKLIGHT != null && Player.SelectedCharacter != null && Attackable.BLACKLIGHT.Contains(Player.SelectedCharacter.Ship.Id)))
                {
                    int heal = Maths.GetPercentage(damage, 7);
                    Player.Heal(heal);
                }
                else
                {
                    int heal = Maths.GetPercentage(damage, 10);
                    Player.Heal(heal);
                }
            } catch(Exception ex)
            {
                Logger.Log("error_log", $"- [EnergyLeech.cs] Execute void exception: {ex}");
            }
        }

        public DateTime cooldown = new DateTime();
        public void Send()
        {
            if (cooldown.AddMilliseconds(TimeManager.ENERGY_LEECH_DURATION + TimeManager.ENERGY_LEECH_COOLDOWN) < DateTime.Now || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.SendCooldown(TechManager.TECH_ENERGY_LEECH, TimeManager.ENERGY_LEECH_DURATION, true);
                Player.Storage.EnergyLeech = true;
               // Player.AddVisualModifier(VisualModifierCommand.ENERGY_LEECH_ARRAY, 0, "", 0, true);
                Active = true;
                cooldown = DateTime.Now;
            }
        }

        public void Disable()
        {
            //Player.RemoveVisualModifier(VisualModifierCommand.ENERGY_LEECH_ARRAY);

            Player.SendCooldown(TechManager.TECH_ENERGY_LEECH, TimeManager.ENERGY_LEECH_COOLDOWN);
            Player.Storage.EnergyLeech = false;
            Active = false;
        }
    }
}

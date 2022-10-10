namespace Darkorbit.Game.Objects.Players.Managers
{
    class SkillManager : AbstractManager
    {
        public const String SENTINEL = "ability_sentinel";
        public const String DIMINISHER = "ability_diminisher";
        public const String VENOM = "ability_venom";
        public const String SPECTRUM = "ability_spectrum";
        public const String SOLACE = "ability_solace";
        public const String AEGIS_HP_REPAIR = "ability_aegis_hp-repair";
        public const String AEGIS_REPAIR_POD = "ability_aegis_repair-pod";
        public const String AEGIS_SHIELD_REPAIR = "ability_aegis_shield-repair";
        public const String CITADEL_DRAW_FIRE = "ability_citadel_draw-fire";
        public const String CITADEL_FORTIFY = "ability_citadel_fortify";
        public const String CITADEL_PROTECTION = "ability_citadel_protection";
        public const String CITADEL_TRAVEL = "ability_citadel_travel";
        public const String SPEARHEAD_DOUBLE_MINIMAP = "ability_spearhead_double-minimap";
        public const String SPEARHEAD_JAM_X = "ability_spearhead_jam-x";
        public const String SPEARHEAD_TARGET_MARKER = "ability_spearhead_target-marker";
        public const String SPEARHEAD_ULTIMATE_CLOAK = "ability_spearhead_ultimate-cloak";
        public const String LIGHTNING = "ability_lightning";

        public SkillManager(Player player) : base(player) { InitiateSkills(); }

        public void InitiateSkills(bool updateSlotbar = false)
        {
            Player.Storage.Skills.Clear();

            /*if (Ship.SENTINELS.Contains(Player.Ship.Id))
                Player.Storage.Skills.Add(SkillManager.SENTINEL, new Sentinel(Player));
            else if (Ship.SPECTRUMS.Contains(Player.Ship.Id))
                Player.Storage.Skills.Add(SkillManager.SPECTRUM, new Spectrum(Player));
            else if (Ship.DIMINISHERS.Contains(Player.Ship.Id))
                Player.Storage.Skills.Add(SkillManager.DIMINISHER, new Diminisher(Player));
            else if (Ship.VENOMS.Contains(Player.Ship.Id))
                Player.Storage.Skills.Add(SkillManager.VENOM, new Venom(Player));
            else if (Ship.SOLACES.Contains(Player.Ship.Id))
                Player.Storage.Skills.Add(SkillManager.SOLACE, new Solace(Player));
            else
            {
                switch (Player.Ship.Id)
                {
                    case Ship.SOLACE_FROST:
                    case Ship.SOLACE_ASIMOV:
                    case Ship.SOLACE_ARGON:
                    case Ship.SOLACE_BLAZE:
                    case Ship.SOLACE_BOREALIS:
                    case Ship.SOLACE_OCEAN:
                    case Ship.SOLACE_POISON:
                    case Ship.SOLACE_TYRANNOS:
                    case Ship.SOLACE_CONTAGION:
                    case Ship.GOLIATH_SOLACE:
                        Player.Storage.Skills.Add(SkillManager.SOLACE, new Solace(Player));
                        break;
                    case Ship.DIMINISHER_PHANTASM:
                    case Ship.DIMINISHER_ULLRIN:
                        Player.Storage.Skills.Add(SkillManager.DIMINISHER, new Diminisher(Player));
                        break;
                    case Ship.CYBORG:
                    case Ship.CYBORG_INFINITE:
                    case Ship.CYBORG_LAVA:
                    case Ship.CYBORG_CARBONITE:
                    case Ship.CYBORG_FIRESTAR:
                    case Ship.CYBORG_NOBILIS:
                    case Ship.CYBORG_SCOURGE:
                    case Ship.CYBORG_INFERNO:
                    case Ship.CYBORG_ULLRIN:
                    case Ship.CYBORG_DUSKLIGHT:
                    case Ship.CYBORG_FROZEN:
                    case Ship.CYBORG_SUNSTORM:
                    case Ship.CYBORG_STARSCREAM:
                    case Ship.CYBORG_CELESTIAL:
                    case Ship.CYBORG_MAELSTORM:
                    case Ship.CYBORG_ASIMOV:
                    case Ship.CYBORG_TYRANNOS:
                    case Ship.CYBORG_OCEAN:
                    case Ship.CYBORG_POISON:
                    case Ship.CYBORG_PROMETHEUS:
                    case Ship.CYBORG_BLAZE:
                    case Ship.VENOM_ARGON:
                    case Ship.GOLIATH_VENOM:
                    case Ship.CYBORG_ARGON:
                    case Ship.CYBORG_SMITE:
                    case Ship.CYBORG_OSIRIS:
                    case Ship.CYBORG_SERAPH:
                        Player.Storage.Skills.Add(SkillManager.VENOM, new Venom(Player));
                        break;
                    case Ship.VENGEANCE_LIGHTNING:
                        Player.Storage.Skills.Add(SkillManager.LIGHTNING, new Afterburner(Player));
                        break;
                    case Ship.AEGIS:
                    case Ship.AEGIS_VETERAN:
                    case Ship.HAMMERCLAW:
                    case Ship.HAMMERCLAW_FROZEN:
                    case Ship.HAMMERCLAW_NOBILIS:
                    case Ship.HAMMERCLAW_ULLRIN:
                    case Ship.HAMMERCLAW_BANE:
                    case Ship.HAMMERCLAW_CARBONITE:
                    case Ship.HAMMERCLAW_LAVA:
                    case Ship.HAMMERCLAW_TYRANNOS:
                    case Ship.HAMMERCLAW_PROMETHEUS:
                    case Ship.AEGIS_ELITE:
                        Player.Storage.Skills.Add(SkillManager.AEGIS_HP_REPAIR, new AegisHpRepair(Player));
                        Player.Storage.Skills.Add(SkillManager.AEGIS_SHIELD_REPAIR, new AegisShieldRepair(Player));
                        Player.Storage.Skills.Add(SkillManager.AEGIS_REPAIR_POD, new AegisRepairPod(Player));
                        break;
                    case Ship.CITADEL:
                        Player.Storage.Skills.Add(SkillManager.CITADEL_DRAW_FIRE, new DrawFire(Player));
                        break;
                    case Ship.SENTINEL_ARIOS:
                    case Ship.SENTINEL_NEIKOS:
                    case Ship.SENTINEL_CONTAGION:
                    case Ship.SENTINEL_HARBINGER:
                    case Ship.SENTINEL_ULLRIN:
                        Player.Storage.Skills.Add(SkillManager.SENTINEL, new Sentinel(Player));
                        break;
                    case Ship.SPECTRUM_TYRANNOS:
                    case Ship.SPECTRUM_ARGON:
                        Player.Storage.Skills.Add(SkillManager.SPECTRUM, new Spectrum(Player));
                        break;
                }
            }*/

            if (updateSlotbar)
                Player.SettingsManager.SendSlotBarCommand();
        }

        public void Tick()
        {
            foreach (var skill in Player.Storage.Skills.Values)
                skill.Tick();
        }

        public void DisableAllSkills()
        {
            foreach (var skill in Player.Storage.Skills.Values)
                skill.Disable();
        }
    }
}
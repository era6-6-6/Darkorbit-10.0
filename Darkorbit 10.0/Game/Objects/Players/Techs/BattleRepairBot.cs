namespace Darkorbit.Game.Objects.Players.Techs
{
    class BattleRepairBot
    {
        public Player Player { get; set; }
        private static int HEALTH = 10000;
        public bool Active = false;

        public BattleRepairBot(Player player) { Player = player; }

        public void Tick()
        {
            if (Active)
                if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION) < DateTime.Now)
                    Disable();
                else
                    ExecuteHeal();
        }

        public DateTime lastRepairTime = new DateTime();
        public void ExecuteHeal()
        {
            if (lastRepairTime.AddSeconds(1) < DateTime.Now)
            {
                Player.Heal(HEALTH);
                lastRepairTime = DateTime.Now;
            }
        }

        public DateTime cooldown = new DateTime();
        public void Send()
        {
            if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION + 60000) < DateTime.Now && Player.SkillTree.battlecd == 0 && Player.SkillTree.backupcd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.AddVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT, 0, "", 0, true);
                Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION + 58000) < DateTime.Now && Player.SkillTree.battlecd == 1 && Player.SkillTree.backupcd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.AddVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT, 0, "", 0, true);
                Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION + 56000) < DateTime.Now && Player.SkillTree.battlecd == 2 && Player.SkillTree.backupcd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.AddVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT, 0, "", 0, true);
                Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION + 54000) < DateTime.Now && Player.SkillTree.battlecd == 3 && Player.SkillTree.backupcd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.AddVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT, 0, "", 0, true);
                Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION + 52000) < DateTime.Now && Player.SkillTree.battlecd == 4 && Player.SkillTree.backupcd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.AddVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT, 0, "", 0, true);
                Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION + 50000) < DateTime.Now && Player.SkillTree.battlecd == 5 && Player.SkillTree.backupcd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.AddVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT, 0, "", 0, true);
                Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(TimeManager.BATTLE_REPAIR_BOT_DURATION + 60000) < DateTime.Now || Player.Storage.GodMode)
            {
                Player.LoadData();
                Player.AddVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT, 0, "", 0, true);
                Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_DURATION, true);
                Active = true;
                cooldown = DateTime.Now;
            }
        }

        public void Disable()
        {
            Player.RemoveVisualModifier(VisualModifierCommand.BATTLE_REPAIR_BOT);
            Player.SendCooldown(TechManager.TECH_BATTLE_REPAIR_BOT, TimeManager.BATTLE_REPAIR_BOT_COOLDOWN);
            Active = false;
        }
    }
}



namespace Darkorbit.Game.Objects.Players.Techs
{
    class BackupShields
    {
        public Player Player { get; set; }

        public static int SHIELD = 75000;

        public BackupShields(Player player) { Player = player; }

        public DateTime cooldown = new DateTime();
        public void Send()
        {
            if (cooldown.AddMilliseconds(60000) < DateTime.Now && Player.SkillTree.backupcd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                string packet = "0|TX|A|S|SBU|" + Player.Id;
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.Heal(SHIELD, Player.Id, HealType.SHIELD);

                Player.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 60000);

                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(58000) < DateTime.Now && Player.SkillTree.backupcd == 1 && Player.SkillTree.battlecd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                string packet = "0|TX|A|S|SBU|" + Player.Id;
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.Heal(SHIELD, Player.Id, HealType.SHIELD);

                Player.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 58000);

                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(56000) < DateTime.Now && Player.SkillTree.backupcd == 2 && Player.SkillTree.battlecd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                string packet = "0|TX|A|S|SBU|" + Player.Id;
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.Heal(SHIELD, Player.Id, HealType.SHIELD);

                Player.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 56000);

                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(54000) < DateTime.Now && Player.SkillTree.backupcd == 3 && Player.SkillTree.battlecd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                string packet = "0|TX|A|S|SBU|" + Player.Id;
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.Heal(SHIELD, Player.Id, HealType.SHIELD);

                Player.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 54000);

                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(52000) < DateTime.Now && Player.SkillTree.backupcd == 4 && Player.SkillTree.battlecd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                string packet = "0|TX|A|S|SBU|" + Player.Id;
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.Heal(SHIELD, Player.Id, HealType.SHIELD);

                Player.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 52000);

                cooldown = DateTime.Now;
            }
            else if (cooldown.AddMilliseconds(50000) < DateTime.Now && Player.SkillTree.backupcd == 5 && Player.SkillTree.battlecd == 0 || Player.Storage.GodMode)
            {
                Player.LoadData();
                string packet = "0|TX|A|S|SBU|" + Player.Id;
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.Heal(SHIELD, Player.Id, HealType.SHIELD);

                Player.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 50000);

                cooldown = DateTime.Now;
            }
             else if (cooldown.AddMilliseconds(60000) < DateTime.Now || Player.Storage.GodMode)
            {
                Player.LoadData();
                string packet = "0|TX|A|S|SBU|" + Player.Id;
                Player.SendPacket(packet);
                Player.SendPacketToInRangePlayers(packet);

                Player.Heal(SHIELD, Player.Id, HealType.SHIELD);

                Player.SendCooldown(TechManager.TECH_BACKUP_SHIELDS, 60000);

                cooldown = DateTime.Now;
            }
        }
    }
}

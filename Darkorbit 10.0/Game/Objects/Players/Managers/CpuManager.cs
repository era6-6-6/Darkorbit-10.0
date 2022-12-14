using System;

namespace Darkorbit.Game.Objects.Players.Managers
{
    class CpuManager : AbstractManager
    {
        public const String CLK_XL = "equipment_extra_cpu_cl04k-xl";
        public const String AUTO_ROCKET_CPU = "equipment_extra_cpu_arol-x";
        public const String AUTO_HELLSTROM_CPU = "equipment_extra_cpu_rllb-x";
        public const String ROCKET_LAUNCHER = "equipment_weapon_rocketlauncher_hst-2";
        public const String GALAXY_JUMP_CPU = "equipment_extra_cpu_jp-02";

        public int CloakCooldownTime => Player.Premium ? 10000 : 20000;
        private const int CLOAK_PRICE = 250;

        public CpuManager(Player player) : base(player) { }

        public DateTime cloakCooldown = new DateTime();
        public void Cloak()
        {
            if (Player.Spacemap.Options.CloakBlocked || Player.Invisible) return;
            if (Player.AmmunitionManager.cloacks > 0)
            {
                if (cloakCooldown.AddMilliseconds(CloakCooldownTime) < DateTime.Now || Player.Storage.GodMode)
                {
                    //if(!Player.Premium)
                    //Player.ChangeData(DataType.URIDIUM, CLOAK_PRICE, ChangeType.DECREASE);
                    EnableCloak();

                    Player.SendCooldown(CLK_XL, CloakCooldownTime);
                    cloakCooldown = DateTime.Now;
                }
            }
            else Player.SendPacket("0|A|STD|You don't have enough uridium for buy a cloak.");
        }

        public void ArolX()
        {
            if (!Player.Storage.AutoRocket)
                EnableArolX();
            else
                DisableArolX();
        }

        public void RllbX()
        {
            if (!Player.Storage.AutoRocketLauncher)
                EnableRllbX();
            else
                DisableRllbX();
        }

        public void EnableCloak()
        {
            if (!Player.Invisible)
            {
                if (Player.Spacemap.Id == 121)
                {
                    Player.RemoveVisualModifier(VisualModifierCommand.CAMERA);
                }
                Player.AmmunitionManager.UseAmmo("equipment_extra_cpu_cl04k-xl", 1);
                AddSelectedCpu(CLK_XL);
                Player.Invisible = true;
                string cloakPacket = "0|n|INV|" + Player.Id + "|1";
                Player.SendPacket(cloakPacket);
                Player.SendPacketToInRangePlayers(cloakPacket);

                if (Player.Pet != null && Player.Pet.Activated)
                {
                    Player.Pet.Invisible = true;
                    Player.Pet.SendPacketToInRangePlayers("0|n|INV|" + Player.Pet.Id + "|1");
                }

                Player.SettingsManager.SendNewItemStatus(CLK_XL);
            }
        }

        public void DisableCloak()
        {
            if (Player.Invisible)
            {
                if(Player.Spacemap.Id == 121)
                {
                    Player.AddVisualModifier(VisualModifierCommand.CAMERA, 0, "", 0, true);
                }
                RemoveSelectedCpu(CLK_XL);
                Player.Invisible = false;
                string cloakPacket = "0|n|INV|" + Player.Id + "|0";
                Player.SendPacket("0|A|STM|msg_uncloaked");
                Player.SendPacket(cloakPacket);
                Player.SendPacketToInRangePlayers(cloakPacket);
                Player.SettingsManager.SendNewItemStatus(CLK_XL);
            }
        }

        public void Disablepet()
        {
            if (Player.Pet != null && Player.Pet.Activated)
            {
                Player.Pet.Deactivate(true);
                Player.SendPacket($"0|A|STD|-=- THE BEST TO DEACTIVATE THE PET FOR THE NON-FAILURE GG -= -");
            }
        }

        public void EnableArolX()
        {
            AddSelectedCpu(AUTO_ROCKET_CPU);
            Player.Storage.AutoRocket = true;
            Player.SettingsManager.SendNewItemStatus(AUTO_ROCKET_CPU);
        }

        public void DisableArolX()
        {
            RemoveSelectedCpu(AUTO_ROCKET_CPU);
            Player.Storage.AutoRocket = false;
            Player.SettingsManager.SendNewItemStatus(AUTO_ROCKET_CPU);
        }

        public void EnableRllbX()
        {
            AddSelectedCpu(AUTO_HELLSTROM_CPU);
            Player.Storage.AutoRocketLauncher = true;
            Player.SettingsManager.SendNewItemStatus(AUTO_HELLSTROM_CPU);
        }

        public void DisableRllbX()
        {
            RemoveSelectedCpu(AUTO_HELLSTROM_CPU);
            Player.Storage.AutoRocketLauncher = false;
            Player.SettingsManager.SendNewItemStatus(AUTO_HELLSTROM_CPU);
        }

        public void AddSelectedCpu(string cpu)
        {
            if (!Player.Settings.InGameSettings.selectedCpus.Contains(cpu))
                Player.Settings.InGameSettings.selectedCpus.Add(cpu);
        }

        public void RemoveSelectedCpu(string cpu)
        {
            if (Player.Settings.InGameSettings.selectedCpus.Contains(cpu))
                Player.Settings.InGameSettings.selectedCpus.Remove(cpu);
        }
    }
}

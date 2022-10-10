using Darkorbit.Game.Objects.Players.Stations;

namespace Darkorbit.Game.Objects.Players.Managers
{
    class AttackManager : AbstractManager, Tick
    {
        public RocketLauncher RocketLauncher { get; set; }
        public bool Attacking = false;
        public int mult;
        public bool scatter = true;
        public bool miss = false;
        public bool full = false;
        public bool fulllf5 = false;
        public bool proc = false;

        public AttackManager(Player player) : base(player) { RocketLauncher = new RocketLauncher(Player); }

        public DateTime lastAttackTime = new DateTime();
        public DateTime lastRSBAttackTime = new DateTime();
        public DateTime mineCooldown = new DateTime();
        public DateTime lastScatter = new DateTime();
        public DateTime lastLf5Proc = new DateTime();

        //public int damageDone = 0;




        public void Tick()
        {

            if (Player.lf4lasers / 2 > 0)
            {
                if (Player.lf4lasers / 2 == Player.Ship.Lasers)
                {
                    full = true;
                }
                else
                {
                    full = false;
                }

                if (miss)
                {

                    scatter = true;
                    mult = Player.lf4 * (Player.lf4Damage + 300);
                    lastScatter = DateTime.Now;


                }
                else
                {
                    if (lastScatter.AddSeconds(5) <= DateTime.Now)
                    {
                        scatter = true;
                    }
                    else
                    {
                        scatter = false;
                    }
                }
            }
            if (Player.lf5lasers / 2 > 0)
            {
                if (Player.lf5lasers / 2 >= Player.Ship.Lasers)
                {
                    fulllf5 = true;
                }
                else
                {
                    fulllf5 = false;
                }

                if (miss)
                {

                    scatter = true;
                    mult = Player.lf5lasers * (Player.lf5Damage + 1500);
                    lastScatter = DateTime.Now;


                }
                else
                {
                    if (lastScatter.AddSeconds(5) <= DateTime.Now)
                    {
                        scatter = true;
                    }
                    else
                    {
                        scatter = false;
                    }
                }
                if (lastLf5Proc.AddSeconds(5) <= DateTime.Now)
                {
                    proc = true;
                }
                else
                {
                    proc = false;
                }
            }
            else
            {
                scatter = false;
            }
        }


        public async void LaserAttack()
        {
            Program.TickManager.AddTick(this);

            if (Attacking)
            {
                var target = Player.Selected;

                if (target == null) return;
                if (!Player.TargetDefinition(target)) return;

                if (CheckLaserAttackTime())
                {
                    if (Player.AmmunitionManager.GetAmmo(Player.Settings.InGameSettings.selectedLaser) >= Player.lasers / 2 || Player.AmmunitionManager.GetAmmo(Player.Settings.InGameSettings.selectedLaser) >= Player.lasers / 2)
                    {
                        if (Player.Damage == 0)
                        {
                            Player.SendPacket("0|A|STM|no_lasers_on_board");
                            Player.DisableAttack(Player.Settings.InGameSettings.selectedLaser);
                            return;
                        }

                        //var lasers = (Player.lf4lasers > 0) ? Player.lf4lasers / 2 : (Player.lf5lasers > 0) ? Player.lf5lasers / 2 : 1;
                        var lasers = Player.lasers / 2;

                        Player.AmmunitionManager.UseAmmo(Player.Settings.InGameSettings.selectedLaser, lasers);
                        //Console.WriteLine($"Player Attacking: Used Ammo: {Player.Settings.InGameSettings.selectedLaser}, Lasers: {Player.lasers}");

                        /*Player.Skylab.ReduceLaserOre(lasers);*/

                        if (scatter || miss)
                        {

                            if (Player.CurrentConfig == 1)
                                mult = Player.conf1prome * 300;
                            else
                                mult = Player.conf2prome * 300;
                            lastScatter = DateTime.Now;
                            miss = false;
                        }
                        else
                        {

                            mult = 0;

                        }

                        var missP = Player.Storage.underPLD8 ? 0.4 : 0;
                        if (Player.Settings.InGameSettings.selectedFormation == DroneManager.STAR_FORMATION)
                        {
                            missP += 0.1;
                        }


                        var damage = RandomizeDamage((GetDamageMultiplier() * (Player.Damage + mult)), missP);

                        if (Player.Storage.Spectrum)
                        {
                            damage -= Maths.GetPercentage(damage, 25);
                        }
                        if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 && target is Player && Player.SkillTree.electroOptics == 0)
                        {
                            damage -= Maths.GetPercentage(damage, 5);
                        }
                        else if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 && target is Player && Player.SkillTree.electroOptics == 1)
                        {
                            damage -= Maths.GetPercentage(damage, 4);
                        }
                        else if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 && target is Player && Player.SkillTree.electroOptics == 2)
                        {
                            damage -= Maths.GetPercentage(damage, 3);
                        }
                        else if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 && target is Player && Player.SkillTree.electroOptics == 3)
                        {
                            damage -= Maths.GetPercentage(damage, 2);
                        }
                        else if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 && target is Player && Player.SkillTree.electroOptics == 4)
                        {
                            damage -= Maths.GetPercentage(damage, 1);
                        }
                        else if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50 && target is Player && Player.SkillTree.electroOptics == 5)
                        {
                            damage -= Maths.GetPercentage(damage, 0);
                        }
                        if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75 && target is Player)
                        {
                            damage += Maths.GetPercentage(damage, 5);
                        }
                        if (target is Npc)
                        {
                            if (Player.Settings.InGameSettings.selectedFormation == DroneManager.BAT_FORMATION)
                            {
                                damage += Maths.GetPercentage(damage, 8);
                            }
                            if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.MCB_500)
                            {
                                if (Attackable.BLACKLIGHT.Contains((target as Character).Ship.Id))
                                {
                                    damage *= 2;
                                }
                            }
                        }

                        if (target is Player)
                        {
                            if ((target as Player).Storage.Spectrum)
                                damage -= Maths.GetPercentage(damage, 25);
                        }

                        if (fulllf5 && proc)
                        {
                            damage += Maths.GetPercentage(damage, 50);
                            lastLf5Proc = DateTime.Now;
                        }

                        Damage(Player, target, DamageType.LASER, damage, Player.ShieldPenetration);

                        if (Player.Storage.AutoRocket)
                            RocketAttack();

                        if (Player.Storage.AutoRocketLauncher)
                            if (RocketLauncher.CurrentLoad != RocketLauncher.MaxLoad)
                                RocketLauncher.Reload();
                            else
                                LaunchRocketLauncher();
                        RocketLauncher.Reload();

                        UpdateAttacker(target, Player);

                        if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75)
                        {
                            Player.SendCooldown(AmmunitionManager.RSB_75, 3000);
                            lastRSBAttackTime = DateTime.Now;                           
                            return;
                        }
                        else if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.UCB_100)
                        {
                            lastAttackTime = DateTime.Now;
                        }
                        lastAttackTime = DateTime.Now;
                    }
                    else
                    {
                        Player.DisableAttack(Player.Settings.InGameSettings.selectedLaser);
                        return;
                    }

                }
            }
        }

        public DateTime lastRocketAttack = new DateTime();
        public DateTime r_ic3Cooldown = new DateTime();
        public DateTime pld8Cooldown = new DateTime();
        public DateTime wiz_xCooldown = new DateTime();
        public DateTime dcr_250Cooldown = new DateTime();

        public async void RocketAttack()
        {
            var enemy = Player.SelectedCharacter;
            if (enemy == null) return;

            if (Player.Settings.InGameSettings.selectedRocket != AmmunitionManager.WIZ_X)
                if (!Player.TargetDefinition(enemy, true, true)) return;
            Player.CpuManager.DisableCloak();

            if (Player.AmmunitionManager.GetAmmo(Player.Settings.InGameSettings.selectedRocket) < 1)
            {
                return;
            }

            if (Player.RocketInProgress) return;

            Player.SelectedRocket = GetSelectedRocket();

            bool canLaunchIce = false;
            bool canLaunchPLD8 = false;
            bool canLaunchwiz_x = false;
            bool canLaunchdcr_250 = false;

            switch (Player.SelectedRocket)
            {
                case 5:
                    if (pld8Cooldown.AddMilliseconds(TimeManager.PLD8_COOLDOWN) < DateTime.Now || Player.Storage.GodMode)
                    {
                        canLaunchPLD8 = true;
                        Player.SendCooldown(AmmunitionManager.PLD_8, TimeManager.PLD8_COOLDOWN);
                        pld8Cooldown = DateTime.Now;
                    }
                    else return;
                    break;
                case 6:
                    if (wiz_xCooldown.AddMilliseconds(TimeManager.WIZARD_COOLDOWN) < DateTime.Now || Player.Storage.GodMode)
                    {
                        Player.SendCooldown(AmmunitionManager.WIZ_X, TimeManager.WIZARD_COOLDOWN);
                        wiz_xCooldown = DateTime.Now;
                        canLaunchwiz_x = true;
                    }
                    else return;
                    break;
                case 10:
                    if (dcr_250Cooldown.AddMilliseconds(TimeManager.DCR_250_COOLDOWN) < DateTime.Now || Player.Storage.GodMode)
                    {
                        //canLaunchdcr_250 = true;
                        Player.SendCooldown(AmmunitionManager.DCR_250, TimeManager.DCR_250_COOLDOWN);
                        dcr_250Cooldown = DateTime.Now;
                        canLaunchdcr_250 = true;
                    }
                    else return;
                    break;
                case 18:
                    if (r_ic3Cooldown.AddMilliseconds(TimeManager.R_IC3_COOLDOWN) < DateTime.Now || Player.Storage.GodMode)
                    {
                        Player.SendCooldown(AmmunitionManager.R_IC3, TimeManager.R_IC3_COOLDOWN);
                        r_ic3Cooldown = DateTime.Now;
                        canLaunchIce = true;
                    }
                    else return;
                    break;
                default:
                    if (lastRocketAttack.AddSeconds(Player.RocketSpeed) < DateTime.Now)
                    {
                        Player.SendCooldown(AmmunitionManager.R_310, Player.Premium ? 1250 : 1250);
                        lastRocketAttack = DateTime.Now;
                    }
                    else return;
                    break;
            }


            Player.AmmunitionManager.UseAmmo(Player.Settings.InGameSettings.selectedRocket, 1);

            /* Player.Skylab.ReduceRocketOre(1); */

            var rocketRunPacket = $"0|v|{Player.Id}|{enemy.Id}|H|{Player.SelectedRocket}|{(Player.SkillTree.rocketFusion == 5 ? 1 : 0)}|{(Player.Storage.PrecisionTargeter || Player.SkillTree.heatseekingMissiles == 5 ? 1 : 0)}";
            Player.SendPacket(rocketRunPacket);
            Player.SendPacketToInRangePlayers(rocketRunPacket);

            Player.RocketInProgress = true;

            if (Player.SelectedRocket != 18)
                await Task.Delay(1000);
            else
                await Task.Delay(500);

            UpdateAttacker(enemy, Player);

            Player.RocketInProgress = false;

            if (Player.RocketMissProbability < Randoms.random.NextDouble() && (!(enemy is Player) || (enemy is Player && (enemy as Player).Attackable())))
            {
                switch (Player.SelectedRocket)
                {
                    case 5:
                        if (canLaunchPLD8 == false) return;

                        enemy.Storage.underPLD8 = true;
                        enemy.Storage.underPLD8Time = DateTime.Now;
                        canLaunchPLD8 = false;

                        if (enemy is Player)
                        {
                            (enemy as Player).SendPacket("0|n|MAL|SET|" + enemy.Id + "");
                            (enemy as Player).SendPacket("0|n|fx|start|PLD_EFFECT|" + enemy.Id + "");

                            enemy.SendPacketToInRangePlayers("0|n|MAL|SET|" + enemy.Id + "");
                            enemy.SendPacketToInRangePlayers("0|n|fx|start|PLD_EFFECT|" + enemy.Id + "");
                        }
                        break;
                    case 6:
                        if (canLaunchwiz_x == false) return;

                        var shipId = Ship.GetRandomShipId(enemy.Ship.Id);
                        canLaunchwiz_x = false;
                        enemy.AddVisualModifier(VisualModifierCommand.WIZARD_ATTACK, 0, GameManager.GetShip(shipId).LootId, 0, true);
                        break;
                    case 18:
                        if (canLaunchIce == false) return;

                        enemy.Storage.underR_IC3 = true;
                        enemy.Storage.underR_IC3Time = DateTime.Now;
                        canLaunchIce = false;

                        if (enemy is Player)
                        {
                            (enemy as Player).SendPacket("0|n|fx|start|ICY_CUBE|" + enemy.Id + "");
                            (enemy as Player).SendCommand(SetSpeedCommand.write(0, 0));
                        }

                        enemy.SendPacketToInRangePlayers("0|n|fx|start|ICY_CUBE|" + enemy.Id + "");
                        break;
                    case 10:
                        if (canLaunchdcr_250 == false) return;

                        enemy.Storage.underDCR_250 = true;
                        enemy.Storage.underDCR_250Time = DateTime.Now;
                        canLaunchdcr_250 = false;

                        if (enemy is Player)
                        {

                            (enemy as Player).SendPacket("0|n|fx|start|SABOTEUR_DEBUFF|" + enemy.Id + "");
                            (enemy as Player).SendCommand(SetSpeedCommand.write(enemy.Speed, enemy.Speed));
                        }

                        enemy.SendPacketToInRangePlayers("0|n|fx|start|SABOTEUR_DEBUFF|" + enemy.Id + "");

                        break;
                    default:
                        var damage = RandomizeDamage(Player.RocketDamage, Player.RocketMissProbability);
                        Damage(Player, enemy, DamageType.ROCKET, damage, 0, false);
                        break;
                }
            }
            else AttackMissed(enemy, DamageType.ROCKET);
        }


        public async void LaunchRocketLauncher()
        {
            var enemy = Player.Selected;
            if (enemy == null) return;
            if (!Player.TargetDefinition(enemy, false)) return;
            if (Player.AmmunitionManager.GetAmmo(Player.Settings.InGameSettings.selectedRocketLauncher) < 5)
            {
                return;
            }

            Player.SendPacket("0|RL|A|" + Player.Id + "|" + enemy.Id + "|" + RocketLauncher.CurrentLoad + "|" + GetSelectedLauncherId());
            Player.SendPacketToInRangePlayers("0|RL|A|" + Player.Id + "|" + enemy.Id + "|" + RocketLauncher.CurrentLoad + "|" + GetSelectedLauncherId());

            Player.AmmunitionManager.UseAmmo(Player.Settings.InGameSettings.selectedRocketLauncher, 5);
            Player.SettingsManager.SendNewItemStatus(CpuManager.ROCKET_LAUNCHER);
            RocketLauncher.LastReloadTime = DateTime.Now;

            int damage = 0;
            DamageType damageType = GetSelectedLauncherId() == (int)DamageType.SHIELD_ABSORBER_ROCKET_URIDIUM ? DamageType.SHIELD_ABSORBER_ROCKET_URIDIUM : DamageType.ROCKET;



            for (var i = 0; i < RocketLauncher.CurrentLoad; i++)
            {
                damage += RandomizeDamage(GetRocketLauncherRocketDamage(), Player.RocketMissProbability);
            }

            RocketLauncher.CurrentLoad = 0;


            if (enemy.Invincible || (enemy is Satellite satellite && satellite.BattleStation.Invincible) || (enemy is Player && !(enemy as Player).Attackable()))
                damage = 0;

            await Task.Delay(1000);

            if (damage != 0)
            {
                if (GetSelectedLauncherId() == (int)DamageType.SHIELD_ABSORBER_ROCKET_URIDIUM)
                    Absorbation(Player, enemy, damageType, damage);
                else
                    Damage(Player, enemy, damageType, damage, 0);
            }

            UpdateAttacker(enemy, Player);
        }

        public void UpdateAttacker(Attackable target, Player player)
        {
            if (!target.Destroyed)
            {
                if (target.MainAttacker == null)
                    target.MainAttacker = player;

                if (!target.Attackers.ContainsKey(player.Id))
                    target.Attackers.TryAdd(player.Id, new Attacker(player));
                else
                    target.Attackers[player.Id].Refresh();
            }
        }

        public DateTime EmpCooldown = new DateTime();
        public void EMP()
        {
            if ((EmpCooldown.AddMilliseconds(35000) < DateTime.Now && Player.AmmunitionManager.GetAmmo("ammunition_specialammo_emp-01") > 0) && Player.SkillTree.empcd == 0 && Player.SkillTree.ishcd == 0 || Player.Storage.GodMode)
            {
                Player.SendCooldown(AmmunitionManager.EMP_01, 35000);
                EmpCooldown = DateTime.Now;

                Player.Storage.DeactiveR_RIC3();
                Player.Storage.DeactiveDCR_250();
                Player.Storage.DeactiveSLM_01();
                Player.Storage.DeactiveDrawFireEffect();

                string empPacket = "0|n|EMP|" + Player.Id;
                Player.AmmunitionManager.UseAmmo("ammunition_specialammo_emp-01", 1);
                Player.SendPacket(empPacket);
                Player.SendPacketToInRangePlayers(empPacket);


                foreach (var otherPlayers in Player.Spacemap.Characters.Values)
                {
                    if (otherPlayers is Player otherPlayer && otherPlayer.Selected == Player)
                        otherPlayer.Deselection(true);

                    if (otherPlayers is Npc npc && npc.Selected == Player)
                    {
                        npc.UnderEmp = true;
                        npc.startemp = DateTime.Now;
                        npc.EmpFrom = Player;
                    }
                }

                foreach (var otherPlayers in Player.InRangeCharacters.Values)
                {
                    if (otherPlayers is Player otherPlayer)
                    {
                        if (otherPlayer.Position.DistanceTo(Player.Position) > 700) continue;

                        //if (otherPlayer.FactionId != Player.FactionId)
                            otherPlayer.CpuManager.DisableCloak();
                    }
                }
            }
            else if ((EmpCooldown.AddMilliseconds(30000) < DateTime.Now && Player.AmmunitionManager.GetAmmo("ammunition_specialammo_emp-01") > 0) && Player.SkillTree.empcd == 1 && Player.SkillTree.ishcd == 0 || Player.Storage.GodMode)
            {
                Player.SendCooldown(AmmunitionManager.EMP_01, 30000);
                EmpCooldown = DateTime.Now;

                Player.Storage.DeactiveR_RIC3();
                Player.Storage.DeactiveDCR_250();
                Player.Storage.DeactiveSLM_01();
                Player.Storage.DeactiveDrawFireEffect();

                string empPacket = "0|n|EMP|" + Player.Id;
                Player.AmmunitionManager.UseAmmo("ammunition_specialammo_emp-01", 1);
                Player.SendPacket(empPacket);
                Player.SendPacketToInRangePlayers(empPacket);


                foreach (var otherPlayers in Player.Spacemap.Characters.Values)
                {
                    if (otherPlayers is Player otherPlayer && otherPlayer.Selected == Player)
                        otherPlayer.Deselection(true);

                    if (otherPlayers is Npc npc && npc.Selected == Player)
                    {
                        npc.UnderEmp = true;
                        npc.startemp = DateTime.Now;
                        npc.EmpFrom = Player;
                    }
                }

                foreach (var otherPlayers in Player.InRangeCharacters.Values)
                {
                    if (otherPlayers is Player otherPlayer)
                    {
                        if (otherPlayer.Position.DistanceTo(Player.Position) > 700) continue;

                        //if (otherPlayer.FactionId != Player.FactionId)
                        otherPlayer.CpuManager.DisableCloak();
                    }
                }
            }
            else if ((EmpCooldown.AddMilliseconds(30000) < DateTime.Now && Player.AmmunitionManager.GetAmmo("ammunition_specialammo_emp-01") > 0) || Player.Storage.GodMode)
            {
                Player.SendCooldown(AmmunitionManager.EMP_01, 35000);
                EmpCooldown = DateTime.Now;

                Player.Storage.DeactiveR_RIC3();
                Player.Storage.DeactiveDCR_250();
                Player.Storage.DeactiveSLM_01();
                Player.Storage.DeactiveDrawFireEffect();

                string empPacket = "0|n|EMP|" + Player.Id;
                Player.AmmunitionManager.UseAmmo("ammunition_specialammo_emp-01", 1);
                Player.SendPacket(empPacket);
                Player.SendPacketToInRangePlayers(empPacket);


                foreach (var otherPlayers in Player.Spacemap.Characters.Values)
                {
                    if (otherPlayers is Player otherPlayer && otherPlayer.Selected == Player)
                        otherPlayer.Deselection(true);

                    if (otherPlayers is Npc npc && npc.Selected == Player)
                    {
                        npc.UnderEmp = true;
                        npc.startemp = DateTime.Now;
                        npc.EmpFrom = Player;
                    }
                }

                foreach (var otherPlayers in Player.InRangeCharacters.Values)
                {
                    if (otherPlayers is Player otherPlayer)
                    {
                        if (otherPlayer.Position.DistanceTo(Player.Position) > 700) continue;

                        //if (otherPlayer.FactionId != Player.FactionId)
                        otherPlayer.CpuManager.DisableCloak();
                    }
                }
            }
        }

        public DateTime SmbCooldown = new DateTime();
        public async Task SMBAsync()
        {
            if (Player.Storage.IsInDemilitarizedZone) return;

            if ((SmbCooldown.AddMilliseconds(TimeManager.SMB_COOLDOWN) < DateTime.Now && Player.AmmunitionManager.GetAmmo("ammunition_mine_smb-01") > 0) || Player.Storage.GodMode)
            {
                Player.SendCooldown(AmmunitionManager.SMB_01, TimeManager.SMB_COOLDOWN);
                SmbCooldown = DateTime.Now;
                Player.AmmunitionManager.UseAmmo("ammunition_mine_smb-01", 1);
                await Task.Delay(1);

                var smbPacket = "0|n|SMB|" + Player.Id;
                Player.SendPacket(smbPacket);
                Player.SendPacketToInRangePlayers(smbPacket);


                foreach (var otherPlayer in Player.InRangeCharacters.Values)
                {
                    if (otherPlayer == null || !(otherPlayer is Player)) continue;
                    if (otherPlayer.Position.DistanceTo(Player.Position) > 700) continue;
                    //if (!Player.TargetDefinition(otherPlayer, false)) continue;
                    if(otherPlayer.Storage.IsInDemilitarizedZone) continue;
                    if (otherPlayer.Spacemap.Options.PvpDisabled) continue;

                    int damageHP = Maths.GetPercentage(otherPlayer.CurrentHitPoints, 20);


                        Damage(Player, otherPlayer as Player, DamageType.MINE, damageHP, true, true, false);
                }
            }
        }
        public DateTime IshCooldownSkill = new DateTime();

        public DateTime IshCooldown = new DateTime();
        public void ISH()
        {
            if ((IshCooldown.AddMilliseconds(30000) < DateTime.Now && Player.AmmunitionManager.GetAmmo("equipment_extra_cpu_ish-01") > 0) && Player.SkillTree.ishcd == 0 && Player.SkillTree.empcd == 0 || Player.Storage.GodMode)
            {
                IshCooldown = DateTime.Now;
                Player.SendCooldown(AmmunitionManager.ISH_01, 30000);

                var ishPacket = "0|n|ISH|" + Player.Id;
                Player.AmmunitionManager.UseAmmo("equipment_extra_cpu_ish-01", 1);
                Player.SendPacket(ishPacket);
                Player.SendPacketToInRangePlayers(ishPacket);
            }
            else if ((IshCooldown.AddMilliseconds(27000) < DateTime.Now && Player.AmmunitionManager.GetAmmo("equipment_extra_cpu_ish-01") > 0) && Player.SkillTree.ishcd == 1 && Player.SkillTree.empcd == 0 || Player.Storage.GodMode)
            {
                IshCooldown = DateTime.Now;
                Player.SendCooldown(AmmunitionManager.ISH_01, 27000);

                var ishPacket = "0|n|ISH|" + Player.Id;
                Player.AmmunitionManager.UseAmmo("equipment_extra_cpu_ish-01", 1);
                Player.SendPacket(ishPacket);
                Player.SendPacketToInRangePlayers(ishPacket);
            }
            else if ((IshCooldown.AddMilliseconds(30000) < DateTime.Now && Player.AmmunitionManager.GetAmmo("equipment_extra_cpu_ish-01") > 0) || Player.Storage.GodMode)
            {
                IshCooldown = DateTime.Now;
                Player.SendCooldown(AmmunitionManager.ISH_01, 30000);

                var ishPacket = "0|n|ISH|" + Player.Id;
                Player.AmmunitionManager.UseAmmo("equipment_extra_cpu_ish-01", 1);
                Player.SendPacket(ishPacket);
                Player.SendPacketToInRangePlayers(ishPacket);
            }

        }

        public void ECI()
        {
            var damage = RandomizeDamage(ChainImpulse.DAMAGE);

            var targets = new Dictionary<int, Character>();

            foreach (var entry in Player.InRangeCharacters.Values)
            {
                if (entry != null && entry is Player && Player.TargetDefinition(entry, false) && entry.Position.DistanceTo(Player.Position) <= 1000)
                    if (targets.Count < 7)
                        targets.Add(entry.Id, entry);
            }

            foreach (var target in targets.Values)
            {
                if (target == null) continue;

                string eciPacket = "0|TX|ECI||" + Player.Id;
                eciPacket += "|" + target.Id;

                Player.SendPacket(eciPacket);
                Player.SendPacketToInRangePlayers(eciPacket);

                Damage(Player, target, DamageType.ECI, damage, true, true, false);
            }
        }

        public static int RandomizeDamage(int baseDmg, double missProbability = 0.5)
        {
            var value = baseDmg;

            switch (Randoms.random.Next(0, 0))
            {
                case 0:
                    value = (int)(baseDmg * 1.10);
                    break;
                case 1:
                    value = (int)(baseDmg * 0.98);
                    break;
                case 2:
                    value = (int)(baseDmg * 1.02);
                    break;
                case 3:
                    value = (int)(baseDmg * 1.05);
                    break;
                case 4:
                    value = (int)(baseDmg * 0.92);
                    break;
                case 5:
                    value = (int)(baseDmg * 0.99);
                    break;
                case 6:
                    value = (int)(baseDmg * 0.97);
                    break;
                default:
                    value = (int)(baseDmg * 1.01);
                    break;
            }

            if (missProbability > Randoms.random.NextDouble())
                value = 0;

            return value;
        }

        public void Absorbation(Player attacker, Attackable target, DamageType damageType, int damage)
        {
            if (attacker.Invincible)
                attacker.Storage.DeactiveInvincibilityEffect();

            Player.CpuManager.DisableCloak();

            if (target is Player && !(target as Player).Attackable())
            {

                damage = 0;
            }
            if ((target.CurrentShieldPoints - damage) < 0)
                damage = target.CurrentShieldPoints;

            target.CurrentShieldPoints -= damage;
            Player.CurrentShieldPoints += damage;
            target.LastCombatTime = DateTime.Now;

            if (damageType == DamageType.LASER)
            {
                var laserRunCommand = AttackLaserRunCommand.write(Player.Id, target.Id, mult != 0 ? 0 : GetSelectedLaser(), false, Player.bountyHunterAquired);
                Player.SendCommand(laserRunCommand);

                Player.SendCommandToSelected(target, laserRunCommand);
            }

            if (damage == 0)
            {
                if (damageType == DamageType.LASER)
                    AttackMissed(target, damageType);
            }
            else
            {

                var attackHitCommand =
                AttackHitCommand.write(new AttackTypeModule((short)damageType), Player.Id,
                     target.Id, target.CurrentHitPoints,
                     target.CurrentShieldPoints, target.CurrentNanoHull,
                     damage, false);

                Player.SendCommand(attackHitCommand);
                Player.SendCommandToSelected(target, attackHitCommand);
            }

            target.UpdateStatus();
            Player.UpdateStatus();
        }

        public void Damage(Player attacker, Attackable target, DamageType damageType, int damage, double shieldPenetration, bool deactiveCloak = true)
        {
            if (damageType == DamageType.MINE && target.Invincible) return;

            /*if ((attacker.Spacemap.Id == 306 || attacker.Spacemap.Id == 307 || attacker.Spacemap.Id == 308) && attacker.Level <= 23 && target is Npc npcInfo)
            {
                damage *= 3;
            }*/
            if (target is Player)
            {
                damage += Maths.GetPercentage(damage, attacker.GetSkillPercentage("Luck"));
            }

            if (target is Npc npcinfo2 || target is NpcGG npcinfo3)
            {
                damage += Maths.GetPercentage(damage, attacker.GetSkillPercentage("Explosives"));
            }

            if (damageType == DamageType.LASER && Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.SAB_50)
            {
                Absorbation(attacker, target, damageType, damage);
                return;
            }

            int damageShd = 0, damageHp = 0;

            if (attacker.Invincible)
                attacker.Storage.DeactiveInvincibilityEffect();

            if (target is Spaceball)
            {
                var spaceball = target as Spaceball;
                spaceball.AddDamage(attacker, damage);
            }
            if (target is Hitac)
            {
                var spaceball = target as Hitac;
                spaceball.AddDamage(attacker, damage);
            }
            if (target is Npc && target.Name == "..::{ BADASS-NPC }::..")
            {
                (target as Npc).AddDamage(attacker, damage);
            }

            double shieldAbsorb = System.Math.Abs(target.ShieldAbsorption - shieldPenetration);

            if (shieldAbsorb > 1)
                shieldAbsorb = 1;

            if ((target.CurrentShieldPoints - damage) >= 0)
            {
                damageShd = (int)(damage * shieldAbsorb);
                damageHp = damage - damageShd;
            }
            else
            {
                int newDamage = damage - target.CurrentShieldPoints;
                damageShd = target.CurrentShieldPoints;
                damageHp = (int)(newDamage + (damageShd * shieldAbsorb));
            }

            if ((target.CurrentHitPoints - damageHp) < 0)
            {
                damageHp = target.CurrentHitPoints;
            }

            if (target.Invincible || (target is Satellite satellite && satellite.BattleStation.Invincible) || (target is Player && !(target as Player).Attackable()))
            {
                damage = 0;
                damageShd = 0;
                damageHp = 0;
            }

            if (target is NPCFlagship)
            {
                if (!(target as NPCFlagship).canBeAttacked)
                {
                    //Console.WriteLine("DAMAGE = 0");
                    damage = 0;
                    damageShd = 0;
                    damageHp = 0;
                }
            }

            if (deactiveCloak)
                Player.CpuManager.DisableCloak();

            //sharesystem adding
            if (target is Npc n && (n.Ship.Id == 80 || n.Ship.Id == 33 || n.Ship.Id == 101 || n.Ship.Id == 90 || n.Ship.Id == 94 || n.Ship.Id == 81 || n.Ship.Id == 103 || n.Spacemap.Id == 61 || n.Spacemap.Id == 16 || n.Spacemap.Id == 55 || n.Spacemap.Id == 58))
            {
                bool facadeExist = false;
                int facadeIndex = 0;

                for (int i = n.causedDamage.Count - 1; i >= 0; i--)
                {
                    NpcDamageCause facade = n.causedDamage[i];
                    if (facade != null)
                    {
                        if (facade.player == Player)
                        {
                            facadeExist = true;
                            facadeIndex = i;
                            break;
                        }
                    }
                }

                if (facadeExist)
                {
                    n.causedDamage[facadeIndex].damage += damage;
                }
                else
                {
                    NpcDamageCause tmp = new NpcDamageCause(Player, damage);
                    n.causedDamage.Add(tmp);
                }

                n.overallCausedDamage += damage;
            }

            if (damageType == DamageType.LASER)
            {
                if (target is Player && (target as Player).Storage.Sentinel)
                    damageShd -= Maths.GetPercentage(damageShd, 50);

                if (Player.Storage.Diminisher)
                    if (target == Player.Storage.UnderDiminisherEntity)
                        damageShd += Maths.GetPercentage(damage, 20);

                if (target is Player && (target as Player).Storage.Diminisher)
                    if ((target as Player).Storage.UnderDiminisherEntity == Player)
                        damageShd += Maths.GetPercentage(damage, 20);

                target.AddDamage(attacker, damage);

                bool shieldMechanics = false;

                if (target is Player p)
                {
                    if (p.shieldMechanics) shieldMechanics = true;
                }

                var laserRunCommand = AttackLaserRunCommand.write(Player.Id, target.Id, GetSelectedLaser(), shieldMechanics, Player.bountyHunterAquired);
                if (Player.lf4lasers > 0 && full)
                {
                    laserRunCommand = AttackLaserRunCommand.write(Player.Id, target.Id, mult != 0 ? 0 : GetSelectedLaser(), shieldMechanics, Player.bountyHunterAquired);
                }
                else if (Player.lf4lasers / 2 > 0 && !full)
                {
                    laserRunCommand = AttackLaserRunCommand.write(Player.Id, target.Id, mult != 0 ? 1 : GetSelectedLaser(), shieldMechanics, Player.bountyHunterAquired);
                }
                if (Player.lf5lasers / 2 > 0 && fulllf5)
                {
                    laserRunCommand = AttackLaserRunCommand.write(Player.Id, target.Id, GetSelectedLaser(), shieldMechanics, Player.bountyHunterAquired);
                    if (proc)
                    {
                        laserRunCommand = AttackLaserRunCommand.write(Player.Id, target.Id, 5, shieldMechanics, Player.bountyHunterAquired);
                    }
                    if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75)
                    {
                        laserRunCommand = AttackLaserRunCommand.write(Player.Id, target.Id, GetSelectedLaser(), shieldMechanics, Player.bountyHunterAquired);
                    }
                }

                Player.SendCommand(laserRunCommand);
                Player.SendCommandToInRangePlayers(laserRunCommand);

                if (Player.Settings.InGameSettings.selectedLaser != AmmunitionManager.RSB_75 && Player.Settings.InGameSettings.selectedLaser != AmmunitionManager.CBO_100)
                {
                    if (Player.Storage.EnergyLeech)
                        Player.TechManager.EnergyLeech.ExecuteHeal(damage);
                }
            }
            if (target is Pet pet)
            {
                if (pet.CurrentConfig == 1)
                {
                    if (pet.CurrentShieldConfig1 > 0)
                        damageHp = 0;

                }
                else
                {
                    if (pet.CurrentShieldConfig2 > 0)
                        damageHp = 0;
                }
            }

            //if (target is npc && (target as npc).ship.id == 80)
            //{
            //    int rand = randoms.random.next(1, 100);

            //    if (rand <= 1 && rand < 35 && (target as npc).lastempowered.addseconds(130) < datetime.now && !(target as npc).empowered)
            //    {
            //        (target as npc).empower();
            //    }
            //    if ((target as npc).empowered)
            //    {
            //        target.heal(damage);
            //        target.currentshieldpoints += damage;
            //    }
            //}


            if (damage == 0)
            {
                if (damageType == DamageType.LASER || damageType == DamageType.ROCKET)
                {
                    AttackMissed(target, damageType);


                }

            }
            else
            {
                if (target is Npc)
                    (target as Npc).ReceiveAttack(Player);

                if (target is NpcGG)
                    (target as NpcGG).ReceiveAttack(Player);

                if (target is Npcx2)
                    (target as Npcx2).ReceiveAttack(Player);
                if (target is NPCFlagship)
                    (target as NPCFlagship).ReceiveAttack(Player);



                var attackHitCommand =
                        AttackHitCommand.write(new AttackTypeModule((short)damageType), Player.Id,
                                             target.Id, target.CurrentHitPoints,
                                             target.CurrentShieldPoints, target.CurrentNanoHull,
                                             damage > damageShd ? damage : damageShd, false);

                Player.SendCommand(attackHitCommand);

                Player.SendCommandToSelected(target, attackHitCommand);
            }

            if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.CBO_100)
            {
                var sabDamage = RandomizeDamage(2 * Player.Damage, (Player.Storage.underPLD8 ? 0.5 : 0.1));

                if (Player.Storage.Spectrum)
                    sabDamage -= Maths.GetPercentage(sabDamage, 50);

                if (target is Player)
                {
                    if ((target as Player).Storage.Spectrum)
                        sabDamage -= Maths.GetPercentage(sabDamage, 50);
                }

                Player.CurrentShieldPoints += sabDamage;
            }

            if (damageHp >= target.CurrentHitPoints || target.CurrentHitPoints <= 0)
                target.Destroy(Player, DestructionType.PLAYER);
            else
            {
                if (target.CurrentNanoHull > 0)
                {
                    if (target.CurrentNanoHull - damageHp < 0)
                    {
                        var nanoDamage = damageHp - target.CurrentNanoHull;
                        target.CurrentNanoHull = 0;
                        target.CurrentHitPoints -= nanoDamage;
                    }
                    else
                        target.CurrentNanoHull -= damageHp;
                }
                else
                    target.CurrentHitPoints -= damageHp;

                target.CurrentShieldPoints -= damageShd;
                target.LastCombatTime = DateTime.Now;
            }

            if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.CBO_100)
                Player.UpdateStatus();

            target.UpdateStatus();
        }

        public static void Damage(Player attacker, Attackable target, DamageType damageType, int damage, bool toDestroy, bool toHp, bool toShd, bool missedEffect = true, int toSHD = 0)
        {
            if (damageType == DamageType.MINE && target.Invincible) return;

            if (attacker.Invincible && damageType != DamageType.RADIATION)
                attacker.Storage.DeactiveInvincibilityEffect();

            if (target is Player && !(target as Player).Attackable())
            {
                if (missedEffect)
                {
                    var attackMissedCommandToInRange = AttackMissedCommand.write(new AttackTypeModule((short)damageType), target.Id, 0);
                    var attackMissedCommand = AttackMissedCommand.write(new AttackTypeModule((short)damageType), target.Id, 0);
                    attacker.SendCommand(attackMissedCommand);

                    attacker.SendCommandToSelected(target, attackMissedCommandToInRange);
                }
                damage = 0;
                return;
            }

            target.LastCombatTime = DateTime.Now;

            if (toHp && toDestroy && (damage >= target.CurrentHitPoints || target.CurrentHitPoints <= 0))
            {
                if (damageType == DamageType.RADIATION)
                    target.Destroy(null, DestructionType.RADIATION);
                else if (damageType == DamageType.MINE && attacker.Attackers.Count <= 0)
                    target.Destroy(null, DestructionType.MINE);
                else
                    target.Destroy(attacker, DestructionType.PLAYER);
            }
            else if (toHp)
            {
                if (target.CurrentNanoHull > 0)
                {
                    if (target.CurrentNanoHull - damage < 0)
                    {
                        var nanoDamage = damage - target.CurrentNanoHull;
                        target.CurrentNanoHull = 0;
                        target.CurrentHitPoints -= nanoDamage;
                    }
                    else
                        target.CurrentNanoHull -= damage;
                }
                else
                {
                    int hp = Maths.GetPercentage(damage, 40);
                    int shd = Maths.GetPercentage(damage, 60);
                    if (target.CurrentShieldPoints - shd < 0)
                    {
                        target.CurrentShieldPoints = 0;
                        target.CurrentHitPoints -= Math.Abs(target.CurrentShieldPoints - shd);
                    }
                    else
                    {
                        target.CurrentHitPoints -= hp;
                        target.CurrentShieldPoints -= shd;
                    }
                }
            }

            if (toShd)
            {
                if (target.CurrentShieldPoints - toSHD < 0)
                {
                    target.CurrentShieldPoints = 0;
                    target.CurrentHitPoints -= Math.Abs(target.CurrentShieldPoints - toSHD);
                }
                target.CurrentShieldPoints -= toSHD;
            }

            var attackHitCommand =
                    AttackHitCommand.write(new AttackTypeModule((short)damageType), attacker.Id,
                                         target.Id, target.CurrentHitPoints,
                                         target.CurrentShieldPoints, target.CurrentNanoHull,
                                         damage, false);

            attacker.SendCommand(attackHitCommand);
            attacker.SendCommandToSelected(target, attackHitCommand);

            target.UpdateStatus();
        }

        public void AttackMissed(Attackable target, DamageType damageType)
        {
            if (scatter)
                miss = true;
            var attackMissedCommand = AttackMissedCommand.write(new AttackTypeModule((short)damageType), target.Id, 0);
            var attackMissedCommandToInRange = AttackMissedCommand.write(new AttackTypeModule((short)damageType), target.Id, 1);
            Player.SendCommand(attackMissedCommand);

            Player.SendCommandToSelected(target, attackMissedCommandToInRange);
        }

        private bool CheckLaserAttackTime()
        {
            if (Player.Settings.InGameSettings.selectedLaser == AmmunitionManager.RSB_75)
            {
                return lastRSBAttackTime.AddSeconds(3) < DateTime.Now;
            }
            return lastAttackTime.AddSeconds(1) < DateTime.Now;
        }

        public int GetRocketRange()
        {
            switch (Player.Settings.InGameSettings.selectedRocket)
            {
                case AmmunitionManager.R_310:
                    return 600;
                case AmmunitionManager.PLT_2026:
                    return 600;
                case AmmunitionManager.PLT_2021:
                    return 600;
                case AmmunitionManager.PLT_3030:
                    return 600;
                case AmmunitionManager.PLD_8:
                case AmmunitionManager.DCR_250:
                    return 675;
                case AmmunitionManager.R_IC3:
                    return 700;
                case AmmunitionManager.WIZ_X:
                    return 600;
                default:
                    return 0;
            }
        }

        private int GetSelectedRocket()
        {
            switch (Player.Settings.InGameSettings.selectedRocket)
            {
                case AmmunitionManager.R_310:
                    return 1;
                case AmmunitionManager.PLT_2026:
                    return 2;
                case AmmunitionManager.PLT_2021:
                    return 3;
                case AmmunitionManager.PLT_3030:
                    return 4;
                case AmmunitionManager.PLD_8:
                    return 5;
                case AmmunitionManager.WIZ_X:
                    return 6;
                case AmmunitionManager.DCR_250:
                    return 10;
                case AmmunitionManager.R_IC3:
                    return 18;
                default:
                    return 0;
            }
        }

        public int GetSelectedLauncherId()
        {
            switch (Player.Settings.InGameSettings.selectedRocketLauncher)
            {
                case AmmunitionManager.HSTRM_01:
                    return 7;
                case AmmunitionManager.UBR_100:
                    return 8;
                case AmmunitionManager.ECO_10:
                    return 9;
                case AmmunitionManager.SAR_01:
                    return 12;
                case AmmunitionManager.SAR_02:
                    return 13;
                case AmmunitionManager.CBR:
                    return 14;
                default:
                    return 7;
            }
        }

        public int GetRocketDamage()
        {
            if (EventManager.Getx2RocketEvent() == true)
            {
                switch (Player.Settings.InGameSettings.selectedRocket)
                {
                    case AmmunitionManager.R_310:
                        return 1400;
                    case AmmunitionManager.PLT_2026:
                        return 2000;
                    case AmmunitionManager.PLT_2021:
                        return 2250;
                    case AmmunitionManager.PLT_3030:
                        return 4300;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (Player.Settings.InGameSettings.selectedRocket)
                {
                    case AmmunitionManager.R_310:
                        return 700; //1000
                    case AmmunitionManager.PLT_2026:
                        return 1000; //1500
                    case AmmunitionManager.PLT_2021:
                        return 1500; //2500
                    case AmmunitionManager.PLT_3030:
                        return 2500; //3500
                    default:
                        return 0;
                }
            }
        }

        private int GetRocketLauncherRocketDamage()
        {
            switch (Player.Settings.InGameSettings.selectedRocketLauncher)
            {
                case AmmunitionManager.ECO_10:
                    return 2000;
                case AmmunitionManager.HSTRM_01:
                    return 4000;
                case AmmunitionManager.UBR_100:
                    return 3200;
                case AmmunitionManager.SAR_01:
                    return 3200;
                case AmmunitionManager.SAR_02:
                    return 3300;
                default:
                    return 0;
            }
        }

        private int GetDamageMultiplier()
        {

            switch (Player.Settings.InGameSettings.selectedLaser)
            {
                case AmmunitionManager.LCB_10:
                    return 1;
                case AmmunitionManager.MCB_25:
                    return 2;
                case AmmunitionManager.CBO_100:
                case AmmunitionManager.MCB_50:
                    return 3;
                case AmmunitionManager.UCB_100:
                    return 4;
                case AmmunitionManager.RSB_75:
                    return 6;
                case AmmunitionManager.SAB_50:
                    return 2;
                case AmmunitionManager.JOB_100:
                    return 3;
                case AmmunitionManager.RB_214:
                    return 4;
                case AmmunitionManager.MCB_100:
                    return 5;
                case AmmunitionManager.MCB_250:
                    return 7;
                case AmmunitionManager.MCB_500:
                    return 4;
                default:
                    return 1;
            }
        }

        public int GetSelectedLaser()
        {
            switch (Player.Settings.InGameSettings.selectedLaser)
            {
                case AmmunitionManager.LCB_10:
                    return 0;
                case AmmunitionManager.MCB_25:
                    return 1;
                case AmmunitionManager.MCB_50:
                    return 2;
                case AmmunitionManager.UCB_100:
                    return 3;
                case AmmunitionManager.SAB_50:
                    return 4;
                case AmmunitionManager.RSB_75:
                    return 6;
                case AmmunitionManager.CBO_100:
                    return 8;
                case AmmunitionManager.JOB_100:
                    return 9;
                case AmmunitionManager.RB_214:
                    return 5;
                case AmmunitionManager.MCB_100:
                    return 2;
                case AmmunitionManager.MCB_250:
                    return 8;
                case AmmunitionManager.MCB_500:
                    return 5;
                default:
                    return 0;
            }
        }
    }
}

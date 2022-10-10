using Newtonsoft.Json;
using System;

namespace Darkorbit.Game.Objects.Players.Managers
{
    class AmmunitionManager : AbstractManager
    {
        public const string LCB_10 = "ammunition_laser_lcb-10";
        public const string MCB_25 = "ammunition_laser_mcb-25";
        public const string MCB_50 = "ammunition_laser_mcb-50";
        public const string MCB_100 = "ammunition_laser_mcb-100";
        public const string MCB_250 = "ammunition_laser_mcb-250";
        public const string MCB_500 = "ammunition_laser_mcb-500";
        public const string UCB_100 = "ammunition_laser_ucb-100";
        public const string SAB_50 = "ammunition_laser_sab-50";
        public const string CBO_100 = "ammunition_laser_cbo-100";
        public const string RSB_75 = "ammunition_laser_rsb-75";
        public const string JOB_100 = "ammunition_laser_job-100";
        public const string RB_214 = "ammunition_laser_rb-214";
        public const string R_310 = "ammunition_rocket_r-310";
        public const string PLT_2026 = "ammunition_rocket_plt-2026";
        public const string PLT_2021 = "ammunition_rocket_plt-2021";
        public const string PLT_3030 = "ammunition_rocket_plt-3030";
        public const string PLD_8 = "ammunition_specialammo_pld-8";
        public const string DCR_250 = "ammunition_specialammo_dcr-250";
        public const string WIZ_X = "ammunition_specialammo_wiz-x";
        public const string BDR_1211 = "ammunition_rocket_bdr-1211";
        public const string BDR_1212 = "ammunition_rocket_bdr-1212";
        public const string R_IC3 = "ammunition_specialammo_r-ic3";
        public const string HSTRM_01 = "ammunition_rocketlauncher_hstrm-01";
        public const string UBR_100 = "ammunition_rocketlauncher_ubr-100";
        public const string ECO_10 = "ammunition_rocketlauncher_eco-10";
        public const string SAR_01 = "ammunition_rocketlauncher_sar-01";
        public const string SAR_02 = "ammunition_rocketlauncher_sar-02";
        public const string CBR = "ammunition_rocketlauncher_cbr";
        public const string BDR1212 = "ammunition_rocketlauncher_bdr1212";
        public const string EMP_01 = "ammunition_specialammo_emp-01";
        public const string FWX_COM = "ammunition_firework_fwx-com";
        public const string FWX_L = "ammunition_firework_fwx-l";
        public const string FWX_M = "ammunition_firework_fwx-m";
        public const string FWX_RZ = "ammunition_firework_fwx-rz";
        public const string FWX_S = "ammunition_firework_fwx-s";
        public const string ACM_01 = "ammunition_mine_acm-01";
        public const string DDM_01 = "ammunition_mine_ddm-01";
        public const string EMPM_01 = "ammunition_mine_empm-01";
        public const string SABM_01 = "ammunition_mine_sabm-01";
        public const string SLM_01 = "ammunition_mine_slm-01";
        public const string IM_01 = "ammunition_mine_im-01";
        public const string SMB_01 = "ammunition_mine_smb-01";
        public const string ISH_01 = "equipment_extra_cpu_ish-01";
        public const string ROCKET_LAUNCHER = "equipment_weapon_rocketlauncher_hst-2";
        public const string FIREWORK_IGNITE = "ammunition_firework_ignite";
        public const string CLK_XL = "equipment_extra_cpu_cl04k-xl";


        public int ucb { get; set; }
        public int hstrm { get; set; }
        public int sar2 { get; set; }
        public int rsb { get; set; }
        public int sab { get; set; }
        public int pib { get; set; }
        public int ish { get; set; }
        public int emp { get; set; }
        public int smb { get; set; }
        public int plt3030 { get; set; }
        public int ice { get; set; }
        public int dcr { get; set; }
        public int wiz { get; set; }
        public int pld { get; set; }
        public int slm { get; set; }
        public int ddm { get; set; }
        public int empm { get; set; }
        public int sabm { get; set; }
        public int cloacks { get; set; }
        public int mcb50 { get; set; }
        public int mcb25 { get; set; }
        public int cbo100 { get; set; }
        public int job100 { get; set; }
        public int rb214 { get; set; }
        public int mcb100 { get; set; }
        public int mcb250 { get; set; }
        public int mcb500 { get; set; }

        public int lcb10 { get; set; }
        public int r310 { get; set; }
        public int plt26 { get; set; }
        public int plt21 { get; set; }
        public int eco { get; set; }


        public AmmunitionManager(Player player) : base(player)
        {

            using (var mySqlClient = SqlDatabaseManager.GetClient())
            {
                var querySet = mySqlClient.ExecuteQueryRow($"SELECT * FROM player_accounts WHERE userId = {Player.Id}");
                dynamic ammo = JsonConvert.DeserializeObject(querySet["ammo"].ToString());

                ucb = ammo["ucb"];
                cbo100 = ammo["cbo100"];
                job100 = ammo["job100"];
                rb214 = ammo["rb214"];
                mcb100 = ammo["mcb100"];
                mcb250 = ammo["mcb250"];
                mcb500 = ammo["mcb500"];
                hstrm = ammo["hstrm"];
                sar2 = ammo["sar2"];
                rsb = ammo["rsb"];
                sab = ammo["sab"];
                pib = ammo["pib"];
                ish = ammo["ish"];
                emp = ammo["emp"];
                smb = ammo["smb"];
                plt3030 = ammo["plt3030"];
                ice = ammo["ice"];
                dcr = ammo["dcr"];
                wiz = ammo["wiz"];
                pld = ammo["pld"];
                slm = ammo["slm"];
                ddm = ammo["ddm"];
                empm = ammo["empm"];
                sabm = ammo["sabm"];
                cloacks = ammo["cloacks"];
                mcb50 = ammo["mcb50"];
                mcb25 = (ammo["mcb25"] != null) ? ammo["mcb25"] : 99999;
                lcb10 = (ammo["lcb10"] != null) ? ammo["lcb10"] : 99999;
                r310 = (ammo["r310"] != null) ? ammo["r310"] : 500;
                plt26 = (ammo["plt26"] != null) ? ammo["plt26"] : 500;
                plt21 = (ammo["plt21"] != null) ? ammo["plt21"] : 500;
                eco = ammo["eco"];
            }



        }

        public int GetAmmo(string itemId)
        {
            switch (itemId)
            {
                case AmmunitionManager.R_310:
                    return r310;
                case AmmunitionManager.PLT_2026:
                    return plt26;
                case AmmunitionManager.PLT_2021:
                    return plt21;
                case AmmunitionManager.MCB_50:
                    return mcb50;
                case AmmunitionManager.MCB_100:
                    return mcb100;
                case AmmunitionManager.MCB_250:
                    return mcb250;
                case AmmunitionManager.MCB_500:
                    return mcb500;
                case AmmunitionManager.MCB_25:
                    return mcb25;
                case AmmunitionManager.LCB_10:
                    return lcb10;
                case AmmunitionManager.HSTRM_01:
                    return hstrm;
                case AmmunitionManager.SAR_02:
                    return sar2;
                case AmmunitionManager.UCB_100:
                    return ucb;
                case AmmunitionManager.SAB_50:
                    return sab;
                case AmmunitionManager.RSB_75:
                    return rsb;
                case AmmunitionManager.ISH_01:
                    return ish;
                case AmmunitionManager.EMP_01:
                    return emp;
                case AmmunitionManager.PLT_3030:
                    return plt3030;
                case AmmunitionManager.SMB_01:
                    return smb;
                case AmmunitionManager.R_IC3:
                    return ice;
                case AmmunitionManager.DCR_250:
                    return dcr;
                case AmmunitionManager.WIZ_X:
                    return wiz;
                case AmmunitionManager.PLD_8:
                    return pld;
                case AmmunitionManager.CBO_100:
                    return cbo100;
                case AmmunitionManager.JOB_100:
                    return job100;
                case AmmunitionManager.RB_214:
                    return rb214;
                case AmmunitionManager.CLK_XL:
                    return cloacks;
                case AmmunitionManager.ECO_10:
                    return eco;
                default:
                    return 0;

            }
        }

        public void UseAmmo(string itemId, int amount)
        {
            switch (itemId)
            {
                case AmmunitionManager.R_310:
                    r310 -= amount;
                    break;
                case AmmunitionManager.PLT_2021:
                    plt21 -= amount;
                    break;
                case AmmunitionManager.PLT_2026:
                    plt26 -= amount;
                    break;
                case AmmunitionManager.MCB_50:
                    mcb50 -= amount;
                    break;
                case AmmunitionManager.MCB_100:
                    mcb100 -= amount;
                    break;
                case AmmunitionManager.MCB_250:
                    mcb250 -= amount;
                    break;
                case AmmunitionManager.MCB_500:
                    mcb500 -= amount;
                    break;
                case AmmunitionManager.MCB_25:
                    mcb25 -= amount;
                    break;
                case AmmunitionManager.LCB_10:
                    lcb10 -= amount;
                    break;
                case AmmunitionManager.HSTRM_01:
                    hstrm -= amount;
                    break;
                case AmmunitionManager.SAR_02:
                    sar2 -= amount;
                    break;
                case AmmunitionManager.UCB_100:
                    ucb -= amount;
                    break;
                case AmmunitionManager.SAB_50:
                    sab -= amount;
                    break;
                case AmmunitionManager.RSB_75:
                    rsb -= amount;
                    break;
                case AmmunitionManager.ISH_01:
                    ish -= amount;
                    break;
                case AmmunitionManager.EMP_01:
                    emp -= amount;
                    break;
                case AmmunitionManager.PLT_3030:
                    plt3030 -= amount;
                    break;
                case AmmunitionManager.SMB_01:
                    smb -= amount;
                    break;
                case AmmunitionManager.R_IC3:
                    ice -= amount;
                    break;
                case AmmunitionManager.DCR_250:
                    dcr -= amount;
                    break;
                case AmmunitionManager.WIZ_X:
                    wiz -= amount;
                    break;
                case AmmunitionManager.PLD_8:
                    pld -= amount;
                    break;
                case AmmunitionManager.CBO_100:
                    cbo100 -= amount;
                    break;
                case AmmunitionManager.JOB_100:
                    job100 -= amount;
                    break;
                case AmmunitionManager.RB_214:
                    rb214 -= amount;
                    break;
                case AmmunitionManager.CLK_XL:
                    cloacks -= amount;
                    break;
                case AmmunitionManager.ECO_10:
                    eco -= amount;
                    break;

            }
            Player.SettingsManager.SendNewItemStatus(itemId);
        }

        public void AddAmmo(string itemId, int amount)
        {
            string name = "";
            switch (itemId)
            {
                case AmmunitionManager.R_310:
                    r310 += amount;
                    name = "R_310";
                    break;
                case AmmunitionManager.PLT_2021:
                    plt21 += amount;
                    name = "PLT-2021";
                    break;
                case AmmunitionManager.PLT_2026:
                    plt26 += amount;
                    name = "PLT-2026";
                    break;
                case AmmunitionManager.MCB_50:
                    mcb50 += amount;
                    name = "MCB-50";
                    break;
                case AmmunitionManager.MCB_100:
                    mcb100 += amount;
                    name = "MCB-100";
                    break;
                case AmmunitionManager.MCB_250:
                    mcb250 += amount;
                    name = "MCB-250";
                    break;
                case AmmunitionManager.MCB_500:
                    mcb500 += amount;
                    name = "MCB-500";
                    break;
                case AmmunitionManager.HSTRM_01:
                    hstrm += amount;
                    name = "HSTRM-01";
                    break;
                case AmmunitionManager.MCB_25:
                    mcb25 += amount;
                    name = "MCB-25";
                    break;
                case AmmunitionManager.LCB_10:
                    lcb10 += amount;
                    name = "LCB-10";
                    break;
                case AmmunitionManager.SAR_02:
                    sar2 += amount;
                    name = "SAR-02";
                    break;
                case AmmunitionManager.UCB_100:
                    ucb += amount;
                    name = "UCB-100";
                    break;
                case AmmunitionManager.SAB_50:
                    sab += amount;
                    name = "SAB-50";
                    break;
                case AmmunitionManager.RSB_75:
                    rsb += amount;
                    name = "RSB-75";
                    break;
                case AmmunitionManager.ISH_01:
                    ish += amount;
                    name = "ISH-01";
                    break;
                case AmmunitionManager.EMP_01:
                    emp += amount;
                    name = "EMP-01";
                    break;
                case AmmunitionManager.PLT_3030:
                    plt3030 += amount;
                    name = "PLT-3030";
                    break;
                case AmmunitionManager.SMB_01:
                    smb += amount;
                    name = "SMB-01";
                    break;
                case AmmunitionManager.R_IC3:
                    ice += amount;
                    name = "R-IC3";
                    break;
                case AmmunitionManager.DCR_250:
                    dcr += amount;
                    name = "DCR-250";
                    break;
                case AmmunitionManager.WIZ_X:
                    wiz += amount;
                    name = "WIZ-X";
                    break;
                case AmmunitionManager.PLD_8:
                    pld += amount;
                    name = "PLD-8";
                    break;
                case AmmunitionManager.CBO_100:
                    cbo100 += amount;
                    name = "CBO-100";
                    break;
                case AmmunitionManager.JOB_100:
                    job100 += amount;
                    name = "JOB-100";
                    break;
                case AmmunitionManager.RB_214:
                    rb214 += amount;
                    name = "RB-214";
                    break;
                case AmmunitionManager.CLK_XL:
                    cloacks += amount;
                    name = "CLKL";
                    break;
                case AmmunitionManager.ECO_10:
                    eco += amount;
                    name = "ECO-10";
                    break;
            }
            Player.SettingsManager.SendNewItemStatus(itemId);
            if(amount > 0) Player.SendPacket($"0|A|STD|You received {String.Format("{0:n0}", amount)} {name}");
            QueryManager.SavePlayer.Ammunition(Player);
        }
    }
}

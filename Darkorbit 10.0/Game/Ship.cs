
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game
{
    class ShipRewards
    {
        public int Experience { get; set; }
        public int Honor { get; set; }
        public int Credits { get; set; }
        public int Uridium { get; set; }
        public int Logfiles { get; set; }
    }

    class Ship
    {
        public static int SPACEBALL_SUMMER = 442;
        public static int SPACEBALL_WINTER = 443;
        public static int SPACEBALL_SOCCER = 444;
        public static int SHIP116 = 116;

        public const int GOLIATH = 10;
        public const int GOLIATH_ENFORCER = 56;
        public const int GOLIATH_INDENPENDENCE = 57;
        public const int GOLIATH_BASTION = 59;
        public const int GOLIATH_VETERAN = 61;
        public const int GOLIATH_EXALTED = 62;
        public const int GOLIATH_SOLACE = 63;
        public const int GOLIATH_VENOM = 67;
        public const int GOLIATH_KICK = 86;
        public const int GOLIATH_REFEREE = 87;
        public const int GOLIATH_GOAL = 88;
        public const int GOLIATH_SATURN = 109;
        public const int GOLIATH_CENTAUR = 110;
        public const int GOLIATH_RAZER = 153;
        public const int GOLIATH_ORION = 170;
        public const int GOLIATH_DUSKLIGHT = 317;
        public const int GOLIATH_BRONZE = 498;
        public const int GOLIATH_SILVER = 499;
        public const int GOLIATH_GOLD = 500;
        public const int GOLIATH_IRON = 321;
        public const int GOLIATH_ENFORCER_BONUS = 450;

        public const int SENTINEL_ASIMOV = 343;
        public const int SENTINEL_ARIOS = 344;
        public const int SENTINEL_NEIKOS = 345;
        public const int SENTINEL_LAVA = 346;
        public const int SENTINEL_TYRANNOS = 347;
        public const int SENTINEL_CONTAGION = 377;
        public const int SENTINEL_HARBINGER = 394;
        public const int SENTINEL_ULLRIN = 397;

        public const int SPECTRUM_TYRANNOS = 349;
        public const int SPECTRUM_ARGON = 378;
        public const int SPECTRUM_INFERNO = 286;

        public const int DIMINISHER_FROST = 360;
        public const int DIMINISHER_PHANTASM = 380;
        public const int DIMINISHER_ULLRIN = 381;
        public const int DIMINISHER_PROMETHEUS = 384;
        public const int DIMINISHER_CARBONITE = 453;

        public const int SURGEON = 156;
        public const int SURGEON_CICADA = 361;
        public const int SURGEON_LOCUST = 362;

        public const int CHAMPION_LAVA = 363;
        public const int CHAMPION_ARGON = 364;
        public const int CHAMPION_LEGEND = 365;
        public const int CHAMPION_TYRANNOS = 366;

        public const int VENOM_ARGON = 350;
        public const int VENOM_BLAZE = 351;
        public const int VENOM_BOREALIS = 352;
        public const int VENOM_OCEAN = 353;
        public const int VENOM_POISON = 354;

        public const int SOLACE_FROST = 260;
        public const int SOLACE_ASIMOV = 261;
        public const int SOLACE_ARGON = 262;
        public const int SOLACE_BLAZE = 263;
        public const int SOLACE_BOREALIS = 264;
        public const int SOLACE_OCEAN = 340;
        public const int SOLACE_POISON = 341;
        public const int SOLACE_TYRANNOS = 342;
        public const int SOLACE_CONTAGION = 376;
        public const int SOLACE_NOBILIS = 411;
        public const int SOLACE_ULLRIN = 414;


        public const int CYBORG = 245;
        public const int CYBORG_INFINITE = 281;
        public const int CYBORG_LAVA = 249;
        public const int CYBORG_CARBONITE = 273;
        public const int CYBORG_FIRESTAR = 274;
        public const int CYBORG_NOBILIS = 275;
        public const int CYBORG_SCOURGE = 276;
        public const int CYBORG_INFERNO = 277;
        public const int CYBORG_ULLRIN = 278;
        public const int CYBORG_DUSKLIGHT = 279;
        public const int CYBORG_FROZEN = 280;
        public const int CYBORG_SUNSTORM = 282;
        public const int CYBORG_STARSCREAM = 255;
        public const int CYBORG_CELESTIAL = 256;
        public const int CYBORG_MAELSTORM = 257;
        public const int CYBORG_ASIMOV = 258;
        public const int CYBORG_TYRANNOS = 259;
        public const int CYBORG_OCEAN = 355;
        public const int CYBORG_POISON = 356;
        public const int CYBORG_PROMETHEUS = 357;
        public const int CYBORG_BLAZE = 358;
        public const int CYBORG_ARGON = 452;
        public const int CYBORG_SERAPH = 455;
        public const int CYBORG_EPION = 456;
        public const int CYBORG_OSIRIS = 457;
        public const int CYBORG_SMITE = 458;

        public const int HAMMERCLAW = 246;
        public const int HAMMERCLAW_ULLRIN = 253;
        public const int HAMMERCLAW_NOBILIS = 252;
        public const int HAMMERCLAW_FROZEN = 251;
        public const int HAMMERCLAW_BANE = 250;
        public const int HAMMERCLAW_CARBONITE = 248;
        public const int HAMMERCLAW_LAVA = 247;
        public const int HAMMERCLAW_TYRANNOS = 367;
        public const int HAMMERCLAW_PROMETHEUS = 368;

        public const int PUSAT = 130;
        public const int PUSAT_EXPO = 370;
        public const int PUSAT_BLAZE = 369;
        public const int PUSAT_LAVA = 371;
        public const int PUSAT_LEGEND = 372;
        public const int PUSAT_OCEAN = 373;
        public const int PUSAT_POISON = 374;
        public const int PUSAT_SANDSTORM = 375;


        public const int VENGEANCE_ADEPT = 16;
        public const int VENGEANCE_CORSAIR = 17;
        public const int VENGEANCE_LIGHTNING = 18;
        public const int VENGEANCE_REVENGE = 58;
        public const int VENGEANCE_AVENGER = 60;

        public const int AEGIS = 49;
        public const int AEGIS_VETERAN = 157;
        public const int AEGIS_ELITE = 158;

        public const int SPEARHEAD = 70;
        public const int SPEARHEAD_VETERAN = 161;
        public const int SPEARHEAD_ELITE = 162;

        public const int CITADEL = 69;
        public const int CITADEL_VETERAN = 159;
        public const int CITADEL_ELITE = 160;

        public const int HECATE_DUSKLIGHT = 1186;
        public const int HECATE_TYRANNOS = 1189;
        public const int HECATE_FROST = 1190;

        public const int TARTARUS_EPION = 1001;
        public const int TARTARUS_SMITE = 1003;
        public const int TARTARUS_OSIRIS = 1002;

        public const int CENTURION_DAMAGE = 1153;
        public const int CENTURION_HP = 1154;
        public const int CENTURION_SHIELD = 1155;
        public const int CENTURION_TYRANNOS = 1157;

        public const int SOLARIS_PSYCHE = 1141;
        public const int SOLARIS_AMOR = 1142;

        public const int RETIARUS = 1300;
        public const int RETIARUS_ARIOS = 1301;
        public const int RETIARUS_NEIKOS = 1302;
        public const int RETIARUS_FROST = 1303;


        public const int ORCUS_HARBINGER = 1403;
        public const int ORCUS_SERAPH = 1404;
        public const int ORCUS_FROST = 1402;
        public const int ORCUS_NOBILIS = 1401;

        public const int VANQUISHERHP = 142;
        public const int VANQUISHERDMG = 140;
        public const int VANQUISHERSHD = 141;

        public const int BERSERKER_AMOR = 1126;


        public List<int> G_CHAMPIONS = new List<int>()
        {
            155, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 363, 364
        };

        public static List<int> SURGEONS = new List<int>()
        {
            156, 361, 362
        };

        public static List<int> SENTINELS = new List<int>()
        {
            66, 265, 266, 283, 284, 285, 343, 346, 393, 395, 396, 398, 888, 344, 397, 347
        };

        public static List<int> DIMINISHERS = new List<int>()
        {
            64, 268, 269, 293, 294, 360, 379, 382, 383, 384, 453, 381
        };

        public static List<int> SPECTRUMS = new List<int>()
        {
            65, 286, 287, 288, 289, 290, 291, 292, 378, 486, 487, 424, 425, 426, 427, 428
        };

        public static List<int> CYBORGS = new List<int>()
        {
            245, 249, 255, 256, 257, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 355, 356, 358, 456
        };

        public static List<int> HAMMERCLAWS = new List<int>()
        {
            246 , 247, 248, 250, 251, 252, 253, 367, 368
        };

        public static List<int> SOLACES = new List<int>()
        {
            63, 262, 263, 264, 340, 341, 410, 412, 413, 415, 416
        };

        public static List<int> VENOMS = new List<int>()
        {
            67, 351, 352, 353, 354, 350
        };

        public static List<int> PUSATS = new List<int>()
        {
            130, 369, 370, 371, 373, 374, 375, 454
        };

        public static List<int> AEGISs = new List<int>()
        {
            49, 157, 158
        };

        public static List<int> SOLARIS = new List<int>()
        {
            1140, 1141, 1142
        };

        public static List<int> KERES = new List<int>()
        {
            1165, 1166
        };

        public static List<int> DISRUPTORS = new List<int>()
        {
            1130, 1131, 1132, 1133
        };

        public static List<int> BERSERKERS = new List<int>()
        {
            1120, 1121, 1122, 1123, 1124, 1125, 1126, 1127
        };

        public static List<int> zephyr = new List<int>()
        {
            1100
        };

        public static List<int> CustomShips20 = new List<int>()
        {
            450, 451, 452
        };

        public static List<int> HECATES = new List<int>()
        {
            1189, 1190, 1191
        };

        public static List<int> ORCUS = new List<int>()
        {
           1400, 1401, 1402, 10403, 1404
        };

        

        public static List<int> UBASHIPS = new List<int>()
        {
            498, 499, 500, 321
        };


        public string Name { get; set; }
        public int Id { get; set; }
        public int Damage { get; set; }
        public int BaseHitpoints { get; set; }
        public int BaseShieldPoints { get; set; }
        public int BaseSpeed { get; set; }
        public string LootId { get; set; }
        public bool Aggressive { get; set; }
        public bool Respawnable { get; set; }
        public ShipRewards Rewards { get; set; }
        public int Lasers { get; set; }


        public Ship(string name, int id, int baseHitpoints, int baseShieldPoints, int speed, string lootID, int damage, bool aggressive, bool respawnable, ShipRewards rewards, int lasers)
        {
            Name = name;
            Id = id;
            Damage = damage;
            BaseShieldPoints = baseShieldPoints;
            BaseHitpoints = baseHitpoints;
            BaseSpeed = speed;
            LootId = lootID;
            Aggressive = aggressive;
            Respawnable = respawnable;
            Rewards = rewards;
            Lasers = lasers;
        }

        public int GetHitPointsBoost(int hitpoints) //SUBIR HP
        {

            if (CustomShips20.Contains(Id) || UBASHIPS.Contains(Id))
                return hitpoints += Maths.GetPercentage(hitpoints, 20);
            else if (PUSATS.Contains(Id) || KERES.Contains(Id) || SOLARIS.Contains(Id) || BERSERKERS.Contains(Id))
                return hitpoints += Maths.GetPercentage(hitpoints, 10);
            else
            {
                switch (Id)
                {

                    case VANQUISHERHP:
                    case GOLIATH_GOAL:
                        return hitpoints += Maths.GetPercentage(hitpoints, 10);
                    case GOLIATH_CENTAUR:
                    case CHAMPION_LEGEND:
                        return hitpoints += Maths.GetPercentage(hitpoints, 10);
                    case GOLIATH_BRONZE:
                        return hitpoints += Maths.GetPercentage(hitpoints, 10);
                    case HECATE_FROST:
                        return hitpoints += Maths.GetPercentage(hitpoints, 15);
                    case HECATE_DUSKLIGHT:
                        return hitpoints += Maths.GetPercentage(hitpoints, 15);
                    case TARTARUS_OSIRIS:
                        return hitpoints += Maths.GetPercentage(hitpoints, 15);
                    case CENTURION_TYRANNOS:
                        return hitpoints += Maths.GetPercentage(hitpoints, 14);
                    case GOLIATH_SILVER:
                        return hitpoints += Maths.GetPercentage(hitpoints, 20);
                    case GOLIATH_DUSKLIGHT:
                        return hitpoints += Maths.GetPercentage(hitpoints, 20);
                    case CENTURION_HP:
                        return hitpoints += Maths.GetPercentage(hitpoints, 25);
                    case GOLIATH_SATURN:
                        return hitpoints += Maths.GetPercentage(hitpoints, 15);
                    case RETIARUS_ARIOS:
                        return hitpoints += Maths.GetPercentage(hitpoints, 17);
                    case RETIARUS_FROST:
                        return hitpoints += Maths.GetPercentage(hitpoints, 17);
                    case ORCUS_SERAPH:
                        return hitpoints += Maths.GetPercentage(hitpoints, 17);
                    case ORCUS_HARBINGER:
                        return hitpoints += Maths.GetPercentage(hitpoints, 10);
                    case ORCUS_FROST:
                        return hitpoints += Maths.GetPercentage(hitpoints, 10);
                    case ORCUS_NOBILIS:
                        return hitpoints += Maths.GetPercentage(hitpoints, 10);
                    case CYBORG_EPION:
                        return hitpoints += Maths.GetPercentage(hitpoints, 5);
                    case GOLIATH_IRON:
                        return hitpoints += Maths.GetPercentage(hitpoints, 50);
                    case BERSERKER_AMOR:
                        return hitpoints += Maths.GetPercentage(hitpoints, 25);
                    default:
                        return hitpoints;
                }
            }
        }

        public int GetShieldPointsBoost(int shield) //SUBIR SHIELD
        {

            if (KERES.Contains(Id))
                return shield += Maths.GetPercentage(shield, 5);
            else if (SENTINELS.Contains(Id) || SOLARIS.Contains(Id) || SOLACES.Contains(Id))
                return shield += Maths.GetPercentage(shield, 10);
            else if (CustomShips20.Contains(Id) || BERSERKERS.Contains(Id) || UBASHIPS.Contains(Id))
                return shield += Maths.GetPercentage(shield, 20);
            else if (SPECTRUMS.Contains(Id) || HAMMERCLAWS.Contains(Id))
                return shield += Maths.GetPercentage(shield, 10);
            else
            {
                switch (Id)
                {

                    case SOLARIS_PSYCHE:
                        return shield += Maths.GetPercentage(shield, 8);
                    case GOLIATH_KICK:
                    case GOLIATH_BASTION:
                    case VANQUISHERSHD:
                        return shield += Maths.GetPercentage(shield, 10);
                    case VENGEANCE_AVENGER:
                        return shield += Maths.GetPercentage(shield, 10);
                    case GOLIATH_BRONZE:
                        return shield += Maths.GetPercentage(shield, 10);
                    case HECATE_FROST:
                        return shield += Maths.GetPercentage(shield, 15);
                    case TARTARUS_EPION:
                        return shield += Maths.GetPercentage(shield, 10);
                    case TARTARUS_SMITE:
                        return shield += Maths.GetPercentage(shield, 10);
                    case CENTURION_TYRANNOS:
                        return shield += Maths.GetPercentage(shield, 15);
                    case GOLIATH_SILVER:
                        return shield += Maths.GetPercentage(shield, 20);
                    case GOLIATH_DUSKLIGHT:
                        return shield += Maths.GetPercentage(shield, 20);
                    case TARTARUS_OSIRIS:
                        return shield += Maths.GetPercentage(shield, 15);
                    case HECATE_DUSKLIGHT:
                        return shield += Maths.GetPercentage(shield, 15);
                    case CYBORG_TYRANNOS:
                    case CYBORG_ASIMOV:
                        return shield += Maths.GetPercentage(shield, 15);
                    case CYBORG_INFINITE:
                        return shield += Maths.GetPercentage(shield, 15);
                    case HAMMERCLAW_PROMETHEUS:
                        return shield += Maths.GetPercentage(shield, 15);
                    case CENTURION_SHIELD:
                        return shield += Maths.GetPercentage(shield, 25);
                    case CHAMPION_LEGEND:
                        return shield += Maths.GetPercentage(shield, 20);
                    case GOLIATH_ORION:
                        return shield += Maths.GetPercentage(shield, 10);
                    case RETIARUS_ARIOS:
                        return shield += Maths.GetPercentage(shield, 17);
                    case RETIARUS_FROST:
                        return shield += Maths.GetPercentage(shield, 17);
                    case RETIARUS_NEIKOS:
                        return shield += Maths.GetPercentage(shield, 15);
                    case CHAMPION_TYRANNOS:
                        return shield += Maths.GetPercentage(shield, 5);
                    case ORCUS_SERAPH:
                        return shield += Maths.GetPercentage(shield, 20);
                    case ORCUS_HARBINGER:
                        return shield += Maths.GetPercentage(shield, 15);
                    case ORCUS_FROST:
                        return shield += Maths.GetPercentage(shield, 15);
                    case ORCUS_NOBILIS:
                        return shield += Maths.GetPercentage(shield, 15);
                    case CYBORG_EPION:
                        return shield += Maths.GetPercentage(shield, 5);
                    case GOLIATH_IRON:
                        return shield += Maths.GetPercentage(shield, 50);
                    case BERSERKER_AMOR:
                        return shield += Maths.GetPercentage(shield, 20);
                    default:
                        return shield;
                }
            }
        }

        public int GetLaserDamageBoost(int damage, int thisFactionId, int otherFactionId) //SUBIR DAÑO LASER
        {
            if (CYBORGS.Contains(Id)  || SOLARIS.Contains(Id) || KERES.Contains(Id))
                return damage += Maths.GetPercentage(damage, 5);
            else if (G_CHAMPIONS.Contains(Id) || PUSATS.Contains(Id) || SURGEONS.Contains(Id))
                return damage += Maths.GetPercentage(damage, 5);
            else if (VENOMS.Contains(Id) || DIMINISHERS.Contains(Id) || HAMMERCLAWS.Contains(Id) || AEGISs.Contains(Id) || ORCUS.Contains(Id) || UBASHIPS.Contains(Id))
                return damage += Maths.GetPercentage(damage, 5);
            else if (CustomShips20.Contains(Id) || BERSERKERS.Contains(Id))
                return damage += Maths.GetPercentage(damage, 20);
            /*else if (COMPANY_GOLIATHS.Contains(Id))
            {
                if (otherFactionId != 0 && thisFactionId != otherFactionId)
                    return damage += Maths.GetPercentage(damage, 7);
                else return damage;
            }*/
            else
            {
                switch (Id)
                {
                    case VANQUISHERDMG:
                        return damage += Maths.GetPercentage(damage, 5);
                    case HECATE_FROST:
                        return damage += Maths.GetPercentage(damage, 12);
                    case HECATE_DUSKLIGHT:
                        return damage += Maths.GetPercentage(damage, 15);
                    case GOLIATH_ENFORCER:
                        return damage += Maths.GetPercentage(damage, 5);
                    case GOLIATH_REFEREE:
                        return damage += Maths.GetPercentage(damage, 5);
                    case VENGEANCE_LIGHTNING:
                        return damage += Maths.GetPercentage(damage, 5);
                    case SPEARHEAD_ELITE:
                    case CITADEL_ELITE:
                    case PUSAT:
                    case CYBORG_TYRANNOS:
                        return damage += Maths.GetPercentage(damage, 10);
                    case CYBORG_ASIMOV:
                        return damage += Maths.GetPercentage(damage, 16);
                    case CYBORG_INFINITE:
                        return damage += Maths.GetPercentage(damage, 25);
                    case CHAMPION_LEGEND:
                        return damage += Maths.GetPercentage(damage, 30);
                    case PUSAT_LEGEND:
                        return damage += Maths.GetPercentage(damage, 20);
                    case GOLIATH_BRONZE:
                    case TARTARUS_EPION:
                        return damage += Maths.GetPercentage(damage, 10);
                    case CENTURION_TYRANNOS:
                        return damage += Maths.GetPercentage(damage, 15);
                    case TARTARUS_OSIRIS:
                        return damage += Maths.GetPercentage(damage, 15);
                    case GOLIATH_RAZER:
                        return damage += Maths.GetPercentage(damage, 5);
                    case GOLIATH_GOLD:
                        return damage += Maths.GetPercentage(damage, 5);
                    case CENTURION_DAMAGE:
                        return damage += Maths.GetPercentage(damage, 20);
                    case CHAMPION_TYRANNOS:
                        return damage += Maths.GetPercentage(damage, 10);
                    case HAMMERCLAW_PROMETHEUS:
                        return damage += Maths.GetPercentage(damage, 15);
                    case VENGEANCE_REVENGE:
                        return damage += Maths.GetPercentage(damage, 5);
                    case RETIARUS:
                        return damage += Maths.GetPercentage(damage, 10);
                    case RETIARUS_ARIOS:
                        return damage += Maths.GetPercentage(damage, 17);
                    case RETIARUS_FROST:
                        return damage += Maths.GetPercentage(damage, 17);
                    case RETIARUS_NEIKOS:
                        return damage += Maths.GetPercentage(damage, 15);
                    case GOLIATH_SILVER:
                        return damage += Maths.GetPercentage(damage, 20);
                    case GOLIATH_DUSKLIGHT:
                        return damage += Maths.GetPercentage(damage, 20);
                    case GOLIATH_ENFORCER_BONUS:
                        return damage += Maths.GetPercentage(damage, 5);
                    case ORCUS_SERAPH:
                        return damage += Maths.GetPercentage(damage, 20);
                    case ORCUS_HARBINGER:
                        return damage += Maths.GetPercentage(damage, 15);
                    case ORCUS_FROST:
                        return damage += Maths.GetPercentage(damage, 15);
                    case ORCUS_NOBILIS:
                        return damage += Maths.GetPercentage(damage, 15);
                    case CYBORG_EPION:
                        return damage += Maths.GetPercentage(damage, 10);
                    case GOLIATH_IRON:
                        return damage += Maths.GetPercentage(damage, 50);
                    case BERSERKER_AMOR:
                        return damage += Maths.GetPercentage(damage, 25);
                    default:
                        return damage;
                }
            }
        }

        public int GetHonorBoost(int honor) //SUBIR HONOR
        {
            if (ORCUS.Contains(Id))
                return honor += Maths.GetPercentage(honor, 10);
            else if (CYBORGS.Contains(Id))
                return honor += Maths.GetPercentage(honor, 5);
            else if (CustomShips20.Contains(Id) || BERSERKERS.Contains(Id))
                return honor += Maths.GetPercentage(honor, 20);
            else
            {
                switch (Id)
                {
                    case SOLARIS_AMOR:
                        return honor += Maths.GetPercentage(honor, 8);
                    case TARTARUS_SMITE:
                        return honor += Maths.GetPercentage(honor, 5);
                    case GOLIATH_EXALTED:
                    case SPEARHEAD_VETERAN:
                    case CITADEL_VETERAN:
                        return honor += Maths.GetPercentage(honor, 10);
                    case CYBORG_TYRANNOS:
                    case CHAMPION_LEGEND:
                        return honor += Maths.GetPercentage(honor, 15);
                    case CHAMPION_TYRANNOS:
                        return honor += Maths.GetPercentage(honor, 25);
                    case VENGEANCE_CORSAIR:
                        return honor += Maths.GetPercentage(honor, 10);
                    case RETIARUS_ARIOS:
                    case GOLIATH_GOLD:
                        return honor += Maths.GetPercentage(honor, 20);
                    case RETIARUS_FROST:
                        return honor += Maths.GetPercentage(honor, 20);
                    case RETIARUS_NEIKOS:
                        return honor += Maths.GetPercentage(honor, 15);
                    case GOLIATH_BRONZE:
                    case GOLIATH_SILVER:
                        return honor += Maths.GetPercentage(honor, 20);
                    case GOLIATH_DUSKLIGHT:
                        return honor += Maths.GetPercentage(honor, 25);
                    case VENOM_BLAZE:
                        return honor += Maths.GetPercentage(honor, 25);
                    case ORCUS_SERAPH:
                        return honor += Maths.GetPercentage(honor, 20);
                    case ORCUS_HARBINGER:
                        return honor += Maths.GetPercentage(honor, 15);
                    case ORCUS_FROST:
                        return honor += Maths.GetPercentage(honor, 15);
                    case ORCUS_NOBILIS:
                        return honor += Maths.GetPercentage(honor, 15);
                    case BERSERKER_AMOR:
                        return honor += Maths.GetPercentage(honor, 25);
                    default:
                        return honor;

                }
            }
        }

        public int GetExperienceBoost(int experience) //SUBIR EXP NPC
        {
            if (AEGISs.Contains(Id))
                return experience += Maths.GetPercentage(experience, 8);
            else if (CustomShips20.Contains(Id))
                return experience += Maths.GetPercentage(experience, 20);
            else
            {
                switch (Id)
                {
                    case SOLARIS_AMOR:
                        return experience += Maths.GetPercentage(experience, 8);
                    case TARTARUS_SMITE:
                        return experience += Maths.GetPercentage(experience, 8);
                    case HECATE_TYRANNOS:
                        return experience += Maths.GetPercentage(experience, 25);
                    case SPEARHEAD_VETERAN:
                    case CITADEL_VETERAN:
                        return experience += Maths.GetPercentage(experience, 10);
                    case VENGEANCE_ADEPT:
                    case CHAMPION_LEGEND:
                        return experience += Maths.GetPercentage(experience, 15);
                    case RETIARUS_ARIOS:
                        return experience += Maths.GetPercentage(experience, 20);
                    case RETIARUS_FROST:
                        return experience += Maths.GetPercentage(experience, 20);
                    case RETIARUS_NEIKOS:
                        return experience += Maths.GetPercentage(experience, 15);
                    case GOLIATH_BRONZE:
                        return experience += Maths.GetPercentage(experience, 15);
                    case GOLIATH_SILVER:
                        return experience += Maths.GetPercentage(experience, 20);
                    case GOLIATH_DUSKLIGHT:
                        return experience += Maths.GetPercentage(experience, 20);
                    case ORCUS_SERAPH:
                        return experience += Maths.GetPercentage(experience, 25);
                    case ORCUS_HARBINGER:
                        return experience += Maths.GetPercentage(experience, 15);
                    case ORCUS_FROST:
                        return experience += Maths.GetPercentage(experience, 15);
                    case ORCUS_NOBILIS:
                        return experience += Maths.GetPercentage(experience, 15);
                    case SPECTRUM_INFERNO:
                        return experience += Maths.GetPercentage(experience, 30);
                    case BERSERKER_AMOR:
                        return experience += Maths.GetPercentage(experience, 25);
                    case GOLIATH_VETERAN:
                        return experience += Maths.GetPercentage(experience, 20);
                    default:
                        return experience;
                }
            }
        }

        public short GroupShipId
        {
            get
            {
                if (SENTINELS.Contains(Id) || SPECTRUMS.Contains(Id) || DIMINISHERS.Contains(Id) || SOLACES.Contains(Id) || CYBORGS.Contains(Id) || VENOMS.Contains(Id) || SENTINELS.Contains(Id) || G_CHAMPIONS.Contains(Id) || ORCUS.Contains(Id))
                    return GroupPlayerShipModule.ENFORCER;
                else
                {
                    switch (Id)
                    {
                        case 22:
                            return GroupPlayerShipModule.PET;
                        case GOLIATH:
                        case GOLIATH_ENFORCER:
                        case GOLIATH_BASTION:
                        case GOLIATH_VETERAN:
                        case GOLIATH_EXALTED:
                        case GOLIATH_VENOM:
                            return GroupPlayerShipModule.ENFORCER;
                        case VENGEANCE_LIGHTNING:
                        case VENGEANCE_REVENGE:
                        case VENGEANCE_AVENGER:
                        case VENGEANCE_ADEPT:
                        case VENGEANCE_CORSAIR:
                            return GroupPlayerShipModule.REVENGE;
                        default:
                            return GroupPlayerShipModule.DEFAULT;
                    }
                }
            }
        }

        private static Random random = new Random();
        public static int GetRandomShipId(int currentShipId)
        {
            int randomed = random.Next(63, 67);

            if (randomed == currentShipId)
                return GetRandomShipId(currentShipId);
            else
                return randomed;
        }
    }
}

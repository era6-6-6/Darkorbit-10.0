using Darkorbit.Game.Events;
using Ow.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Hitac = Darkorbit.Game.Events.Hitac;
using Spaceball = Darkorbit.Game.Events.Spaceball;

namespace Darkorbit.Managers
{
    class EventManager
    {


        public static JackpotBattle JackpotBattle { get; set; }
        public static Spaceball Spaceball { get; set; }
        public static Invasion Invasion { get; set; }
        public static UltimateBattleArena UltimateBattleArena { get; set; }
       
        public static TeamDeathmatchNew TeamDeathmatchNew { get; set; }
        public static GroupEvent groupEvent { get; set; }
        public static BattleRoyal battleRoyal { get; set; }
        public static BattleCompany BattleCompany { get; set; }
        public static Hitac Hitac { get; set; }

        public static AutoEventManager AutoEventManager { get; set; }



        public static DemanerEvent demanerEvent { get; set; }
        public static IceMetorit meteorit { get; set; }
        public static SuperIceMetorit supermeteorit { get; set; }
        public static Cubikon cubikon { get; set; }
        public static BLMaps blmaps { get; set; }
        public static Emperator emperator { get; set; }

        public static bool X2RocketEvent { get; set; }
        public static bool UbaEvent { get; set; }
        public static bool TdmEvent { get; set; }
        public static bool IceMeteorid { get; set; }
        public static bool SuperIceMeteorid { get; set; }


        public static int countJackpotRound = 1;
        public static int countCompanyRound = 1;
        public static int countRoyalRound = 1;
        public static void InitiateEvents()
        {
            JackpotBattle = new JackpotBattle();
            Spaceball = new Spaceball();
            Invasion = new Invasion();
            UltimateBattleArena = new UltimateBattleArena();
            
            TeamDeathmatchNew = new TeamDeathmatchNew();
            groupEvent = new GroupEvent();
            battleRoyal = new BattleRoyal();
            BattleCompany = new BattleCompany();
            cubikon = new Cubikon();
            blmaps = new BLMaps();
            demanerEvent = new DemanerEvent();
            meteorit = new IceMetorit();
            supermeteorit = new SuperIceMetorit();
            Hitac = new Hitac();
            AutoEventManager = new AutoEventManager();

            /*var cubikonStatus = Cubikon.Status();
            if (!cubikonStatus)
                Cubikon.Start();
                Console.WriteLine("The Cubikon event has started automatically.");*/

            var blacklightstatus = BLMaps.Status();
            if (!blacklightstatus)
                BLMaps.Start();
            Console.WriteLine("The Blacklight event has started automatically.");

            AutoEventManager.Start();

        }
        public static void SetIceMeteorid(bool state)
        {
            IceMeteorid = state;
            if (state) IceMetorit.Start();
        }

        public static bool GetIceMeteorid()
        {
            return IceMeteorid;
        }

        public static void SetSuperIceMeteorid(bool state)
        {
            SuperIceMeteorid = state;
            if (state) SuperIceMetorit.Start();
        }

        public static bool GetSuperIceMeteorid()
        {
            return SuperIceMeteorid;
        }

        public static void Setx2RocketEvent(bool state)
        {
            X2RocketEvent = state;
        }

        public static bool Getx2RocketEvent()
        {
            return X2RocketEvent;
        }
        public static void SetUbaEvent(bool state)
        {
            UbaEvent = state;
        }

        public static bool GetUbaEvent()
        {
            return UbaEvent;
        }

        public static void SetTdmEvent(bool state, string mode)
        {
            TdmEvent = state;
            if (state) TeamDeathmatchNew.Start(mode);
        }

        public static bool GetTdmEvent()
        {
            return TdmEvent;
        }
    }
}





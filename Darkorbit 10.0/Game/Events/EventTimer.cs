using Darkorbit;
using Darkorbit.Game.Events;
using Darkorbit.Game.Ticks;
using System;
using System.Collections.Generic;
namespace Ow.Game.Events
{
    internal class EventTimer : Tick
    {
        private readonly DateTime timer = DateTime.Now;
        private readonly Random ran = new Random();
        private readonly List<string> events = new List<string>();

        /*   Spaceball
           JackpotBattle
             BattleRoyal
           Invasion
           Battle Company
           DemaNer
           */
        private int hour9Am = 9;
        private int hour10Am = 10;
        private int hour11Am = 11;
        private int hour12PM = 12;
        private int hour2PM = 14;
        private int hour3PM = 15;
        private int hour4PM = 16;
        private int hour5PM = 17;
        private int hour6PM = 18;
        private int hour7PM = 19;
        private int hour8PM = 20;
        private int hour9PM = 22;









        public EventTimer()
        {



            Program.TickManager.AddTick(this);
            events.Add(EventManager.battleRoyal.ToString());

            events.Add(EventManager.Spaceball.ToString());
            events.Add(EventManager.JackpotBattle.ToString());
            events.Add(EventManager.BattleCompany.ToString());
            events.Add(EventManager.demanerEvent.ToString());
            events.Add(EventManager.meteorit.ToString());

        }

        public void Tick()
        {






            if (DateTime.Now.Hour == hour9Am)
            {
                randomEvent();
                hour9Am = 0;
            }
            if (DateTime.Now.Hour == hour10Am)
            {
                EventManager.Spaceball.Start();
                hour10Am = 0;
            }
            if (DateTime.Now.Hour == hour11Am)
            {
                EventManager.JackpotBattle.Start();
                hour11Am = 0;
            }
            if (DateTime.Now.Hour == hour12PM)
            {
                EventManager.Invasion.Start();
                hour12PM = 0;
            }
            if (DateTime.Now.Hour == hour2PM)
            {
                DemanerEvent.Start();
                hour2PM = 0;
            }
            if (DateTime.Now.Hour == hour3PM)
            {
                //  EventManager.BattleCompany.Start();
                EventManager.Spaceball.Start();
                hour3PM = 0;
            }
            if (DateTime.Now.Hour == hour4PM)
            {
                EventManager.battleRoyal.Start();
                hour4PM = 0;
            }
            if (DateTime.Now.Hour == hour5PM)
            {
                randomEvent();
                hour5PM = 0;
            }
            if (DateTime.Now.Hour == hour6PM)
            {
                EventManager.Spaceball.Start();
                hour6PM = 0;
            }

            if (DateTime.Now.Hour == hour7PM)
            {
                // EventManager.BattleCompany.Start();
                EventManager.Spaceball.Start();
                hour7PM = 0;

            }
            if (DateTime.Now.Hour == hour8PM)
            {
                DemanerEvent.Start();
                hour8PM = 0;
            }
            if (DateTime.Now.Hour == hour9PM)
            {

                EventManager.Spaceball.Start();
                hour9PM = 0;
            }

            if (DateTime.Now.Hour == 24)
            {

                GameManager.Restart(120, "Automatic restart");
            }













        }


        public void randomEvent()
        {

            int random = ran.Next(events.Count);
           

        }
    }
}

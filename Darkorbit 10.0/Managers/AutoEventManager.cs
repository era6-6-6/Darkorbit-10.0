using System.Data;
using Darkorbit.Game.Events;
using Darkorbit.Net;

namespace Ow.Managers
{
    class AutoEventManager
    {
        private readonly List<string> events = new List<string>();
        private readonly DateTime timer = DateTime.Now;
        public static bool Active = false;
        String currentHourMinute = DateTime.Now.ToString("HH:mm");
        private List<Event> eventList = new List<Event>();
        public int UserId { get; set; }

        public class Event
        {
            public string name { get; set; }
            public int enabled { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public int monday { get; set; }
            public int tuesday { get; set; }
            public int wednesday { get; set; }
            public int thursday { get; set; }
            public int friday { get; set; }
            public int saturday { get; set; }
            public int sunday { get; set; }
            public int multipleDays { get; set; }
            public int testing { get; set; }
        }

        public static bool Status()
        {
            return Active;
        }

        public void Start()
        {
            Console.WriteLine("AutoEventManager successfully started.");
            Out.WriteLine("Successfully started!", "Auto Event Manager", ConsoleColor.Magenta);
            //Console.WriteLine($"[AutoEventTimer] Current Time: {DateTime.Now.ToString()}, Current Hour: {currentHourMinute}");

            /*var spacemapTest = GameManager.GetSpacemap(308);
            Console.WriteLine($"DEBUG: {spacemapTest}");*/

            Task reloadEvents = Task.Run(async () => await LoadEvents());
        }

        public async Task LoadEvents()
        {
            try
            {
                while (true)
                {
                    eventList = new List<Event>();

                    using (var mySqlClient = SqlDatabaseManager.GetClient())
                    {
                        var data = (DataTable)mySqlClient.ExecuteQueryTable($"SELECT * FROM event_table");
                        foreach (DataRow row in data.Rows)
                        {
                            if (int.Parse(row["enabled"].ToString()) == 1)
                            {
                                var tmp = new Event();
                                tmp.name = row["event"].ToString();
                                tmp.enabled = int.Parse(row["enabled"].ToString());
                                tmp.start = row["start_new"].ToString();
                                tmp.end = row["end_new"].ToString();
                                tmp.monday = int.Parse(row["monday"].ToString());
                                tmp.tuesday = int.Parse(row["tuesday"].ToString());
                                tmp.wednesday = int.Parse(row["wednesday"].ToString());
                                tmp.thursday = int.Parse(row["thursday"].ToString());
                                tmp.friday = int.Parse(row["friday"].ToString());
                                tmp.saturday = int.Parse(row["saturday"].ToString());
                                tmp.sunday = int.Parse(row["sunday"].ToString());
                                tmp.multipleDays = int.Parse(row["multipleDays"].ToString());
                                tmp.testing = int.Parse(row["testing"].ToString());
                                if (tmp.testing == 1 && GameServer.Port == 7001)
                                {
                                    eventList.Add(tmp);
                                } else if(tmp.testing == 0)
                                {
                                    eventList.Add(tmp);
                                }
                            }
                        }
                    }

                    Recheck();

                    await Task.Delay(60_000);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [AutoEventManager.cs] Main void exception: {ex}");
            }
        }

        public void ActivateX2Event()
        {
            bool statusEvent = Attackable.x2EventActive;
            if (statusEvent == false)
            {
                Attackable.x2EventActive = true;
                GameManager.SendPacketToAll($"0|A|STD|X2 Rewards Event has been activated");
            }
        }

        public void DeactivateX2Event()
        {
            bool statusEvent = Attackable.x2EventActive;
            if (statusEvent == true)
            {
                Attackable.x2EventActive = false;
                GameManager.SendPacketToAll($"0|A|STD|X2 Rewards Event has been deactivated");
            }
        }

        public void InvasionStart()
        {
            bool statusEvent = EventManager.Invasion.Status();
            if (statusEvent == false)
            {
                EventManager.Invasion.Start();
            }
        }

        public void SpaceballStart()
        {
            bool statusEvent = EventManager.Spaceball.Status();
            if (statusEvent == false)
            {
                EventManager.Spaceball.Start();
            }
        }

        public void SpaceballStop()
        {
            bool statusEvent = EventManager.Spaceball.Status();
            if (statusEvent == true)
            {
                EventManager.Spaceball.Stop();
            }
        }

        public void IceMeteoritStart()
        {
            bool statusEvent = EventManager.IceMeteorid;
            if (statusEvent == false)
            {
                EventManager.SetIceMeteorid(true);
                GameManager.SendPacketToAll($"0|A|STD|Meteorit Event activated");
            }
        }
        public void IceMeteoritStop()
        {
            bool statusEvent = EventManager.IceMeteorid;
            if (statusEvent == true)
            {
                EventManager.SetIceMeteorid(false);
                GameManager.SendPacketToAll($"0|A|STD|Meteorit Event deactivated");
            }
        }

        public void SuperIceMeteoritStart()
        {
            bool statusEvent = EventManager.SuperIceMeteorid;
            if (statusEvent == false)
            {
                EventManager.SetSuperIceMeteorid(true);
                GameManager.SendPacketToAll($"0|A|STD|Super Meteorit Event activated");
            }
        }
        public void SuperIceMeteoritStop()
        {
            bool statusEvent = EventManager.SuperIceMeteorid;
            if (statusEvent == true)
            {
                EventManager.SetSuperIceMeteorid(false);
                GameManager.SendPacketToAll($"0|A|STD|Super Meteorit Event deactivated");
            }
        }

        public void X2RocketStart()
        {
            bool statusEvent = EventManager.Getx2RocketEvent();
            if (statusEvent == false)
            {
                EventManager.Setx2RocketEvent(true);
                GameManager.SendPacketToAll($"0|A|STD|Rocket Event has been activated");
            }
        }

        public void X2RocketEnd()
        {
            bool statusEvent = EventManager.Getx2RocketEvent();
            if (statusEvent == true)
            {
                EventManager.Setx2RocketEvent(false);
                GameManager.SendPacketToAll($"0|A|STD|Rocket Event has been deactivated");
            }
        }
        public void ActivateUbaEvent()
        {
            bool statusEvent = EventManager.GetUbaEvent();
            if (statusEvent == false)
            {
                EventManager.SetUbaEvent(true);
                GameManager.SendPacketToAll($"0|A|STD|UBA Event Started! You can join with the button left up!");
                EventManager.UltimateBattleArena.UBAdisabled = false;
                foreach(GameSession p in GameManager.GameSessions.Values)
                {
                    p.Player.SettingsManager.SendMenuBarsCommand();
                }
            }
        }

        public void DeactivateUbaEvent()
        {
            bool statusEvent = EventManager.GetUbaEvent();
            if (statusEvent == true)
            {
                EventManager.SetUbaEvent(false);
                GameManager.SendPacketToAll($"0|A|STD|UBA event Ended!");
                EventManager.UltimateBattleArena.UBAdisabled = true;
                foreach (GameSession p in GameManager.GameSessions.Values)
                {
                    p.Player.SettingsManager.SendMenuBarsCommand();
                }
            }
        }

        public void ActivateTdmEvent(string mode)
        {
            bool statusEvent = EventManager.GetTdmEvent();
            if (statusEvent == false)
            {
                EventManager.SetTdmEvent(true, mode);
                GameManager.SendPacketToAll($"0|A|STD|TDM Event Started! Type /register into the chat, to join the TDM.");
            }
        }

        public void DeactivateTdmEvent()
        {
           // var gameSession = GameManager.GetGameSession(UserId);
            bool statusEvent = EventManager.GetTdmEvent();
            if (statusEvent == true)
            {
                // gameSession.Player.Storage.TdmEnd = true;
                TeamDeathmatchNew.Active = false;
                EventManager.SetTdmEvent(false, "");
                GameManager.SendPacketToAll($"0|A|STD|TDM event Ended!");
            }
        }

        public bool CheckEventRunning(string start, string end)
        {
            var eventIsRunning = false;

            if (DateTime.Now >= DateTime.Parse(start) && DateTime.Now <= DateTime.Parse(end)) eventIsRunning = true;
            
            return eventIsRunning;
        }

        public static DateTime EquivalentWeekDay(int dayOfWeek)
        {
            int num2 = (int)DateTime.Today.DayOfWeek;
            return DateTime.Today.AddDays(dayOfWeek - num2);
        }

        public void Recheck()
        {
            bool x2Deactivate = true;
            bool ubaDeactivate = true;
            bool rocketDeactivate = true;
            bool spaceballDeactivate = true;
            bool meteoritDeactivate = true;
            bool supermeteoritDeactivate = true;
            bool tdmDeactivate = true;

            foreach (Event e in eventList)
            {
                var eventIsRunning = false;

                DateTime tmpStart = EquivalentWeekDay((int)DateTime.Now.DayOfWeek);
                tmpStart = tmpStart.AddHours(double.Parse(e.start.Split(':')[0]));
                tmpStart = tmpStart.AddMinutes(double.Parse(e.start.Split(':')[1]));
                tmpStart = tmpStart.AddSeconds(double.Parse(e.start.Split(':')[2]));
                DateTime tmpEnd = EquivalentWeekDay((int)DateTime.Now.DayOfWeek);
                tmpEnd = tmpEnd.AddHours(double.Parse(e.end.Split(':')[0]));
                tmpEnd = tmpEnd.AddMinutes(double.Parse(e.end.Split(':')[1]));
                tmpEnd = tmpEnd.AddSeconds(double.Parse(e.end.Split(':')[2]));

                e.start = tmpStart.ToString();
                e.end = tmpEnd.ToString();

                if (tmpEnd <= tmpStart)
                {
                    tmpEnd = tmpEnd.AddDays(1);
                    e.end = tmpEnd.ToString();
                }

                if (e.multipleDays > 0)
                {
                    tmpEnd = tmpEnd.AddDays(e.multipleDays);
                    e.end = tmpEnd.ToString();
                }

                //0 = montag, 1 = dienstag and so on
                switch ((int)DateTime.Now.DayOfWeek)
                {
                    case 1:
                        if (e.monday == 1) eventIsRunning = CheckEventRunning(e.start, e.end);
                        break;
                    case 2:
                        if (e.tuesday == 1) eventIsRunning = CheckEventRunning(e.start, e.end);
                        break;
                    case 3:
                        if (e.wednesday == 1) eventIsRunning = CheckEventRunning(e.start, e.end);
                        break;
                    case 4:
                        if (e.thursday == 1) eventIsRunning = CheckEventRunning(e.start, e.end);
                        break;
                    case 5:
                        if (e.friday == 1) eventIsRunning = CheckEventRunning(e.start, e.end);
                        break;
                    case 6:
                        if (e.saturday == 1) eventIsRunning = CheckEventRunning(e.start, e.end);
                        break;
                    case 0:
                        if (e.sunday == 1) eventIsRunning = CheckEventRunning(e.start, e.end);
                        break;
                }

                if (DateTime.Parse(e.end).Day > DateTime.Parse(e.start).Day && DateTime.Now.Day > DateTime.Parse(e.start).Day)
                {
                    eventIsRunning = CheckEventRunning(DateTime.Parse(e.start).AddDays(-1).ToString(), DateTime.Parse(e.end).AddDays(-1).ToString());
                }

                if (eventIsRunning)
                {
                    bool eventStarted = false;

                    switch (e.name)
                    {
                        case "invasion":
                            if (!EventManager.Invasion.Status())
                            {
                                InvasionStart();
                                eventStarted = true;
                            }
                            break;
                        case "x2":
                            if (!Attackable.x2EventActive)
                            {
                                ActivateX2Event();
                                eventStarted = true;
                            }
                            x2Deactivate = false;
                            break;
                        case "spaceball":
                            if (!EventManager.Spaceball.Active)
                            {
                                SpaceballStart();
                                eventStarted = true;
                            }
                            spaceballDeactivate = false;
                            break;
                        case "ice_meteorit":
                            if (!EventManager.IceMeteorid)
                            {
                                IceMeteoritStart();
                                eventStarted = true;
                            }
                            meteoritDeactivate = false;
                            break;
                        case "super_ice_meteorit":
                            if (!EventManager.SuperIceMeteorid)
                            {
                                SuperIceMeteoritStart();
                                eventStarted = true;
                            }
                            supermeteoritDeactivate = false;
                            break;
                        case "x2_rocket":
                            if (!EventManager.Getx2RocketEvent())
                            {
                                X2RocketStart();
                                eventStarted = true;
                            }
                            rocketDeactivate = false;
                            break;
                        case "ubaevent":
                            if (!EventManager.GetUbaEvent())
                            {
                                ActivateUbaEvent();
                                eventStarted = true;
                            }
                            ubaDeactivate = false;
                            break;
                        case "tdm_2v2":
                            if (!EventManager.GetTdmEvent())
                            {
                                ActivateTdmEvent(e.name);
                                eventStarted = true;
                            }
                            tdmDeactivate = false;
                            break;
                        case "tdm_3v3":
                            if (!EventManager.GetTdmEvent())
                            {
                                ActivateTdmEvent(e.name);
                                eventStarted = true;
                            }
                            tdmDeactivate = false;
                            break;
                        case "tdm_4v4":
                            if (!EventManager.GetTdmEvent())
                            {
                                ActivateTdmEvent(e.name);
                                eventStarted = true;
                            }
                            tdmDeactivate = false;
                            break;
                    }

                    if (eventStarted) Console.WriteLine("activate event: " + e.name);
                }
            }

            if (spaceballDeactivate)
            {
                if (EventManager.Spaceball.Active)
                {
                    SpaceballStop();
                    Console.WriteLine("deactivate event: spaceball");
                }
            }
            if (x2Deactivate)
            {
                if (Attackable.x2EventActive)
                {
                    DeactivateX2Event();
                    Console.WriteLine("deactivate event: x2");
                }
            }
            if (ubaDeactivate)
            {
                if (EventManager.GetUbaEvent())
                {
                    DeactivateUbaEvent();
                    Console.WriteLine("deactivate Uba-Event");
                }
            }
            if (tdmDeactivate)
            {
                if (EventManager.GetTdmEvent())
                {
                    DeactivateTdmEvent();
                    Console.WriteLine("deactivate Tdm-Event");
                }
            }
            if (rocketDeactivate)
            {
                if (EventManager.Getx2RocketEvent())
                {
                    X2RocketEnd();
                    Console.WriteLine("deactivate event: x2 rocket");
                }
            }
            if (meteoritDeactivate)
            {
                if (EventManager.IceMeteorid)
                {
                    IceMeteoritStop();
                    Console.WriteLine("deactivate event: ice meteorit");
                }
            }
            if (supermeteoritDeactivate)
            {
                if (EventManager.SuperIceMeteorid)
                {
                    SuperIceMeteoritStop();
                    Console.WriteLine("deactivate event: super ice meteorit");
                }
            }
        }

    }
}

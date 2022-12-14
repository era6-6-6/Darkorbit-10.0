using Darkorbit.Chat;
using Darkorbit.Game.Ticks;
using Darkorbit.Helper.packets;
using Darkorbit.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Darkorbit
{
    class Program
    {
        public static bool Running { get; set; } = false;
        
        public static TickManager TickManager = new TickManager();
        public static DateTime cronjobTime = new DateTime();
        public static DateTime timeOnline { get; set; }

        private static bool RestartOngoing = false;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Running = true;
                AppDomain? currentDomain = default(AppDomain);
                currentDomain = AppDomain.CurrentDomain;
                // Handler for unhandled exceptions.
                currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;

                Console.OutputEncoding = Encoding.UTF8;
                CheckMySQLConnection();
                LoadDatabase();
                InitiateServer();
                cronjobTime = DateTime.Now;
                timeOnline = DateTime.Now;
                KeepAlive();
                

            }
            catch (Exception e)
            {
                Out.WriteLine("Main void exception: " + e, "Program.cs");
                Logger.Log("error_log", $"- [Program.cs] Main void exception: {e}");
            }
        }
       
        private static  void KeepAlive()
        {

            while (true)
            {
                Thread.Sleep(34000);
                if (cronjobTime.AddHours(5).AddMinutes(10) < DateTime.Now)
                {
                    cronjobTime = DateTime.Now;
                }
            }
        }

        public static bool CheckMySQLConnection()
        {
            if (!SqlDatabaseManager.Initialized)
            {
                int tries = 0;
            TRY:
                try
                {
                    SqlDatabaseManager.Initialize();
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();

                    Out.WriteLine("Project Darkorbit 10.0 by yusuf rewrited to new net version by era","", ConsoleColor.DarkRed);

                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error mysql " + e);
                    Console.WriteLine("Error mysql " + e);
                    if (tries < 6)
                    {
                        Out.WriteLine("Reconectando mysql .. " + tries + " segundos.");
                        Thread.Sleep(1000);
                        tries++;
                        goto TRY;
                    }
                    else Environment.Exit(0);
                }
            }
            return false;
        }

        public static void LoadDatabase()
        {
            QueryManager.LoadClans();
            QueryManager.LoadShips();
            QueryManager.LoadMaps();
            QueryManager.LoadPremiumMaps();
        }

        public static void InitiateServer()
        {
            //var license = checkLicense();
            var license = "1";
            if (license == "1")
            {
                //Console.WriteLine("License checked sucesfully.");
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
               
                Room.AddRooms();
                EventManager.InitiateEvents();
                StartListening();
            }
            else
            {
                Console.WriteLine("License Error.");
            }

        }

        
        public static void StartListening()
        {
            try
            {
                Net.netty.Handler.AddCommands();
                Task.Run(GameServer.StartListening);
                Out.WriteLine("Listening on port " + GameServer.Port + ".", "Gaming Server", ConsoleColor.Magenta);
                Task.Run(ChatServer.StartListening);
                Out.WriteLine("Listening on port " + ChatServer.Port + ".", "Chat Server", ConsoleColor.Magenta);
                Task.Run(SocketServer.StartListening);
                Out.WriteLine("Listening on port " + SocketServer.Port + ".", "Socket Server", ConsoleColor.Magenta);

                var thread = new Thread(new ThreadStart(TickManager.StartTicker));
                thread.IsBackground = true;
                thread.Start();

                Thread restart = new Thread(new ThreadStart(Autorestart));
                restart.IsBackground = true;
                restart.Start();
                


                Out.WriteLine("Initialized", "Autorestart", ConsoleColor.Magenta);

                ChatClient.LoadFilter();

                Out.WriteLine("Initialized", "ChatFilter", ConsoleColor.Magenta);
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Program.cs] Execute void exception: {ex}");
            }
        }

        public static void Autorestart()
        {
            try
            {
                while (!RestartOngoing)
                {
                    if (DateTime.Now.Hour == 6 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {
                        GameManager.Restart(300, "Daily server restart in 5 minutes 1/5");
                        RestartOngoing = true;
                    }
                    if (DateTime.Now.Hour == 11 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {
                        GameManager.Restart(300, "Daily server restart in 5 minutes 2/5");
                        RestartOngoing = true;
                    }
                    if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {
                        GameManager.Restart(300, "Daily server restart in 5 minutes 3/5");
                        RestartOngoing = true;
                    }
                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {
                        GameManager.Restart(300, "Daily server restart in 5 minutes 4/5");
                        RestartOngoing = true;
                    }
                    if (DateTime.Now.Hour == 1 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {
                        GameManager.Restart(300, "Daily server restart in 5 minutes 5/5");
                        RestartOngoing = true;
                    }

                    Thread.Sleep(14000);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Program.cs] Execute void exception: {ex}");
            }
        }

        public static void ExecuteCommand(string txt)
        {
            var packet = txt.Replace("/", "");
            var splitted = packet.Split(' ');

            switch (splitted[0])
            {
                case "restart":
                    string ms = "";
                    GameManager.Restart(Convert.ToInt32(splitted[1]), ms);
                    break;
                case "list_players":
                    foreach (var gameSession in GameManager.GameSessions.Values)
                    {
                        if (gameSession != null)
                            Out.WriteLine($"{gameSession.Player.Name} ({gameSession.Player.Id})");
                    }
                    break;
            }
        }
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            Logger.Log("error_log", $"- [Program.cs] Execute void exception: {ex}");
        }
    }
}
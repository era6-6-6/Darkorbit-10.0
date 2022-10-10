
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Utils
{
    class Logger
    {
        public static void Log(string fileName, string message)
        {
            if (!message.Contains("ThreadAbortException"))
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                fileName += $"_{DateTime.Now.ToString("dd.MM.yyyy")}.txt";

                try
                {
                    if (!File.Exists(Path.Combine(path, fileName)))
                    {
                        using (FileStream fs = File.Create(path + fileName))
                        {
                            fs.Flush();
                            fs.Close();
                        }
                    }

                    using (StreamWriter sw = File.AppendText(path + fileName))
                    {
                        sw.WriteLine($"[{DateTime.Now.ToString("dd.MM.yyyy HH: mm:ss")}] " + message);
                        sw.Flush();
                        sw.Close();
                    }
                }
                catch (Exception e)
                {
                    Out.WriteLine("Log void exception: " + e, "Logger.cs");
                }
            }
        }

        public static void LogInDb(string playerName, int playerId, string content)
        {
            
            try
            {
                var dateNow = DateTime.Now.ToString("dd.MM.yyyy hh.mm.ss");

                using (var mySqlClient = SqlDatabaseManager.GetClient())
                {
                    mySqlClient.ExecuteNonQuery($"INSERT INTO `chat_log` (`playerName`, `playerId`, `content`, `date`) VALUES ('{playerName}', '{playerId}', '{content}', '{dateNow}');");
                    Out.WriteLine("Saved log for user " + playerName + " (" + playerId + "): \"" + content + "\" in table chat_log.", "", ConsoleColor.Yellow);
                }

            }
            catch (Exception e)
            {
                Out.WriteLine("LogInDb void exception: " + e, "Logger.cs");
            }
        }

    }
}

using System.Net;
using System.Net.Sockets;
using Darkorbit.Chat;

namespace Darkorbit.Net
{
    class ChatServer
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static int Port = 8001;

        public static void StartListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);

            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset();

                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Logger.Log("error_log", $"- [ChatServer.cs] StartListening void exception: {e}");
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();

                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                new ChatClient(handler);
            } 
            catch (Exception e)
            {
                Logger.Log("error_log", $"- [ChatServer.cs] AcceptCallback void exception: {e}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace conrod
{
    public class StateObject
    {
        public Socket clientSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    static class CrankService
    {
        const int PORT = 726;
        /*static private Socket Listener { get
            {
                if (_listener == null)
                    throw new Exception("Socket has not been initialised yet");
                return _listener;
            }
            set 
            {
                if (_listener != null)
                    throw new Exception("Socket has already been initialised");
                _listener = value;
            }
        }
        static private Socket _listener;*/

        public static void Initialise()
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Loopback, PORT));
            listener.Listen(1);
            Socket handler = listener.Accept();
            String data = "";

            while (true)
            {
                byte[] bytes = new byte[1];
                int bytesRecieved = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRecieved);
                if (data.IndexOf("\n") > -1)
                {
                    string[] dataChunks = data.Split('\n');
                    for (int i = 0; i < dataChunks.Length - 1; i++)
                    {
                        Command[] commands = Command.LoadCommands(dataChunks[i]);
                        Console.WriteLine($"Loaded {commands.Length} commands");
                    }
                    data = dataChunks[dataChunks.Length - 1];
                }
                if (data.IndexOf("<EOF>") > -1)
                    break;
            }
            Console.WriteLine("Closed connection");
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}

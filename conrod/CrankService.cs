using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
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
        static private Socket Listener { get
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
        static private Socket _listener;

        public static void Initialise()
        {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Listener.Bind(new IPEndPoint(IPAddress.Loopback, PORT));
            Listener.Listen(1);
            StartListening();
        }

        private static void StartListening()
        {
            Listener.BeginAccept(new AsyncCallback(AcceptCallback), Listener);
        }

        private static void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket client = ((Socket)asyncResult.AsyncState).EndAccept(asyncResult);
            StateObject state = new StateObject();
            state.clientSocket = client;
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult asyncResult)
        {
            StateObject state = (StateObject)asyncResult.AsyncState;
            Socket client = state.clientSocket;

            int read = client.EndReceive(asyncResult);

            if (read > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                if (state.sb.Length > 0)
                {
                    string content = state.sb.ToString();
                    Console.WriteLine($"Read {content.Length} bytes from socket.\nData=({content})");
                }
                client.Close();
                StartListening();
            }
        }
    }
}

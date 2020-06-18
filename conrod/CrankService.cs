using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.IO;

namespace conrod
{
    public class StateObject
    {
        public Socket clientSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    public class CrankService: SetIfDifferentHelper
    {
        const int PORT_SOCKET = 726;
        const string PREFIX_WEB = "http://*:727/socket/";
        public enum Status
        {
            NotInitialised,
            Initialising,
            Listening,
            Accepting,
            InProgress,
            ClosedAndListening
        }
        private object _currentStatusLockObject = new object();
        private Status _currentStatus = Status.NotInitialised;
        private readonly CommandStack commandStack;
        public Status CurrentStatus { get
            {
                lock (_currentStatusLockObject)
                {
                    return _currentStatus;
                }
            }
            set
            {
                lock (_currentStatusLockObject)
                {
                    SetIfDifferent<Status>(ref _currentStatus, value);
                }
            }
        }
        private Socket socketListener;
        private HttpListener httpListener;

        public CrankService(CommandStack commandStack)
        {
            this.commandStack = commandStack;
        }

        public void Initialise()
        {
            socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketListener.Bind(new IPEndPoint(IPAddress.Loopback, PORT_SOCKET));
            socketListener.Listen(1);
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(PREFIX_WEB);
            httpListener.Start();
            CurrentStatus = Status.Listening;
        }

        private void AcceptNewCommandsSocket()
        {
            CurrentStatus = Status.Accepting;
            Socket handler = socketListener.Accept();
            CurrentStatus = Status.InProgress;
            string data = "";

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
                        bool anyLoaded = commandStack.LoadCommandsToStack(dataChunks[i]);
                        if (anyLoaded)
                            NotifyPropertyChange("CommandStackContent");
                    }
                    data = dataChunks[dataChunks.Length - 1];
                }
                if (data.IndexOf("<EOF>") > -1)
                    break;
            }
            Console.WriteLine("Closed connection");
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            CurrentStatus = Status.ClosedAndListening;
        }

        private void AcceptNewCommandsWeb()
        {
            while (true)
            {
                HttpListenerContext context = httpListener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                response.AddHeader("Access-Control-Allow-Origin", "*");
                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.OutputStream.Close();
                    return;
                }
                else if (request.HttpMethod == "POST")
                {
                    StreamReader reader = new StreamReader(request.InputStream);
                    string data = reader.ReadToEnd();
                    response.StatusCode = 200;
                    response.OutputStream.Close();
                    commandStack.LoadCommandsToStack(data);
                }
                
            }
        }

        public void AcceptNewCommandsForever()
        {
            Task.Run(() => AcceptNewCommandsSocket());
            Task.Run(() => AcceptNewCommandsWeb());
        }
    }

    public class StatusTextConverter: IValueConverter
    {
        private const string NotInitialised = "Socket not initialised";
        private const string Initialising = "Socket initialising";
        private const string Listening = "Socket initialised";
        private const string Accepting = "Waiting for connection from crank";
        private const string InProgress = "Connected to crank";
        private const string ClosedAndListening = "Connection to crank closed";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CrankService.Status status = (CrankService.Status)value;
            switch (status)
            {
                case CrankService.Status.NotInitialised:
                    return NotInitialised;
                case CrankService.Status.Initialising:
                    return Initialising;
                case CrankService.Status.Listening:
                    return Listening;
                case CrankService.Status.Accepting:
                    return Accepting;
                case CrankService.Status.InProgress:
                    return InProgress;
                case CrankService.Status.ClosedAndListening:
                    return ClosedAndListening;
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

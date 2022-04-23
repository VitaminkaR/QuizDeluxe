using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuizDeluxe
{
    class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        private IPEndPoint endPoint;
        private IPAddress address;
        private int port;
        private Thread handler;

        public int PacketSize { get; private set; } = 1024;

        public delegate void Handler(string data);
        public Handler HandlerMethod;


        public Client(Handler method)
        {
            client = new TcpClient();
            HandlerMethod = method;
        }



        public void Connect(string ip = "127.0.0.1", int port = 28288)
        {
            try
            {
                address = IPAddress.Parse(ip);
                this.port = port;
                endPoint = new IPEndPoint(address, this.port);
                client.Connect(endPoint);
                stream = client.GetStream();
                handler = new Thread(new ThreadStart(Handle));
                handler.Start();

                "CLIENT:CONNECTING".Log();
            }
            catch
            {
                "CLIENT:CONNECTING:ERROR".Log();
            }
        }

        public void Disconnect()
        {
            handler.Interrupt();
            stream.Close();
            client.Dispose();

            "CLIENT:DISCONNECT".Log();
        }

        private void Handle()
        {
            while (true)
            {
                while (stream.DataAvailable)
                {
                    byte[] bytes = new byte[PacketSize];
                    stream.Read(bytes, 0, bytes.Length);
                    string str = Encoding.UTF8.GetString(bytes);
                    HandlerMethod(str);

                    "CLIENT:RECEIVE".Log();
                }
                Thread.Sleep(1);
            }
        }

        public void Send(string msg)
        {
            try
            {
                byte[] bytes = new byte[PacketSize];
                bytes = Encoding.UTF8.GetBytes(msg + '\n');
                stream.Write(bytes, 0, bytes.Length);

                "CLIENT:SEND".Log();
            }
            catch
            {
                "CLIENT:SEND:ERROR".Log();
            }
        }
    }
}

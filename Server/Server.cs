using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Text;

namespace Server
{
    class Server
    {
        // физическая часть
        private TcpListener server;
        private IPEndPoint endPoint;
        private IPAddress address;
        private int port;

        private List<NetworkStream> clients;
        private Thread serverLoop;

        public bool IsClose { get; private set; }
        public int PacketSize { get; private set; } = 1024;

        public Server(string ip = "127.0.0.1", int port = 28288)
        {
            try
            {
                address = IPAddress.Parse(ip);
                this.port = port;
                IsClose = true;
                endPoint = new IPEndPoint(address, this.port);
                server = new TcpListener(endPoint);
                clients = new List<NetworkStream>();

                "SERVER:CREATE".Log();
                ("SERVER:INFO:" + endPoint.ToString()).Log();
                StartServer();
            }
            catch
            {
                "SERVER:CREATE:ERROR".Log(ConsoleColor.Red);
            }
        }

        public bool StartServer()
        {
            try
            {
                server.Start();
                server.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), server);
                IsClose = false;

                serverLoop = new Thread(new ThreadStart(ServerLoop));
                serverLoop.Start();

                "SERVER:START".Log();

                return true;
            }
            catch
            {
                "SERVER:START:ERROR".Log(ConsoleColor.Red);
                return false;
            }
        }

        public void Stop()
        {
            server.Stop();
            server.Server.Close();
            IsClose = true;

            "SERVER:STOP".Log(ConsoleColor.Red);
        }

        private void AcceptClient(IAsyncResult result)
        {
            try
            {
                TcpListener _server = (TcpListener)result.AsyncState;
                clients.Add(_server.EndAcceptTcpClient(result).GetStream());
                "SERVER:ACCEPT_CLIENT".Log();
                server.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), server);
            }
            catch
            {
                if (!IsClose)
                    "SERVER:ACCEPT_CLIENT:ERROR".Log(ConsoleColor.Red);
            }
        }

        private void ServerLoop()
        {
            while (true)
            {
                if (IsClose)
                    break;

                for (int i = 0; i < clients.Count; i++)
                {
                    while (clients[i].DataAvailable)
                    {
                        byte[] bytes = new byte[PacketSize];
                        clients[i].Read(bytes, 0, bytes.Length);

                        string msg = Encoding.UTF8.GetString(bytes);
                        Handler(msg.Split('\n'), bytes, i);
                    }
                }
                if (!IsClose)
                    Thread.Sleep(1);
            }
        }

        protected void SendPacket(byte[] packet, int id)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                try
                {
                    if (i != id)
                        clients[i].Write(packet, 0, packet.Length);
                }
                catch (System.IO.IOException e)
                {
                    if (e != null)
                    {
                        "SERVER:CLIENT:DISCONNECTED".Log();
                    }
                    "SERVER:SEND:ERROR".Log(ConsoleColor.Red);
                }
                "SERVER:SEND".Log();
            }
        }

        protected void SendPacket(string msg, int id)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(msg + '\n');
                clients[id].Write(bytes, 0, bytes.Length);
                "SERVER:SEND".Log();
            }
            catch (System.IO.IOException e)
            {
                if (e != null)
                {
                    "SERVER:CLIENT:DISCONNECTED".Log();
                }
                "SERVER:SEND:ERROR".Log(ConsoleColor.Red);
            }
        }

        protected void SendPacket(string msg)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(msg + '\n');
                    clients[i].Write(bytes, 0, bytes.Length);
                }
                catch (System.IO.IOException e)
                {
                    if (e != null)
                    {
                        "SERVER:CLIENT:DISCONNECTED".Log();
                    }
                    "SERVER:SEND:ERROR".Log(ConsoleColor.Red);
                }
                "SERVER:SEND".Log();
            }
        }



        protected virtual void Handler(string[] data, byte[] source, int sender)
        {
            $"SERVER:RECEIVE:{data.Length}:{sender}".Log();
        }
    }
}

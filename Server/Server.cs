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
                "SERVER:CREATE:ERROR".Log();
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
                "SERVER:START:ERROR".Log();
                return false;
            }
        }

        public void Stop()
        {
            server.Stop();
            server.Server.Close();
            IsClose = true;

            "SERVER:STOP".Log();
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
                if(!IsClose)
                    "SERVER:ACCEPT_CLIENT:ERROR".Log();
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
                        "SERVER:RECEIVE".Log();
                        byte[] bytes = new byte[PacketSize];
                        clients[i].Read(bytes, 0, bytes.Length);

                        SendPacket(bytes, i);

                        // удаление клиента при его выходе
                        string msg = Encoding.UTF8.GetString(bytes);
                        if (msg.Split('\n')[0] == "disconnect")
                        {
                            clients.Remove(clients[i]);
                            break;
                        }     
                    }
                }
                if (!IsClose)
                    Thread.Sleep(1);
            }
        }

        private void SendPacket(byte[] packet, int id)
        {
            try
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (i != id)
                        clients[i].Write(packet, 0, packet.Length);
                }
                "SERVER:SEND".Log();
            }
            catch
            {
                "SERVER:SEND:ERROR".Log();
            }
        }
    }
}

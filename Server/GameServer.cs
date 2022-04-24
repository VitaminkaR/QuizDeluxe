using System.Collections.Generic;

namespace Server
{
    class GameServer : Server
    {
        // игровая часть
        // ники игроков
        public List<string> players;
        // таблица вопросов
        public Table table;




        public GameServer(string ip = "127.0.0.1", int port = 28288) : base(ip, port)
        {
            players = new List<string>();
            table = new Table();
            table.Load("table\\standart.txt");
            "table load".Log();
        }


        protected override void Handler(string[] data, byte[] source, int sender)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Contains("\0"))
                    break;

                string[] msg = data[i].Split('=');
                string com = msg[0];
                string param = msg[1];

                if (com == "connect")
                {
                    if (!players.Contains(param))
                        players.Add(param);
                    SendPacket(source, sender);
                }
                if(com == "request_nick")
                {
                    string request = "nicknames=";
                    foreach (string nick in players)
                        request += nick + '|';
                    SendPacket(request, sender);
                }
                if(com == "disconnect")
                {
                    players.Remove(param);
                    SendPacket(source, sender);
                }
            }

            base.Handler(data, source, sender);
        }
    }
}

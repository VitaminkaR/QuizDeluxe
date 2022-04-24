using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;

namespace Server
{
    class GameServer : Server
    {
        // игровая часть
        // ники игроков
        public List<string> players;
        // таблица вопросов
        public Table table;
        // правильный ответ
        public string answer;
        // никнейм игрока, который ходит
        public string step;
        public int stepIndex;




        public GameServer(string ip = "127.0.0.1", int port = 28288) : base(ip, port)
        {
            players = new List<string>();
            table = new Table();
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
                if(com == "ans")
                {
                    if(param == answer)
                    {
                        SendPacket("check=true");
                    }
                    else
                        SendPacket("check=false");

                    SetQues();
                }
            }

            base.Handler(data, source, sender);
        }



        public void StartGame()
        {
            "SERVER:GAME:STARTED".Log();

            table.Load("table\\standart.txt");
            "SERVER:TABLE:LOAD".Log();

            SetQues();
        }

        public void SetQues()
        {
            List<string> x = table.Get().Split('|').ToList<string>();
            string q = "q=" + x[0] + '|';
            answer = x[1];
            x.Remove(x[0]);
            for (int i = 0; i < 4; i++)
            {
                int z = new Random().Next(0, x.Count);
                q += x[z] + '|';
                x.RemoveAt(z);
            }
            
            SendPacket(q);
            Thread.Sleep(100);
            step = players[stepIndex];
            stepIndex += 1;
            if (stepIndex == players.Count)
                stepIndex = 0;
            SendPacket($"step={step}");
        }
    }
}

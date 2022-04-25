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
        public List<Player> players;
        // таблица вопросов
        public Table table;
        // правильный ответ
        public string answer;
        // никнейм игрока, который ходит
        public string step;
        public int stepIndex;
        // название таблицы
        public string tableName = "standart";




        public GameServer(string ip = "127.0.0.1", int port = 28288) : base(ip, port)
        {
            players = new List<Player>();
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
                    players.Add(new Player(param));
                    SendPacket(source, sender);
                }
                if(com == "request_nick")
                {
                    string request = "nicknames=";
                    foreach (Player player in players)
                        request += player.name + '|';
                    SendPacket(request, sender);
                }
                if(com == "disconnect")
                {
                    for (int j = 0; j < players.Count; j++)
                        if (players[i].name == param)
                            players.Remove(players[i]);
                    SendPacket(source, sender);
                }
                if(com == "ans")
                {
                    if(param == answer)
                    {
                        SendPacket("check=true");
                        for (int j = 0; j < players.Count; j++)
                            if (players[i].name == step)
                                players[i].score += 1;
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

            table = new Table();
            table.Load($"table\\{tableName}.txt");
            "SERVER:TABLE:LOAD".Log();

            SetQues();
        }

        public void SetQues()
        {
            if(table.Questions.Count == 0)
            {
                "GAME OVER".Log();
                SendPacket("game=over");

                string winner = "";
                int max = 0;
                foreach (Player player in players)
                {
                    if(player.score > max)
                    {
                        max = player.score;
                        winner = player.name;
                    }
                }

                SendPacket($"q=GAME OVER --------------------- WINNER:{winner}|-|-|-|-");

                step = "";
                return;
            }


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
            step = players[stepIndex].name;
            stepIndex += 1;
            if (stepIndex == players.Count)
                stepIndex = 0;
            SendPacket($"step={step}");
        }
    }
}

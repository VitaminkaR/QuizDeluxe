using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private static GameServer server;

        [STAThread]
        static private void Main()
        {
            string _ip = Console.ReadLine();
            if (_ip == "") _ip = "127.0.0.1";
            server = new GameServer(_ip);
            Commands();
        }

        static private void Commands()
        {
            while (true)
            {
                string command = Console.ReadLine();
                if(command == "exit")
                {
                    server.Stop();
                    break;
                }

                if (command == "start")
                    server.StartGame();

                if(command == "players")
                {
                    "\n[PLAYERS]".Log();
                    foreach (string player in server.players)
                        (player).Log();
                }

                if (command == "table")
                {
                    "\n[Table]".Log();
                    foreach (string ques in server.table.Questions)
                        ques.Log();
                }

                if (command == "step")
                    server.step.Log();

                if(command == "help")
                {
                    "\n[HELP]\nexit - stop server\nstart - start game\nplayers - show clients(nicknames)\ntable - show questions\nstep - show sped player".Log();
                }
            }
            Console.ReadKey();
        }
    }
}

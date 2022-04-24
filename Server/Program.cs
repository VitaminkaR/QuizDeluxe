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
            }
            Console.ReadKey();
        }
    }
}

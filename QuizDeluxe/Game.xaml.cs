using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuizDeluxe
{
    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        private Client client;
        private List<string> nicknames;

        public Game(string ip, string nick)
        {
            InitializeComponent();

            client = new Client(Handler);
            client.Connect(ip);
            client.Send($"connect={nick}");

            nicknames = new List<string>();
            nicknames.Add(nick);
            UpdateNicknames();

            Closing += (object sender, System.ComponentModel.CancelEventArgs e) => client.Disconnect();
        }



        // обработка данных клиента
        public void Handler(string data)
        {
            string[] fragments = data.Split('\n');

            for (int i = 0; i < fragments.Length; i++)
            {
                string fragment = fragments[i];
                string com = fragment.Split('=')[0];
                string param = fragment.Split('=')[1];

                if(com == "connect")
                {
                    nicknames.Add(param);
                    UpdateNicknames();
                }
            }
        }

        // обновляет список игроков
        private void UpdateNicknames()
        {
            Players_TextBlock.Text = "Players:\n";
            foreach (string nick in nicknames)
            {
                Players_TextBlock.Text += nick + '\n';
            }
        }
    }
}

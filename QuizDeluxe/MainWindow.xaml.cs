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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizDeluxe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Client client;



        public MainWindow()
        {
            InitializeComponent();

            client = new Client(Handler);
            Connect_Button.Click += (object sender, RoutedEventArgs e) => client.Connect(IP_TextBox.Text);
        }



        // обработка данных клиента
        public void Handler(string data)
        {

        }
    }
}

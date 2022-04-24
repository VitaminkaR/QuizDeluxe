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

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Nick_TextBox.Text = "Player#" + new Random().Next(1000, 10000);

            Connect_Button.Click += (object sender, RoutedEventArgs e) =>
            {
                Close();
                QuizDeluxe.Program.Main(IP_TextBox.Text, Nick_TextBox.Text);
            };
            Exit_Button.Click += (object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        }
    }
}

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

namespace Client
{
    /// <summary>
    /// Interaction logic for PrevsGamesWindow.xaml
    /// </summary>
    public partial class PrevsGamesWindow : Window
    {
        private int a1, a2;

        public PrevsGamesWindow()
        {
            InitializeComponent();
        }

        private void watchButton_Click(object sender, RoutedEventArgs e)
        {
            //make user offline
            //GameWindow window = new GameWindow(a1, a2, true);
            //window.Show();
            this.Close();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow window = new MenuWindow();
            window.Show();
            this.Close();
        }
    }
}

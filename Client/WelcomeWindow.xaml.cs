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
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        int choosenSize, choosenLevel;

        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Show();
            this.Close();
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow window = new RegisterWindow();
            window.Show();
            this.Close();
        }

        private void playOfflineButton_Click(object sender, RoutedEventArgs e)
        {
            playOfflineButton.Visibility = Visibility.Hidden;
            tableSizeGrid.Visibility = Visibility.Visible;
        }

        private void chooseSizeTableClick(object sender, RoutedEventArgs e)
        {
            choosenSize = Int32.Parse(((Button)sender).Tag.ToString());
            tableSizeGrid.Visibility = Visibility.Hidden;
            gameLevelGrid.Visibility = Visibility.Visible;
        }

        private void backSizeTableBtn_Click(object sender, RoutedEventArgs e)
        {
            tableSizeGrid.Visibility = Visibility.Hidden;
            playOfflineButton.Visibility = Visibility.Visible;
        }

        //handle level
        private void chooseLevelGameClick(object sender, RoutedEventArgs e)
        {
            choosenLevel = Int32.Parse(((Button)sender).Tag.ToString());

            GameWindow window = new GameWindow(choosenSize, Level.Easy, true);
            window.Show();
            this.Close();
        }

        private void backLevelBtn_Click(object sender, RoutedEventArgs e)
        {
            gameLevelGrid.Visibility = Visibility.Hidden;
            playOfflineButton.Visibility = Visibility.Visible;
        }
    }
}

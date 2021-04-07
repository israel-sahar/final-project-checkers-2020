using Client.CheckersServiceReference;
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
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private int chosenSize;
        private Level chosenLevel;

        public CheckersServiceClient Client { get; internal set; }
        public ClientCallback Callback { get; internal set; }
        public string User { get; internal set; }

        public MenuWindow()
        {
            InitializeComponent();
        }

        private void vsComputer_Click(object sender, RoutedEventArgs e)
        {
            cmpBtn.Visibility = Visibility.Hidden;
            tableSizeCPUGrid.Visibility = Visibility.Visible;
        }

        private void backCPUBtn_Click(object sender, RoutedEventArgs e)
        {
            tableSizeCPUGrid.Visibility = Visibility.Hidden;
            cmpBtn.Visibility = Visibility.Visible;
        }

        private void chooseSizeTableClick(object sender, RoutedEventArgs e)
        {
            chosenSize = Int32.Parse(((Button)sender).Tag.ToString());
            tableSizeCPUGrid.Visibility = Visibility.Hidden;
            gameLevelGrid.Visibility = Visibility.Visible;
        }

        private void chooseLevelGameClick(object sender, RoutedEventArgs e)
        {
            string level = ((Button)sender).Tag.ToString();
            switch (level)
            {
                case ("Easy"):chosenLevel = Level.Easy;
                    break;
                case ("Medium"):
                    chosenLevel = Level.Medium;
                    break;
                case ("Hard"):
                    chosenLevel = Level.Hard;
                    break;
            }
            GameWindow window = new GameWindow(chosenSize, chosenLevel, true);
            window.Client = Client;
            window.Callback = Callback;
            var gameDetails = Client.JoinGame(User, true, chosenSize);
            window.GameId = gameDetails.Item1;
            window.UserName = User;
            window.Show();
            this.Close();
        }

        private void backLevelBtn_Click(object sender, RoutedEventArgs e)
        {
            gameLevelGrid.Visibility = Visibility.Hidden;
            cmpBtn.Visibility = Visibility.Visible;
        }

        private void vsPlayer_Click(object sender, RoutedEventArgs e)
        {
            playerBtn.Visibility = Visibility.Hidden;
            tableSizePlayerGrid.Visibility = Visibility.Visible;
        }


        private void backPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            tableSizePlayerGrid.Visibility = Visibility.Hidden;
            playerBtn.Visibility = Visibility.Visible;
        }


        private void chooseSizeTablePlayerClick(object sender, RoutedEventArgs e)
        {
            chosenSize = Int32.Parse(((Button)sender).Tag.ToString());

            WaitingWindow window = new WaitingWindow(chosenSize, Level.Human, false);
            window.Callback = Callback;
            window.Client = Client;
            window.Show();
            this.Close();
        }

        private void watchPrevGame_Click(object sender, RoutedEventArgs e)
        {
            PrevsGamesWindow window = new PrevsGamesWindow();
            window.Show();
            this.Close();
        }

        private void watchLiveGame_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

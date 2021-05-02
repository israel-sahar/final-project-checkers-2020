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

            gameLevelGrid.Visibility = Visibility.Hidden;
            eatModePCGrid.Visibility = Visibility.Visible;
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

            tableSizePlayerGrid.Visibility = Visibility.Hidden;
            eatModePlayerGrid.Visibility = Visibility.Visible;
        }
        

        private void watchPrevGame_Click(object sender, RoutedEventArgs e)
        {
            PrevsGamesWindow window = new PrevsGamesWindow(Client, Callback,User);

            window.Show();
            this.Close();
        }

        private void watchLiveGame_Click(object sender, RoutedEventArgs e)
        {

        }
        bool EatMode = true;
        private void chooseEatModePCClick(object sender, RoutedEventArgs e)
        {
            string eatMode = ((Button)sender).Tag.ToString();
            switch (eatMode)
            {
                case ("True"):
                    EatMode = true;
                    break;
                case ("False"):
                    EatMode = false;
                    break;
            }

            GameWindow window = new GameWindow(chosenSize, chosenLevel, true, EatMode);
            window.Client = Client;
            window.Callback = Callback;
            window.UserName = User;

            var gameDetails = Client.JoinGame(User, true, chosenSize,EatMode);
            window.GameId = gameDetails.Item1;
            window.Show();
            this.Close();
        }

        private void backEatPCBtn_Click(object sender, RoutedEventArgs e)
        {
            eatModePCGrid.Visibility = Visibility.Hidden;
            gameLevelGrid.Visibility = Visibility.Visible;
        }

        private void chooseEatModePlayerClick(object sender, RoutedEventArgs e)
        {
            string eatMode = ((Button)sender).Tag.ToString();
            switch (eatMode)
            {
                case ("True"):
                    EatMode = true;
                    break;
                case ("False"):
                    EatMode = false;
                    break;
            }

            WaitingWindow window = new WaitingWindow(Callback, Client, User, chosenSize, Level.Human,EatMode);
            //window.Show();
            this.Close();
        }

        private void backEatPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            eatModePlayerGrid.Visibility = Visibility.Hidden;
            tableSizePlayerGrid.Visibility = Visibility.Visible;
        }
    }
}

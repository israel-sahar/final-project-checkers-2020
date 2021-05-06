using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        int choosenSize;
        public ClientCallback Callback { set; get; }
        public CheckersServiceClient Client { get; set; }


        public WelcomeWindow()
        {
            InitializeComponent();
            Callback = new ClientCallback();
            Client = new CheckersServiceClient(new InstanceContext(Callback));
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Client = Client;
            window.Callback = Callback;
            window.Show();
            this.Close();
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow window = new RegisterWindow();
            window.Client = Client;
            window.Callback = Callback;
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
        Level selectedLevel = Level.Human;
        private void chooseLevelGameClick(object sender, RoutedEventArgs e)
        {
            string level = ((Button)sender).Tag.ToString();
            switch (level)
            {
                case ("Easy"):
                    selectedLevel = Level.Easy;
                    break;
                case ("Medium"):
                    selectedLevel = Level.Medium;
                    break;
                case ("Hard"):
                    selectedLevel = Level.Hard;
                    break;
            }

            gameLevelGrid.Visibility = Visibility.Hidden;
            eatModeGrid.Visibility = Visibility.Visible;
        }

        private void backLevelBtn_Click(object sender, RoutedEventArgs e)
        {
            gameLevelGrid.Visibility = Visibility.Hidden;
            playOfflineButton.Visibility = Visibility.Visible;
        }

        private void chooseEatModeClick(object sender, RoutedEventArgs e)
        {
            string mode = ((Button)sender).Tag.ToString();
            EatMode EatMode = EatMode.Off;
            switch (mode)
            {
                case ("True"):
                    EatMode = EatMode.On;
                    break;
                case ("False"):
                    EatMode = EatMode.Off;
                    break;
            }
            GameWindow window = new GameWindow(choosenSize, selectedLevel, true, EatMode);
            
            window.Show();
            this.Close();
        }

        private void backEatBtn_Click(object sender, RoutedEventArgs e)
        {
            eatModeGrid.Visibility = Visibility.Hidden;
            gameLevelGrid.Visibility = Visibility.Visible;
        }
    }
}

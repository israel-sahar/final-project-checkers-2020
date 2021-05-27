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
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for LiveGamesWindow.xaml
    /// </summary>
    public partial class LiveGamesWindow : Window
    {
        public CheckersServiceClient Client { get; internal set; }
        public ClientCallback Callback { get; internal set; }
        public string UserName { get; internal set; }
        public DispatcherTimer updateTimer { get; set; }

        public LiveGamesWindow(CheckersServiceClient client, ClientCallback callback, string user)
        {
            InitializeComponent();
            Client = client;
            Callback = callback;
            UserName = user;

            NoGamesText.Visibility = Visibility.Visible;
            gamesList.Visibility = Visibility.Hidden;

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 5);
            updateTimer.Tick += UpdateList;
            updateTimer.Start();


        }

        private void UpdateList(object sender, EventArgs e)
        {
            var games = Client.GetGames(true);
            if (games.Count == 0)
            {
                NoGamesText.Visibility = Visibility.Visible;
                gamesList.Visibility = Visibility.Hidden;
            }
            else
            {
                NoGamesText.Visibility = Visibility.Hidden;
                gamesList.Visibility = Visibility.Visible;
                List<LiveGameShow> gamesView = new List<LiveGameShow>();
                foreach (var game in games)
                {
                    gamesView.Add(new LiveGameShow()
                    {
                        GameNumber = game.Item1,
                        Start = game.Item5.TimeOfDay,
                        Player1 = game.Item2,
                        Player2 = game.Item3,
                        TableSize = game.Item7,
                        EatMode = game.Item6
                    });
                }
                gamesList.ItemsSource = gamesView;
            }
        }

        private void watchButton_Click(object sender, RoutedEventArgs e)
        {
            var gameD = ((LiveGameShow)gamesList.SelectedItem);
            var connectionDetails = Client.StartWatchGame(gameD.GameNumber);
            //var moves = Client.GetAllMoves(p.Item1);
            var gameDetails = ConvertGame(gameD);
            WatchingGameWindow window = new WatchingGameWindow(gameDetails, Client, Callback, UserName, connectionDetails.Item1, connectionDetails.Item2);
            window.Show();
            this.Closing -= Window_Closing;
            updateTimer.Stop();
            this.Close();
        }

        private (Game, string, string) ConvertGame(LiveGameShow p)
        {
            Game newGame = new Game()
            {
                GameId = p.GameNumber,
                Status = (int)Status.Unfinished,
                Date = new DateTime(),
                EatMode = p.EatMode,
                BoardSize = p.TableSize
            };
            return (newGame, p.Player1, p.Player2);
        }

        //(moveId,record,(posX,posY),pathI,usrName)

        private List<Move> ConvertMoves(List<(int, DateTime, (int, int), int, string)> item)
        {
            List<Move> move = new List<Move>();
            foreach (var m in item)
            {
                move.Add(new Move()
                {
                    MoveId = m.Item1,
                    RecordTime = m.Item2,
                    posX = m.Item3.Item1,
                    posY = m.Item3.Item2,
                    pathIndex = m.Item4,
                    UserName = m.Item5
                });

            }
            return move;
        }

        private void gamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            watchButton.IsEnabled = gamesList.SelectedItem != null ? true : false;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow window = new MenuWindow();
            window.Callback = Callback;
            window.Client = Client;
            window.User = UserName;
            window.Show();
            this.Closing -= Window_Closing;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("This step will close the app,OK?",
"Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                Client.Disconnect(UserName, -1);
            }
        }
    }

    class LiveGameShow
    {
        public int GameNumber { get; set; }
        public TimeSpan Start { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public bool EatMode { get; internal set; }
        public int TableSize { get; internal set; }
    }


}

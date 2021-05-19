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
    /// Interaction logic for PrevsGamesWindow.xaml
    /// </summary>
    public partial class PrevsGamesWindow : Window
    {

        public CheckersServiceClient Client { get; internal set; }
        public ClientCallback Callback { get; internal set; }
        public string UserName { get; internal set; }
        public DispatcherTimer updateListTimer { get; set; }

        public PrevsGamesWindow(CheckersServiceClient client, ClientCallback callback, string user)
        {
            InitializeComponent();
            Client = client;
            Callback = callback;
            UserName = user;

            NoGamesText.Visibility = Visibility.Visible;
            gamesList.Visibility = Visibility.Hidden;

            updateListTimer = new DispatcherTimer();
            updateListTimer.Interval = new TimeSpan(0, 0, 5);
            updateListTimer.Tick += UpdatePrevList;
            updateListTimer.Start();
        }
        
        
        private void UpdatePrevList(object sender, EventArgs e)
        {
            var games = Client.GetPlayedGames(null, null);
            if (games.Count == 0)
            {
                NoGamesText.Visibility = Visibility.Visible;
                gamesList.Visibility = Visibility.Hidden;
            }
            else
            {
                NoGamesText.Visibility = Visibility.Hidden;
                gamesList.Visibility = Visibility.Visible;
                List<GameShow> gamesView = new List<GameShow>();
                foreach (var game in games)
                {
                    string winner = "";
                    switch (game.Item4)
                    {
                        case Status.isTie:
                            winner = "isTie";
                            break;
                        case Status.OneWon:
                            winner = game.Item2;
                            break;
                        case Status.TwoWon:
                            winner = game.Item3;
                            break;
                    }
                    gamesView.Add(new GameShow()
                    {
                        GameNumber = game.Item1,
                        Start = game.Item5,
                        Player1 = game.Item2,
                        Player2 = game.Item3,
                        Winner = winner
                    });
                }
                gamesList.ItemsSource = gamesView;
            }
        }

        private void watchButton_Click(object sender, RoutedEventArgs e)
        {
            var p = Client.GetGame(((GameShow)gamesList.SelectedItem).GameNumber);
            var moves = Client.GetAllMoves(p.Item1);
            var gameDetails = ConvertGame(p,moves);
            WatchingGameWindow window = new WatchingGameWindow(gameDetails,Client,Callback,UserName);
            window.Show();
            this.Closing -= Window_Closing;
            updateListTimer.Stop();
            this.Close();
        }

        /*(gameId,Status,date,EatMode,boardSize,usr1,usr2,Moves)*/
        private (Game,string,string) ConvertGame((int, Status, DateTime, bool, int, string, string) p,List<(int, DateTime, (int, int), int, string)> moves)
        {
            Game newGame = new Game()
            {
                GameId = p.Item1,
                Status = (int)p.Item2,
                Date = p.Item3,
                EatMode = p.Item4,
                BoardSize = p.Item5,
                Moves = ConvertMoves(moves)
            };
            return (newGame,p.Item6,p.Item7);
        }

        //(moveId,record,(posX,posY),pathI,usrName)

        private List<Move> ConvertMoves(List<(int, DateTime, (int, int), int, string)> item)
        {
            List<Move> move = new List<Move>();
            foreach(var m in item)
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
            updateListTimer.Stop();

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("This step will close the app,OK?",
"Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                
                        Client.Disconnect(UserName, Mode.Lobby, -1);
                
            }
        }
    }

    class GameShow
    {
        public int GameNumber { get; set; }
        public DateTime Start { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Winner { get; set; }
    }


}

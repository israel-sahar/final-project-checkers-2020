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
    /// Interaction logic for WaitingWindow.xaml
    /// </summary>
    public partial class WaitingWindow : Window
    {

        public WaitingWindow(ClientCallback callback,CheckersServiceClient client,string userName, int chosenSize, Level human, EatMode eatMode)
        {
            InitializeComponent();
            ChosenSize = chosenSize;
            Human = human;
            Client = client;
            UserName = userName;
            Callback = callback;
            Callback.OpenNewGame = CreateGameWindow;
            EatMode = eatMode;
            //need to get from client turn,gameid,OpponentName
            //the second player will get this return value the first one will get -1
            (int, string, bool) gameId_OppName_Turn = Client.JoinGame(userName, false, chosenSize, eatMode==EatMode.On?true:false);
            if(gameId_OppName_Turn.Item1!=-1)
                CreateGameWindow(gameId_OppName_Turn.Item1, gameId_OppName_Turn.Item2, gameId_OppName_Turn.Item3);
            
            else
            
                this.Show();
            
        }

        public int ChosenSize { get; }
        public Level Human { get; }
        public bool V { get; }
        public ClientCallback Callback { get; internal set; }
        public CheckersServiceClient Client { get; internal set; }
        public string UserName { get; internal set; }
        public EatMode EatMode { get; private set; }

        public void CreateGameWindow(int gameId, string OpponentName, bool myTurn)
        {
            GameWindow window = new GameWindow(Client, Callback, gameId, UserName, OpponentName,ChosenSize, myTurn,EatMode);

            window.Show();
            this.Close();
        }
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Client.StopWaitingGame(UserName, ChosenSize, (int)EatMode);
            MenuWindow win = new MenuWindow();
            win.Client = Client;
            win.Content = Content;
            win.User = UserName;
            win.Show();
            this.Close();
        }
    }
}

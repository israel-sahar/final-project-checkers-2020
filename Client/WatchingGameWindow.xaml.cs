using Client.CheckersServiceReference;
using SimpleTcp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Interaction logic for WatchingGameWindow.xaml
    /// </summary>
    public partial class WatchingGameWindow : Window
    {
        public CheckersServiceClient Client { get; internal set; }
        public ClientCallback Callback { get; internal set; }
        public string UserName { get; internal set; }

        public EatMode EatMode { get; private set; }
        public int BoardSize { get; set; }
        public string userOne { get; set; }
        public string userTwo { get; set; }
        public bool IsLive { get; set; }
        private Board Game { get; set; }
        public bool Reset { get; private set; }
        public int portToConnect { get; internal set; }
        public string ipToConnect { get; internal set; }

        private Queue<Move> Moves;
        private Queue<Move> MovesCopy;

        public SimpleTcpClient tcpClient { get; set; }
        private readonly SolidColorBrush redColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)0, (byte)0));
        private readonly SolidColorBrush blueColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)0, (byte)0, (byte)255));
        private SolidColorBrush CorrentColor;
        DispatcherTimer animationTimer;

        public WatchingGameWindow((Game, string, string) gameDetails, CheckersServiceClient client, ClientCallback callback, string userName)
        {
            Client = client;
            Callback = callback;
            UserName = userName;
            Moves = new Queue<Move>(gameDetails.Item1.Moves);
            MovesCopy = new Queue<Move>(Moves);
            EatMode = gameDetails.Item1.EatMode?EatMode.On:EatMode.Off;
            BoardSize = gameDetails.Item1.BoardSize;
            userOne = gameDetails.Item2;
            userTwo = gameDetails.Item3;
            Reset = false;
            IsLive = false;
            Init(IsLive);
            ellipse.Fill = redColorPiece;
            Turn.Text = $"This is {userOne} Turn";

        }

        public WatchingGameWindow((Game, string, string) gameDetails, CheckersServiceClient client, ClientCallback callback, string userName, string ip, int port)
        {

            Client = client;
            Callback = callback;
            UserName = userName;
            Moves = new Queue<Move>();
            EatMode = gameDetails.Item1.EatMode ? EatMode.On : EatMode.Off;
            BoardSize = gameDetails.Item1.BoardSize;
            userOne = gameDetails.Item2;
            userTwo = gameDetails.Item3;
            Reset = false;
            IsLive = true;

            Init(IsLive);
            ellipse.Fill = redColorPiece;
            Turn.Text = $"This is {userOne} Turn";

            ipToConnect = ip;
            portToConnect = port;
            tcpClient = new SimpleTcpClient(ipToConnect, portToConnect);

            // set events
            tcpClient.Events.Connected += Connected;
            tcpClient.Events.Disconnected += Disconnected;
            tcpClient.Events.DataReceived += DataReceived;

            // let's go!
            tcpClient.Connect();
        }

        static void Connected(object sender, EventArgs e)
        {
            Console.WriteLine("*** Server connected");
        }

        async void Disconnected(object sender, EventArgs e)
        {
            while (Moves.Count>0) await Dispatcher.Yield();
            if (res == Result.Continue) {
                MessageBox.Show("Some of the players disconnected. The game is stop.");
                animationTimer.Tick -= Animation;

                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MenuWindow window = new MenuWindow();
                    window.Client = Client;
                    window.User = UserName;
                    window.Callback = Callback;
                    window.Show();
                    this.Closed -= Window_Closed;
                    this.Close();
                });

            }
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            var moves = ConvertMoves((List<(int, DateTime, (int, int), int, string)>)ByteArrayToObject(e.Data));
           
            foreach(var move in moves)
                Moves.Enqueue(move);
            
            Application.Current.Dispatcher.Invoke((Action)delegate {
                StartGameAsync();
            });
        }


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

        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
        private void CheckerBord_Initialized(object sender, EventArgs e)
        {
            //Initialize rows and Columns
            for (int i = 0; i < BoardSize; i++)
            {
                GameBoardGrid.RowDefinitions.Add(new RowDefinition());
                GameBoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            //Initialize Board
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Rectangle square = new Rectangle
                    {
                        Fill = (i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0) ? Brushes.SandyBrown : Brushes.Black
                    };

                    AddObject(square, new Point(i, j));
                }
            }

            InitializePieces();
        }

        private void AddObject(object obj, Point pointTo)
        {
            Grid.SetColumn((UIElement)obj, (int)pointTo.Y);
            Grid.SetRow((UIElement)obj, (int)pointTo.X);
            GameBoardGrid.Children.Add((UIElement)obj);
        }

        private void InitializePieces()
        {
            //Initialize first team
            for (int i = 0; i < (BoardSize / 2) - 1; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Ellipse pieces = new Ellipse
                    {
                        Fill = redColorPiece,
                        Margin = new Thickness(5),
                        Stroke = Brushes.White,
                        StrokeThickness = 1,
                        Tag = false
                    };

                    AddObject(pieces, new Point(i, j));
                }
            }

            //Initialize second team
            for (int i = (BoardSize / 2) + 1; i < BoardSize; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Ellipse pieces = new Ellipse
                    {
                        Fill = blueColorPiece,
                        Margin = new Thickness(5),
                        Stroke = Brushes.White,
                        StrokeThickness = 1,
                        Tag = false
                    };

                    AddObject(pieces, new Point(i, j));
                }
            }
        }

        private void Init(bool isLive)
        {
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            animationTimer.Start();

            Game = new Board(BoardSize, EatMode);

            InitializeComponent();
            startBtn.Visibility = (isLive)?Visibility.Hidden:Visibility.Visible;

        }

        private Ellipse GetEllipse(Point pointFrom)
        {
            return (Ellipse)GameBoardGrid.Children.Cast<UIElement>()
            .First(e => Grid.GetRow(e) == pointFrom.X && Grid.GetColumn(e) == pointFrom.Y && (e is Ellipse == true));
        }

        private Ellipse GetEllipse(int x, int y)
        {
            return (Ellipse)GameBoardGrid.Children.Cast<UIElement>()
            .First(e => Grid.GetRow(e) == x && Grid.GetColumn(e) == y && (e is Ellipse == true));
        }

        Ellipse chosenPiece = null;
        private Result MakeMove(Move currentMove,bool animation)
        {
            Piece pToMove = Game.GetPieceAt(currentMove.posX, currentMove.posY);
            chosenPiece = GetEllipse(currentMove.posX, currentMove.posY);

            Path path = Game.GetPathByIndex(pToMove, currentMove.pathIndex);
            (Result, bool) result = Game.MovePiece(pToMove, currentMove.pathIndex);
            Game.VerifyCrown(pToMove);
            MakeAnimationMove(GetPosition(chosenPiece), path, result.Item2);
            return result.Item1;
        }

        Path pathToAnimate;
        Point currentLocation;
        private void MakeAnimationMove(Point currentPosition, Path path, bool isBurn)
        {
            if (isBurn)
            {
                RemoveEllipse(currentPosition);
            }
            else
            {
                pathToAnimate = path;
                currentLocation = currentPosition;
                canContinue = false;
                animationTimer.Tick += Animation;

            }
        }

        bool canContinue;
        private void Animation(object sender, EventArgs e)
        {
            if (pathToAnimate.PathOfPiece.Count != 0)
            {
                Point next = pathToAnimate.GetNextPositin();
                MoveEllipse(currentLocation, next);
                currentLocation = next;
                if (pathToAnimate.EatenPieces.Count != 0)
                {
                    RemoveEllipse(pathToAnimate.EatenPieces[0]);
                    pathToAnimate.EatenPieces.RemoveAt(0);
                }
            }
            else
            {
                animationTimer.Tick -= Animation;
                canContinue = true;
                if (Game.GetPieceAt(GetPosition(chosenPiece)).IsKing) AddKingIcon(Game.GetPieceAt(GetPosition(chosenPiece)).Coordinate);
            }
        }

        private void AddKingIcon(Point coordinate)
        {
            Ellipse ell = GetEllipse(coordinate);

            if (((bool)ell.Tag) == false)
            {
                Image kingImg = new Image()
                {
                    Source = new BitmapImage(new Uri(@"\assets\images\king.png",UriKind.RelativeOrAbsolute)),
                    Width = ell.Width,
                    Height = ell.Height,
                    Margin = new Thickness(12),
                    Cursor = null
                };
                ell.Tag = true;

                AddObject(kingImg, coordinate);
                Grid.SetZIndex(kingImg, Grid.GetZIndex(ell) + 1);
            }
        }

        private void MoveEllipse(Point pointFrom, Point pointTo)
        {
            Ellipse ellipseToMove = GetEllipse(pointFrom);
            Grid.SetColumn(ellipseToMove, (int)pointTo.Y);
            Grid.SetRow(ellipseToMove, (int)pointTo.X);

            if ((bool)ellipseToMove.Tag)
            {
                Image kingImgToMove = GetImage(pointFrom);
                Grid.SetColumn(kingImgToMove, (int)pointTo.Y);
                Grid.SetRow(kingImgToMove, (int)pointTo.X);
            }
        }

        private void RemoveEllipse(Point pointFrom)
        {
            Ellipse ellipseToMove = GetEllipse(pointFrom);
            GameBoardGrid.Children.Remove(ellipseToMove);

            if ((bool)ellipseToMove.Tag)
                GameBoardGrid.Children.Remove(GetImage(pointFrom));
        }

        private Image GetImage(Point pointFrom)
        {
            return (Image)GameBoardGrid.Children.Cast<UIElement>()
                        .First(e => Grid.GetRow(e) == pointFrom.X && Grid.GetColumn(e) == pointFrom.Y && (e is Image == true));
        }

        private Point GetPosition(object objectOnBoard)
        {
            return new Point(Grid.GetRow((UIElement)objectOnBoard), Grid.GetColumn((UIElement)objectOnBoard));
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            startBtn.Content = "START";
            startBtn.IsEnabled = false;
            if (Reset)
            {
                resetTable();
                Moves = new Queue<Move>(MovesCopy);
                Reset = false;
                res = Result.Continue;
            }
            StartGameAsync();

        }
        Result res = Result.Continue;

        private async Task StartGameAsync()
            {
            
                while (res == Result.Continue)
                {
                var p = Moves.Dequeue();
                    if (p.UserName == null) p.UserName = "Computer";
                    Turn.Text = $"This is {p.UserName} Turn";
                    ellipse.Fill = GetEllipse(p.posX, p.posY).Fill;
                    res = MakeMove(p,true);
                    while (!canContinue) await Dispatcher.Yield();
                  Turn.Text = $"This is {(p.UserName == userOne ? userTwo : userOne)} Turn";
                  ellipse.Fill = (ellipse.Fill == redColorPiece) ? blueColorPiece : redColorPiece;
                if (res != Result.Continue)
                    {
                    ellipse.Visibility = Visibility.Collapsed;
                    Turn.HorizontalAlignment = HorizontalAlignment.Center;
                    Turn.FontSize = 30;
                    Turn.FontWeight = FontWeights.Bold;

                        if (res == Result.Lost)
                            Turn.Text = $"{(p.UserName == userOne ? userOne : userTwo)} Lost the Game, {(p.UserName != userOne ? userOne : userTwo)} is the Winner!";

                        if (res == Result.Tie)
                            Turn.Text = "is Tie!";

                        if (res == Result.Win)
                            Turn.Text = $"{(p.UserName != userOne ? userOne : userTwo)} Lost the Game, {(p.UserName == userOne ? userOne : userTwo)} is the Winner!";

                        MessageBox.Show("The Game is ended!");
                    if (IsLive == false)
                    {
                        startBtn.IsEnabled = true;
                        startBtn.Content = "AGAIN";
                        Reset = true;
                    }
                    else
                    {
                        tcpClient.Disconnect();
                    }
                }
                else
                {
                    while (Moves.Count==0) await Dispatcher.Yield();

                }
            }
        }

            public void resetTable()
            {
                Game = new Board(BoardSize, EatMode);
                RemoveAllPieces();
                InitializePieces();
            ellipse.Visibility = Visibility.Visible;
            Turn.HorizontalAlignment = HorizontalAlignment.Left;
            Turn.FontWeight = FontWeights.Normal;

            Turn.FontSize = 20;
        }

            private void RemoveAllPieces()
            {
                var allLefts = GameBoardGrid.Children.Cast<UIElement>()
                            .Where(e => (e is Ellipse == true));
                foreach (var ell in allLefts.ToList())
                {

                    if ((bool)((Ellipse)ell).Tag)
                        GameBoardGrid.Children.Remove(GetImage(GetPosition(ell)));

                    GameBoardGrid.Children.Remove((Ellipse)ell);
                }
            }

        private void Window_Closed(object sender, EventArgs e)
        {
            
            if (MessageBox.Show("This step will close the app,OK?",
"Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                animationTimer.Tick -= Animation;
                Client.Disconnect(UserName, -1);
                if (tcpClient != null)
                {
                    tcpClient.Events.Disconnected -= Disconnected;

                    tcpClient.Disconnect();
                }
            }
        }

        private void leaveBtn_Click(object sender, RoutedEventArgs e)
        {
            animationTimer.Tick -= Animation;

            MenuWindow window = new MenuWindow();
            if (tcpClient != null) {
            tcpClient.Events.Disconnected -= Disconnected;

            tcpClient.Disconnect();
            }
            window.Client = Client;
            window.User = UserName;
            window.Callback = Callback;
            window.Show();
            this.Closed -= Window_Closed;
            this.Close();
        }
    }
    }
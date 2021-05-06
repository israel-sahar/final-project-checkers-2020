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
    /// Interaction logic for WatchingGameWindow.xaml
    /// </summary>
    public partial class WatchingGameWindow : Window
    {
        public CheckersServiceClient Client { get; internal set; }
        public ClientCallback Callback { get; internal set; }
        public string UserName { get; internal set; }

        public int GameId { get; internal set; }
        public EatMode EatMode { get; private set; }
        public int BoardSize { get; set; }
        public string userOne { get; set; }
        public string userTwo { get; set; }
        public bool IsLive { get; set; }
        private Board Game { get; set; }
        public int MoveIndex { get; set; }
        public bool Reset { get; private set; }

        private List<Move> Moves;

        private readonly SolidColorBrush redColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)0, (byte)0));
        private readonly SolidColorBrush blueColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)0, (byte)0, (byte)255));

        DispatcherTimer animationTimer;

        public WatchingGameWindow((Game, string, string) gameDetails, CheckersServiceClient client, ClientCallback callback, string userName, bool isLive)
        {
            Client = client;
            Callback = callback;
            UserName = userName;
            Moves = gameDetails.Item1.Moves;
            EatMode = gameDetails.Item1.EatMode?EatMode.On:EatMode.Off;
            GameId = gameDetails.Item1.GameId;
            BoardSize = gameDetails.Item1.BoardSize;
            userOne = gameDetails.Item2;
            userTwo = gameDetails.Item3;
            Reset = false;

            Init();
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

        private void Init()
        {
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            animationTimer.Start();

            Game = new Board(BoardSize, EatMode);

            InitializeComponent();
            startBtn.IsEnabled = (IsLive) ? false : true;
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
        private Result MakeMove(Move currentMove)
        {
            Piece pToMove = Game.GetPieceAt(currentMove.posX, currentMove.posY);
            chosenPiece = GetEllipse(currentMove.posX, currentMove.posY);

            Path path = Game.GetPathByIndex(pToMove, currentMove.pathIndex);
            (Result, bool) result = Game.MovePiece(pToMove, currentMove.pathIndex);
            Game.VerifyCrown(pToMove);
            MakeAnimationMove(GetPosition(chosenPiece), path, result.Item2);
            return result.Item1;
        }

        private void leaveBtn_Click(object sender, RoutedEventArgs e)
        {
            animationTimer.Tick -= Animation;

            MenuWindow window = new MenuWindow();
            window.Client = Client;
            window.User = UserName;
            window.Callback = Callback;
            window.Show();
            this.Close();
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
                    Source = new BitmapImage(new Uri(@"C:\Users\sahar\Desktop\Github\final-project-checkers-2020\Client\assets\images\king.png")),
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
                Reset = false;
            }
            StartGameAsync();

        }
            private async Task StartGameAsync()
            {
                foreach (var p in Moves)
                {
                    if (p.UserName == null) p.UserName = "Computer";
                    Turn.Text = $"This is {(p.UserName == Moves.ElementAt(0).UserName ? userOne : userTwo)} Turn";
                    ellipse.Fill = GetEllipse(p.posX, p.posY).Fill;
                    var res = MakeMove(p);
                    while (!canContinue) await Dispatcher.Yield();
                    if (res != Result.Continue)
                    {
                    ellipse.Visibility = Visibility.Collapsed;
                    Turn.HorizontalAlignment = HorizontalAlignment.Center;
                    Turn.FontSize = 30;
                    Turn.FontWeight = FontWeights.Bold;

                    if (res == Result.Lost)
                            Turn.Text = $"{(p.UserName == Moves.ElementAt(0).UserName ? userOne : userTwo)} Lost the Game, {(p.UserName != Moves.ElementAt(0).UserName ? userOne : userTwo)} is the Winner!";

                        if (res == Result.Tie)
                            Turn.Text = "is Tie!";

                        if (res == Result.Win)
                            Turn.Text = $"{(p.UserName != Moves.ElementAt(0).UserName ? userOne : userTwo)} Lost the Game, {(p.UserName == Moves.ElementAt(0).UserName ? userOne : userTwo)} is the Winner!";

                        MessageBox.Show("The Game is ended!");
                        startBtn.IsEnabled = true;
                        startBtn.Content = "AGAIN";
                        Reset = true;
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
        }
    }
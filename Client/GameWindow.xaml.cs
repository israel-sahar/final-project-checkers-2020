
using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        //if Client is null so the game is offline
        public CheckersServiceClient Client { get; internal set; }
        public ClientCallback Callback { get; internal set; }

        public int BoardSize { get; set; }
        public bool MyTurn { get; set; }
        private Board Game { get; set; }
        public int GameId { get; internal set; }
        public string UserName { get; internal set; }
        public string OpponentUserName { get; internal set; }
        public Mode PlayerMode { get; set; }

        ComputerMove pcPlayer;
        //colors
        private SolidColorBrush myColor;
        private SolidColorBrush opponentColor;

        private readonly SolidColorBrush redColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)0, (byte)0));
        private readonly SolidColorBrush blueColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)0, (byte)0, (byte)255));

        //timers
        DispatcherTimer animationTimer;

        public Team MyTeam { get; set; }
        public GameWindow(int chosenSize, Level chosenLevel, bool myTurn,EatMode eatMode)
        {
            MyTeam = Team.One;
            
            Client = null;
            Callback = null;
            UserName = null;
            
            BoardSize = chosenSize;
            MyTurn = myTurn;
            OpponentUserName = "Computer";
            pcPlayer = new ComputerMove(chosenLevel);
            Init(eatMode);
        }

        public GameWindow(CheckersServiceClient client, ClientCallback callback, int gameId, string userName, string opponentName, int chosenSize, bool myTurn, EatMode eatMode)
        {
            MyTeam = MyTurn ? Team.One : Team.Two;
            Client = client;
            Callback = callback;
            GameId = gameId;
            UserName = userName;
            MyTurn = myTurn;
            Callback.MakeOpponentMove = MakeOpponentMove;
            BoardSize = chosenSize;
            OpponentUserName = opponentName;
            pcPlayer = null;
            Init(eatMode);
        }

        private void MakeOpponentMove(Point correntPos, int indexPath, Result res)
        {
            resO = res;            
            Piece pToMove = Game.GetPieceAt(correntPos);
            chosenPiece = GetEllipse(correntPos);

            Path path = Game.GetPathByIndex(pToMove, indexPath);
            (Result, bool) result = Game.MovePiece(pToMove, indexPath);
            Game.VerifyCrown(pToMove);
            MakeAnimationMove(GetPosition(chosenPiece), path, result.Item2);
        }

        //Piece to move
        private Ellipse chosenPiece;
        private void MakeMove(object sender, RoutedEventArgs e)
        {
            Path path = ((Button)sender).Tag as Path;
            btnGroup.Clear();
            var lastP = GetPosition(chosenPiece);
            Piece pToMove = Game.GetPieceAt(lastP);
            int pathIndex = Game.GetPathIndex(pToMove, path);
            (Result, bool) res = Game.MovePiece(pToMove, path);
            resO = res.Item1;
            Game.VerifyCrown(pToMove);
            MakeAnimationMove(lastP, path, res.Item2);
            if (Client != null)
                Client.MakeMove(UserName, GameId, DateTime.Now, lastP, pathIndex , resO);
        }

        private void MakeComputerTurn()
        {
            var move = pcPlayer.getNextMove(Game);
            Piece pieceToMove = Game.GetPieceAt(move.Item1.Coordinate);
            Point tempP = pieceToMove.Coordinate;
            int pathIndex = Game.GetPathIndex(pieceToMove, move.Item2);

            chosenPiece = GetEllipse(pieceToMove.Coordinate);
            var res = Game.MovePiece(pieceToMove, move.Item2);
            resO = res.Item1;

            Game.VerifyCrown(move.Item1);

            MakeAnimationMove(GetPosition(chosenPiece), move.Item2, res.Item2);

            if (Client != null)
                Client.MakeMove("Computer", GameId, DateTime.Now, tempP, pathIndex, resO);

            // if (!res.Item2 && Game.GetPieceAt(move.Item1.Coordinate).IsKing) AddKingIcon(move.Item1.Coordinate);
            //do something with result
        }

        Result resO = Result.Continue;
        Point firstP;
        Path pathP = null;
        private void MakeAnimationMove(Point currentPosition, Path path, bool isBurn)
        {
            if (isBurn)
            {
                RemoveEllipse(currentPosition);
                SwitchTurns(false);
            }
            else
            {
                firstP = currentPosition;
                pathP = path;
                animationTimer.Tick += Animation;
            }
        }

        private void Animation(object sender, EventArgs e)
        {
            if (pathP.PathOfPiece.Count != 0)
            {
                Point next = pathP.GetNextPositin();
                MoveEllipse(firstP, next);
                firstP = next;
                if (pathP.EatenPieces.Count != 0)
                {
                    RemoveEllipse(pathP.EatenPieces[0]);
                    pathP.EatenPieces.RemoveAt(0);
                }
            }
            else
            {
                animationTimer.Tick -= Animation;

                if (Game.GetPieceAt(GetPosition(chosenPiece)).IsKing) AddKingIcon(Game.GetPieceAt(GetPosition(chosenPiece)).Coordinate);
                SwitchTurns(false);
            }
        }

        private void Init(EatMode eatMode)
        {
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            animationTimer.Start();

            myColor = (MyTurn) ? redColorPiece : blueColorPiece;
            opponentColor = (MyTurn) ? blueColorPiece : redColorPiece;

            //get eatmode from user
            Game = new Board(BoardSize , eatMode);

            InitializeComponent();
            SwitchTurns(true);
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
                        Fill = MyTurn ? myColor : opponentColor,
                        Margin = new Thickness(5),
                        Stroke = Brushes.White,
                        StrokeThickness = 1,
                        Tag = false
                    };

                    if (pieces.Fill == myColor) pieces.MouseDown += ClickPiece;
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
                        Fill = MyTurn ? opponentColor : myColor,
                        Margin = new Thickness(5),
                        Stroke = Brushes.White,
                        StrokeThickness = 1,
                        Tag = false
                    };

                    if (pieces.Fill == myColor) pieces.MouseDown += ClickPiece;
                    AddObject(pieces, new Point(i, j));
                }
            }
        }

        ObservableCollection<Button> btnGroup;
        private void ClickPiece(object sender, MouseButtonEventArgs e)
        {
            if (MyTurn == false) return;

            Point position = GetPosition(sender);
            chosenPiece = GetEllipse(position);

            Piece selectedPiece = Game.GetPieceAt(position);
            List<Path> paths = selectedPiece.GetPossibleMoves(Game);

            btnGroup = new ObservableCollection<Button>();
            for (int i = 0; i < paths.Count; i++)
            {
                Button b = new Button()
                {
                    Tag = paths[i],
                    Content = i + 1,
                    Style = (Style)this.Resources["RoundedButtonStyle"],
                    Padding = new Thickness(15),
                    Margin = new Thickness(3)
                };
                b.MouseEnter += ShowPath;
                b.MouseLeave += HidePath;
                b.Click += MakeMove;

                btnGroup.Add(b);

            }
            DataContext = btnGroup;
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
                kingImg.MouseDown += ClickPiece;
                ell.Tag = true;

                AddObject(kingImg, coordinate);
                Grid.SetZIndex(kingImg, Grid.GetZIndex(ell) + 1);
            }
        }

        List<Rectangle> recs = new List<Rectangle>();
        List<TextBlock> tbs = new List<TextBlock>();
        private void HidePath(object sender, MouseEventArgs e)
        {
            foreach (var a in recs)
                GameBoardGrid.Children.Remove(a);
            foreach (var a in tbs)
                GameBoardGrid.Children.Remove(a);
            tbs.Clear();
            recs.Clear();
        }

        private void ShowPath(object sender, MouseEventArgs e)
        {
            var path = ((Button)sender).Tag as Path;
            foreach (Point p in path.PathOfPiece)
            {
                Rectangle square = new Rectangle
                {
                    Fill = myColor,
                    Opacity = 0.5,
                    Margin = new Thickness(5)
                };

                AddObject(square, p);

                TextBlock indexSquare = new TextBlock()
                {
                    Text = (path.PathOfPiece.IndexOf(p) + 1).ToString(),
                    FontSize = 24,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)255, (byte)255)),
                    FontWeight = FontWeights.Bold
                };

                AddObject(indexSquare, p);

                recs.Add(square);
                tbs.Add(indexSquare);
            }
        }

        /// <summary>
        /// get position of an element on board by index
        /// </summary>
        /// <param name="objectTag">Tag of an element</param>
        /// <returns>(Row,Column)</returns>
        private Point GetPosition(object objectOnBoard)
        {
            return new Point(Grid.GetRow((UIElement)objectOnBoard), Grid.GetColumn((UIElement)objectOnBoard));
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

        private void SwitchTurns(bool isInit)
        {
            chosenPiece = null;

            if ((isInit && MyTurn) || (!isInit&&!MyTurn))
                Turn.Text = "This is Your Turn";
            else
                Turn.Text = $"This is {OpponentUserName} Turn";


            ellipse.Fill = (isInit && MyTurn) || (!isInit && !MyTurn) ? myColor: opponentColor;
            MyTurn = isInit?MyTurn:!MyTurn;
            //switched turns
            if (resO == Result.Continue && !MyTurn && !isInit && pcPlayer!=null)
                MakeComputerTurn();
            else
            {
                //fix!
                if (resO != Result.Continue) {
                    ellipse.Visibility = Visibility.Collapsed;
                    Turn.HorizontalAlignment = HorizontalAlignment.Center;
                    Turn.FontSize = 30;
                    Turn.FontWeight = FontWeights.Bold;
                    if (MyTurn && resO == Result.Lost || !MyTurn && resO == Result.Win)
                        Turn.Text = "Great! You Won!";
                    if(resO == Result.Tie)
                        Turn.Text = "is Tie!";
                    if (MyTurn && resO == Result.Win || !MyTurn && resO == Result.Lost)
                        Turn.Text = "Try next time!";

                    MessageBox.Show("The Game is ended!");
                }
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

        private Ellipse GetEllipse(Point pointFrom)
        {
            return (Ellipse)GameBoardGrid.Children.Cast<UIElement>()
            .First(e => Grid.GetRow(e) == pointFrom.X && Grid.GetColumn(e) == pointFrom.Y && (e is Ellipse == true));
        }

        private void AddObject(object obj, Point pointTo)
        {
            Grid.SetColumn((UIElement)obj, (int)pointTo.Y);
            Grid.SetRow((UIElement)obj, (int)pointTo.X);
            GameBoardGrid.Children.Add((UIElement)obj);
        }

        private void leaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //handle things before closing the game
            if (Client == null) {
                WelcomeWindow win = new WelcomeWindow();
                win.Show();
            }
            else
            {
                if(resO == Result.Continue)
                    Client.CloseUnFinishedGame(GameId, UserName);
                MenuWindow window = new MenuWindow();
                window.Client = Client;
                window.User = UserName;
                window.Callback = Callback;
                window.Show();
            }

            this.Close();
        }
    }
}

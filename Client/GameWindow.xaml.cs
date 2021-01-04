
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public int BoardSize { get; set; }
        public bool MyTurn { get; set; }

        private Ellipse[,] UiBoard;
        //private Ellipse[,] FullBoardAnimation;
       private Board Game { get; set; }

        private bool isOffline;
        private int chosenLevel;
        
        //colors
        private SolidColorBrush myColor;
        private SolidColorBrush opponentColor;

        private SolidColorBrush brownBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dbbd97"));
        private SolidColorBrush blackBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
        private SolidColorBrush redColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)0, (byte)0));
        private SolidColorBrush blueColorPiece = new SolidColorBrush(Color.FromArgb(150, (byte)0, (byte)0, (byte)255));


        public GameWindow(int chosenSize, int chosenLevel, bool v)
        {
            //assuming
            BoardSize = chosenSize;
            this.chosenLevel = chosenLevel;
            MyTurn = v;
            Init();


        }

        public GameWindow() {
            BoardSize = 10;
            MyTurn = true;
            Init();
        }

        private void Init() {
            myColor = (MyTurn) ? redColorPiece : blueColorPiece;
            opponentColor = (MyTurn) ? blueColorPiece : redColorPiece;

            Game = new Board(BoardSize, (MyTurn) ? Direction.Down : Direction.Up);

            UiBoard = new Ellipse[BoardSize, BoardSize];
            //FullBoardAnimation = new Ellipse[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {

                    UiBoard[i, j] = null;
                    //FullBoardAnimation[i, j] = null;
                }
            }
            InitializeComponent();

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
                        Fill = (i % 2 == 0 && j % 2 == 0)||(i % 2 != 0 && j % 2 !=0) ? brownBrush : blackBrush
                    };
                    //if(square.Fill.Equals(blackBrush)) square.MouseDown += ClickSquare;
                    Grid.SetColumn(square, i);
                    Grid.SetRow(square, j);

                    GameBoardGrid.Children.Add(square);
                }
            }

            InitializePieces();
        }

        private void InitializePieces()
        {
            //Initialize my team
            for (int i = 0; i < (BoardSize / 2) - 1; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                Ellipse pieces = new Ellipse
                    {
                        Fill = MyTurn?myColor:opponentColor,
                        Margin = new Thickness(5),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White")),
                        StrokeThickness = 1
                    };

                    if(pieces.Fill == myColor) pieces.MouseDown += ClickPiece;
                    Grid.SetColumn(pieces, j);
                    Grid.SetRow(pieces, i);

                    UiBoard[i, j] = pieces;
                    GameBoardGrid.Children.Add(pieces);
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
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White")),
                        StrokeThickness = 1
                    };
                    
                    if (pieces.Fill == myColor) pieces.MouseDown += ClickPiece;
                    Grid.SetColumn(pieces, j);
                    Grid.SetRow(pieces, i);

                    UiBoard[i, j] = pieces;
                    GameBoardGrid.Children.Add(pieces);
                }
            }
        }

        ObservableCollection<Button> btnGroup;
        private void ClickPiece(object sender, MouseButtonEventArgs e)
        {
            if (MyTurn == false) return;
             Ellipse piece = sender as Ellipse;
            Point position = GetPosition(piece);

                   chosenPiece = piece;
                Piece p = Game.GetPieceAt(position);
                p.CalculatePossibleMoves(Game);
                List<Path> paths = p.OptionalPaths;
            btnGroup = new ObservableCollection<Button>();
            for (int i = 0; i < paths.Count; i++) {
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

        //Piece to move
        private Ellipse chosenPiece;
        Point currentPositionPiece;
        private void MakeMove(object sender, RoutedEventArgs e)
        {
            Path path = ((Button)sender).Tag as Path;
            btnGroup.Clear();
            //make animatin

            //switch turns
            currentPositionPiece = GetPosition(chosenPiece);
            (Result,bool) res = Game.MovePiece(Game.GetPieceAt(GetPosition(chosenPiece)), path);
            //Grid.SetZIndex(chosenPiece, 3);
            UiBoard[(int)currentPositionPiece.X, (int)currentPositionPiece.Y] = null;
            if (res.Item2)
            {
                GameBoardGrid.Children.Remove(chosenPiece);
            }
            else
            {
                UiBoard[(int)(path.getLastPosition().X), (int)(path.getLastPosition().Y)] = chosenPiece;

                for (int i = 0; i < path.PathOfPiece.Count; i++)
                {
                    // toWhereOnCanvas = FullBoardAnimation[(int)p.X, (int)p.Y].TranslatePoint(new Point(0, 0), CheckerBord);
                    //FromWhereOnCanvas = FullBoardAnimation[(int)currentPositionPiece.X, (int)currentPositionPiece.Y].TranslatePoint(new Point(0, 0), CheckerBord);

                    GameBoardGrid.Children.Remove(chosenPiece);
                    Grid.SetColumn(chosenPiece, (int)path.PathOfPiece[i].Y);
                    Grid.SetRow(chosenPiece, (int)path.PathOfPiece[i].X);
                    GameBoardGrid.Children.Add(chosenPiece);
                    if (path.EatenPieces.Count != 0)
                    {
                        GameBoardGrid.Children.Remove(UiBoard[(int)(path.EatenPieces[0].Coordinate.X), (int)(path.EatenPieces[0].Coordinate.Y)]);
                        UiBoard[(int)(path.EatenPieces[0].Coordinate.X), (int)(path.EatenPieces[0].Coordinate.Y)] = null;
                        path.EatenPieces.RemoveAt(0);
                    }
                }
                //do something with result
            }
            chosenPiece = null;
            MyTurn = false;
            MakeComputerTurn();
        }

        private void MakeComputerTurn()
        {
            var camputerPieces = Game.OpponentTeamPieces;
            (Piece,Path) reqPiece=(null,null);
            List<(Piece, Path)> forRandomChoose = new List<(Piece, Path)>();
            foreach (Piece p in camputerPieces) {
                p.CalculatePossibleMoves(Game);
                foreach (Path path in p.OptionalPaths) {
                forRandomChoose.Add((p, path));
                if (reqPiece.Item1 == null || path.EatenPieces.Count > reqPiece.Item2.EatenPieces.Count)
                {
                    reqPiece.Item1 = p; reqPiece.Item2 = path;
                }
            }
            }
            if (reqPiece.Item2.EatenPieces.Count==0)
            {
                int rInt = (new Random(DateTime.Now.Millisecond)).Next(1, forRandomChoose.Count + 1);
                reqPiece.Item1 = forRandomChoose.ElementAt(rInt - 1).Item1;
                reqPiece.Item2 = forRandomChoose.ElementAt(rInt - 1).Item2;
            }
            currentPositionPiece = new Point(reqPiece.Item1.Coordinate.X, reqPiece.Item1.Coordinate.Y);
            var res = Game.MovePiece(reqPiece.Item1,reqPiece.Item2);
            chosenPiece = UiBoard[(int)currentPositionPiece.X, (int)currentPositionPiece.Y];

            UiBoard[(int)currentPositionPiece.X, (int)currentPositionPiece.Y] = null;
            if (res.Item2)
            {
                GameBoardGrid.Children.Remove(chosenPiece);
            }
            else
            {
                UiBoard[(int)(reqPiece.Item2.getLastPosition().X), (int)(reqPiece.Item2.getLastPosition().Y)] = chosenPiece;

                for (int i = 0; i < reqPiece.Item2.PathOfPiece.Count; i++)
                {
                    // toWhereOnCanvas = FullBoardAnimation[(int)p.X, (int)p.Y].TranslatePoint(new Point(0, 0), CheckerBord);
                    //FromWhereOnCanvas = FullBoardAnimation[(int)currentPositionPiece.X, (int)currentPositionPiece.Y].TranslatePoint(new Point(0, 0), CheckerBord);

                    GameBoardGrid.Children.Remove(chosenPiece);
                    Grid.SetColumn(chosenPiece, (int)reqPiece.Item2.PathOfPiece[i].Y);
                    Grid.SetRow(chosenPiece, (int)reqPiece.Item2.PathOfPiece[i].X);
                    GameBoardGrid.Children.Add(chosenPiece);
                    if (reqPiece.Item2.EatenPieces.Count != 0)
                    {
                        GameBoardGrid.Children.Remove(UiBoard[(int)(reqPiece.Item2.EatenPieces[0].Coordinate.X), (int)(reqPiece.Item2.EatenPieces[0].Coordinate.Y)]);
                        UiBoard[(int)(reqPiece.Item2.EatenPieces[0].Coordinate.X), (int)(reqPiece.Item2.EatenPieces[0].Coordinate.Y)] = null;
                        reqPiece.Item2.EatenPieces.RemoveAt(0);
                    }
                }
            }
            MyTurn = true ;
            //do something with result
        }

        private void HidePath(object sender, MouseEventArgs e)
        {
            foreach (var a in recs)
                GameBoardGrid.Children.Remove(a);
            foreach(var a in tbs)
                GameBoardGrid.Children.Remove(a);
            tbs.Clear();
            recs.Clear();
        }

        List<Rectangle> recs = new List<Rectangle>();
        List<TextBlock> tbs = new List<TextBlock>();

        private void ShowPath(object sender, MouseEventArgs e)
        {
            var path = ((Button)sender).Tag as Path;
            foreach (Point p in path.PathOfPiece)
            {
                Rectangle square = new Rectangle
                {
                    Fill = myColor
                };
                square.Opacity = 0.5;
                square.Margin = new Thickness(5);
                Grid.SetColumn(square, (int)p.Y);
                Grid.SetRow(square, (int)p.X);

                GameBoardGrid.Children.Add(square);

                TextBlock tb = new TextBlock();
                tb.Text = (path.PathOfPiece.IndexOf(p)+1).ToString();
                tb.FontSize = 24;
                tb.Margin = new Thickness(5);
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Foreground = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)255, (byte)255));
                tb.FontWeight = FontWeights.Bold;
                Grid.SetColumn(tb, (int)p.Y);
                Grid.SetRow(tb, (int)p.X);

                GameBoardGrid.Children.Add(tb);
                recs.Add(square);
                tbs.Add(tb);
            }
        }

        /// <summary>
        /// get position of an element(sqaure,piece) on board by index
        /// </summary>
        /// <param name="objectTag">Tag of an element</param>
        /// <returns>(Row,Column)</returns>
        private Point GetPosition(object objectOnBoard)
        {
            if (objectOnBoard is Ellipse)
                return new Point(Grid.GetRow((Ellipse)objectOnBoard), Grid.GetColumn((Ellipse)objectOnBoard));

            return new Point(Grid.GetRow((Rectangle)objectOnBoard), Grid.GetColumn((Rectangle)objectOnBoard));
        }

        private Ellipse GetEllipse(Point point)
        {
            return (Ellipse)GameBoardGrid.Children.Cast<UIElement>()
            .First(e => Grid.GetRow(e) == point.X && Grid.GetColumn(e) == point.Y && (e is Ellipse==true));
        }

        private void SetEllipse(Point pointFrom,Point pointTo)
        {
            
        }

        private void SetEllipse(object obj, Point pointTo)
        {

        }

        private void leaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //handle things before closing the game

            MenuWindow window = new MenuWindow();
            window.Show();
            this.Close();
        }
    }
}

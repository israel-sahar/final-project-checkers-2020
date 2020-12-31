
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
       private Board Game { get; set; }
      
        //Piece to move
        private Ellipse chosenPiece;

        private bool isOffline;
        private int chosenSize, chosenLevel;
        
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
            BoardSize = 10;
            MyTurn = true;

            myColor = (MyTurn) ? redColorPiece : blueColorPiece;
            opponentColor = (MyTurn) ? blueColorPiece : redColorPiece;

            Game = new Board(BoardSize, (MyTurn)?Direction.Down:Direction.Up);

            UiBoard = new Ellipse[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                    UiBoard[i, j] = null;
            }

            InitializeComponent();

        }

        private void FourInARowBord_Initialized(object sender, EventArgs e)
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
                    if(square.Fill.Equals(blackBrush)) square.MouseDown += ClickSquare;
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
                        StrokeThickness = 1,

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

        private void ClickPiece(object sender, MouseButtonEventArgs e)
        {
            Ellipse piece = sender as Ellipse;
            Point position = GetPosition(piece);
            if (MyTurn)
            {
                    chosenPiece = piece;
                   //Game.PossibleMoves(position);
            }
                

        }

        private void ClickSquare(object sender, MouseButtonEventArgs e)
        {
            Rectangle square = sender as Rectangle;
            Point positionTo = GetPosition(square);
           if (Game.IsSqaureEmpty(positionTo))
            {
                // there is a piece to move
                if (chosenPiece != null)
                {
                    Point positionFrom = GetPosition(chosenPiece);
                    GameBoardGrid.Children.Remove(chosenPiece);

                    Grid.SetColumn(chosenPiece, (int)positionTo.Y);
                    Grid.SetRow(chosenPiece, (int)positionTo.X);

                    UiBoard[(int)positionFrom.X, (int)positionFrom.Y] = null;
                    UiBoard[(int)positionTo.X, (int)positionTo.Y] = chosenPiece;
                    //Game.MovePiece(Game.GetPieceAt(positionFrom), positionTo)
                    GameBoardGrid.Children.Add(chosenPiece);
                    chosenPiece = null;
                }
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

        private void leaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //handle things before closing the game

            MenuWindow window = new MenuWindow();
            window.Show();
            this.Close();
        }
    }
}

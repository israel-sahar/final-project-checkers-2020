
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

        Ellipse[,] UiBoard;
        GameChecker BackendGame { get; set; }
        public bool MyTurn { get; set; }

        //Piece to move
        Ellipse chosenPiece;

        private bool isOffline;
        private int chosenSize, chosenLevel;
        //colors
        private SolidColorBrush myColor;
        private SolidColorBrush opponentColor;

        SolidColorBrush brownBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dbbd97"));
        SolidColorBrush blackBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));

        public GameWindow()
        {
            //assuming
            BoardSize = 10;
            MyTurn = true;

            if (!MyTurn)
            {
                myColor = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)0, (byte)0));
                opponentColor = new SolidColorBrush(Color.FromArgb(150, (byte)0, (byte)0, (byte)255));
            }
            else
            {
                myColor = new SolidColorBrush(Color.FromArgb(150, (byte)0, (byte)0, (byte)255));
                opponentColor = new SolidColorBrush(Color.FromArgb(150, (byte)255, (byte)0, (byte)0));
            }

            BackendGame = new GameChecker(BoardSize, MyTurn);
            UiBoard = new Ellipse[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                    UiBoard[i, j] = null;
            }
            InitializeComponent();

        }

        public GameWindow(int chosenSize, int chosenLevel, bool v)
        {
            this.chosenSize = chosenSize;
            this.chosenLevel = chosenLevel;
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
                        Tag = $"{SYM.None}_{j}_{i}",
                        Fill = j % 2 == 0 ? brownBrush : blackBrush
                    };
                    square.MouseDown += ClickSquare;
                    Grid.SetColumn(square, i);
                    Grid.SetRow(square, j);

                    GameBoardGrid.Children.Add(square);
                }
                //swap color
                SolidColorBrush temp = brownBrush;
                brownBrush = blackBrush;
                blackBrush = temp;
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
                    int Team = (int)(MyTurn ? SYM.Me : SYM.Opponent);
                Ellipse pieces = new Ellipse
                    {
                        Tag = $"{Team}_{i}_{j}",
                        Fill = MyTurn?myColor:opponentColor,
                        Margin = new Thickness(5),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White")),
                        StrokeThickness = 1,

                    };
                    pieces.MouseDown += ClickPiece;
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
                    int Team = (int)(MyTurn ? SYM.Opponent : SYM.Me);
                    Ellipse pieces = new Ellipse
                    {
                        Tag = $"{Team}_{i}_{j}",
                        Fill = MyTurn ? opponentColor : myColor,
                        Margin = new Thickness(5),
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White")),
                        StrokeThickness = 1
                    };
                    pieces.MouseDown += ClickPiece;
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
            SYM pieceTeam = Int32.Parse(piece.Tag.ToString()[0].ToString())== (int)SYM.Me? SYM.Me:SYM.Opponent;
            Vector position = GetPosition(piece.Tag.ToString());
            if (MyTurn)
            {
                if (pieceTeam == SYM.Me) {
                    chosenPiece = piece;
                    BackendGame.PossibleMoves(position);
                }
            }
                

        }

        private void ClickSquare(object sender, MouseButtonEventArgs e)
        {
            Rectangle square = sender as Rectangle;
            if (square.Fill.Equals(brownBrush)) return ;
            Vector position = GetPosition(square.Tag.ToString());
           if (BackendGame.IsSqaureEmpty(position))
            {
                // there is a piece to move
                if (chosenPiece != null)
                {
                    if (UiBoard[(int)position.X, (int)position.Y] != null)
                    {
                        chosenPiece = null;
                        return;
                    }
                    GameBoardGrid.Children.Remove(chosenPiece);

                    Grid.SetColumn(chosenPiece, (int)position.Y);
                    Grid.SetRow(chosenPiece, (int)position.X);
                    int lastX = Int32.Parse(((string)chosenPiece.Tag)[2].ToString()),
                        lastY = Int32.Parse(((string)chosenPiece.Tag)[4].ToString());
                    UiBoard[lastX, lastY] = null;
                    UiBoard[(int)position.X, (int)position.Y] = chosenPiece;
                    chosenPiece.Tag = ((string)chosenPiece.Tag)[0] + $"_{(int)position.X}_{(int)position.Y}";

                    GameBoardGrid.Children.Add(chosenPiece);
                    chosenPiece = null;

                }
            }

        }

        /// <summary>
        /// get position of an element(sqaure,piece) on board by index
        /// </summary>
        /// <param name="objectTag">Tag of an element</param>
        /// <returns></returns>
        private Vector GetPosition(string objectTag)
        {
            String[] strlist = objectTag.Split('_');
            if(strlist.Length==3)
                return new Vector(Int32.Parse(strlist[1]), Int32.Parse(strlist[2]));

            return new Vector(Int32.Parse(strlist[0]), Int32.Parse(strlist[1]));
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

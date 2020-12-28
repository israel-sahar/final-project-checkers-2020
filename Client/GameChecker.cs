using System;
using System.Collections.Generic;
using System.Windows;

namespace Client
{
    enum SYM { Me, Opponent, None }
    enum Direction { Up = -1, Down = 1 }

    public class GameChecker
    {
        Piece[,] board;
        public int BoardSize { get; private set; }
        Direction MyDirection { get; set; }

        public GameChecker(int boardSize, bool startToPlay)
        {
            this.BoardSize = boardSize;
            MyDirection = startToPlay ? Direction.Down : Direction.Up;
            InitBoard();
        }

        public void InitBoard()
        {
            board = new Piece[BoardSize, BoardSize];

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                    board[i, j] = null;
            }

            GeneratePiece();
        }

        private void GeneratePiece()
        {
            //ganerate my team
            for (int i = 0; i < (BoardSize / 2) - 1; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Piece pieces = new Piece
                    {
                        Team = MyDirection == Direction.Down ? (int)SYM.Me : (int)SYM.Opponent
                    };
                    board[i, j] = pieces;
                }
            }

            //generate opponent
            for (int i = (BoardSize / 2) + 1; i < BoardSize; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Piece pieces = new Piece
                    {
                        Team = MyDirection == Direction.Down ? (int)SYM.Opponent : (int)SYM.Me
                    };
                    board[i, j] = pieces;
                }
            }
        }

        public List<List<Vector>> PossibleMoves(Vector coord)
        {
            List<List<Vector>> options = new List<List<Vector>>();
            Vector right, left;
            (left, right) = isHaveToEat(coord);
            if (left.X != -1 || right.X != -1) {
                if (left.X != -1)
                {
                    List<Vector> option = new List<Vector>();
                    option.Add(left);
                    options.Add(option);
                }
                if (right.X != -1)
                {
                    List<Vector> option = new List<Vector>();
                    option.Add(right);
                    options.Add(option);
                }
            }
                do
                {
                foreach (List<Vector> option in options.ToArray()) {
                    (left, right) = isHaveToEat(option[option.Count-1]);
                    List<Vector> tempLeft = new List<Vector>(option);
                    List<Vector> tempRight = new List<Vector>(option);

                    if(left.X!=-1) { 
                        tempLeft.Add(left);
                        options.Add(tempLeft);
                        options.Remove(option);
                    }

                    if (right.X != -1)
                    {
                        tempRight.Add(right);
                        options.Add(tempRight);
                        try
                        {
                            options.Remove(option);
                        }
                        catch (Exception) { };
                    }
                }
                } 
                while (!isDonePossibleMoves(options));
            if (options.Count == 0)
            {
                if (isCoordinateOnBoard((int)coord.X + (int)MyDirection, (int)coord.Y + 1) &&
                    board[(int)coord.X + (int)MyDirection, (int)coord.Y + 1] == null)
                {
                    List<Vector> option = new List<Vector>();
                    option.Add(new Vector((int)coord.X + (int)MyDirection, (int)coord.Y + 1));
                    options.Add(option);
                }
                if (isCoordinateOnBoard((int)coord.X + (int)MyDirection, (int)coord.Y - 1) &&
                    board[(int)coord.X + (int)MyDirection, (int)coord.Y - 1] == null)
                {
                    List<Vector> option = new List<Vector>();
                    option.Add(new Vector((int)coord.X + (int)MyDirection, (int)coord.Y - 1));
                    options.Add(option);
                }
            }
            return options;
        }

        private bool isDonePossibleMoves(List<List<Vector>> options)
        {
            foreach (List<Vector> option in options) {
                if (isHaveToEat(option[option.Count - 1]).Item1.X!=-1 || isHaveToEat(option[option.Count - 1]).Item2.X!=-1) return false;
            }
            return true;
        }

        public bool IsMoveLegal(int TeamTurn, Vector coordFrom, Vector coordTo)
        {
            if (board[(int)coordFrom.X, (int)coordFrom.Y] == null) return false;
            if (board[(int)coordTo.X, (int)coordTo.Y] != null) return false;
            if (board[(int)coordFrom.X, (int)coordFrom.Y].Team != TeamTurn) return false;



            return true;
        }

        public bool IsSqaureEmpty(Vector coord)
        {
            return board[(int)coord.X, (int)coord.Y] == null;
        }

        public (Vector, Vector) isHaveToEat(Vector coord)
        {
            Vector leftSide =new Vector(-1,-1)
                , rightSide = new Vector(-1, -1);
            if (isCoordinateOnBoard((int)coord.X + (int)MyDirection, (int)coord.Y + 1) &&
                board[(int)coord.X + (int)MyDirection, (int)coord.Y + 1]!=null&&
                board[(int)coord.X + (int)MyDirection, (int)coord.Y + 1].Team == (int)SYM.Opponent)
            {
                if (isCoordinateOnBoard((int)coord.X + (int)MyDirection * 2, (int)coord.Y + 2) &&
                    board[(int)coord.X + (int)MyDirection * 2, (int)coord.Y + 2] == null)
                    rightSide = new Vector((int)coord.X + (int)MyDirection * 2, (int)coord.Y + 2);
            }

            if (isCoordinateOnBoard((int)coord.X + (int)MyDirection, (int)coord.Y - 1) &&
                board[(int)coord.X + (int)MyDirection, (int)coord.Y - 1]!=null&&
                board[(int)coord.X + (int)MyDirection, (int)coord.Y - 1].Team == (int)SYM.Opponent)
            {
                if (isCoordinateOnBoard((int)coord.X + (int)MyDirection * 2, (int)coord.Y - 2) &&
                    board[(int)coord.X + (int)MyDirection * 2, (int)coord.Y - 2] == null)
                    leftSide = new Vector((int)coord.X + (int)MyDirection * 2, (int)coord.Y - 2);
            }
            return (leftSide, rightSide);
        }

        private bool isCoordinateOnBoard(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < BoardSize && y < BoardSize);
        }

        private void movePiece(Vector coordFrom,List<Vector> moves) {
            Vector lastCoord = coordFrom;
            foreach (Vector move in moves) {

                if (Math.Abs(move.X - lastCoord.X) == 2)
                {
                    int coordXToEat = (int)(move.X + lastCoord.X) / 2;
                    int coordYToEat = (int)(move.Y + lastCoord.Y) / 2;

                    board[coordXToEat, coordYToEat] = null;

                }

                board[(int)move.X, (int)move.Y] = board[(int)lastCoord.X, (int)lastCoord.Y];
                lastCoord = move;
                makeKing((int)lastCoord.X, (int)lastCoord.Y);
            }
        }

        private void makeKing(int x,int y)
        {
            if ((MyDirection == Direction.Down && x == BoardSize - 1)||
                (MyDirection == Direction.Up && x == 0)) {
                board[x, y].IsKing = true;
            }
        }

        // Main Method 
        static public void Main(String[] args)
        {
            GameChecker s = new GameChecker(10, true);
/*            s.board[4, 1] = s.board[7, 2];
            s.board[4, 3] = s.board[7, 6];

            s.board[7, 2] = null;
            s.board[7, 6] = null;
            s.board[9, 4] = null;
            s.board[9, 8] = null;
            s.board[9, 0] = null;*/
            Console.WriteLine(s.PossibleMoves(new Vector(3, 2)));
            
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public enum Team { Me, Opponent }
    public enum Direction { Up = -1, Down = 1 }
    //public enum Result { Win, Lost, Tie, Continue }

    public class Board
    {
        private static readonly int MAX_MOVES_WITHOUT_EATING = 15;

        private Piece[,] board;

        public List<Piece> MyTeamPieces { get; set; }
        public List<Piece> OpponentTeamPieces { get; set; }

        public bool EatMode { get; set; }
        private int withoutEatingCounter = 0;
        private bool onlyKingsLeft = false;
        private Direction ownBoardDirection;
        public int BoardSize { get; }

        /*         //this builder for testing.build the board only
         *        private Board(int boardSize, Direction ownBoardDirection,bool testing)
                {
                    BoardSize = boardSize;
                    this.ownBoardDirection = ownBoardDirection;
                    board = new Piece[BoardSize, BoardSize];

                    for (int i = 0; i < BoardSize; i++)
                    {
                        for (int j = 0; j < BoardSize; j++)
                            board[i, j] = null;
                    }
                    MyTeamPieces = new List<Piece>();
                    OpponentTeamPieces = new List<Piece>();
                }

                //add checker to the game
                private void addChecker(Team team, int x, int y) {
                    Direction negetiveDir = ownBoardDirection == Direction.Down ? Direction.Up : Direction.Down;
                    Piece p = new Checker(team,team==Team.Me?ownBoardDirection: negetiveDir,new Point(x,y));

                    board[x, y] = p;
                    if (team == Team.Me)
                        MyTeamPieces.Add(p);
                    else
                        OpponentTeamPieces.Add(p);
                }

                //add king to the game
                public void addKing(Team team, int x, int y)
                {
                    Direction negetiveDir = ownBoardDirection == Direction.Down ? Direction.Up : Direction.Down;
                    Piece p = new Checker(team, team == Team.Me ? ownBoardDirection : negetiveDir, new Point(x, y));
                    Piece k = new King((Checker)p);
                    board[x, y] = k;
                    if (team == Team.Me)
                        MyTeamPieces.Add(k);
                    else
                        OpponentTeamPieces.Add(k);
                }*/

        public Board(Board boardToCopy)
        {
            EatMode = boardToCopy.EatMode;
            ownBoardDirection = boardToCopy.ownBoardDirection;
            BoardSize = boardToCopy.BoardSize;
            withoutEatingCounter = boardToCopy.withoutEatingCounter;
            onlyKingsLeft = boardToCopy.onlyKingsLeft;

            board = new Piece[BoardSize, BoardSize];

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                    board[i, j] = null;
            }

            MyTeamPieces = new List<Piece>();
            OpponentTeamPieces = new List<Piece>();

            foreach (Piece p in boardToCopy.MyTeamPieces)
            {
                if (p.IsKing)
                    MyTeamPieces.Add(new King((King)p));
                else
                    MyTeamPieces.Add(new Checker((Checker)p));
            }

            foreach (Piece p in boardToCopy.OpponentTeamPieces)
            {
                if (p.IsKing)
                    OpponentTeamPieces.Add(new King((King)p));
                else
                    OpponentTeamPieces.Add(new Checker((Checker)p));
            }

            foreach (Piece p in MyTeamPieces)
                board[(int)p.Coordinate.X, (int)p.Coordinate.Y] = p;


            foreach (Piece p in OpponentTeamPieces)
                board[(int)p.Coordinate.X, (int)p.Coordinate.Y] = p;
        }

        public Board(int boardSize, Direction ownBoardDirection, bool eatMode)
        {
            EatMode = eatMode;
            BoardSize = boardSize;
            this.ownBoardDirection = ownBoardDirection;
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
            MyTeamPieces = new List<Piece>();
            OpponentTeamPieces = new List<Piece>();

            for (int i = 0; i < (BoardSize / 2) - 1; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Piece piece = new Checker(ownBoardDirection == Direction.Down ? Team.Me : Team.Opponent, ownBoardDirection, new Point(i, j));

                    board[i, j] = piece;
                    if (ownBoardDirection == Direction.Down)
                        MyTeamPieces.Add(piece);
                    else
                        OpponentTeamPieces.Add(piece);
                }
            }

            Direction negetiveDir = ownBoardDirection == Direction.Down ? Direction.Up : Direction.Down;
            for (int i = (BoardSize / 2) + 1; i < BoardSize; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Piece piece = new Checker(ownBoardDirection == Direction.Down ? Team.Opponent : Team.Me, negetiveDir, new Point(i, j));

                    board[i, j] = piece;
                    if (ownBoardDirection == Direction.Down)
                        OpponentTeamPieces.Add(piece);
                    else
                        MyTeamPieces.Add(piece);
                }
            }
        }


        public (CheckersServiceReference.Result, bool) MovePiece(Piece pieceToMove, Path path)
        {
            if (path.EatenPieces.Count != 0)
            {
                foreach (Point pToEat in path.EatenPieces)
                    RemovePieceFromBoard(this.GetPieceAt(pToEat));
            }
            else
            {
                if (EatMode)
                {
                    bool isAnyPathToEat = false;
                    //we need to check if there is a path he can eat. if true, the piece should be out the game
                    foreach (Piece piece in pieceToMove.Team == Team.Me ? MyTeamPieces : OpponentTeamPieces)
                    {
                        //check paths for current piece
                        if (piece == pieceToMove)
                        {
                            var paths = pieceToMove.GetPossibleMoves(this);
                            foreach (var p in paths)
                                if (path != p && p.EatenPieces.Count != 0) { isAnyPathToEat = true; break; }
                        }
                        else
                        {
                            //check paths for other pieces
                            var paths = piece.GetPossibleMoves(this);
                            foreach (var p in paths)
                                if (p.EatenPieces.Count != 0) { isAnyPathToEat = true; break; }

                        }

                    }
                    if (isAnyPathToEat)
                    {
                        RemovePieceFromBoard(pieceToMove);
                        return (CheckResultGame(), true);
                    }
                }
            }

            SetPieceAt(pieceToMove, path.GetLastPosition());

            //VerifyCrown(pieceToMove);
            if (IsKingsOnlyLeft() && path.EatenPieces.Count == 0)
            {
                withoutEatingCounter++;
            }
            else
            {
                withoutEatingCounter = 0;
            }

            return (CheckResultGame(), false);
        }

        private bool IsKingsOnlyLeft()
        {
            if (onlyKingsLeft) return true;
            else
            {
                foreach (Piece p in MyTeamPieces)
                {
                    if (p.IsKing == false) return false;
                }
                foreach (Piece p in OpponentTeamPieces)
                {
                    if (p.IsKing == false) return false;
                }
                onlyKingsLeft = true;
            }
            return onlyKingsLeft;
        }

        public void RemovePieceFromBoard(Piece pieceToRemove)
        {
            if (pieceToRemove.Team == Team.Me)
                MyTeamPieces.Remove(pieceToRemove);
            else
                OpponentTeamPieces.Remove(pieceToRemove);
            board[(int)pieceToRemove.Coordinate.X, (int)pieceToRemove.Coordinate.Y] = null;
        }

        //check scenario when cpu cant move
        public CheckersServiceReference.Result CheckResultGame()
        {
            if (GetOpponentPiecesCount() == 0) return CheckersServiceReference.Result.Win;
            if (GetMyPiecesCount() == 0) return CheckersServiceReference.Result.Lost;

            bool moreMovesLeft = false;
            foreach (Piece p in MyTeamPieces)
            {
                var possibleMoves = p.GetPossibleMoves(this);
                if (possibleMoves.Count != 0) { moreMovesLeft = true; break; }

            }
            if (moreMovesLeft == false) return CheckersServiceReference.Result.Lost;

            foreach (Piece p in OpponentTeamPieces)
            {
                var possibleMoves = p.GetPossibleMoves(this);
                if (possibleMoves.Count != 0) { moreMovesLeft = true; break; }

            }
            if (moreMovesLeft == false) return CheckersServiceReference.Result.Win;

            if (withoutEatingCounter == MAX_MOVES_WITHOUT_EATING) return CheckersServiceReference.Result.Tie;

            return CheckersServiceReference.Result.Continue;
        }

        private int GetMyPiecesCount()
        {
            return MyTeamPieces.Count;
        }

        private int GetOpponentPiecesCount()
        {
            return OpponentTeamPieces.Count;
        }

        public void VerifyCrown(Piece piece)
        {
            if (piece.IsKing) return;
            if (piece.Team == Team.Me)
            {
                if (ownBoardDirection == Direction.Down)
                {
                    if (piece.Coordinate.X == BoardSize - 1)
                    {
                        King newK = new King((Checker)piece);
                        MyTeamPieces.Remove(piece);
                        MyTeamPieces.Add(newK);
                        SetPieceAt(newK, piece.Coordinate);
                    }
                }
                else
                {
                    if (piece.Coordinate.X == 0)
                    {
                        King newK = new King((Checker)piece);
                        MyTeamPieces.Remove(piece);
                        MyTeamPieces.Add(newK);
                        SetPieceAt(newK, piece.Coordinate);
                    }
                }
            }
            else
            {
                Direction opponentDirection = ownBoardDirection == Direction.Down ? Direction.Up : Direction.Down;
                if (opponentDirection == Direction.Down)
                {
                    if (piece.Coordinate.X == BoardSize - 1)
                    {
                        King newK = new King((Checker)piece);
                        OpponentTeamPieces.Remove(piece);
                        OpponentTeamPieces.Add(newK);
                        SetPieceAt(newK, piece.Coordinate);
                    }
                }
                else
                {
                    if (piece.Coordinate.X == 0)
                    {
                        King newK = new King((Checker)piece);
                        OpponentTeamPieces.Remove(piece);
                        OpponentTeamPieces.Add(newK);
                        SetPieceAt(newK, piece.Coordinate);
                    }
                }
            }
        }

        private void SetPieceAt(Piece pieceToMove, Point to)
        {
            board[(int)pieceToMove.Coordinate.X, (int)pieceToMove.Coordinate.Y] = null;
            board[(int)to.X, (int)to.Y] = pieceToMove;

            pieceToMove.Coordinate = to;
        }

        public ref Piece GetPieceAt(Point position)
        {
            return ref board[(int)position.X, (int)position.Y];
        }

        public bool IsCoordinateOnBoard(Point pointToCheck)
        {
            return (pointToCheck.X >= 0 && pointToCheck.Y >= 0 && pointToCheck.X < BoardSize && pointToCheck.Y < BoardSize);
        }

        public bool IsEnemeyOnSquare(Point pointTeam, Point pointOpp)
        {
            return IsCoordinateOnBoard(pointOpp) &&
                   !IsSqaureEmpty(pointOpp) &&
                   GetPieceAt(pointOpp).Team != GetPieceAt(pointTeam).Team;
        }
        public bool IsFreeToLand(Point point)
        {
            return IsCoordinateOnBoard(point) && IsSqaureEmpty(point);
        }

        public bool IsSqaureEmpty(Point coord)
        {
            return board[(int)coord.X, (int)coord.Y] == null;
        }

        //this about a way to be more complex
        public double Evaluate()
        {
            return GetOpponentPiecesCount() - GetMyPiecesCount() + (GetOpponentKingsCount() * 0.5 - GetMyKingsCount() * 0.5);
        }

        private int GetOpponentKingsCount()
        {
            return OpponentTeamPieces.FindAll(x => x.IsKing).Count;
        }

        private int GetMyKingsCount()
        {
            return MyTeamPieces.FindAll(x => x.IsKing).Count;
        }

        static void Main(string[] args)
        {
            /*            Board newB = new Board(10, Direction.Down,true);

                        newB.addKing(Team.Me, 3, 6);
                        newB.addChecker(Team.Opponent, 4, 7);
                        newB.addChecker(Team.Opponent, 6, 7);
                        //newB.addChecker(Team.Opponent, 8, 7);
                        //newB.addKing(Team.Me, 9, 6);

                        Piece p = newB.GetPieceAt(new Point(3, 6));

                        p.CalculatePossibleMoves(newB);
                        Path last =p.OptionalPaths.Last();
                        foreach(Path pat in p.OptionalPaths)
                          Console.WriteLine(pat);
                        //Result res = newB.MovePiece(p, last);
                       // Console.WriteLine(res);
            */
        }
    }
}

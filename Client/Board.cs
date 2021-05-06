using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public enum Team { One, Two }
    //the dir of teamOne is always down
    public enum Direction { Up = -1, Down = 1 }
    //
    //off - you dont need to eat opponent
    //on - nust eat your opponent
    public enum EatMode { Off, On }
    public class Board
    {
        private static readonly int MAX_MOVES_WITHOUT_EATING = 15;
        private int withoutEatingCounter = 0;
        private bool onlyKingsLeft = false;

        private Piece[,] board;
        public int BoardSize { get; }
        public EatMode EatMode { get; set; }

        public List<Piece> TeamOnePieces { get; set; }
        public List<Piece> TeamTwoPieces { get; set; }

        public Board(Board boardToCopy)
        {
            EatMode = boardToCopy.EatMode;
            BoardSize = boardToCopy.BoardSize;
            withoutEatingCounter = boardToCopy.withoutEatingCounter;
            onlyKingsLeft = boardToCopy.onlyKingsLeft;

            board = new Piece[BoardSize, BoardSize];

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                    board[i, j] = null;
            }

            TeamOnePieces = new List<Piece>();
            TeamTwoPieces = new List<Piece>();

            foreach (Piece p in boardToCopy.TeamOnePieces)
            {
                if (p.IsKing)
                    TeamOnePieces.Add(new King((King)p));
                else
                    TeamOnePieces.Add(new Checker((Checker)p));
            }

            foreach (Piece p in boardToCopy.TeamTwoPieces)
            {
                if (p.IsKing)
                    TeamTwoPieces.Add(new King((King)p));
                else
                    TeamTwoPieces.Add(new Checker((Checker)p));
            }

            foreach (Piece p in TeamOnePieces)
                board[(int)p.Coordinate.X, (int)p.Coordinate.Y] = p;


            foreach (Piece p in TeamTwoPieces)
                board[(int)p.Coordinate.X, (int)p.Coordinate.Y] = p;
        }

        public Board(int boardSize, EatMode eatMode)
        {
            EatMode = eatMode;
            BoardSize = boardSize;
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
            TeamOnePieces = new List<Piece>();
            TeamTwoPieces = new List<Piece>();

            for (int i = 0; i < (BoardSize / 2) - 1; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Piece piece = new Checker(Team.One, Direction.Down, new Point(i, j));

                    board[i, j] = piece;
                    TeamOnePieces.Add(piece);
                }
            }

            for (int i = (BoardSize / 2) + 1; i < BoardSize; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Piece piece = new Checker(Team.Two, Direction.Up, new Point(i, j));

                    board[i, j] = piece;
                    TeamTwoPieces.Add(piece);
                }
            }
        }

        public int GetPathIndex(Piece pieceToMove, Path path)
        {
            var paths = pieceToMove.GetPossibleMoves(this);
            return paths.IndexOf(path);
        }

        public Path GetPathByIndex(Piece pieceToMove, int pathIndex)
        {
            var paths = pieceToMove.GetPossibleMoves(this);
            return paths[pathIndex];
        }
        public (Result, bool) MovePiece(Piece pieceToMove, int pathIndex)
        {
            var paths = pieceToMove.GetPossibleMoves(this);
            return MovePiece(pieceToMove, paths[pathIndex]);
        }
        public (Result, bool) MovePiece(Piece pieceToMove, Path path)
        {
            if (path.EatenPieces.Count != 0)
            {
                foreach (Point pToEat in path.EatenPieces)
                    RemovePieceFromBoard(this.GetPieceAt(pToEat));
            }
            else
            {
                if (EatMode == EatMode.On)
                {
                    bool isAnyPathToEat = false;
                    //we need to check if there is a path he can eat. if true, the piece should be out the game
                    foreach (Piece piece in pieceToMove.Team == Team.One ? TeamOnePieces : TeamTwoPieces)
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
                        return (CheckResultGame(pieceToMove.Team), true);
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

            return (CheckResultGame(pieceToMove.Team), false);
        }

        private bool IsKingsOnlyLeft()
        {
            if (onlyKingsLeft) return true;
            else
            {
                foreach (Piece p in TeamOnePieces)
                {
                    if (p.IsKing == false) return false;
                }
                foreach (Piece p in TeamTwoPieces)
                {
                    if (p.IsKing == false) return false;
                }
                onlyKingsLeft = true;
            }
            return onlyKingsLeft;
        }

        public void RemovePieceFromBoard(Piece pieceToRemove)
        {
            if (pieceToRemove.Team == Team.One)
                TeamOnePieces.Remove(pieceToRemove);
            else
                TeamTwoPieces.Remove(pieceToRemove);
            board[(int)pieceToRemove.Coordinate.X, (int)pieceToRemove.Coordinate.Y] = null;
        }

        //check scenario when cpu cant move
        //return result for the current Team
        public Result CheckResultGame(Team team)
        {
            if (team == Team.One)
            {
                if (GetPiecesCount(Team.Two) == 0) return Result.Win;
                if (GetPiecesCount(Team.One) == 0) return Result.Lost;
            }
            else
            {
                if (GetPiecesCount(Team.Two) == 0) return Result.Lost;
                if (GetPiecesCount(Team.One) == 0) return Result.Win;
            }

            bool moreMovesLeft = false;
            foreach (Piece p in TeamOnePieces)
            {
                var possibleMoves = p.GetPossibleMoves(this);
                if (possibleMoves.Count != 0) { moreMovesLeft = true; break; }

            }
            if (moreMovesLeft == false)
                if (team == Team.One)
                    return Result.Lost;
                else
                    return Result.Win;


            foreach (Piece p in TeamTwoPieces)
            {
                var possibleMoves = p.GetPossibleMoves(this);
                if (possibleMoves.Count != 0) { moreMovesLeft = true; break; }

            }
            if (moreMovesLeft == false)
                if (team == Team.One)
                    return Result.Win;
                else
                    return Result.Lost;

            if (withoutEatingCounter == MAX_MOVES_WITHOUT_EATING) return Result.Tie;

            return Result.Continue;
        }

        private int GetPiecesCount(Team team)
        {
            return (team == Team.One ? TeamOnePieces : TeamTwoPieces).Count;
        }

        public void VerifyCrown(Piece piece)
        {
            if (piece.IsKing) return;
            if (piece.Team == Team.One)
            {
                if (piece.Coordinate.X == BoardSize - 1)
                {
                    King newK = new King((Checker)piece);
                    TeamOnePieces.Remove(piece);
                    TeamOnePieces.Add(newK);
                    SetPieceAt(newK, piece.Coordinate);
                }
            }

            else
            {
                if (piece.Coordinate.X == 0)
                {
                    King newK = new King((Checker)piece);
                    TeamTwoPieces.Remove(piece);
                    TeamTwoPieces.Add(newK);
                    SetPieceAt(newK, piece.Coordinate);
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

        public ref Piece GetPieceAt(int x, int y)
        {
            return ref board[x, y];
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
        //vrsus cpu only. cpu always teamTwo
        public double Evaluate()
        {
            return GetPiecesCount(Team.Two) - GetPiecesCount(Team.One) + (GetKingsCount(Team.Two) * 0.5 - GetKingsCount(Team.One) * 0.5);
        }

        private int GetKingsCount(Team team)
        {
            return (team == Team.One ? TeamOnePieces : TeamTwoPieces).FindAll(x => x.IsKing).Count;
        }
    }
}

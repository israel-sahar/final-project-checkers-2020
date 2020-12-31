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
    public enum Result { Win,Lost,Tie,Continue }
    public class Board
    {
        private Piece[,] board;
        private Direction ownBoardDirection;
        public int BoardSize { get; }

        public List<Piece> MyTeamPieces { get; set; }
        public List<Piece> OpponentTeamPieces { get; set; }

        private int withoutEatingCounter=0;
        private bool onlyKingsLeft;
        private readonly int MAX_MOVES_WITHOUT_EATING = 15;

        public Board(int boardSize, Direction ownBoardDirection,bool v)
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
        public void addChecker(Team team, int x, int y) {
            Direction negetiveDir = ownBoardDirection == Direction.Down ? Direction.Up : Direction.Down;
            Piece p = new Checker(team,team==Team.Me?ownBoardDirection: negetiveDir,new Point(x,y));

            board[x, y] = p;
            if (team == Team.Me)
                MyTeamPieces.Add(p);
            else
                OpponentTeamPieces.Add(p);
        }
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
        }


        public Board(int boardSize, Direction ownBoardDirection)
        {
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

        public Board(Board boardToCopy) {
            this.ownBoardDirection = boardToCopy.ownBoardDirection;
            this.BoardSize = boardToCopy.BoardSize;
            MyTeamPieces = new List<Piece>();
            foreach (Piece p in boardToCopy.MyTeamPieces) { 
            if(p.IsKing)
                    MyTeamPieces.Add(new King((King)p));
            else
                    MyTeamPieces.Add(new Checker((Checker)p));
            }
                
            OpponentTeamPieces = new List<Piece>();
            foreach (Piece p in boardToCopy.OpponentTeamPieces)
            {
                if (p.IsKing)
                    OpponentTeamPieces.Add(new King((King)p));
                else
                    OpponentTeamPieces.Add(new Checker((Checker)p));
            }

            board = new Piece[BoardSize, BoardSize];

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                    board[i,j] = null;
            }

            foreach (Piece p in MyTeamPieces) {
                board[(int)p.Coordinate.X,(int) p.Coordinate.Y] = p;
            }

            foreach (Piece p in OpponentTeamPieces)
            {
                board[(int)p.Coordinate.X, (int)p.Coordinate.Y] = p;
            }
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
                    Piece pieces = new Checker(ownBoardDirection == Direction.Down ? Team.Me : Team.Opponent, ownBoardDirection, new Point(i, j));

                    board[i, j] = pieces;
                    if (ownBoardDirection == Direction.Down)
                        MyTeamPieces.Add(pieces);
                    else
                        OpponentTeamPieces.Add(pieces);
                }
            }

            Direction negetiveDir = ownBoardDirection == Direction.Down ? Direction.Up : Direction.Down;
            for (int i = (BoardSize / 2) + 1; i < BoardSize; i++)
            {
                int j = 1;
                if (i % 2 == 1) j = 0;
                for (; j < BoardSize; j += 2)
                {
                    Piece pieces = new Checker(ownBoardDirection == Direction.Down ? Team.Opponent : Team.Me, negetiveDir, new Point(i, j));

                    board[i, j] = pieces;
                    if (ownBoardDirection == Direction.Down)
                        OpponentTeamPieces.Add(pieces);
                    else
                        MyTeamPieces.Add(pieces);
                }
            }
        }


        //if need to eat and not.the piece go out from the game
        public Result MovePiece(Piece pieceToMove, Point To, List<Piece> piecesToEat)
        {
            foreach (Piece piece in piecesToEat)
                RemovePieceFromBoard(piece);
            SetPieceAt(pieceToMove, To);

            VerifyCrown(pieceToMove);
            if (isKingsOnlyLeft() && piecesToEat.Count == 0)
            {
                withoutEatingCounter++;
            }
            else
            {
                withoutEatingCounter = 0;
            }

            return CheckResultGame(pieceToMove.Team);
        }

        public void MovePieceWithoutResult(Piece pieceToMove, Point To, List<Piece> piecesToEat)
        {
            foreach (Piece piece in piecesToEat)
                RemovePieceFromBoard(piece);
            SetPieceAt(pieceToMove, To);

            VerifyCrown(pieceToMove);
            if (isKingsOnlyLeft() && piecesToEat.Count == 0)
            {
                withoutEatingCounter++;
            }
            else
            {
                withoutEatingCounter = 0;
            }
        }

        private bool isKingsOnlyLeft()
        {
            if (onlyKingsLeft) return true;
            else {
                foreach (Piece p in MyTeamPieces) {
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

        private Result CheckResultGame(Team teamTurn)
        {
            bool moreMovesLeft = false;
            foreach (Piece p in MyTeamPieces)
            {
                p.CalculatePossibleMoves(this);
                if (p.OptionalPaths.Count != 0) { moreMovesLeft = true;break; }

            }
            if (moreMovesLeft == false) return Result.Lost;
            foreach (Piece p in OpponentTeamPieces)
            {
                p.CalculatePossibleMoves(this);
                if (p.OptionalPaths.Count != 0) { moreMovesLeft = true; break; }

            }
            if (moreMovesLeft == false) return Result.Win;
            if (Team.Me == teamTurn) {
                if (GetOpponentPiecesCount() == 0) return Result.Win;
                if (GetMyPiecesCount() == 0) return Result.Lost;
            }
            else {
                if (GetOpponentPiecesCount() == 0) return Result.Lost;
                if (GetMyPiecesCount() == 0) return Result.Win;
            }
            if (withoutEatingCounter == MAX_MOVES_WITHOUT_EATING) return Result.Tie;
            
            return Result.Continue;
        }

        private int GetMyPiecesCount()
        {
            return MyTeamPieces.Count;
        }

        private int GetOpponentPiecesCount()
        {
            return OpponentTeamPieces.Count;
        }

        private void VerifyCrown(Piece piece)
        {
            if (piece.IsKing) return;
            if (piece.Team == Team.Me)
            {
                if (ownBoardDirection == Direction.Down)
                {
                    if (piece.Coordinate.Y == BoardSize - 1)
                    {
                        King newK = new King((Checker)piece);
                        MyTeamPieces.Remove(piece);
                        MyTeamPieces.Add(newK);
                        SetPieceAt(newK, piece.Coordinate);
                    }
                }
                else
                {
                    if (piece.Coordinate.Y == 0)
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
                    if (piece.Coordinate.Y == BoardSize - 1)
                    {
                        King newK = new King((Checker)piece);
                        OpponentTeamPieces.Remove(piece);
                        OpponentTeamPieces.Add(newK);
                        SetPieceAt(newK, piece.Coordinate);
                    }
                }
                else
                {
                    if (piece.Coordinate.Y == 0)
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

        public ref Piece GetPieceAt(Point position) {
            return ref board[(int)position.X, (int)position.Y];
        }

        public bool IsCoordinateOnBoard(Point pointToCheck)
        {
            return (pointToCheck.X >= 0 && pointToCheck.Y >= 0 && pointToCheck.X < BoardSize && pointToCheck.Y < BoardSize);
        }

        public bool IsSqaureEmpty(Point coord)
        {
            return board[(int)coord.X, (int)coord.Y] == null;
        }

        static void Main(string[] args)
        {
            Board newB = new Board(10, Direction.Down,true);

            newB.addChecker(Team.Opponent, 4, 5);
            newB.addChecker(Team.Opponent, 6, 5);
            newB.addChecker(Team.Opponent, 8, 5);
            newB.addChecker(Team.Opponent, 2, 3);
            newB.addKing(Team.Me, 9, 6);

            Piece p = newB.GetPieceAt(new Point(9, 6));
            
            p.CalculatePossibleMoves(newB);
            //the position of the piece is changinf after CalculatePossibleMoves Method
            Path last =p.OptionalPaths.Last();
            foreach(Path pat in p.OptionalPaths)
              Console.WriteLine(pat);
            Result res = newB.MovePiece(p, last.getLastPosition(), last.EatenPieces);
            Console.WriteLine(res);

        }
    }
}

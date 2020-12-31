using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class King : Piece
    {
        public King(Checker piece) {
            OptionalPaths = new List<Path>();
            this.IsKing = true;
            this.Team = piece.Team;
            this.Coordinate = new Point(piece.Coordinate.X, piece.Coordinate.Y);
        }

        public King(King piece)
        {
            OptionalPaths = new List<Path>();
            foreach (Path path in piece.OptionalPaths)
            {
                OptionalPaths.Add(new Path(path));
            }
            this.IsKing = true;
            this.Team = piece.Team;
            this.Coordinate = new Point(piece.Coordinate.X, piece.Coordinate.Y);
        }

        public override bool Equals(object obj)
        {
            King other = obj as King;
            if (other == null) return false;

            return this.Coordinate== other.Coordinate && this.Team==other.Team &&this.IsKing==other.IsKing;
        }

        public override string ToString()
        {
            return base.ToString()+",is King";
        }

        public override void CalculatePossibleMoves(Board boardGame)
        {
            OptionalPaths = new List<Path>();
            var canEat = PiecesToEat(Coordinate, boardGame);

            foreach (Piece p in canEat.Keys)
            {
                Point lastPoint = new Point(canEat[p].X + p.Coordinate.X, canEat[p].Y + p.Coordinate.Y);
                while (boardGame.IsCoordinateOnBoard(lastPoint) && boardGame.IsSqaureEmpty(lastPoint))
                {
                    Path option = new Path();
                    option.AddRecord(lastPoint,p);
                    OptionalPaths.Add(option);
                    lastPoint.Y += canEat[p].Y;
                    lastPoint.X += canEat[p].X;
                }
            }
            for (int j = 0; j < OptionalPaths.Count; j++)
            {
                Board newBoard = new Board(boardGame);
                    var pCopy = newBoard.GetPieceAt(this.Coordinate);
                    newBoard.MovePieceWithoutResult(pCopy, OptionalPaths[j].getLastPosition(), OptionalPaths[j].EatenPieces);
                    canEat = PiecesToEat(OptionalPaths[j].getLastPosition(), newBoard);


                    foreach (Piece p in canEat.Keys)
                    {
                        Point lastPoint = new Point(p.Coordinate.X + canEat[p].X, p.Coordinate.Y + canEat[p].Y);
                        while (newBoard.IsCoordinateOnBoard(lastPoint) && newBoard.IsSqaureEmpty(lastPoint))
                        {
                            Path newPath = new Path(OptionalPaths[j]);
                            newPath.AddRecord(lastPoint,p);
                            OptionalPaths.Add(newPath);
                            lastPoint.Y += canEat[p].Y;
                            lastPoint.X += canEat[p].X;
                        }
                    }
                }
            if (OptionalPaths.Count == 0)
            {
                for (int row = -1; Math.Abs(row) == 1; row += 2)
                {
                    for (int column = -1; Math.Abs(column) == 1; column += 2)
                    {
                        Point pointTo = new Point((int)Coordinate.X + row, (int)Coordinate.Y + column);
                        while (boardGame.IsCoordinateOnBoard(pointTo) &&
                               boardGame.IsSqaureEmpty(pointTo)) {
                            Path option = new Path();
                            option.AddRecord(pointTo);
                            OptionalPaths.Add(option);
                            pointTo.Y += column;
                            pointTo.X += row;
                        }
                    }
                }
            }
        }

        public Dictionary<Piece, Point> PiecesToEat(Point pieceCoordinate, Board boardGame)
        {
            Dictionary<Piece, Point> piecesToEat = new Dictionary<Piece, Point>();

            for (int row = -1; Math.Abs(row) == 1; row += 2)
            {
                for (int column = -1; Math.Abs(column) == 1; column += 2)
                {
                    Point pointToCheck = new Point((int)pieceCoordinate.X + row, (int)pieceCoordinate.Y + column);
                    if (boardGame.IsCoordinateOnBoard(pointToCheck) &&
                       !boardGame.IsSqaureEmpty(pointToCheck) &&
                        boardGame.GetPieceAt(pointToCheck).Team == Team.Opponent)
                    {
                        Point pointToMove = new Point((int)pieceCoordinate.X + row * 2, (int)pieceCoordinate.Y + column * 2);

                        if (boardGame.IsCoordinateOnBoard(pointToMove) && boardGame.IsSqaureEmpty(pointToMove))
                        {
                            piecesToEat.Add(boardGame.GetPieceAt(pointToCheck), new Point(row, column));
                        }
                    }
                }
            }
            return piecesToEat;
        }

        private bool IsDonePossibleMoves(Board boardGame)
        {
            foreach (Path path in OptionalPaths)
            {
                Board newBoard = new Board(boardGame);
                var p=newBoard.GetPieceAt(this.Coordinate);
                newBoard.MovePiece(p, path.getLastPosition(), path.EatenPieces);

                if (PiecesToEat(path.getLastPosition(), newBoard).Count != 0) return false;
            }
            return true;
        }
    }
}

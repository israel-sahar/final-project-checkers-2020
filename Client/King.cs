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

        public override int GetHashCode()
        {
            return (IsKing?1:0) + Coordinate.GetHashCode() + Team.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString()+",is King";
        }

        public override void CalculatePossibleMoves(Board boardGame)
        {
            OptionalPaths = new List<Path>();
            var FirstcanEat = PiecesToEat(Coordinate, boardGame);

            foreach (Piece p in FirstcanEat.Keys)
            {
                Point lastPoint = new Point(FirstcanEat[p].X + p.Coordinate.X, FirstcanEat[p].Y + p.Coordinate.Y);
                while (boardGame.IsCoordinateOnBoard(lastPoint) && boardGame.IsSqaureEmpty(lastPoint))
                {
                    Path option = new Path();
                    option.AddRecord(lastPoint,p);
                    OptionalPaths.Add(option);
                    lastPoint.Y += FirstcanEat[p].Y;
                    lastPoint.X += FirstcanEat[p].X;
                }
            }
            for (int j = 0; j < OptionalPaths.Count; j++)
            {
                Board newBoard = new Board(boardGame);
                    var pCopy = newBoard.GetPieceAt(this.Coordinate);
                    newBoard.MovePieceWithoutResult(pCopy, OptionalPaths[j].getLastPosition(), OptionalPaths[j].EatenPieces);
                    var NextcanEat = PiecesToEat(OptionalPaths[j].getLastPosition(), newBoard);


                    foreach (Piece p in NextcanEat.Keys)
                    {
                        Point lastPoint = new Point(p.Coordinate.X + NextcanEat[p].X, p.Coordinate.Y + NextcanEat[p].Y);
                        while (newBoard.IsCoordinateOnBoard(lastPoint) && newBoard.IsSqaureEmpty(lastPoint))
                        {
                            Path newPath = new Path(OptionalPaths[j]);
                            newPath.AddRecord(lastPoint,p);
                            OptionalPaths.Add(newPath);
                            lastPoint.Y += NextcanEat[p].Y;
                            lastPoint.X += NextcanEat[p].X;
                        }
                    }
                }
            if (FirstcanEat.Count<4)
            {
                for (int deltaRow = -1; Math.Abs(deltaRow) == 1; deltaRow += 2)
                {
                    for (int deltaColumn = -1; Math.Abs(deltaColumn) == 1; deltaColumn += 2)
                    {
                        Point pointTo = new Point((int)Coordinate.X + deltaRow, (int)Coordinate.Y + deltaColumn);
                        while (boardGame.IsCoordinateOnBoard(pointTo) &&
                               boardGame.IsSqaureEmpty(pointTo)) {
                            Path option = new Path();
                            option.AddRecord(pointTo);
                            OptionalPaths.Add(option);
                            pointTo.Y += deltaColumn;
                            pointTo.X += deltaRow;
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
                        boardGame.GetPieceAt(pointToCheck).Team != boardGame.GetPieceAt(pieceCoordinate).Team)
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
    }
}

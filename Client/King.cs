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
            //OptionalPaths = new List<Path>();
            this.IsKing = true;
            this.Team = piece.Team;
            this.Coordinate = new Point(piece.Coordinate.X, piece.Coordinate.Y);
        }

        public King(King piece)
        {
            //OptionalPaths = new List<Path>();
            //foreach (Path path in piece.OptionalPaths)
            //{
             //   OptionalPaths.Add(new Path(path));
            //}
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

        public override List<Path> GetPossibleMoves(Board boardGame)
        {
            List<Path> optionalPaths = new List<Path>();
            var firstCanEat = IsAnyPiecesToEat(boardGame);

            foreach (Piece p in firstCanEat.Keys)
            {
                Point moveToPoint = new Point(firstCanEat[p].X + p.Coordinate.X, firstCanEat[p].Y + p.Coordinate.Y);
                while (boardGame.IsFreeToLand(moveToPoint))
                {
                    Path option = new Path();
                    option.AddRecord(moveToPoint,p);
                    optionalPaths.Add(option);
                    moveToPoint.Y += firstCanEat[p].Y;
                    moveToPoint.X += firstCanEat[p].X;
                }
            }
            for (int j = 0; j < optionalPaths.Count; j++)
            {
                Board tempBoard = new Board(boardGame);
                    var pCopy = tempBoard.GetPieceAt(this.Coordinate);
                    tempBoard.MovePiece(pCopy, optionalPaths[j]);
                    var nextCanEat = pCopy.IsAnyPiecesToEat(tempBoard);

                    foreach (Piece p in nextCanEat.Keys)
                    {
                        Point lastPoint = new Point(p.Coordinate.X + nextCanEat[p].X, p.Coordinate.Y + nextCanEat[p].Y);
                        while (tempBoard.IsFreeToLand(lastPoint))
                        {
                            Path newPath = new Path(optionalPaths[j]);
                            newPath.AddRecord(lastPoint,p);
                            optionalPaths.Add(newPath);
                            lastPoint.Y += nextCanEat[p].Y;
                            lastPoint.X += nextCanEat[p].X;
                        }
                    }
                }
            if (firstCanEat.Count<4)
            {
                for (int deltaRow = -1; Math.Abs(deltaRow) == 1; deltaRow += 2)
                {
                    for (int deltaColumn = -1; Math.Abs(deltaColumn) == 1; deltaColumn += 2)
                    {
                        Point pointTo = new Point((int)Coordinate.X + deltaRow, (int)Coordinate.Y + deltaColumn);
                        while (boardGame.IsFreeToLand(pointTo)) {
                            Path option = new Path();
                            option.AddRecord(pointTo);
                            optionalPaths.Add(option);
                            pointTo.Y += deltaColumn;
                            pointTo.X += deltaRow;
                        }
                    }
                }
            }
            return optionalPaths;
        }

        
         public override Dictionary<Piece, Point> IsAnyPiecesToEat(Board boardGame)
        {
            Dictionary<Piece, Point> piecesToEat = new Dictionary<Piece, Point>();

            for (int row = -1; Math.Abs(row) == 1; row += 2)
            {
                for (int column = -1; Math.Abs(column) == 1; column += 2)
                {
                    Point pointToCheck = new Point((int)Coordinate.X + row, (int)Coordinate.Y + column);
                    if (boardGame.IsEnemeyOnSquare(Coordinate, pointToCheck))
                    {
                        Point pointToMove = new Point((int)Coordinate.X + row * 2, (int)Coordinate.Y + column * 2);

                        if (boardGame.IsFreeToLand(pointToMove))
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

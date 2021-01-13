using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    
    public class Checker:Piece
    {
        public Direction Direction { get; set; }

        public Checker(Team team,Direction direction,Point coordinate) {
            //OptionalPaths = new List<Path>();
            this.IsKing = false;
            this.Team = team;
            this.Coordinate = coordinate;
            Direction = direction;
        }

        public Checker(Checker pToCopy)
        {
            //OptionalPaths = new List<Path>();
            //foreach (Path path in pToCopy.OptionalPaths)
           // {
            //    OptionalPaths.Add(new Path(path));
           // }
            this.IsKing = false;
            this.Team = pToCopy.Team;
            this.Coordinate = new Point(pToCopy.Coordinate.X,pToCopy.Coordinate.Y);
            Direction = pToCopy.Direction;
        }

        public override bool Equals(object obj)
        {
            Checker other = obj as Checker;
            if (other == null) return false;

            return this.Coordinate == other.Coordinate && this.Team == other.Team && this.IsKing == other.IsKing;
        }

        public override List<Path> GetPossibleMoves(Board boardGame)
        {
            List<Path>  optionalPaths = new List<Path>();
            Dictionary<Piece, Point> firstCanEat = IsAnyPiecesToEat(boardGame);

            foreach (Piece p in firstCanEat.Keys) {
                    Point pointToMove = new Point(firstCanEat[p].X + p.Coordinate.X, firstCanEat[p].Y + p.Coordinate.Y);
                    Path option = new Path();
                    option.AddRecord(pointToMove, p);
                optionalPaths.Add(option);  
            }

                for(int j=0;j< optionalPaths.Count;j++)
                {
                    Board tempBoard = new Board(boardGame);
                    Piece tempP= tempBoard.GetPieceAt(Coordinate);
                    tempBoard.MovePiece(tempP, optionalPaths[j]);
                Dictionary<Piece, Point>  canEat = tempP.IsAnyPiecesToEat(tempBoard);
                    foreach (Piece p in canEat.Keys) {
                        Point lastPoint = new Point(p.Coordinate.X + canEat[p].X, p.Coordinate.Y + canEat[p].Y);
                        Path newPath = new Path(optionalPaths[j]);
                        newPath.AddRecord(lastPoint,p);
                    optionalPaths.Add(newPath);
                    }
                }
            if (firstCanEat.Count <=1)
            {
                for (int column = -1; Math.Abs(column) == 1; column += 2)
                {
                    Point coordToMove = new Point((int)Coordinate.X + (int)Direction, (int)Coordinate.Y + column);
                    if (boardGame.IsFreeToLand(coordToMove))
                    {
                        Path option = new Path();
                        
                        option.AddRecord(coordToMove);
                        if (!optionalPaths.Contains(option))
                            optionalPaths.Add(option);
                    }
                }
            }
            return optionalPaths;
        }

        public override Dictionary<Piece, Point> IsAnyPiecesToEat(Board boardGame)
        {
            Dictionary<Piece, Point> piecesToEat = new Dictionary<Piece, Point>();
            
            for (int column = -1; Math.Abs(column) == 1; column += 2)
                {
                    Point pointToCheck = new Point((int)Coordinate.X + (int)this.Direction, (int)Coordinate.Y + column);
                    if (boardGame.IsEnemeyOnSquare(Coordinate,pointToCheck))
                    {
                        Point pointToMove = new Point((int)Coordinate.X + (int)this.Direction * 2, (int)Coordinate.Y + column * 2);
                        if (boardGame.IsFreeToLand(pointToMove))
                            piecesToEat.Add(boardGame.GetPieceAt(pointToCheck), new Point((int)this.Direction, column));
                    }
                }
            return piecesToEat;
        }

        public override int GetHashCode()
        {
            return (IsKing ? 1 : 0) + Coordinate.GetHashCode() + Team.GetHashCode();
        }
    }
}

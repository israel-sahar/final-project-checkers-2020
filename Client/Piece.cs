using System.Collections.Generic;
using System.Windows;

namespace Client
{
    public abstract class Piece
    {
        public bool IsKing { get; set; }
        public Point Coordinate { get; set; }
        public Team Team { get; set; }
        //public List<Path> OptionalPaths { get; set; }
        public virtual string ToString()
        {
            return $"Team:{Team},Coordinate:({Coordinate.X},{Coordinate.Y})";
        }
        public abstract List<Path> GetPossibleMoves(Board boardGame);
        public abstract Dictionary<Piece, Point> IsAnyPiecesToEat(Board boardGame);
        public override int GetHashCode()
        {
            return Team.GetHashCode() + Coordinate.GetHashCode();
        }
    }
}
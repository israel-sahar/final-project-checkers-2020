using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class Path
    {
        public List<Point> PathOfPiece { get; set; }
        public List<Piece> EatenPieces { get; set; }

        public Path()
        {
            PathOfPiece = new List<Point>();
            EatenPieces = new List<Piece>();
        }

        public Path(Path path)
        {
            PathOfPiece = new List<Point>();
            foreach (Point p in path.PathOfPiece)
                PathOfPiece.Add(new Point(p.X, p.Y));
            EatenPieces = new List<Piece>();
            foreach (Piece p in path.EatenPieces)
                if (p.IsKing)
                    EatenPieces.Add(new King((King)p));
                else
                    EatenPieces.Add(new Checker((Checker)p));

        }

        public void AddRecord(Point path, Piece eaten)
        {
            PathOfPiece.Add(path);
            EatenPieces.Add(eaten);
        }

        public void AddRecord(Point path)
        {
            PathOfPiece.Add(path);
        }

        public Point getLastPosition()
        {
            return PathOfPiece[PathOfPiece.Count - 1];
        }

        public bool isContainCoordinate(Point p)
        {
            return PathOfPiece.Contains(p);
        }
        public override string ToString()
        {
            string str;
            str = "The Path Piece do:\n";
            foreach (Point p in PathOfPiece)
                str += $"({p.X},{p.Y}), ";
            str += "\nThe Pieces It Eating:\n";
            foreach (Piece p in EatenPieces)
                str += $"({p.Coordinate.X},{p.Coordinate.Y}), ";
            str += "\n==========\n";
            return str;
        }

        internal bool isContainPath(Path path)
        {
            foreach (Point p in path.PathOfPiece)
                if (!PathOfPiece.Contains(p)) return false;
            return true;
        }
    }
}

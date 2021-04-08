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
        public List<Point> EatenPieces { get; set; }

        public Path()
        {
            PathOfPiece = new List<Point>();
            EatenPieces = new List<Point>();
        }

        public Path(List<Point> pathOfPiece, List<Point> eatenPieces)
        {
            PathOfPiece = new List<Point>(pathOfPiece);
            EatenPieces = new List<Point>(eatenPieces);
        }

        public Path(Path path)
        {
            PathOfPiece = new List<Point>();
            foreach (Point p in path.PathOfPiece)
                PathOfPiece.Add(new Point(p.X, p.Y));
            EatenPieces = new List<Point>();
            foreach (Point p in path.EatenPieces)
                    EatenPieces.Add(new Point(p.X, p.Y));
        }

        public void AddRecord(Point path, Piece eaten)
        {
            PathOfPiece.Add(path);
            EatenPieces.Add(new Point(eaten.Coordinate.X, eaten.Coordinate.Y));
        }

        public void AddRecord(Point path)
        {
            PathOfPiece.Add(path);
        }

        public Point GetLastPosition()
        {
            return PathOfPiece[PathOfPiece.Count - 1];
        }

        public Point GetNextPositin()
        {
            Point p= new Point(PathOfPiece[0].X, PathOfPiece[0].Y);
            PathOfPiece.RemoveAt(0);
            return p;
        }
        public bool IsContainCoordinate(Point p)
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
            foreach (Point p in EatenPieces)
                str += $"({p.X},{p.Y}), ";
            str += "\n==========\n";
            return str;
        }

        internal bool IsContainPath(Path path)
        {
            foreach (Point p in path.PathOfPiece)
                if (!PathOfPiece.Contains(p)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            Path p = obj as Path;
            if (p == null) return false;
            if (p.EatenPieces.Count != this.EatenPieces.Count || p.PathOfPiece.Count != this.PathOfPiece.Count) return false;
            foreach (Point point in p.PathOfPiece) {
                if (!this.PathOfPiece.Contains(point)) return false;
            }
            foreach (Point piece in p.EatenPieces)
            {
                if (!this.EatenPieces.Contains(piece)) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (Point p in PathOfPiece)
                hash += p.GetHashCode();
            foreach (Point p in EatenPieces)
                hash += p.GetHashCode();
            return hash;
        }

    }
}

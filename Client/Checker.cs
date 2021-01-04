﻿using System;
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
            OptionalPaths = new List<Path>();
            this.IsKing = false;
            this.Team = team;
            this.Coordinate = coordinate;
            Direction = direction;
        }

        public Checker(Checker pToCopy)
        {
            OptionalPaths = new List<Path>();
            foreach (Path path in pToCopy.OptionalPaths)
            {
                OptionalPaths.Add(new Path(path));
            }
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

        public override void CalculatePossibleMoves(Board boardGame)
        {
            OptionalPaths = new List<Path>();
            Dictionary<Piece, Point> firstCanEat = PiecesToEat(this.Coordinate, boardGame);

            foreach (Piece p in firstCanEat.Keys) {
                    Point lastPoint = new Point(firstCanEat[p].X + p.Coordinate.X, firstCanEat[p].Y + p.Coordinate.Y);
                    Path option = new Path();
                    option.AddRecord(lastPoint, p);
                    OptionalPaths.Add(option);  
            }

                for(int j=0;j< OptionalPaths.Count;j++)
                {
                    Board tempBoard = new Board(boardGame);
                    Piece tempP= tempBoard.GetPieceAt(this.Coordinate);
                    tempBoard.MovePieceWithoutResult(tempP, OptionalPaths[j].getLastPosition(), OptionalPaths[j].EatenPieces);
                Dictionary<Piece, Point>  NextcanEat = PiecesToEat(OptionalPaths[j].getLastPosition(), tempBoard);
                    foreach (Piece p in NextcanEat.Keys) {
                        Point lastPoint = new Point(p.Coordinate.X + NextcanEat[p].X, p.Coordinate.Y + NextcanEat[p].Y);
                        Path newPath = new Path(OptionalPaths[j]);
                        newPath.AddRecord(lastPoint,p);
                        OptionalPaths.Add(newPath);
                    }
                }
            if (firstCanEat.Count <=1)
            {
                for (int column = -1; Math.Abs(column) == 1; column += 2)
                {
                    Point coordToMove = new Point((int)Coordinate.X + (int)Direction, (int)Coordinate.Y + column);
                    if (boardGame.IsCoordinateOnBoard(coordToMove) &&
                        boardGame.IsSqaureEmpty(coordToMove))
                    {
                        Path option = new Path();
                        
                        option.AddRecord(coordToMove);
                        if (!OptionalPaths.Contains(option))
                            OptionalPaths.Add(option);
                    }
                }
            }
        }

        public Dictionary<Piece,Point> PiecesToEat(Point pieceCoordinate,Board boardGame)
        {
            Dictionary<Piece, Point> piecesToEat = new Dictionary<Piece, Point>();
            
            for (int column = -1; Math.Abs(column) == 1; column += 2)
                {
                    Point pointToCheck = new Point((int)pieceCoordinate.X + (int)this.Direction, (int)pieceCoordinate.Y + column);
                    if (boardGame.IsCoordinateOnBoard(pointToCheck) &&
                       !boardGame.IsSqaureEmpty(pointToCheck) &&
                        boardGame.GetPieceAt(pointToCheck).Team != boardGame.GetPieceAt(pieceCoordinate).Team)
                    {
                        Point pointToMove = new Point((int)pieceCoordinate.X + (int)this.Direction * 2, (int)pieceCoordinate.Y + column * 2);
                        if (boardGame.IsCoordinateOnBoard(pointToMove) && boardGame.IsSqaureEmpty(pointToMove))
                        {
                            piecesToEat.Add(boardGame.GetPieceAt(pointToCheck), new Point((int)this.Direction, column));
                        }
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

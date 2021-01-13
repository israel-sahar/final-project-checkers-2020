using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public enum Level { Easy, Hard, Human }
    class ComputerMove
    {
        private static readonly int DEPTH_NUM = 4;
        private static Level cpuLevel;
        public ComputerMove(Level level)
        {
            cpuLevel = level;
        }

        public (Piece, Path) getNextMove(Board game)
        {
            switch (cpuLevel)
            {
                case Level.Easy:
                    return GetEasyNextMove(game);
                case Level.Hard:
                    return GetHardNextMove(game);
            }
            return (null, null);
        }

        private (Piece, Path) GetHardNextMove(Board game)
        {
            var nextMove = MiniMaxAlgorithm(game,null,null, DEPTH_NUM, Team.Opponent);
            return (nextMove.Item2, nextMove.Item3);
        }

        private (double,Piece, Path) MiniMaxAlgorithm(Board game,Piece currentPiece,Path currentPath, int depth, Team turn)
        {
            if (depth == 0 || game.CheckResultGame() != Result.Continue)
                return (game.Evaluate(), currentPiece, currentPath);

            if (turn == Team.Opponent)
            {
                Path bestPath=null;
                Piece bestPiece=null;
                double max = Double.NegativeInfinity;
                var all_pieces = game.OpponentTeamPieces;
                List<(Piece, Path)> allPaths = new List<(Piece, Path)>();
                foreach (var piece in all_pieces)
                {
                    var paths = piece.GetPossibleMoves(game);
                    foreach (var path in paths)
                        allPaths.Add((piece, path));
                }
                foreach (var pp in allPaths){
                    Board tempBoard = new Board(game);
                    tempBoard.MovePiece(tempBoard.GetPieceAt(pp.Item1.Coordinate), pp.Item2);

                    double eval = MiniMaxAlgorithm(tempBoard, pp.Item1, pp.Item2, depth - 1, Team.Me).Item1;
                    max = max > eval ? max : eval;
                    if (max == eval) {
                        bestPath = pp.Item2;
                        bestPiece = pp.Item1;
                    }

                }

                return (max, bestPiece,bestPath);
            }
            else
            {
                Path bestPath = null;
                Piece bestPiece = null;
                double min = Double.PositiveInfinity;
                var all_pieces = game.MyTeamPieces;
                List<(Piece, Path)> allPaths = new List<(Piece, Path)>();
                foreach (var piece in all_pieces)
                {
                    var paths = piece.GetPossibleMoves(game);
                    foreach (var path in paths)
                        allPaths.Add((piece, path));
                }
                foreach (var pp in allPaths)
                {
                    Board tempBoard = new Board(game);
                    tempBoard.MovePiece(tempBoard.GetPieceAt(pp.Item1.Coordinate), pp.Item2);

                    double eval = MiniMaxAlgorithm(tempBoard, pp.Item1, pp.Item2, depth - 1, Team.Opponent).Item1;
                    min = min < eval ? min : eval;
                    if (min == eval)
                    {
                        bestPath = pp.Item2;
                        bestPiece = pp.Item1;
                    }

                }

                return (min, bestPiece, bestPath);
            }
        }

        private (Piece, Path) GetEasyNextMove(Board game)
        {
            var computerPieces = game.OpponentTeamPieces;
            (Piece, Path) reqPiece = (null, null);
            List<(Piece, Path)> forRandomChoose = new List<(Piece, Path)>();
            foreach (Piece p in computerPieces)
            {
                var pm = p.GetPossibleMoves(game);
                foreach (Path path in pm)
                {
                    forRandomChoose.Add((p, path));
                    if (reqPiece.Item1 == null || path.EatenPieces.Count > reqPiece.Item2.EatenPieces.Count)
                    {
                        reqPiece.Item1 = p; reqPiece.Item2 = path;
                    }
                }
            }

            if (reqPiece.Item2.EatenPieces.Count == 0)
            {
                int rInt = (new Random(DateTime.Now.Millisecond)).Next(1, forRandomChoose.Count + 1);
                reqPiece.Item1 = forRandomChoose.ElementAt(rInt - 1).Item1;
                reqPiece.Item2 = forRandomChoose.ElementAt(rInt - 1).Item2;
            }
            return reqPiece;
        }
    }
}

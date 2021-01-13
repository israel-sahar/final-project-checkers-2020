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
            var nextMove = MiniMaxAlgorithm(game,null,null, DEPTH_NUM, Team.Opponent,Double.NegativeInfinity,Double.PositiveInfinity);
            return (nextMove.Item2, nextMove.Item3);
        }

        private (double,Piece, Path) MiniMaxAlgorithm(Board game,Piece currentPiece,Path currentPath, int depth, Team turn,double alpha, double beta)
        {
            if (depth == 0 || game.CheckResultGame() != Result.Continue)
                return (game.Evaluate(), currentPiece, currentPath);

            if (turn == Team.Opponent)
            {
                Path bestPath=null;
                Piece bestPiece=null;
                double value = Double.NegativeInfinity;
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

                    double eval = MiniMaxAlgorithm(tempBoard, pp.Item1, pp.Item2, depth - 1, Team.Me,alpha,beta).Item1;

                    if (eval > value)
                    {
                        value = eval;
                        bestPath = pp.Item2;
                        bestPiece = pp.Item1;
                    }
                    if (eval >= beta) return (value,bestPiece,bestPath);
                    if (eval > alpha) alpha = eval;

                }

                return (value, bestPiece,bestPath);
            }
            else
            {
                Path bestPath = null;
                Piece bestPiece = null;
                double value = Double.PositiveInfinity;
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

                    double eval = MiniMaxAlgorithm(tempBoard, pp.Item1, pp.Item2, depth - 1, Team.Opponent,alpha,beta).Item1;
                    value = value < eval ? value : eval;
                    if (eval < value)
                    {
                        value = eval;
                    }
                    if (eval <= alpha) return (value, bestPiece, bestPath);
                    if (eval < beta) beta = eval;

                }

                return (value, bestPiece, bestPath);
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

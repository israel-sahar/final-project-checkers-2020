using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientCallback : ICheckersServiceCallback
    {
        internal Action<int,string, bool> OpenNewGame;
        internal Action<List<System.Windows.Point>, List<System.Windows.Point>, Result> MakeOpponentMove;


        public void SendOpponentMove(List<System.Windows.Point> PathOfPiece, List<System.Windows.Point> EatenPieces, Result result)
        {
            MakeOpponentMove(PathOfPiece, EatenPieces, result);
        }

        public void StartGame(int GameId, string OpponentName, bool MyTurn)
        {
            OpenNewGame(GameId, OpponentName, MyTurn);
        }

        public void Test()
        {
            return;
        }
    }
}

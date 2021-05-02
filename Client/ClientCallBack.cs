using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class ClientCallback : ICheckersServiceCallback
    {
        internal Action<int,string, bool> OpenNewGame;
        internal Action<Point, int, Result> MakeOpponentMove;

        public void SendOpponentMove(Point correntPos, int indexPath, Result result)
        {
            MakeOpponentMove(correntPos, indexPath, result);
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

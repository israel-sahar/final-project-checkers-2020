using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientCallback : ICheckersServiceCallback
    {
        public void SendOpponentMove(List<System.Windows.Point> PathOfPiece, List<System.Windows.Point> EatenPieces, Result result)
        {

        }

        public void Test()
        {
            return;
        }
    }
}

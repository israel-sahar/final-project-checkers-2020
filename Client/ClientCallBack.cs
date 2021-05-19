using Client.CheckersServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class ClientCallback : ICheckersServiceCallback
    {
        internal Action<int,string, bool> OpenNewGame;
        internal Action<bool> CloseGame;
        internal Action<bool> SendMachineNetDetails;
        public string machineIP { get; set; }
        public int machinePort { get; set; }

        internal Action<Point, int, Result> MakeOpponentMove;

        public void CloseTheGame()
        {
            CloseGame(true);
        }

        public (string,int) GetNetworkDetails()
        {
            return (machineIP, machinePort);
        }

        public void GetIPAddress()
        {
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            string IPAddress = null;
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            machineIP = IPAddress;
        }

        public void FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            machinePort = port;
        }

        public void SendOpponentMove(Point correntPos, int indexPath, Result result)
        {
            MakeOpponentMove(correntPos, indexPath, result);
        }

        public void StartGame(int GameId, string OpponentName, bool MyTurn)
        {
            OpenNewGame(GameId, OpponentName, MyTurn);
        }

        public bool PingClient()
        {
            return true;
        }
    }
}

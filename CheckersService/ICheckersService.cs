using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows;

namespace CheckersService
{
    [ServiceContract(CallbackContract =typeof(ICheckersCallback))]
    public interface ICheckersService
    {
        [OperationContract]
        [FaultContract(typeof(UserAlreadyLoginFault))]
        [FaultContract(typeof(UserNotExistsFault))]
        [FaultContract(typeof(WrongPasswordFault))]
        void Connect(string usrName, string hashedPassword);
        [OperationContract]
        [FaultContract(typeof(UserNameAlreadyExistsFault))]
        void Register(string userName, string hashedPassword);
        [OperationContract]
        void Disconnect(string usrName, int numGame = -1);
        [OperationContract]
        void MakeMove(string UserName, int GameId, Point correntPos, int indexPath, Result result);
        [OperationContract]
        (int, string, bool) JoinGame(string user, bool isVsCPU, int boardSize,bool EatMode);
        [OperationContract]
        (string, int) StartWatchGame(int gameId);
        [OperationContract]
        void CloseUnFinishedGame(int GameId, string UserName,bool sendMsg);
        [OperationContract]
        ICollection<(int, DateTime, (int, int), int, string)> GetAllMoves(int gameId);
        [OperationContract]
        bool Ping();
        [OperationContract]
        void StopWaitingGame(string UserName, int boardSize = -1, bool eatMode = false);
        [OperationContract]
        ICollection<(int, string, string, Status, DateTime, bool, int)> GetGames(bool liveMode);
        [OperationContract]
        (Game, Move) GameMoveRegonizer();
    }

    public interface ICheckersCallback {
        [OperationContract(IsOneWay = true)]
        void SendOpponentMove(Point correntPos, int indexPath, Result result);
        [OperationContract(IsOneWay = true)]
        void StartGame(int GameId, string OpponentName, bool MyTurn);
        [OperationContract(IsOneWay = true)]
        void CloseTheGame();
        [OperationContract]
        (string, int) GetNetworkDetails();
        [OperationContract]
        bool PingClient();
    }
}

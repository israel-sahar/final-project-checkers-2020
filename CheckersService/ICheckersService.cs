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
        [FaultContract(typeof(UserAlreadyExistsFault))]
        [FaultContract(typeof(UserNameAlreadyExistsFault))]
        void Register(string email, string userName, string hashedPassword);
        [OperationContract]
       [FaultContract(typeof(GameIdNotExistsFault))]
        (int, Status, DateTime, bool, int, string, string) GetGame(int gameId);
        [OperationContract]
        void Disconnect(string usrName, Mode userMode, int numGame = -1);
        [OperationContract]
        void MakeMove(string UserName, int GameId, DateTime time, Point correntPos, int indexPath, Result result);
        [OperationContract]
        (int, string, bool) JoinGame(string user, bool isVsCPU, int boardSize,bool EatMode);
        [OperationContract]
        Game WatchGame(string usrName, int gameId);
        [OperationContract]
        void CloseUnFinishedGame(int GameId, string UserName);
        [OperationContract]
        void StopWatchGame(string usrName, int gameId);
        [OperationContract]
        ICollection<(int, DateTime, (int, int), int, string)> GetAllMoves(int gameId);
        [OperationContract]
        bool Ping();
        [OperationContract]
        [FaultContract(typeof(UserNotExistsFault))]
        void ResetPassword(string email);
        [OperationContract]
        bool IsUserNameTaken(string userName);
        [OperationContract]
        void StopWaitingGame(string UserName,int boardSize);
        [OperationContract]
        ICollection<(int, string, string, Status, DateTime)> GetPlayedGames(string usrName1=null, string usrName2 = null);
        [OperationContract]
        ICollection<(int, string, string, Status, DateTime)> GetPlayedGamesByDate(DateTime date);
    }

    public interface ICheckersCallback {
        [OperationContract(IsOneWay = true)]
        void SendOpponentMove(Point correntPos, int indexPath, Result result);
        [OperationContract(IsOneWay = true)]
        void StartGame(int GameId, string OpponentName, bool MyTurn);
    }
}

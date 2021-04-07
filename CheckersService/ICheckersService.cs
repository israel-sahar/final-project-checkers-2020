﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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
        Game GetGame(int gameId);
        [OperationContract]
        void Disconnect(string usrName, Mode userMode, int numGame = -1);
        [OperationContract]
        void MakeMove(string UserName, int GameId, DateTime time, List<System.Windows.Point> PathOfPiece, List<System.Windows.Point> EatenPieces, Result result);
        [OperationContract]
        (int, string) JoinGame(string user, bool isVsCPU, int boardSize);
        [OperationContract]
        Game WatchGame(string usrName, int gameId);
        [OperationContract]
        void CloseUnFinishedGame(int GameId, string UserName);
        [OperationContract]
        void StopWatchGame(string usrName, int gameId);
        [OperationContract]
        ICollection<Move> GetAllMoves(Game game);
        [OperationContract]
        bool Ping();
        [OperationContract]
        [FaultContract(typeof(UserNotExistsFault))]
        void ResetPassword(string email);
        [OperationContract]
        bool IsUserNameTaken(string userName);
    }

    public interface ICheckersCallback {
        [OperationContract(IsOneWay = true)]
        void SendOpponentMove(List<System.Windows.Point> PathOfPiece, List<System.Windows.Point> EatenPieces, Result result);
    }
}

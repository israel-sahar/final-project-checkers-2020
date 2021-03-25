using System;
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
        User Connect(string usrName, string hashedPassword);
        [OperationContract]
        [FaultContract(typeof(UserAlreadyExistsFault))]
        [FaultContract(typeof(UserNameAlreadyExistsFault))]
        void Register(string email, string userName, string hashedPassword);
        [OperationContract]
       [FaultContract(typeof(GameIdNotExists))]
        Game GetGame(int gameId);
        [OperationContract]
        void Disconnect(string usrName, Mode userMode, int numGame = -1);
        [OperationContract]
        void MakeMove(Move move);
        [OperationContract]
        void JoinGame(User user);
        [OperationContract]
        Game WatchGame(string usrName, int gameId);
        [OperationContract]
        void StopWatchGame(string usrName, int gameId);
        [OperationContract]
        ICollection<Move> GetAllMoves(Game game);
        [OperationContract]
        bool Ping();
        [OperationContract]
        [FaultContract(typeof(UserNotExistsFault))]
        void ResetPassword(string email);
    }

    public interface ICheckersCallback {
        [OperationContract(IsOneWay = true)]
        void Test();
    }
}

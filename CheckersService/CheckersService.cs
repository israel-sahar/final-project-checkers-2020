using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CheckersService
{
    [DataContract]
    public enum Status {
        [EnumMember]
        Unfinished,
        [EnumMember]
        OneWon,
        [EnumMember]
        TwoWon,
        [EnumMember]
        isTie
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CheckersService : ICheckersService
    {
        Dictionary<string, (ICheckersCallback,Mode)> onlineUsers  = new Dictionary<string, (ICheckersCallback,Mode)>();
        Dictionary<int, List<UserContact>>           waitingRooms = new Dictionary<int, List<UserContact>>();
        Dictionary<int, (UserContact, UserContact)>  runningGame  = new Dictionary<int, (UserContact, UserContact)>();

        //check every 1 minute the connection with the players
        DispatcherTimer timer;
        public CheckersService()
        {
            waitingRooms.Add(80, new List<UserContact>()); // size 8 without eating
            waitingRooms.Add(81, new List<UserContact>());// size 8 with eating
            waitingRooms.Add(100, new List<UserContact>());// size 10 without eating
            waitingRooms.Add(101, new List<UserContact>());// size 10 with eating

            IEnumerable<Game> games;
            using (var ctx = new CheckersDBEntities1())
            {
                games = (from game in ctx.Games
                         where game.Status == (int)Status.Unfinished
                         select game).ToArray();
            }

            foreach (Game game in games)
                DeleteGameFromDB(game.GameId);

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1, 0, 0);
            timer.Tick += PingAllPlayers;
            timer.Start();
        }

        private void PingAllPlayers(object sender, EventArgs e)
        {
            if (onlineUsers.Count() == 0) return;
            var k = onlineUsers.Keys.ToList();
            foreach (var client in k)
            {
                Thread t = new Thread(new ParameterizedThreadStart(CheckUser));
                t.Start(client);
            }
        }
        private void CheckUser(object name)
        {
            try
            {
                onlineUsers[(string)name].Item1.PingClient();
            }
            catch (Exception)
            {
                Disconnect((string)name);
            }
        }

        public void Connect(string usrName, string hashedPassword)
        {
            if (onlineUsers.ContainsKey(usrName)) {
                UserAlreadyLoginFault fault = new UserAlreadyLoginFault {
                    Message = $" The User {usrName} online",
                    usrName = usrName
                };
                throw new FaultException<UserAlreadyLoginFault>(fault, new FaultReason($" The User {usrName} online"));
            }
            User user;
            using (var ctx = new CheckersDBEntities1()) {
                user = (from u in ctx.Users where u.UserName == usrName select u).FirstOrDefault();
                if (user == null) {
                    UserNotExistsFault fault = new UserNotExistsFault {
                        Message = $"The user {usrName} is not Exists",
                        usrName = usrName
                    };
                    throw new FaultException<UserNotExistsFault>(fault, new FaultReason($"The user {usrName} is not Exists"));
                }
                else {
                    if (user.HashedPassword != hashedPassword) {
                        WrongPasswordFault fault = new WrongPasswordFault {
                            Message = "The password is incorrect",
                            usrName = usrName
                        };
                        throw new FaultException<WrongPasswordFault>(fault, new FaultReason("The password is incorrect"));
                    }

                }
            }
            AddLogin(usrName);
        }
        public void Register(string userName, string hashedPassword)
        {
            User user;
            using (var ctx = new CheckersDBEntities1())
            {
                user = (from u in ctx.Users where u.UserName == userName select u).FirstOrDefault();
                if (user != null)
                {
                    UserNameAlreadyExistsFault f = new UserNameAlreadyExistsFault
                    {
                        Message = "Username already in database",
                        UserName = userName
                    };
                    throw new FaultException<UserNameAlreadyExistsFault>(f, new FaultReason("Username already in database"));
                }
                user = new User
                {
                    HashedPassword = hashedPassword,
                    UserName = userName

                };
                ctx.Users.Add(user);
                ctx.SaveChanges();
            }
            AddLogin(userName);
        }
        
        private void AddLogin(string usrName){
            ICheckersCallback callback = OperationContext.Current.GetCallbackChannel<ICheckersCallback>();
            onlineUsers.Add(usrName, (callback,Mode.Lobby));
        }

        /// <summary>
        /// join to a new game
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isVsCPU">if play against computer,true.Otherwise,false.</param>
        /// <param name="boardSize"></param>
        /// <param name="EatMode"></param>
        /// <returns></returns>
        public (int, string, bool) JoinGame(string user, bool isVsCPU, int boardSize, bool EatMode)
        {
            using (var ctx = new CheckersDBEntities1())
            {
                if (isVsCPU)
                {
                    var Game = new Game
                    {
                        Date = DateTime.Now,
                        Status = (int)Status.Unfinished,
                        EatMode = EatMode,
                        BoardSize = boardSize
                    };
                    var usr = (from u in ctx.Users
                               where u.UserName == user
                               select u).FirstOrDefault();
                    Game.Users.Add(usr);
                    ctx.Games.Add(Game);
                    ctx.SaveChanges();
                    runningGame.Add(Game.GameId, (new UserContact(usr.UserName, onlineUsers[usr.UserName].Item1), null));
                    onlineUsers[usr.UserName] = (onlineUsers[usr.UserName].Item1,Mode.Playing);
                    return (Game.GameId, null, true);
                }
                else
                {
                    //playing against human
                    int key = boardSize * 10 + (EatMode ? 1 : 0);
                    if (waitingRooms[key].Count == 0)
                    {
                        waitingRooms[key].Add(new UserContact(user, onlineUsers[user].Item1));
                        onlineUsers[user] = (onlineUsers[user].Item1, Mode.Waiting);

                    }

                    else
                    {
                        var OpponentPlayer = waitingRooms[key].First();
                        waitingRooms[key].RemoveAt(0);

                        var Game = new Game
                        {
                            Date = DateTime.Now,
                            Status = (int)Status.Unfinished,
                            EatMode = EatMode,
                            BoardSize = boardSize
                        };
                        var usrOne = (from u in ctx.Users
                                      where u.UserName == user
                                      select u).FirstOrDefault();
                        var usrTwo = (from u in ctx.Users
                                      where u.UserName == OpponentPlayer.UserName
                                      select u).FirstOrDefault();
                        Game.Users.Add(usrOne);
                        Game.Users.Add(usrTwo);

                        onlineUsers[usrOne.UserName]= (onlineUsers[usrOne.UserName].Item1, Mode.Playing);
                        onlineUsers[usrTwo.UserName]= (onlineUsers[usrTwo.UserName].Item1, Mode.Playing);
                        ctx.Games.Add(Game);
                        ctx.SaveChanges();
                        OpponentPlayer.CheckersCallback.StartGame(Game.GameId, user, false);
                        runningGame.Add(Game.GameId, (new UserContact(usrOne.UserName, onlineUsers[usrOne.UserName].Item1),
                                                        OpponentPlayer));
                        return (Game.GameId, OpponentPlayer.UserName, true);
                    }

                }
                return (-1, null, false);
            }
        }

        /// <summary>
        /// get port and ip of the game hoster.the starter by default
        /// </summary>
        /// <param name="gameId">the id of the game you want to watch</param>
        /// <returns></returns>
        public (string,int) StartWatchGame(int gameId)
        {
            var d = runningGame[gameId].Item1.CheckersCallback.GetNetworkDetails();
            return (d.Item1,d.Item2);
        }

        /// <summary>
        /// add move to the database. update the opponent
        /// </summary>
        /// <param name="UserName">who make the move</param>
        /// <param name="GameId">id of the game</param>
        /// <param name="correntPos">where the start position</param>
        /// <param name="indexPath">index Path</param>
        /// <param name="result">the result of the move</param>
        public void MakeMove(string UserName, int GameId, Point correntPos, int indexPath, Result result)
        {
            using(var ctx = new CheckersDBEntities1())
            {
                Game game = (from u in ctx.Games where u.GameId == GameId select u).First();
                User OtherUser = null;

                User CurrentUser = UserName.Equals("Computer") ?null:game.Users.Where(x => x.UserName == UserName).First();
                if (game.Users.Count() == 2)
                    OtherUser = game.Users.Where(x => x.UserName != UserName).First();

                var move = new Move
                {
                    Game = game,
                    RecordTime = DateTime.Now,
                    User = CurrentUser,
                    pathIndex = indexPath,
                    posX = (int)correntPos.X,
                    posY = (int)correntPos.Y
                };
                game.Moves.Add(move);

                if (OtherUser != null)
                {
                    UserContact sentTo = runningGame[GameId].Item1.UserName == UserName ? runningGame[GameId].Item2 : runningGame[GameId].Item1;
                    sentTo.CheckersCallback.SendOpponentMove(correntPos, indexPath, result);
                }

                if (result != Result.Continue)
                {
                    switch (result)
                    {
                        case (Result.Lost):
                            if (OtherUser == null)
                                {
                                    if (UserName == "Computer")
                                        game.Status = (int)Status.OneWon;
                                    else
                                        game.Status = (int)Status.TwoWon;
                                }
                                else
                                game.Status = (int)(game.Users.ElementAt(0).UserName == UserName ? Status.TwoWon : Status.OneWon);
                            break;
                        case (Result.Tie):
                            game.Status = (int)Status.isTie;
                            break;
                        case (Result.Win):
                            if (OtherUser == null)
                            {
                                if (UserName == "Computer")
                                    game.Status = (int)Status.TwoWon;
                                else
                                    game.Status = (int)Status.OneWon;
                            }
                            else
                                game.Status = (int)(game.Users.ElementAt(0).UserName == UserName ? Status.OneWon : Status.TwoWon);
                            break;
                    }

                    if(OtherUser!=null) onlineUsers[OtherUser.UserName] = (onlineUsers[OtherUser.UserName].Item1,Mode.Lobby);
                    onlineUsers[UserName] = (onlineUsers[UserName].Item1, Mode.Lobby);

                    runningGame.Remove(GameId);
                }
                ctx.SaveChanges();

            }
        }

        /// <summary>
        /// ping to the server to check if the connection is fine
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            return true;
        }

        /// <summary>
        /// disconnect player from server
        /// </summary>
        /// <param name="usrName">userName of the disconnecter</param>
        /// <param name="numGame">if the player is playing.This field is required</param>
        public void Disconnect(string usrName, int numGame = -1)
        {
            if (onlineUsers.ContainsKey(usrName))
            {
                //need to check if is playing,watching, or in lobby
                if (onlineUsers[usrName].Item2 == Mode.Playing)
                {
                    if (numGame == -1)
                    {
                        numGame = (from g in runningGame
                                   where g.Value.Item1.UserName == usrName ||
             (g.Value.Item2 != null && g.Value.Item2.UserName == usrName)
                                   select g).First().Key;

                    }
                    CloseUnFinishedGame(numGame, usrName, true);
                }
                if (onlineUsers[usrName].Item2 == Mode.Waiting)
                {
                    StopWaitingGame(usrName);
                }

                onlineUsers.Remove(usrName);
            }
        }

        /// <summary>
        /// delete unfinished game and update the opponent if needed
        /// change the mode of two user
        /// </summary>
        /// <param name="GameId"> game id to close</param>
        /// <param name="UserName">the username who decided to close</param>
        /// <param name="sendMsg">true if need to send msg to opponent.Otherwise ,select not</param>
        public void CloseUnFinishedGame(int GameId, string UserName,bool sendMsg)
        {
            DeleteGameFromDB(GameId);
            if (sendMsg) { 
            if(runningGame[GameId].Item1.UserName!= UserName)
            {
                runningGame[GameId].Item1.CheckersCallback.CloseTheGame();
                    onlineUsers[runningGame[GameId].Item1.UserName] = (runningGame[GameId].Item1.CheckersCallback, Mode.Lobby);

                }
                if (runningGame[GameId].Item2!= null && runningGame[GameId].Item2.UserName != UserName)
            {
                runningGame[GameId].Item2.CheckersCallback.CloseTheGame();
                    onlineUsers[runningGame[GameId].Item2.UserName] = (runningGame[GameId].Item2.CheckersCallback, Mode.Lobby);

                }
            }
            runningGame.Remove(GameId);
            onlineUsers[UserName] = (onlineUsers[UserName].Item1, Mode.Lobby);
        }

        /// <summary>
        /// private method.delete game from database
        /// </summary>
        /// <param name="GameId">id of game you want to delete</param>
        private void DeleteGameFromDB(int GameId)
        {
            using (var ctx = new CheckersDBEntities1())
            {
                var movesToD = (from u in ctx.Moves
                                where u.GameId == GameId
                                select u).ToList();
                ctx.Moves.RemoveRange(movesToD);
                var g = (from u in ctx.Games
                         where u.GameId == GameId
                         select u).FirstOrDefault();
                ctx.Games.Remove(g);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// remove player from waiting mode to lobby mode
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="boardSize">board size</param>
        /// <param name="eatMode">true if the piece is burn if not eating.otherwise,false</param>
        public void StopWaitingGame(string UserName, int boardSize =-1,bool eatMode = false){
            UserContact playerToRemove = null;
            if (boardSize == -1)
            {
                foreach (var list in waitingRooms)
                {
                    var filterList = (from u in list.Value
                                      where u.UserName == UserName
                                      select u);
                    if (filterList.Count() != 0)
                    {
                        list.Value.Remove(playerToRemove);
                        playerToRemove = filterList.First();
                        break;
                    }
                }
            }
            else {
                int key = boardSize * 10 + (eatMode ? 1 : 0);

                playerToRemove = (from u in waitingRooms[key]
                                  where u.UserName == UserName
                                  select u).First();
                waitingRooms[key].Remove(playerToRemove);
            }
            onlineUsers[UserName] = (onlineUsers[UserName].Item1, Mode.Lobby);
        }

        /// <summary>
        /// return all moves of specific game in database.
        /// </summary>
        /// <param name="gameId"> the id of the wanted game</param>
        /// <returns>
        /// Collection of:
        /// 1 - int:moveId
        /// 2 - DateTime:when move occured
        /// 3 - (int,int):The start position of piece
        /// 4 - int:path Index
        /// 5 - string:usrName who make the move
        /// </returns>
        public ICollection<(int, DateTime, (int, int), int, string)> GetAllMoves(int gameId)
        {
            using (var ctx = new CheckersDBEntities1())
            {
                var game = (from u in ctx.Games
                            where u.GameId == gameId
                            select u).FirstOrDefault();
                ICollection<(int, DateTime, (int, int), int, string)> moves = new List<(int, DateTime, (int, int), int, string)>();
                foreach (var m in game.Moves)
                {
                    moves.Add((m.MoveId, m.RecordTime, (m.posX, m.posY), m.pathIndex, m.UserName));
                }
                return moves;
            }

        }

        /// <summary>
        /// return all games in database.
        /// </summary>
        /// <param name="liveMode"> true if online game needed.false if want all games that was played</param>
        /// <returns>
        /// Collection of:
        /// 1 - int:gameId
        /// 2 - string:usrName1
        /// 3 - string:usrName2
        /// 4 - Status:status of game(Unfinished,isTie,OneWon...)
        /// 5 - DateTime:time of starting game
        /// 6 - bool:eatMode state
        /// 7 - int:board size</returns>
        public ICollection<(int, string, string, Status, DateTime, bool, int)> GetGames(bool liveMode)
        {
            ICollection<(int, string, string, Status, DateTime, bool, int)> list = new List<(int, string, string, Status, DateTime, bool, int)>();
            using (var ctx = new CheckersDBEntities1())
            {
                List<Game> games=null;
                games = (from g in ctx.Games
                             where (liveMode && g.Status==(int)Status.Unfinished)|| (!liveMode && g.Status != (int)Status.Unfinished)
                         select g).ToList();

                foreach (var ou in games)
                {
                    list.Add((ou.GameId, ou.Users.ElementAt(0).UserName,
                        ou.Users.Count()==2? ou.Users.ElementAt(1).UserName: "Computer", (Status)ou.Status, ou.Date, ou.EatMode, ou.BoardSize));
                }
            }
            return list;
        }

        /// <summary>
        /// this is help to the client know the classes 'Game' and 'Move'
        /// </summary>
        /// <returns></returns>
        public (Game, Move) GameMoveRegonizer()
        {
            return (null,null);
        }
    }
}

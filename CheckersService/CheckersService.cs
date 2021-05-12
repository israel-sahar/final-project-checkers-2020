using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Windows;

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
    //lecture 13_3 -43:48 make a thread to connect with users
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CheckersService : ICheckersService
    {
        private readonly int PASS_LENGTH = 8;

        Dictionary<string, ICheckersCallback> onlineUsers = new Dictionary<string, ICheckersCallback>();
        Dictionary<int, List<UserContact>> waitingRooms = new Dictionary<int, List<UserContact>>();
        //the bool is for load-balance
        Dictionary<int, (UserContact, UserContact,bool)> runningGame = new Dictionary<int, (UserContact, UserContact, bool)>();
        //Dictionary<int, Dictionary<string, ICheckersCallback>> watchingGames = new Dictionary<int, Dictionary<string, ICheckersCallback>>(); 

        public CheckersService()
        {
            waitingRooms.Add(80, new List<UserContact>()); // size 8 without eating
            waitingRooms.Add(81, new List<UserContact>());// size 8 with eating
            waitingRooms.Add(100, new List<UserContact>());// size 10 without eating
            waitingRooms.Add(101, new List<UserContact>());// size 10 with eating
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
            onlineUsers.Add(usrName, callback);
        }

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
                    runningGame.Add(Game.GameId, (new UserContact(usr.UserName, onlineUsers[usr.UserName]), null,true));
                    return (Game.GameId, null, true);
                }
                else
                {
                    //playing against human
                    int key = boardSize * 10 + (EatMode ? 1 : 0);
                    if (waitingRooms[key].Count == 0)
                        waitingRooms[key].Add(new UserContact(user, onlineUsers[user]));

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

                        ctx.Games.Add(Game);
                        ctx.SaveChanges();
                        OpponentPlayer.CheckersCallback.StartGame(Game.GameId, user, false);
                        runningGame.Add(Game.GameId, (new UserContact(usrOne.UserName, onlineUsers[usrOne.UserName]),
                                                        OpponentPlayer,true));
                        return (Game.GameId, OpponentPlayer.UserName, true);
                    }

                }
                return (-1, null, false);
            }
        }

        public (int, Status, DateTime, bool, int, string, string) WatchGame(string usrName, int gameId)
        {
            //runningGame[gameId].Item1.CheckersCallback.AddViewer();
            return GetGame(gameId);
        }

        //(moveId,record,(posX,posY),pathI,usrName)
        public ICollection<(int, DateTime, (int, int), int, string)> GetAllMoves( int gameId)
        {
            using(var ctx = new CheckersDBEntities1())
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

        /*(gameId,Status,date,EatMode,boardSize,usr1,usr2,Moves)*/
        public (int, Status, DateTime, bool, int, string, string) GetGame(int gameId)
        {
            Game game = null;
            using (var ctx = new CheckersDBEntities1())
            {
                game = (from u in ctx.Games
                        where gameId == u.GameId
                        select u).FirstOrDefault();
                if (game == null)
                {
                    GameIdNotExistsFault fault = new GameIdNotExistsFault
                    {
                        Message = "gameId not in database",
                        gameId = gameId
                    };
                    throw new FaultException<GameIdNotExistsFault>(fault,new FaultReason("gameId not in database"));
                }


                return (game.GameId, (Status)game.Status, game.Date, game.EatMode, game.BoardSize,
    game.Users.ElementAt(0).UserName, game.Users.Count == 2 ? game.Users.ElementAt(1).UserName : "Computer");
            }
        }


        public void MakeMove(string UserName, int GameId, DateTime time, Point correntPos, int indexPath, Result result)
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
                    RecordTime = time,
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

                   runningGame.Remove(GameId);
                }
                ctx.SaveChanges();

            }
        }

        public bool Ping()
        {
            return true;
        }

        public void StopWatchGame(string usrName, int gameId)
        {
            //ICheckersCallback callback = watchingGames[gameId][usrName];
            //watchingGames[gameId].Remove(usrName);
            //onlineUsers.Add(usrName, callback);
        }

        public void Disconnect(string usrName, Mode userMode, int numGame = -1)
        {
            //need to check if is playing,watching, or in lobby
            User usr;
            using (var ctx = new CheckersDBEntities1())
            {
                usr = (from u in ctx.Users
                       where u.UserName == usrName
                       select u).FirstOrDefault();
            }
            if (userMode == Mode.Watching)
            {
               // watchingGames[numGame].Remove(usrName);
            }
            if (userMode == Mode.Playing)
            {
                CloseUnFinishedGame(numGame, usrName);
            }


            onlineUsers.Remove(usrName);
        }

        public void CloseUnFinishedGame(int GameId, string UserName)
        {
            DeleteGameFromDB(GameId);
            if(runningGame[GameId].Item1.UserName!= UserName)
            {
                runningGame[GameId].Item1.CheckersCallback.CloseTheGame();
            }
            if (runningGame[GameId].Item2!= null && runningGame[GameId].Item2.UserName != UserName)
            {
                runningGame[GameId].Item2.CheckersCallback.CloseTheGame();
            }
            runningGame.Remove(GameId);
        }

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
        public void StopWaitingGame(string UserName, int boardSize,int eatMode)
        {
            int key = boardSize * 10 + eatMode;

            var playerToRemove = (from u in waitingRooms[key]
                                  where u.UserName == UserName
                                  select u).First();
            waitingRooms[key].Remove(playerToRemove);
        }

        public ICollection<(int, string, string, Status, DateTime)> GetPlayedGames(string usrName1, string usrName2)
        {
            ICollection<(int, string, string, Status, DateTime)> list = new List<(int, string, string, Status, DateTime)>();
            using (var ctx = new CheckersDBEntities1())
            {
                List<Game> games=null;
                if (usrName1 == null&&usrName2==null)
                    games = (from g in ctx.Games
                             where g.Status != (int)Status.Unfinished
                             select g).ToList();
                if (usrName1 != null)
                    games = (from g in ctx.Games
                             select g).Where(g=>g.Users.Where(u=>u.UserName.Equals(usrName1)).Count()>0).ToList();
                if (usrName1 != null && usrName2 != null)
                    games = (from g in ctx.Games
                             select g).Where(g => g.Users.Where(u => u.UserName.Equals(usrName1)).Count()+ g.Users.Where(u => u.UserName.Equals(usrName2)).Count() ==2).ToList();

                foreach (var ou in games)
                {
                    list.Add((ou.GameId, ou.Users.ElementAt(0).UserName,
                        ou.Users.Count()==2? ou.Users.ElementAt(1).UserName: "Computer", (Status)ou.Status, ou.Date));
                }
            }
            return list;
        }

        public ICollection<(int, string, string, Status, DateTime)> GetPlayedGamesByDate(DateTime date)
        {
            ICollection<(int, string, string, Status, DateTime)> list = new List<(int, string, string, Status, DateTime)>();
            using (var ctx = new CheckersDBEntities1())
            {
                List<Game> games = (from g in ctx.Games
                                    where date.Year == g.Date.Year && date.DayOfYear == g.Date.DayOfYear
                                    select g).ToList();

                foreach (var ou in games)
                {
                    list.Add((ou.GameId, ou.Users.ElementAt(0).UserName,
                        ou.Users.Count() == 2 ? ou.Users.ElementAt(1).UserName : "Computer", (Status)ou.Status, ou.Date));
                }
            }
            return list;
        }

        public ICollection<(int, string, string, TimeSpan)> GetLiveGames()
        {
            ICollection<(int, string, string, TimeSpan)> list = new List<(int, string, string, TimeSpan)>();
            using (var ctx = new CheckersDBEntities1())
            {
                List<Game> games = (from g in ctx.Games
                                    where g.Status == (int)Status.Unfinished
                                    select g).ToList();

                foreach (var ou in games)
                {
                    list.Add((ou.GameId, ou.Users.ElementAt(0).UserName,
                        ou.Users.Count() == 2 ? ou.Users.ElementAt(1).UserName : "Computer", ou.Date.TimeOfDay));
                }
            }
            return list;
        }

        public (Game, Move) GameMoveRegonizer()
        {
            return (null,null);
        }
    }
}

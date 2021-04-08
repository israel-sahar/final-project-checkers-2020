using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace CheckersService
{
    public enum Status { Unfinished,OneWon, TwoWon,isTie,}
    //lecture 13_3 -43:48 make a thread to connect with users
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode =ConcurrencyMode.Reentrant)]
    public class CheckersService : ICheckersService
    {
        private readonly int PASS_LENGTH = 8;

        Dictionary<string, ICheckersCallback> onlineUsers = new Dictionary<string, ICheckersCallback>();
        Dictionary<int, List<UserContact>> waitingRoom = new Dictionary<int, List<UserContact>>();

        Dictionary<int, (UserContact, UserContact)> runningGame = new Dictionary<int, (UserContact, UserContact)>();
        //Dictionary<int, Dictionary<string, ICheckersCallback>> watchingGames = new Dictionary<int, Dictionary<string, ICheckersCallback>>(); 

        public CheckersService()
        {
            waitingRoom.Add(8, new List<UserContact>());
            waitingRoom.Add(10, new List<UserContact>());
        }
        public void Connect(string usrName, string hashedPassword)
        {
            if(onlineUsers.ContainsKey(usrName)){
                UserAlreadyLoginFault fault = new UserAlreadyLoginFault {
                    Message = $" The User {usrName} online",
                    usrName = usrName
                };
                throw new FaultException<UserAlreadyLoginFault>(fault);
            }
            User user;
            using (var ctx = new CheckersDBContext()){
                user = (from u in ctx.Users where u.UserName == usrName select u).FirstOrDefault();
                if (user == null){
                    UserNotExistsFault fault = new UserNotExistsFault{
                        Message = $"The user {usrName} is not Exists",
                        usrName = usrName
                    };
                    throw new FaultException<UserNotExistsFault>(fault);
                }
                else {
                    if (user.HashedPassword != hashedPassword) {
                        WrongPasswordFault fault = new WrongPasswordFault{
                            Message = $"The password is incorrect",
                            usrName = usrName
                        };
                        throw new FaultException<WrongPasswordFault>(fault);
                    }
                  
                }
            }
            ICheckersCallback callback = OperationContext.Current.GetCallbackChannel<ICheckersCallback>();
            onlineUsers.Add(usrName, callback);
        }

        public void Register(string email,string userName, string hashedPassword)
        {
            User user;
            using (var ctx = new CheckersDBContext()) {
                user = (from u in ctx.Users where u.Email == email select u).FirstOrDefault();
                if (user != null)
                {
                    UserAlreadyExistsFault f = new UserAlreadyExistsFault
                    {
                        Message = $"User already in database",
                        Email = email
                    };
                    throw new FaultException<UserAlreadyExistsFault>(f);
                }
                user = (from u in ctx.Users where u.UserName == userName select u).FirstOrDefault();
                if (user != null)
                {
                    UserNameAlreadyExistsFault f = new UserNameAlreadyExistsFault
                    {
                        Message = $"Username already in database",
                        Email = email
                    };
                    throw new FaultException<UserNameAlreadyExistsFault>(f);
                }
                user = new User
                {
                    Email = email,
                    HashedPassword = hashedPassword,
                    UserName= userName

                };
                ctx.Users.Add(user);
                ctx.SaveChanges();
            }
            Connect(userName, hashedPassword);
        }
        public bool IsUserNameTaken(string userName)
        {
            using (var ctx = new CheckersDBContext())
            {
                var user = (from u in ctx.Users where u.UserName==userName select u).FirstOrDefault();
                if (user != null)
                    return true;
                return false;
            }
        }

        public Game GetGame(int gameId)
        {
            using (var ctx = new CheckersDBContext()) {
                var game = (from u in ctx.Games
                 where gameId == u.GameId
                 select u).FirstOrDefault();
                if (game == null)
                {
                    GameIdNotExistsFault fault = new GameIdNotExistsFault
                    {
                        Message = "gameId not in database",
                        gameId = gameId
                    };
                    throw new FaultException<GameIdNotExistsFault>(fault);
                }
                return game;
            }
        }

        public (int,string, bool) JoinGame(string user,bool isVsCPU,int boardSize)
        {
            using (var ctx = new CheckersDBContext()) {

                if (isVsCPU) {
                    var Game = new Game
                    {
                        Date = DateTime.Now,
                        Status = (int)Status.Unfinished
                    };
                    var usr = (from u in ctx.Users
                               where u.UserName == user
                               select u).FirstOrDefault();
                    Game.Users.Add(usr);
                    ctx.Games.Add(Game);
                    ctx.SaveChanges();
                    runningGame.Add(Game.GameId, (new UserContact(usr.UserName, onlineUsers[usr.UserName]), null));
                    onlineUsers.Remove(usr.UserName);
                    return (Game.GameId,null,true);
                }
                else
                {
                    //playing against human
                    if (waitingRoom[boardSize].Count == 0) {
                        waitingRoom[boardSize].Add(new UserContact(user, onlineUsers[user]));
                        onlineUsers.Remove(user);
                    }
                    else
                    {
                        var OpponentPlayer = waitingRoom[boardSize].First();
                        waitingRoom[boardSize].RemoveAt(0);

                        var Game = new Game
                        {
                            Date = DateTime.Now,
                            Status = (int)Status.Unfinished
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
                                                        OpponentPlayer));
                        onlineUsers.Remove(user);


                        return (Game.GameId, OpponentPlayer.UserName, true);
                    }
                    
                }
                return (-1,null,false);
            }
        }

        public void MakeMove(string UserName,int GameId,DateTime time, System.Windows.Point correntCord, List<System.Windows.Point> PathOfPiece, List<System.Windows.Point> EatenPieces,Result result)
        {
            using(var ctx = new CheckersDBContext())
            {
                Game game = (from u in ctx.Games where u.GameId == GameId select u).First();
                User OtherUser = null;

                User CurrentUser = UserName.Equals("PC")?null:game.Users.Where(x => x.UserName == UserName).First();
                if (game.Users.Count() == 2)
                    OtherUser = game.Users.Where(x => x.UserName != UserName).First();

                PathOfPiece.Insert(0, correntCord);
                var move = new Move
                {
                    EatenPieces = Convert(EatenPieces),
                    Game = game,
                    PointsToMove = Convert(PathOfPiece),
                    RecordTime = time,
                    User = CurrentUser
                };
                ctx.Moves.Add(move) ;
                ctx.SaveChanges();

                if (OtherUser != null)
                {
                    UserContact sentTo = runningGame[GameId].Item1.UserName == UserName ? runningGame[GameId].Item2 : runningGame[GameId].Item1;
                    sentTo.CheckersCallback.SendOpponentMove(PathOfPiece, EatenPieces, result);
                }

                if (result != Result.Continue)
                {
                    switch (result)
                    {
                        case (Result.Lost):
                            if (OtherUser == null)
                                game.Status = (int)Status.TwoWon;
                            else
                                game.Status = (int)(game.Users.ElementAt(0).UserName == UserName ? Status.TwoWon : Status.OneWon);
                            break;
                        case (Result.Tie):
                            game.Status = (int)Status.isTie;
                            break;
                        case (Result.Win):
                            if (OtherUser == null)
                                game.Status = (int)Status.OneWon;
                            else

                                game.Status = (int)(game.Users.ElementAt(0).UserName == UserName ? Status.OneWon : Status.TwoWon);

                            break;
                    }
                    ctx.Games.Add(game);
                    ctx.SaveChanges();

                   runningGame.Remove(GameId);
                }
            }
        }

        private ICollection<Point> Convert(List<System.Windows.Point> listToConvert)
        {
            ICollection<Point> newList = new List<Point>();
            foreach (var item in listToConvert) {
                Point p = new Point()
                {
                    Column = (int)item.Y,
                    Row = (int)item.X
                };
                newList.Add(p);
            }
            return newList;
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

        public Game WatchGame(string usrName, int gameId)
        {
            //ICheckersCallback callback = onlineUsers[usrName];
            // onlineUsers.Remove(usrName);
            //watchingGames[gameId].Add(usrName, callback);

            //using (var ctx = new CheckersDBContext())
            //{
            //    var game = (from u in ctx.Games where u.GameId == gameId select u).FirstOrDefault();
            //    return game;
            // }
            return new Game();
        }

        public void Disconnect(string usrName, Mode userMode, int numGame = -1)
        {
            //need to check if is playing,watching, or in lobby
            User usr;
            using (var ctx = new CheckersDBContext())
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
                /*Game game = runningGame[numGame];
                User winner*/
            }


            onlineUsers.Remove(usrName);
        }

        public ICollection<Move> GetAllMoves(Game game)
        {
            throw new NotImplementedException();
        }

        public void ResetPassword(string email)
        {
            User usr = null;
            using (var ctx = new CheckersDBContext())
            {
                usr = (from u in ctx.Users
                       where u.Email == email
                       select u).FirstOrDefault();
                if(usr == null)
                {
                    UserNotExistsFault fault = new UserNotExistsFault
                    {
                        Message = $"The email {email} is not Exists",
                        usrName = email
                    };
                    throw new FaultException<UserNotExistsFault>(fault);
                }
                string password = CreatePassword(PASS_LENGTH);

                usr.HashedPassword = HashValue(password);
                ctx.SaveChanges();
                SendEmail(password, usr);
            }
        }

        private void SendEmail(string password, User usr)
        {
            var fromAddress = new MailAddress("spamforme55@gmail.com‬", "‪spam spamy");
            var toAddress = new MailAddress(usr.Email,"Sahar");
            const string fromPassword = "";
            const string subject = "A new Password to log-in";
            string body = $"Dear {usr.UserName},\n This is the new password to login the application:{password}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

        }

        public string CreatePassword(int le)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < le--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private string HashValue(string password)
        {
            using (SHA256 hashObject = SHA256.Create())
            {
                byte[] hashBytes = hashObject.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void CloseUnFinishedGame(int GameId, string UserName)
        {
            throw new NotImplementedException();
        }

        public void StopWaitingGame(string UserName, int boardSize)
        {
            var playerToRemove = (from u in waitingRoom[boardSize]
                                  where u.UserName == UserName
                                  select u).First();
            waitingRoom[boardSize].Remove(playerToRemove);
            onlineUsers.Add(playerToRemove.UserName, playerToRemove.CheckersCallback);
        }
    }
}

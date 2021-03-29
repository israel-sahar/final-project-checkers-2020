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
    
    //lecture 13_3 -43:48 make a thread to connect with users
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode =ConcurrencyMode.Multiple)]
    public class CheckersService : ICheckersService
    {
        private readonly int PASS_LENGTH = 8;

        Dictionary<string, ICheckersCallback> onlineUsers = new Dictionary<string, ICheckersCallback>();
        Dictionary<int, (UserContact, UserContact)> runningGame = new Dictionary<int, (UserContact, UserContact)>();
        Dictionary<int, Dictionary<string, ICheckersCallback>> watchingGames = new Dictionary<int, Dictionary<string, ICheckersCallback>>(); 

        public User Connect(string usrName, string hashedPassword)
        {
            if(onlineUsers.ContainsKey(usrName)){
                UserAlreadyLoginFault fault = new UserAlreadyLoginFault {
                    Details = $" The User {usrName} online",
                    usrName = usrName
                };
                throw new FaultException<UserAlreadyLoginFault>(fault);
            }
            User user;
            using (var ctx = new CheckersDBContext()){
                user = (from u in ctx.Users where u.UserName == usrName select u).FirstOrDefault();
                if (user == null){
                    UserNotExistsFault fault = new UserNotExistsFault{
                        Details = $"The user {usrName} is not Exists",
                        usrName = usrName
                    };
                    throw new FaultException<UserNotExistsFault>(fault);
                }
                else {
                    if (user.HashedPassword != hashedPassword) {
                        WrongPasswordFault fault = new WrongPasswordFault{
                            Details = $"The password is incorrect",
                            usrName = usrName
                        };
                        throw new FaultException<WrongPasswordFault>(fault);
                    }
                  
                }
            }
            ICheckersCallback callback = OperationContext.Current.GetCallbackChannel<ICheckersCallback>();
            onlineUsers.Add(usrName, callback);
            return user;
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
                        Details = $"User already in database",
                        Email = email
                    };
                    throw new FaultException<UserAlreadyExistsFault>(f);
                }
                user = (from u in ctx.Users where u.UserName == userName select u).FirstOrDefault();
                if (user != null)
                {
                    UserNameAlreadyExistsFault f = new UserNameAlreadyExistsFault
                    {
                        Details = $"Username already in database",
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
        }

        public Game GetGame(int gameId)
        {
            using (var ctx = new CheckersDBContext()) {
                var game = (from u in ctx.Games
                 where gameId == u.GameId
                 select u).FirstOrDefault();
                if (game == null)
                {
                    GameIdNotExists fault = new GameIdNotExists
                    {
                        Details = "gameId not in database",
                        gameId = gameId
                    };
                    throw new FaultException<GameIdNotExists>(fault);
                }
                return game;
            }
        }

        public void JoinGame(User user)
        {
            throw new NotImplementedException();
        }

        public void MakeMove(Move move)
        {
            throw new NotImplementedException();
        }

        public bool Ping()
        {
            return true;
        }

        public void StopWatchGame(string usrName, int gameId)
        {
            ICheckersCallback callback = watchingGames[gameId][usrName];
            watchingGames[gameId].Remove(usrName);
            onlineUsers.Add(usrName, callback);
        }

        public Game WatchGame(string usrName, int gameId)
        {
            ICheckersCallback callback = onlineUsers[usrName];
            onlineUsers.Remove(usrName);
            watchingGames[gameId].Add(usrName, callback);

            using (var ctx = new CheckersDBContext())
            {
                var game = (from u in ctx.Games where u.GameId == gameId select u).FirstOrDefault();
                return game;
            }
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
                watchingGames[numGame].Remove(usrName);
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
                        Details = $"The email {email} is not Exists",
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
    }
}

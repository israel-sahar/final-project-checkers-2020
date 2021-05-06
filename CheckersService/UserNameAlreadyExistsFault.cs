using System.Runtime.Serialization;

namespace CheckersService
{
    [DataContract]
    public class UserNameAlreadyExistsFault
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string UserName { get; set; }
    }
}
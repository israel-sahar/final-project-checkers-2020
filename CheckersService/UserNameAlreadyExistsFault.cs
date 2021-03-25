using System.Runtime.Serialization;

namespace CheckersService
{
    [DataContract]
    public class UserNameAlreadyExistsFault
    {
        [DataMember]
        public string Details { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}
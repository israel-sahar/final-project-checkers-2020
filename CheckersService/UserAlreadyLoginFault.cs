using System.Runtime.Serialization;

namespace CheckersService
{
    [DataContract]
    public class UserAlreadyLoginFault
    {
        [DataMember]
        public string Details { get; set; }
        [DataMember]
        public string usrName { get; set; }
    }
}

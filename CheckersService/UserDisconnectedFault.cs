using System.Runtime.Serialization;

namespace CheckersService
{
    [DataContract]
    internal class UserDisconnectedFault
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string usrName { get; set; }
    }
}
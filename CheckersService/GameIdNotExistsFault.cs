using System.Runtime.Serialization;

namespace CheckersService
{
    [DataContract]
    public class GameIdNotExistsFault
    {
        [DataMember]
        public string Message { get; internal set; }
        [DataMember]
        public int gameId { get; internal set; }
    }
}
using System.Runtime.Serialization;

namespace CheckersService
{
    [DataContract]
    public class GameIdNotExists
    {
        [DataMember]
        public string Details { get; internal set; }
        [DataMember]
        public int gameId { get; internal set; }
    }
}
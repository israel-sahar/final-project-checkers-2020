using System.Runtime.Serialization;

namespace CheckersService
{
    [DataContract]
    public enum Mode
    {
        [EnumMember]
        Lobby,
        [EnumMember]
        Watching,
        [EnumMember]
        Playing
    }
}
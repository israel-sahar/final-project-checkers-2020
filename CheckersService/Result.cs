using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CheckersService
{
    [DataContract]
    public enum Result
    {
        [EnumMember]
        Win,
        [EnumMember]
        Lost,
        [EnumMember]
        Tie,
        [EnumMember]
        Continue
    }
}

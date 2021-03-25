using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CheckersService
{
    [DataContract]
    public class UserAlreadyExistsFault
    {
        [DataMember]
        public string Details { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}

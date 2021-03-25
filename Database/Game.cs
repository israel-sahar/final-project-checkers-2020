using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }
        public ICollection<User> Players { get; set; }
        public ICollection<Move> Moves { get; set; }
        public int Status { get; set; }
        public DateTime Date { get; set; }   
    }
}

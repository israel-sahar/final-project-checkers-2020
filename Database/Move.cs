using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Move
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MoveId { get; set; }
       
        public User User { get; set; }
        public DateTime RecordTime { get; set; }

        public ICollection<Point> EatenPieces { get; set; }
        public ICollection<Point> PointsToMove { get; set; }
    }
}

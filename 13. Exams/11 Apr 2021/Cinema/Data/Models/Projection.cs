using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Data.Models
{
    public class Projection
    {
        [Key]
        public int Id { get; set; }

        //required
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        //required
        public DateTime DateTime { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
//•	Id – integer, Primary Key
//•	MovieId – integer, Foreign key (required)
//•	Movie – the Projection’s Movie 
//•	DateTime - DateTime (required)
//•	Tickets - collection of type Ticket

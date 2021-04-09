using Cinema.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        //[MinLength(3)]
        [MaxLength(20)]
        public string Title { get; set; }

        public Genre Genre { get; set; }

        public TimeSpan Duration { get; set; }

        //[Range(typeof(double), "1", "10")]
        public double Rating { get; set; }
        
        [Required]
        //[MinLength(3)]
        [MaxLength(20)]
        public string Director { get; set; }

        public virtual ICollection<Projection> Projections { get; set; } = new HashSet<Projection>();
    }
}
//•	Id – integer, Primary Key
//•	Title – text with length [3, 20] (required)
//•	Genre – enumeration of type Genre, with possible values (Action, Drama, Comedy, Crime, Western, Romance, Documentary, Children, Animation, Musical) (required)
//•	Duration – TimeSpan(required)
//•	Rating – double in the range[1, 10] (required)
//•	Director – text with length [3, 20] (required)
//•	Projections - collection of type Projection


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Data.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        //required
        //[Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        //required
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        //required
        [ForeignKey(nameof(Projection))]
        public int ProjectionId { get; set; }
        public Projection Projection { get; set; }
    }
}
//•	Id – integer, Primary Key
//•	Price – decimal (non-negative, minimum value: 0.01) (required)
//•	CustomerId – integer, Foreign key(required)
//•	Customer – the Ticket’s Customer 
//•	ProjectionId – integer, Foreign key (required)
//•	Projection – the Ticket’s Projection

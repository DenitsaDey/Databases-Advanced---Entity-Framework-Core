using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetClinic.Data.Models
{
    public class AnimalAid
    {
        [Key]
        public int Id { get; set; }

        [Required]
        //[MinLength(3)]
        [MaxLength(30)]
        //HasAlternateKey in fluent API
        public string Name { get; set; }

        [Required]
        //[Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public virtual ICollection<ProcedureAnimalAid> AnimalAidProcedures { get; set; } = new HashSet<ProcedureAnimalAid>();

    }
}
//-Id – integer, Primary Key
//-	Name – text with min length 3 and max length 30 (required, unique)
//-Price – decimal(non - negative, minimum value: 0.01, required)
//- AnimalAidProcedures – collection of type ProcedureAnimalAid


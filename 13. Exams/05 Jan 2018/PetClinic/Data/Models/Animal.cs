using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetClinic.Data.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        //[MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        //[MinLength(3)]
        [MaxLength(20)]
        public string Type { get; set; }

        [Required]
        //[Range(1, 400)]
        public int Age { get; set; }

        [Required]
        [ForeignKey(nameof(Passport))]
        public string PassportSerialNumber { get; set; }
        public virtual Passport Passport { get; set; }

        public virtual ICollection<Procedure> Procedures { get; set; } = new HashSet<Procedure>();
    }
}
//-Id – integer, Primary Key
//-	Name – text with min length 3 and max length 20 (required)
//-Type – text with min length 3 and max length 20 (required)
//-Age – integer, cannot be negative or 0 (required)
//-PassportSerialNumber ¬– string, foreign key
//-	Passport – the passport of the animal (required)
//-Procedures – the procedures, performed on the animal


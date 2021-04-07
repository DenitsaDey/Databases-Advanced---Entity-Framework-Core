using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PetClinic.Data.Models
{
    public class Procedure
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AnimalId { get; set; }
        public virtual Animal Animal { get; set; }

        [Required]
        public int VetId { get; set; }
        public virtual Vet Vet { get; set; }

        public virtual ICollection<ProcedureAnimalAid> ProcedureAnimalAids { get; set; } = new HashSet<ProcedureAnimalAid>();

        [NotMapped]
        public decimal Cost => this.ProcedureAnimalAids.Sum(x => x.AnimalAid.Price);

        public DateTime DateTime { get; set; }
    }
}
//-Id – integer, Primary Key
//-	AnimalId ¬– integer, foreign key
//-	Animal – the animal on which the procedure is performed (required)
//-VetId ¬– integer, foreign key
//-	Vet – the clinic’s employed doctor servicing the patient (required)
//-ProcedureAnimalAids – collection of type ProcedureAnimalAid
//-	Cost – the cost of the procedure, calculated by summing the price of the different services performed; does not need to be inserted in the database
//-	DateTime – the date and time on which the given procedure is performed (required)

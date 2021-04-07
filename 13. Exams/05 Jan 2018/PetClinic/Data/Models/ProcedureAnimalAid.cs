using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Data.Models
{
    public class ProcedureAnimalAid
    {
        [ForeignKey(nameof(Procedure))]
        public int ProcedureId { get; set; }

        public virtual Procedure Procedure { get; set; }

        [ForeignKey(nameof(AnimalAid))]
        public int AnimalAidId { get; set; }

        public virtual AnimalAid AnimalAid { get; set; }
    }
}
//ProcedureAnimalAid
//- ProcedureId – integer, Primary Key
//-	Procedure – the animal aid’s procedure (required)
//-AnimalAidId – integer, Primary Key
//-	AnimalAid – the procedure’s animal aid (required)

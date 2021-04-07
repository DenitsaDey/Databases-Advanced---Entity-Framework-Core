using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetClinic.DataProcessor.Dto.Import
{
    public class AnimalJsonInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Type { get; set; }

        [Required]
        [Range(1, 400)]
        public int Age { get; set; }

        //[Required]
        //[ForeignKey(nameof(Passport))]
        //public string PassportSerialNumber { get; set; }

        public PassportJsonInputModel Passport { get; set; }
    }
}

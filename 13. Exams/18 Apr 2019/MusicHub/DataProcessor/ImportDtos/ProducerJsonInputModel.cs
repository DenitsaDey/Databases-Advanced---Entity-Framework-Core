using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.DataProcessor.ImportDtos
{
    public class ProducerJsonInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }

        [RegularExpression(@"^[A-Z][a-z]+\s[A-Z][a-z]+$")]
        public string Pseudonym { get; set; }

        [RegularExpression(@"^(\+359) (\d{3} ){2}\d{3}$")]
        public string PhoneNumber { get; set; }

        public AlbumJsonInputModel[] Albums { get; set; }
    }
}

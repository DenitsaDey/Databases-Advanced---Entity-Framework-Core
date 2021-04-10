using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class CardInputModel
    {
        [Required]
        [RegularExpression(@"^((\d{4} ){3})(\d{4})$")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"^(\d{3})$")]
        public string CVC { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
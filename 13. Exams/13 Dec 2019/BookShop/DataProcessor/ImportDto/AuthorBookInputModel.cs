using System.ComponentModel.DataAnnotations;

namespace BookShop.DataProcessor.ImportDto
{
    public class AuthorBookInputModel
    {
        [Required]
        public int? Id { get; set; }
    }
}
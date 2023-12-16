using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Category
{
    public class CategoryAddDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}

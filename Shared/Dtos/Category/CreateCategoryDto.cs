using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name must not exceed 100 characters.")]
        public string Name { get; set; } = default!;

        [MaxLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
        public string? Description { get; set; }

        public string? CreatedBy { get; set; }
    }
}
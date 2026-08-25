using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Book
{
    public class CreateBookDto
    {
        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
        public string ISBN { get; set; } = default!;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
        public string Title { get; set; } = default!;

        [StringLength(50, ErrorMessage = "Edition cannot exceed 50 characters")]
        public string? Edition { get; set; }

        public string? Summary { get; set; }

        [Range(1450, 2100, ErrorMessage = "Publication year must be between 1450 and 2100")]
        public int PublicationYear { get; set; }

        public IFormFileCollection? Images { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "LanguageId must be a valid id")]
        public int LanguageId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PublisherId must be a valid id")]
        public int PublisherId { get; set; }

        [EnumDataType(typeof(Shared.Enums.BookStatus), ErrorMessage = "Invalid book status")]
        public Shared.Enums.BookStatus Status { get; set; }

        [MinLength(1, ErrorMessage = "At least one author is required")]
        public List<int> AuthorIds { get; set; } = new();

        [MinLength(1, ErrorMessage = "At least one category is required")]
        public List<int> CategoryIds { get; set; } = new();

        public string? CreatedBy { get; set; }
    }
}
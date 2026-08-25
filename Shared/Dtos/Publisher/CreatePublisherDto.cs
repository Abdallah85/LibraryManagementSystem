using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Publisher
{
    public class CreatePublisherDto
    {
        [Required(ErrorMessage = "Publisher name is required.")]
        [MaxLength(200, ErrorMessage = "Publisher name must not exceed 200 characters.")]
        public string Name { get; set; } = default!;

        [MaxLength(300, ErrorMessage = "Address must not exceed 300 characters.")]
        public string? Address { get; set; }

        [MaxLength(300, ErrorMessage = "Contact email must not exceed 300 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? ContactEmail { get; set; }

        [MaxLength(300, ErrorMessage = "Website must not exceed 300 characters.")]
        [Url(ErrorMessage = "Invalid website URL format.")]
        public string? Website { get; set; }

        public string? CreatedBy { get; set; }
    }
}
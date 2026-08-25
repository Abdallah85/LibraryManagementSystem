

using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Borrowing
{
    public class BorrowBookDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BookId must be a positive integer.")]
        public int BookId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}

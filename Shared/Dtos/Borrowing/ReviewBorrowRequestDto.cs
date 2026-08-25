namespace Shared.Dtos.Borrowing;

using System.ComponentModel.DataAnnotations;

public class ReviewBorrowRequestDto
{
    [Required(ErrorMessage = "Approval status is required.")]
    public bool IsApproved { get; set; }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using ServicesAbstractions;
using Shared.Dtos.Borrowing;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingController : ControllerBase
    {
        private readonly IBorrowingService _borrowingService;

        public BorrowingController(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }


        [Authorize(Roles = "Member")]
        [HttpPost()]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowBookDto dto)
        {
            var userId = User.GetUserId();
            var result = await _borrowingService.BorrowBookAsync(userId!,dto);
            return Ok(result);
        }


        [HttpPut("{transactionId:int}/review")]
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> ReviewBorrowRequest(int transactionId,ReviewBorrowRequestDto dto)
        {
            var librarianId = User.GetUserId();
            var result = await _borrowingService.ReviewBorrowRequestAsync(transactionId,librarianId!,dto);
            return Ok(result);
        }

        [Authorize(Roles = "Member")]
        [HttpGet("my-transactions")]
        public async Task<IActionResult> GetMyTransactions([FromQuery] BorrowingTransactionFilterDto filterDto)
        {
            var userId = User.GetUserId();
            filterDto.UserId = userId;
            var result = await _borrowingService.GetAllTransactionsAsync(filterDto);
            return Ok(result);
        }


        [Authorize(Roles = "Administrator,Librarian")]
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions([FromQuery] BorrowingTransactionFilterDto filterDto)
        {
            var result = await _borrowingService.GetAllTransactionsAsync(filterDto);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPut("{transactionId:int}/confirm-return")]
        public async Task<IActionResult> ConfirmReturn(int transactionId)
        {
            var librarianId = User.GetUserId();
            var result = await _borrowingService.ConfirmReturnAsync(transactionId, librarianId!);
            return Ok(result);
        }


        [Authorize(Roles = "Member")]
        [HttpPut("{transactionId:int}/return")]
        public async Task<IActionResult> ReturnBook(int transactionId)
        {
            var userId = User.GetUserId();
            var result = await _borrowingService.ReturnBookAsync(transactionId, userId!);
            return Ok(result);
        }
    }
}

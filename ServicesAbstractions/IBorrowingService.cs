using Shared;
using Shared.Dtos.Borrowing;

namespace ServicesAbstractions
{
    public interface IBorrowingService
    {
        Task<ApiResponse<string>> BorrowBookAsync(string userId,BorrowBookDto dto);
        Task<ApiResponse<string>> ReturnBookAsync(int transactionId,string userId);
        Task<ApiResponse<string>> ReviewBorrowRequestAsync(int transactionId,string librarianId,ReviewBorrowRequestDto dto);
        Task<ApiResponse<string>> ConfirmReturnAsync(int transactionId, string librarianId);
        Task<ApiResponse<PaginatedResponse<BorrowingTransactionResponseDto>>>GetAllTransactionsAsync(BorrowingTransactionFilterDto filterDto);
    }
}

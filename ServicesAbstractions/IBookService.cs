using Shared;
using Shared.Dtos.Book;

namespace ServicesAbstractions
{
    public interface IBookService
    {
        Task<ApiResponse<string>> CreateBookAsync(CreateBookDto dto);
        Task<ApiResponse<string>> UpdateBookAsync(int bookId, UpdateBookDto dto);
        Task<ApiResponse<string>> DeleteBookAsync(int bookId, string deletedBy);
        Task<ApiResponse<BookResponseDto>> GetBookByIdAsync(int bookId);
        Task<ApiResponse<PaginatedResponse<BookResponseDto>>> GetAllBooksAsync(BookSearchFilterDto filterDto);
    }
}

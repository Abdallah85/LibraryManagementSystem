namespace Shared.Dtos.Book
{
    public class BookSearchFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public int? LanguageId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

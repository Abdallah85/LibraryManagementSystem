namespace Shared.Dtos.Author
{
    public class AuthorResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? Bio { get; set; }
    }


    public class AuthorSearchFilterDto
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
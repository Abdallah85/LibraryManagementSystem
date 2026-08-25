namespace Shared.Dtos.Book
{
    public class BookResponseDto
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public int PublicationYear { get; set; }
        public List<string>? CoverImageUrls { get; set; }
        public string Status { get; set; } = default!;

        public int LanguageId { get; set; }
        public string LanguageName { get; set; } = default!;

        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = default!;

        public List<BookAuthorDto> Authors { get; set; } = new();
        public List<BookCategoryDto> Categories { get; set; } = new();
    }


    public class BookAuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } 
    }

    public class BookCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

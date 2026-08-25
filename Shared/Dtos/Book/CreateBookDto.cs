using Microsoft.AspNetCore.Http;

namespace Shared.Dtos.Book
{
    public class CreateBookDto
    {
        public string ISBN { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public int PublicationYear { get; set; }
        public IFormFileCollection? Images { get; set; }

        public int LanguageId { get; set; }
        public int PublisherId { get; set; }

        public Shared.Enums.BookStatus Status { get; set; }

        public List<int> AuthorIds { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();

        public string? CreatedBy { get; set; }
    }
}

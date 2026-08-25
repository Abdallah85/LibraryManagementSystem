using Microsoft.AspNetCore.Http;
using Shared.Enums;

namespace Shared.Dtos.Book
{
    public class UpdateBookDto
    {
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public int? PublicationYear { get; set; }
        public IFormFileCollection? Images { get; set; }
        public BookStatus? Status { get; set; }

        public int? LanguageId { get; set; }
        public int? PublisherId { get; set; }

        // Null = leave author/category links untouched. Empty list = clear all links.
        public List<int>? AuthorIds { get; set; }
        public List<int>? CategoryIds { get; set; }
    }
}

namespace Shared.Dtos.Publisher
{
    public class PublisherResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? Website { get; set; }
    }


    public class PublisherFilterDto
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
namespace Shared.Dtos.Publisher
{
    public class CreatePublisherDto
    {
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? Website { get; set; }

        public string? CreatedBy { get; set; }
    }
}
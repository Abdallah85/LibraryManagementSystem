namespace Shared.Dtos.Publisher
{
    public class UpdatePublisherDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? Website { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
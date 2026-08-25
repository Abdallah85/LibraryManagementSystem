namespace Shared.Dtos.Author
{
    public class CreateAuthorDto
    {
        public string FullName { get; set; } = default!;
        public string? Bio { get; set; }
        public string? CreatedBy { get; set; }
    }
}
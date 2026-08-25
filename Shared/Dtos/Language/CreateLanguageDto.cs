namespace Shared.Dtos.Language
{
    public class CreateLanguageDto
    {
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? CreatedBy { get; set; }
    }
}
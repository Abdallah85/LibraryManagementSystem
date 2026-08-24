namespace Shared.Dtos.Language
{
    public class LanguageResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
    }


    public class LanguageSearchFilterDto
    {
        public int PaegeNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}
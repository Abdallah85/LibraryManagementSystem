

namespace Domain.Exceptions
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; } = default!;
        public string Message { get; set; } = default!;
        public object? Details { get; set; }
        public string TraceId { get; set; } = default!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

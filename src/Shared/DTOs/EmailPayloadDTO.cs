namespace Shared.DTOs
{
    public record EmailPayloadDTO
    {
        public string To { get; init; }
        public string Subject { get; init; }
        public string Body { get; init; }
        public bool IsHtml { get; init; } = false;
    }
}

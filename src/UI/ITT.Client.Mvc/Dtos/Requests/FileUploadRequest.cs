using ITT.Client.Mvc.Dtos.Common;

namespace ITT.Client.Mvc.Dtos.Requests
{
    public record FileUploadRequest : Request
    {
        public int FileId { get; init; }
        public int FileType { get; init; }
        public string? FileName { get; init; }
        public string? FileExtension { get; init; }
        public string? FileContentType { get; init; }
        public byte[]? FileData { get; init; }
        public DateTimeOffset DateCreated { get; init; }
    }
}

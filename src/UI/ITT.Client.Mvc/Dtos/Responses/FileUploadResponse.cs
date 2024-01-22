using ITT.Client.Mvc.Dtos.Common;

namespace ITT.Client.Mvc.Dtos.Responses
{
    public record FileUploadResponse : Response
    {
        public int FileType { get; init; }
        public string? FileName { get; init; }
        public string? FileExtension { get; init; }


    }
}

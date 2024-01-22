namespace ITT.Client.Mvc.Dtos.Requests
{
    public record TextSummaryRequest
    {
        public int DocumentId { get; init; }
        public int DcoumentTypeId { get; init; }
        public string? DocumentName { get; init; }
        public string? TextContent { get; init; }
        public string? TextSummary { get; init; }


    }
}

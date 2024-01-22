namespace ITT.Client.Mvc.Dtos.Responses
{
    public record TextSummaryResponse
    {
        public int DocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public string? DocumentName { get; set; }
        public string? TextContent { get; set; }
        public string? TextSummary { get; set; }


    }
}

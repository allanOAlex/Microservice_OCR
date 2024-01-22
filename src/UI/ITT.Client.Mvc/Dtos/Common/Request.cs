namespace ITT.Client.Mvc.Dtos.Common
{
    public record Request
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }



    }
}

namespace BookReader.API.Models.Responses
{
    public abstract class BaseApiResponse
    {
        public int Code { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

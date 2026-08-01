namespace BookReader.API.Models.Responses
{
    public class ApiResponse<T> 
    {
        public T? Data { get; set; }
        public int Code { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

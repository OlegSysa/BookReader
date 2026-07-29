namespace BookReader.API.Models.Requests
{
    public class UploadBookRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}

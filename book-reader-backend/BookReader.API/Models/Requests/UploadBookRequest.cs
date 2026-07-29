using System.ComponentModel.DataAnnotations;

namespace BookReader.API.Models.Requests
{
    public class UploadBookRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}

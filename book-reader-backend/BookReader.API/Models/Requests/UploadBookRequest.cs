using System.ComponentModel.DataAnnotations;

namespace BookReader.API.Models.Requests
{
    public record UploadBookRequest
    {
        [Required]
        public required IFormFile File { get; set; } = default!;
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Author { get; set; }
    }
}

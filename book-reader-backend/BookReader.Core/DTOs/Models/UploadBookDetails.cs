namespace BookReader.Core.DTOs.Models
{
    public record UploadBookDetails(string FileName, long FileSize, int UserId);
}

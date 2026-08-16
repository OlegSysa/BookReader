namespace BookReader.Core.DTOs.Models
{
    public record UploadBookDetails(string FileName, string Title, string Author, long FileSize, int UserId);
}

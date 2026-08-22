using BookReader.Core.Enums;

namespace BookReader.Core.DTOs.Models
{
    public record BookModel(int Id,
        string OriginalFileName,
        long FileSize,
        int Status,
        DateTime CreatedAtUtc);

}

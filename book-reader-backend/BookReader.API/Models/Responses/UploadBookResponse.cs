using BookReader.Core.Enums;

namespace BookReader.API.Models.Requests
{
    public class UploadBookResponse
    {
        public int Code { get; set; }
        public bool Success { get; set; }
        public BookStatus Status { get; set; }
    }
}

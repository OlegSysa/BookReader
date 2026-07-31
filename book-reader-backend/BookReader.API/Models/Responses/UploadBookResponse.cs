using BookReader.API.Models.Responses;
using BookReader.Core.Enums;

namespace BookReader.API.Models.Requests
{
    public class UploadBookResponse : BaseApiResponse
    {
        public BookStatus Status { get; set; }
    }
}

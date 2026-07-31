using BookReader.Core.Entities;

namespace BookReader.API.Models.Responses
{
    public class GetChapterResponse : BaseApiResponse
    {
        public Chapter? Chapter { get; set; }
    }
}

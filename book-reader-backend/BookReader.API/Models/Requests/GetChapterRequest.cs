namespace BookReader.API.Models.Requests
{
    public class GetChapterRequest
    {
        public int BookId { get; set; }
        public int Selector {  get; set; }
    }
}

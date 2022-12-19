namespace Habr.Common.Requests
{
    public class CreatePostRequest
    {
        public bool SaveAsDraft { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
namespace Habr.Common.Requests
{
    public class CreateCommentRequest
    {
        public string Text { get; set; }
        public int? ParentId { get; set; }
    }
}
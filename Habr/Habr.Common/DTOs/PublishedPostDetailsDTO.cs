namespace Habr.Common.DTOs
{
    public class PublishedPostDetailsDTO
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
namespace Habr.Common.DTOs
{
    public class CommentDTO
    {
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorEmail { get; set; }
        public List<CommentDTO> Childrens { get; set; }
    }
}
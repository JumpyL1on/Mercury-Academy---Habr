namespace Habr.Common.DTOs
{
    public class PreliminaryCommentDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorEmail { get; set; }
        public int? ParentId { get; set; }
    }
}
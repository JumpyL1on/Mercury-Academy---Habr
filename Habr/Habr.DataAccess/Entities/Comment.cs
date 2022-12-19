namespace Habr.DataAccess.Entities
{
    public class Comment
    {
        public Comment()
        {
            Childrens = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int? ParentId { get; set; }
        public Comment Parent { get; set; }
        public ICollection<Comment> Childrens { get; set; }
    }
}
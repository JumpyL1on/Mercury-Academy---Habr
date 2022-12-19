namespace Habr.DataAccess.Entities
{
    public class Rating
    {
        public int Value { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
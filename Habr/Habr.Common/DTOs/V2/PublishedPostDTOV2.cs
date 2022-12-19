namespace Habr.Common.DTOs.V2
{
    public class PublishedPostDTOV2
    {
        public string Title { get; set; }
        public DateTime PublishedAt { get; set; }
        public AuthorDTO Author { get; set; }
    }
}
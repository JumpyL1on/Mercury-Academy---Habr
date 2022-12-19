namespace Habr.Common.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public TokensDTO Tokens { get; set; }
    }
}
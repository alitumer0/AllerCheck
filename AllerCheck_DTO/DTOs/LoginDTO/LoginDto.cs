namespace AllerCheck.API.DTOs.LoginDTO
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string UserPassword { get; set; }
        public bool RememberMe { get; set; }
    }
}

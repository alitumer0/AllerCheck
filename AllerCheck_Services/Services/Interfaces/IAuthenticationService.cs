using System.Threading.Tasks;
using AllerCheck.API.DTOs.LoginDTO;
using AllerCheck.API.DTOs.RegisterDTO;
using AllerCheck.API.DTOs.UserDTO;

namespace AllerCheck_Services.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<(bool success, string message, UserDto user)> LoginAsync(LoginDto loginDto);
        Task<(bool success, string message)> RegisterAsync(RegisterDto registerDto);
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<bool> CheckUserExistsAsync(string email);
    }
} 
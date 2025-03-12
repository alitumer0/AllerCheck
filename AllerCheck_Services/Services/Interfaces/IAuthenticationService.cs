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
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<UserDto> GetUserByUsernameAsync(string username);
    }
} 
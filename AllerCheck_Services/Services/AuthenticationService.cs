using System;
using System.Threading.Tasks;
using AllerCheck.API.DTOs.LoginDTO;
using AllerCheck.API.DTOs.RegisterDTO;
using AllerCheck.API.DTOs.UserDTO;
using AllerCheck_Services.Services.Interfaces;
using AutoMapper;
using AllerCheck_Data.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;
using AllerCheck_Core.Entities;
using Microsoft.EntityFrameworkCore;
using AllerCheck_Data.Context;

namespace AllerCheck_Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AllerCheckDbContext _context;

        public AuthenticationService(
            IUserRepository userRepository, 
            IMapper mapper,
            AllerCheckDbContext context)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<(bool success, string message, UserDto user)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Debug için log ekleyelim
                Console.WriteLine($"Login attempt for email: {loginDto.Email}");

                var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
                
                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return (false, "Geçersiz e-posta veya şifre.", null);
                }

                // Veritabanındaki şifreyi kontrol edelim
                Console.WriteLine($"DB Password: {user.UserPassword}");
                Console.WriteLine($"Input Password: {loginDto.UserPassword}");

                // Şifreleri direkt karşılaştıralım (geçici olarak)
                if (user.UserPassword != loginDto.UserPassword)
                {
                    Console.WriteLine("Password mismatch");
                    return (false, "Geçersiz e-posta veya şifre.", null);
                }

                var userDto = _mapper.Map<UserDto>(user);
                return (true, "Giriş başarılı.", userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return (false, $"Giriş işlemi sırasında bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message)> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (await CheckUserExistsAsync(registerDto.MailAdress))
                {
                    return (false, "Bu e-posta adresi zaten kayıtlı.");
                }

                var hashedPassword = HashPassword(registerDto.UserPassword);
                var user = _mapper.Map<User>(registerDto);
                user.UserPassword = hashedPassword;
                user.CreatedDate = DateTime.Now;

                var result = await _userRepository.CreateUserWithDetailsAsync(user);
                return result 
                    ? (true, "Kayıt başarıyla tamamlandı.") 
                    : (false, "Kayıt işlemi sırasında bir hata oluştu.");
            }
            catch (Exception ex)
            {
                return (false, $"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            var hashedPassword = HashPassword(password);
            return user.UserPassword == hashedPassword;
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> CheckUserExistsAsync(string email)
        {
            return await _userRepository.CheckUserExistsAsync(email);
        }

        private string HashPassword(string password)
        {
            return password;  // Şimdilik hashleme yapmayalım
        }
    }
} 
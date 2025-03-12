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
                var isValid = await ValidateUserCredentialsAsync(loginDto.UserName, loginDto.UserPassword);
                if (!isValid)
                {
                    return (false, "Geçersiz kullanıcı adı veya şifre.", null);
                }

                var user = await GetUserByUsernameAsync(loginDto.UserName);
                return (true, "Giriş başarılı.", user);
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                return (false, "Giriş işlemi sırasında bir hata oluştu.", null);
            }
        }

        public async Task<(bool success, string message)> RegisterAsync(RegisterDto registerDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kullanıcı adı veya email kontrolü
                if (await _userRepository.CheckUserExistsAsync(registerDto.MailAdress))
                {
                    return (false, "Bu e-posta adresi zaten kayıtlı.");
                }

                // Şifre hash'leme
                registerDto.UserPassword = HashPassword(registerDto.UserPassword);

                var user = _mapper.Map<User>(registerDto);
                var result = await _userRepository.CreateUserWithDetailsAsync(user);

                if (result)
                {
                    await transaction.CommitAsync();
                    return (true, "Kayıt başarıyla tamamlandı.");
                }
                
                await transaction.RollbackAsync();
                return (false, "Kayıt işlemi sırasında bir hata oluştu.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Loglama yapılabilir
                return (false, "Kayıt işlemi sırasında bir hata oluştu.");
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(username);
            if (user == null) return false;

            var hashedPassword = HashPassword(password);
            return user.UserPassword == hashedPassword;
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByEmailAsync(username);
            return _mapper.Map<UserDto>(user);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
} 
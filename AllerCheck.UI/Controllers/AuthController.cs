using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AllerCheck.API.DTOs.LoginDTO;
using AllerCheck.API.DTOs.RegisterDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using AllerCheck_Data.Context;
using AllerCheck_Core.Entities;

namespace AllerCheck.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly AllerCheckDbContext _context;

        public AuthController(AllerCheckDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            // Kullanıcıyı e-posta ile bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.MailAdress == loginDto.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View(loginDto);
            }

            // Şifre kontrolü (hash'lenmiş)
            var hashedPassword = HashPassword(loginDto.UserPassword);
            if (user.UserPassword != hashedPassword)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View(loginDto);
            }

            // Session'a kullanıcı bilgilerini kaydet
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserEmail", user.MailAdress);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserData", JsonSerializer.Serialize(user));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.MailAdress),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = loginDto.RememberMe
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(registerDto);
                }

                // E-posta kontrolü
                if (await _context.Users.AnyAsync(u => u.MailAdress == registerDto.MailAdress))
                {
                    ModelState.AddModelError("", "Bu e-posta adresi zaten kayıtlı.");
                    return View(registerDto);
                }

                // Yeni kullanıcı oluştur
                var user = new User
                {
                    UserName = registerDto.UserName,
                    UserSurname = registerDto.UserSurname,
                    MailAdress = registerDto.MailAdress,
                    UserPassword = HashPassword(registerDto.UserPassword),
                    CreatedDate = DateTime.Now,
                    UyelikTipiId = 1,
                    CreatedBy = 1
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kayıt başarıyla tamamlandı. Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}");
                return View(registerDto);
            }
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
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

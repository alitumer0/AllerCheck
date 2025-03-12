using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Threading.Tasks;
using AllerCheck.API.DTOs.UserDTO;
using AllerCheck.API.DTOs.BlackListDTO;
using AllerCheck.API.DTOs.FavoriteListDTO;
using System.Text.Json;
using System.Collections.Generic;

namespace AllerCheck.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration; //Todo: IConfiguration kullanımı hakkında bilgi ver.

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"])
            };
        }

        public async Task<IActionResult> Profile()  //  Todo: Her profilin kendine ait bir view'i olacak. Bu view'de kullanıcı bilgileri ve güncelleme işlemleri yapılacak. 
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _httpClient.GetAsync($"api/user/profile/{userId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(content);
                return View(user); 
            }
            
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Blacklist()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _httpClient.GetAsync($"api/user/{userId}/blacklist");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var blacklist = JsonSerializer.Deserialize<List<BlackListDto>>(content);
                return View(blacklist);
            }
            
            return View(new List<BlackListDto>());
        }

        public async Task<IActionResult> Favorites()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _httpClient.GetAsync($"api/user/{userId}/favorites");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var favorites = JsonSerializer.Deserialize<List<FavoriteListDto>>(content);
                return View(favorites);
            }
            
            return View(new List<FavoriteListDto>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserDto userDto)
        {
            if (!ModelState.IsValid)
                return View("Profile", userDto);

            var response = await _httpClient.PutAsJsonAsync("api/user/profile", userDto);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                return RedirectToAction(nameof(Profile));
            }
            
            ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu.");
            return View("Profile", userDto);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromBlacklist(int contentId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _httpClient.DeleteAsync($"api/user/{userId}/blacklist/{contentId}");
            
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            //Todo: Transaction kullanılacak mı burada ? burada bir hata oluşursa blacklistten çıkarılan içeriği tekrar eklemek gerekebilir.Buna da bir bak.
            return Json(new { success = false, message = "İçerik kaldırılırken bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int listId, int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _httpClient.DeleteAsync($"api/user/{userId}/favorites/{listId}/products/{productId}");
            
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            
            return Json(new { success = false, message = "Ürün favorilerden kaldırılırken bir hata oluştu." });
        }
    }
}

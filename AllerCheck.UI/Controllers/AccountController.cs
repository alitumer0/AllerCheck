using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using AllerCheck.API.DTOs.BlackListDTO;
using AllerCheck.API.DTOs.FavoriteListDTO;
using AllerCheck.API.DTOs.UserDTO;
using AllerCheck_Services.Services.Interfaces;

namespace AllerCheck.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Profile()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userService.GetUserByIdWithDetailsAsync(userId.Value);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bilgileri bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return View(userDto);
        }

        public async Task<IActionResult> FavoriteLists()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var favoriteLists = await _userService.GetUserFavoriteListsAsync(userId.Value);
            var favoriteListDtos = _mapper.Map<List<FavoriteListDto>>(favoriteLists);
            return View(favoriteListDtos);
        }

        public async Task<IActionResult> BlackList()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var blackList = await _userService.GetUserBlackListAsync(userId.Value);
            var blackListDtos = _mapper.Map<List<BlackListDto>>(blackList);
            return View(blackListDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int productId, string listName = "Favori Listem")
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userService.AddToFavoritesAsync(userId.Value, productId, listName);
            if (result)
            {
                TempData["Success"] = "Ürün favorilere eklendi.";
            }
            else
            {
                TempData["Info"] = "Bu ürün zaten favorilerinizde bulunuyor.";
            }

            return RedirectToAction(nameof(FavoriteLists));
        }

        [HttpPost]
        public async Task<IActionResult> AddToBlacklist(int contentId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            await _userService.AddToBlackListAsync(userId.Value, contentId);
            return RedirectToAction(nameof(BlackList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int favoriteListDetailId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userService.RemoveFromFavoritesAsync(userId.Value, favoriteListDetailId);
            if (result)
            {
                TempData["Success"] = "Ürün favorilerden kaldırıldı.";
            }
            else
            {
                TempData["Error"] = "Ürün favorilerden kaldırılırken bir hata oluştu.";
            }

            return RedirectToAction(nameof(FavoriteLists));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromBlacklist(int blackListId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userService.RemoveFromBlackListAsync(userId.Value, blackListId);
            if (result)
            {
                TempData["Success"] = "İçerik kara listeden kaldırıldı.";
            }
            else
            {
                TempData["Error"] = "İçerik kara listeden kaldırılırken bir hata oluştu.";
            }

            return RedirectToAction(nameof(BlackList));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return View(userDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserDto userDto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var result = await _userService.UpdateUserProfileAsync(userId.Value, userDto);
                if (result)
                {
                    TempData["Success"] = "Profiliniz başarıyla güncellendi.";
                    return RedirectToAction(nameof(Profile));
                }

                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu.");
                return View(userDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu: " + ex.Message);
                return View(userDto);
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
    }
}
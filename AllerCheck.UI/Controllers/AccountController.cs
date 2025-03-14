using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllerCheck_Data.Context;
using AllerCheck_Core.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using AllerCheck.API.DTOs.BlackListDTO;
using AllerCheck.API.DTOs.FavoriteListDTO;
using AllerCheck.API.DTOs.UserDTO;

namespace AllerCheck.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AccountController(AllerCheckDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Profile()  //  Todo: Her profilin kendine ait bir view'i olacak. Bu view'de kullanıcı bilgileri ve güncelleme işlemleri yapılacak. 
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users
                .Include(u => u.FavoriteLists)
                .Include(u => u.BlackLists)
                    .ThenInclude(b => b.Content)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(content);
                return View(user);
            }

            var userDto = _mapper.Map<UserDto>(user);
            return View(userDto);
        }

        public async Task<IActionResult> FavoriteLists()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                
                var favoriteLists = await _context.FavoriteLists
                    .Include(f => f.FavoriteListDetails)
                        .ThenInclude(fd => fd.Product)
                            .ThenInclude(p => p.Category)
                    .Include(f => f.FavoriteListDetails)
                        .ThenInclude(fd => fd.Product)
                            .ThenInclude(p => p.Producer)
                    .Include(f => f.FavoriteListDetails)
                        .ThenInclude(fd => fd.Product)
                            .ThenInclude(p => p.ContentProducts)
                                .ThenInclude(cp => cp.Content)
                                    .ThenInclude(c => c.RiskStatus)
                    .Where(f => f.UserId == userId)
                    .ToListAsync();

                var favoriteListDtos = _mapper.Map<List<FavoriteListDto>>(favoriteLists);
                return View(favoriteListDtos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Favori listeler yüklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> BlackList()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var blackList = await _context.BlackLists
                .Include(b => b.Content)
                    .ThenInclude(c => c.RiskStatus)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var blackListDtos = _mapper.Map<List<BlackListDto>>(blackList);
            return View(blackListDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int productId, string listName = "Varsayılan Liste")
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Ürünü kontrol et
                var product = await _context.Products
                    .Include(p => p.FavoriteListDetails)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    TempData["Error"] = "Ürün bulunamadı.";
                    return RedirectToAction("Products", "Home");
                }

                // Favori listesini bul veya oluştur
                var favoriteList = await _context.FavoriteLists
                    .Include(f => f.FavoriteListDetails)
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ListName == listName);

                if (favoriteList == null)
                {
                    favoriteList = new FavoriteList
                    {
                        UserId = userId,
                        ListName = listName,
                        FavoriteListDetails = new List<FavoriteListDetail>()
                    };
                    _context.FavoriteLists.Add(favoriteList);
                    await _context.SaveChangesAsync();
                }

                // Ürün zaten listede var mı kontrol et
                var existingItem = await _context.FavoriteListDetails
                    .AnyAsync(f => f.FavoriteListId == favoriteList.FavoriteListId && f.ProductId == productId);

                if (!existingItem)
                {
                    var detail = new FavoriteListDetail
                    {
                        FavoriteListId = favoriteList.FavoriteListId,
                        ProductId = productId
                    };
                    _context.FavoriteListDetails.Add(detail);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Ürün favorilere eklendi.";
                }
                else
                {
                    TempData["Info"] = "Bu ürün zaten favorilerinizde bulunuyor.";
                }

                return RedirectToAction(nameof(FavoriteLists));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ürün favorilere eklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Products", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToBlacklist(int contentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // İçerik zaten kara listede var mı kontrol et
            var existingItem = await _context.BlackLists
                .AnyAsync(b => b.UserId == userId && b.ContentId == contentId);

            if (!existingItem)
            {
                var blackListItem = new BlackList
                {
                    UserId = userId,
                    ContentId = contentId
                };
                await _context.BlackLists.AddAsync(blackListItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(BlackList));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int favoriteListDetailId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var detail = await _context.FavoriteListDetails
                .Include(f => f.FavoriteList)
                .FirstOrDefaultAsync(f => f.FavoriteListDetailId == favoriteListDetailId && f.FavoriteList.UserId == userId);

            if (detail != null)
            {
                _context.FavoriteListDetails.Remove(detail);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(FavoriteLists));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromBlacklist(int blackListId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var blackListItem = await _context.BlackLists
                .FirstOrDefaultAsync(b => b.BlackListId == blackListId && b.UserId == userId);

            if (blackListItem != null)
            {
                _context.BlackLists.Remove(blackListItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(BlackList));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }
            
            return Json(new { success = false, message = "İçerik kaldırılırken bir hata oluştu." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserDto userDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Sadece belirli alanların güncellenmesine izin ver
            user.UserName = userDto.UserName;
            user.UserSurname = userDto.UserSurname;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu: " + ex.Message);
                return View(userDto);
            }
        }
    }
}

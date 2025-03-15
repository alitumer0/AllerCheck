using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AllerCheck.UI.Models;
using Microsoft.EntityFrameworkCore;
using AllerCheck_Data.Context;
using AllerCheck_Core.Entities;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using AllerCheck.API.DTOs.ProductDTO;
using System.Security.Claims;
using AllerCheck_Services.Services.Interfaces;
using AllerCheck.UI.Extensions;
using AllerCheck.API.DTOs.ContentDTO;

namespace AllerCheck.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IContentService _contentService;

        public HomeController(
            IProductService productService,
            ICategoryService categoryService,
            ILogger<HomeController> logger,
            IMapper mapper,
            IContentService contentService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;
            _contentService = contentService;
        }

        public IActionResult Index()
        {
            bool isLoggedIn = User.Identity.IsAuthenticated;
            ViewBag.IsLoggedIn = isLoggedIn; //Todo: Index ve Layout'a değişkeni gönder. IsLoggedIn Değişkenlerini ekle.
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Products()
        {
            var products = await _productService.GetAllProductsWithDetailsAsync();
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return View(productDtos);
        }

        [Authorize]
        public async Task<IActionResult> Search(string query)
        {
            var products = await _productService.SearchProductsAsync(query);
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return View("Products", productDtos);
        }

        [Authorize]
        public async Task<IActionResult> Category(int id)
        {
            var products = await _productService.GetProductsByCategoryAsync(id);
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return View("Products", productDtos);
        }

        [Authorize]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Producers = await _productService.GetAllProducersAsync();
            ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, List<string> selectedContents)
        {
            try
            {
                if (product.CategoryId == 0)
                {
                    ModelState.AddModelError("CategoryId", "Kategori seçimi zorunludur.");
                }
                if (product.ProducerId == 0)
                {
                    ModelState.AddModelError("ProducerId", "Üretici seçimi zorunludur.");
                }
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Producers = await _productService.GetAllProducersAsync();
                    ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                    return View(product);
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                product.UserId = userId;
                product.CreatedBy = userId;
                product.CreatedDate = DateTime.Now;

                if (selectedContents == null || !selectedContents.Any())
                {
                    ModelState.AddModelError("", "En az bir içerik seçmelisiniz.");
                    ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Producers = await _productService.GetAllProducersAsync();
                    ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                    return View(product);
                }

                // Yeni içerikleri ve mevcut içerikleri ayır
                var existingContentIds = new List<int>();
                var newContents = new List<(string name, int riskStatusId)>();

                foreach (var content in selectedContents)
                {
                    if (content.StartsWith("new_"))
                    {
                        // Yeni içerik formatı: "new_timestamp:name:riskStatusId"
                        var parts = content.Split(':');
                        if (parts.Length == 3)
                        {
                            newContents.Add((parts[1], int.Parse(parts[2])));
                        }
                    }
                    else
                    {
                        existingContentIds.Add(int.Parse(content));
                    }
                }

                var result = await _productService.CreateProductWithContentsAsync(product, existingContentIds, newContents);
                
                if (result)
                {
                    TempData["Success"] = "Ürün başarıyla eklendi.";
                    return RedirectToAction(nameof(Products));
                }
                
                ModelState.AddModelError("", "Ürün eklenirken bir hata oluştu.");
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Producers = await _productService.GetAllProducersAsync();
                ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                return View(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Producers = await _productService.GetAllProducersAsync();
                ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                return View(product);
            }
        }

        [Authorize]
        public async Task<IActionResult> AddCategory()
        {
            ViewBag.TopCategories = await _categoryService.GetAllCategoriesAsync();
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _categoryService.CreateCategoryAsync(category);
                    if (result)
                    {
                        TempData["Success"] = "Kategori başarıyla eklendi.";
                        return RedirectToAction(nameof(Products));
                    }
                    
                    ModelState.AddModelError("", "Kategori eklenirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
            }
            
            ViewBag.TopCategories = await _categoryService.GetAllCategoriesAsync();
            return View(category);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            try 
            {
                var product = await _productService.GetProductWithContentsAsync(id);
                if (product == null)
                {
                    TempData["Error"] = "Düzenlemek istediğiniz ürün bulunamadı.";
                    return RedirectToAction(nameof(Products));
                }

                var productDto = _mapper.Map<ProductDto>(product);
                ViewBag.Categories = (await _categoryService.GetAllCategoriesAsync())?.ToList() ?? new List<Category>();
                ViewBag.Producers = (await _productService.GetAllProducersAsync())?.ToList() ?? new List<Producer>();
                ViewBag.Contents = (await _productService.GetAllContentsWithRiskStatusAsync())?.ToList() ?? new List<Content>();

                return View(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün düzenleme sayfası yüklenirken hata oluştu");
                TempData["Error"] = "Ürün bilgileri yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Products));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto productDto, List<int> selectedContents)
        {
            try
            {
                if (productDto.CategoryId == 0)
                {
                    ModelState.AddModelError("CategoryId", "Kategori seçimi zorunludur.");
                }
                if (productDto.ProducerId == 0)
                {
                    ModelState.AddModelError("ProducerId", "Üretici seçimi zorunludur.");
                }
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Producers = await _productService.GetAllProducersAsync();
                    ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                    return View(productDto);
                }

                if (selectedContents == null || !selectedContents.Any())
                {
                    ModelState.AddModelError("", "En az bir içerik seçmelisiniz.");
                    ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                    ViewBag.Producers = await _productService.GetAllProducersAsync();
                    ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                    return View(productDto);
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                
                // Mevcut ürünü getir
                var existingProduct = await _productService.GetProductWithContentsAsync(productDto.ProductId);
                if (existingProduct == null)
                {
                    TempData["Error"] = "Güncellemek istediğiniz ürün bulunamadı.";
                    return RedirectToAction(nameof(Products));
                }

                // Yeni değerleri ata
                var product = _mapper.Map<Product>(productDto);
                product.UserId = existingProduct.UserId;
                product.CreatedBy = existingProduct.CreatedBy;
                product.CreatedDate = existingProduct.CreatedDate;
                product.ModifiedBy = userId;
                product.ModifiedDate = DateTime.Now;

                var result = await _productService.UpdateProductWithContentsAsync(product, selectedContents);
                
                if (result)
                {
                    TempData["Success"] = "Ürün başarıyla güncellendi.";
                    return RedirectToAction(nameof(Products));
                }

                ModelState.AddModelError("", "Ürün güncellenirken bir hata oluştu.");
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Producers = await _productService.GetAllProducersAsync();
                ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                return View(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün güncellenirken bir hata oluştu");
                ModelState.AddModelError("", "Ürün güncellenirken bir hata oluştu.");
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Producers = await _productService.GetAllProducersAsync();
                ViewBag.Contents = await _productService.GetAllContentsWithRiskStatusAsync();
                return View(productDto);
            }
        }

        [Authorize]
        public async Task<IActionResult> ManageContents()
        {
            var contents = _contentService.GetAllContents();
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(_mapper.Map<List<ContentDto>>(contents));
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddContent(Content content)
        {
            try
            {
                content.ContentInfo = content.ContentInfo ?? "Kullanıcı tarafından eklendi";
                _contentService.AddContent(content);
                TempData["Success"] = "İçerik başarıyla eklendi.";
                return RedirectToAction("ManageContents");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İçerik eklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("ManageContents");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditContent(Content content)
        {
            try
            {
                var result = await _productService.UpdateContentAsync(content);
                if (result)
                {
                    TempData["Success"] = "İçerik başarıyla güncellendi.";
                }
                else
                {
                    TempData["Error"] = "İçerik güncellenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"İçerik güncellenirken bir hata oluştu: {ex.Message}";
            }
            return RedirectToAction(nameof(ManageContents));
        }

        [Authorize]
        public async Task<IActionResult> DeleteContent(int id)
        {
            try
            {
                await _productService.DeleteContentAsync(id);
                TempData["Success"] = "İçerik başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "İçerik silinirken bir hata oluştu");
            }

            return RedirectToAction(nameof(ManageContents));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

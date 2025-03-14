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

namespace AllerCheck.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;

        public HomeController(
            IProductService productService,
            ICategoryService categoryService,
            ILogger<HomeController> logger,
            IMapper mapper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
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
        public async Task<IActionResult> AddProduct(Product product, List<int> selectedContents)
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

                var result = await _productService.CreateProductWithContentsAsync(product, selectedContents);
                
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

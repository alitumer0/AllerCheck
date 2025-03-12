using System.Diagnostics;
using AllerCheck.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using AllerCheck.API.DTOs.ProductDTO;
using System.Collections.Generic;
using System.Text.Json;

namespace AllerCheck.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"])
            };
        }

        public IActionResult Index()
        {
            bool isLoggedIn = User.Identity.IsAuthenticated;
            ViewBag.IsLoggedIn = isLoggedIn; //Todo: Index ve Layout'a değişkeni gönder. IsLoggedIn Değişkenlerini ekle.
            return View();
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

        public async Task<IActionResult> Products()
        {
            var response = await _httpClient.GetAsync("api/product");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductDto>>(content);
                return View(products);
            }
            return View(new List<ProductDto>());
        }

        public async Task<IActionResult> Search(string query)
        {
            var response = await _httpClient.GetAsync($"api/product/search?query={query}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductDto>>(content);
                return View("Products", products);
            }
            return View("Products", new List<ProductDto>());
        }

        public async Task<IActionResult> Category(int id)
        {
            var response = await _httpClient.GetAsync($"api/category/{id}/products");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductDto>>(content);
                return View("Products", products);
            }
            return View("Products", new List<ProductDto>());
        }
    }
}

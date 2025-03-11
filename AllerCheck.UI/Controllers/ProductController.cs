using Microsoft.AspNetCore.Mvc;

namespace AllerCheck.UI.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

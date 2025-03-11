using Microsoft.AspNetCore.Mvc;

namespace AllerCheck.UI.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

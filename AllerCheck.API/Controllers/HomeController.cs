using Microsoft.AspNetCore.Mvc;

namespace AllerCheck.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { message = "AllerCheck API'ye Hoş Geldiniz!" }); //Todo: API İşlemlerini yapacaksın.
        }
    }
} 
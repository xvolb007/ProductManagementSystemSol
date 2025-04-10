using Microsoft.AspNetCore.Mvc;

namespace ProductManagementSystem.API.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

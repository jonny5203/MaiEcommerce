using Microsoft.AspNetCore.Mvc;

namespace dotnetecommerce.Controllers
{
    public class CategoryController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}
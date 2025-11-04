using Microsoft.AspNetCore.Mvc;

namespace CliCarProject.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

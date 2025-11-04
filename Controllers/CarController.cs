using Microsoft.AspNetCore.Mvc;

namespace CliCarProject.Controllers
{
    public class CarController : Controller
    {
        public IActionResult CarSearch()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace CliCarProject.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserProfile()
        {
            return View();
        }
    }
}

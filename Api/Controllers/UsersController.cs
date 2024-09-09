using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

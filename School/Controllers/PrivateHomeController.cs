using Microsoft.AspNetCore.Mvc;

namespace School.Controllers
{
    public class PrivateHomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
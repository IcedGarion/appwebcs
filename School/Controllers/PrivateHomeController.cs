using Microsoft.AspNetCore.Mvc;

namespace Upo.Controllers
{
    public class PrivateHomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
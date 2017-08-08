using Microsoft.AspNetCore.Mvc;

namespace School.Controllers
{
    public class HomeController : Controller
    {
        //public IActionResult Index() => Redirect(Url.Action("Index", "Students"));

        public IActionResult Index() => View();
        public IActionResult Students() => View();
    }
}
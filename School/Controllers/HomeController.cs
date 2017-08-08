using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using School.Model;

namespace School.Controllers
{
    public class HomeController : Controller
    {
        //public IActionResult Index() => Redirect(Url.Action("Index", "Utente"));

        public IActionResult Index() => View();
    }
}
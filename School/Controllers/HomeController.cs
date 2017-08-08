using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using School.Model;

namespace School.Controllers
{
    public class HomeController : Controller
    {
        //possibile redirect
        //public IActionResult Index() => Redirect(Url.Action("Index", "Utente"));

        //passa subito alla view (Home/index)
        public IActionResult Index() => View();
    }
}
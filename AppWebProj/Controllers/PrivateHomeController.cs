using Microsoft.AspNetCore.Mvc;

namespace Upo.Controllers
{
    public class PrivateHomeController : Controller
    {
        /*
         * Passa semplicemente alla view
         */
        public IActionResult Index() => View();
    }
}
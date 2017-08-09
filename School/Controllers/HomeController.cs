using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using School.Model;
using School.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace School.Controllers
{
    public class HomeController : Controller
    {
        //possibile redirect
        //public IActionResult Index() => Redirect(Url.Action("Index", "Utente"));

        public IActionResult Index() => View();
    }
}
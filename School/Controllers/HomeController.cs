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
        public IActionResult Index()
        {
            SchoolContext context = new SchoolContext();

            //query top 10?
            var query = from prodotti in context.Prodotto select prodotti;

            return View(query.ToList());
        }
    }
}
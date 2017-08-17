using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.Data;
using System.Linq;
using System.Threading.Tasks;

namespace School.Controllers
{
    public class OffertaController : Controller
    {
        public async Task<IActionResult> Index()
        {
            SchoolContext context = new SchoolContext();

            var query = from prodotto in context.Prodotto
                        where prodotto.Sconto > 0
                        select prodotto;

            return View(await query.ToListAsync());
        }
    }
}

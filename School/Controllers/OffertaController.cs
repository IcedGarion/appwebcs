using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Upo.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Upo.Controllers
{
    public class OffertaController : Controller
    {
        public async Task<IActionResult> Index()
        {
            UpoECommerceContext context = new UpoECommerceContext();

            var query = from prodotto in context.Prodotto
                        where prodotto.Sconto > 0
                        select prodotto;

            return View(await query.ToListAsync());
        }
    }
}

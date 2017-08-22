using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Upo.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Upo.Controllers
{
    public class OffertaController : Controller
    {
        /*
         * Espone i prodotti in offerta
         */
        public async Task<IActionResult> Index()
        {
            UpoECommerceContext context = new UpoECommerceContext();

            //prende i prodotti con sconto > 0
            var query = from prodotto in context.Prodotto
                        where prodotto.Sconto > 0
                        select prodotto;

            return View(await query.ToListAsync());
        }
    }
}

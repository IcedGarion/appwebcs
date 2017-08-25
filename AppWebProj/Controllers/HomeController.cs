using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Upo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upo.Controllers
{
    public class HomeController : Controller
    {
        /*
         * Espone i top 10 prodotti piu' venduti del mese
         */
        public async Task<IActionResult> Index()
        {
            UpoECommerceContext context = new UpoECommerceContext();

            //query top 10:
            //prende i prodotti che compaiono in acquisti con data > (data di oggi -3 mesi)
            var query = (from prodotti in context.Prodotto
                         join ordiniProdotti in context.OrdineProdotto on prodotti.CdProdotto equals ordiniProdotti.CdProdotto
                         join ordini in context.Ordine on ordiniProdotti.CdOrdine equals ordini.CdOrdine
                         where ordini.DtInserimento > DateTime.Now.AddMonths(-3)
                         orderby ordiniProdotti.Quantita
                         select prodotti)
                         .GroupBy(p => p.CdProdotto).Select(g => g.First()).Take(10);
            //raggruppa per codice prodotto e prende solo il primo (per evitare duplicati)

            return View(await query.ToListAsync());
        }
    }
}
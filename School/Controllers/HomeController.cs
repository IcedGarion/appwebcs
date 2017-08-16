using Microsoft.AspNetCore.Mvc;
using School.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace School.Controllers
{
    public class HomeController : Controller
    {
        private static readonly int MIN_ORDERS = 5;

        public IActionResult Index()
        {
            SchoolContext context = new SchoolContext();

            //query top 10:
            //prende i prodotti che compaiono in acquisti con data > (data di oggi -3 mesi)
            var query = (from prodotti in context.Prodotto
                         join ordiniProdotti in context.OrdineProdotto on prodotti.CdProdotto equals ordiniProdotti.CdProdotto
                         join ordini in context.Ordine on ordiniProdotti.CdOrdine equals ordini.CdOrdine
                         where ordini.DtInserimento > DateTime.Now.AddMonths(-3)
                         orderby ordiniProdotti.Quantita
                         select prodotti)
                         .GroupBy(p => p.CdProdotto).Select(g => g.First());

            //se non ci sono abbastanza prodotti toglie il filtro data
            if(query.Count() >= MIN_ORDERS)
            {
                query = (from prodotti in context.Prodotto
                        join ordiniProdotti in context.OrdineProdotto on prodotti.CdProdotto equals ordiniProdotti.CdProdotto
                        join ordini in context.Ordine on ordiniProdotti.CdOrdine equals ordini.CdOrdine
                        orderby ordiniProdotti.Quantita
                        select prodotti)
                        .GroupBy(p => p.CdProdotto).Select(g => g.First());
            }

            return View(query.ToList());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using School.Controllers;
using School.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using School.Model;

namespace School.Controllers
{
    public class ProdottoController : CrudController<SchoolContext, int, Prodotto>
    {
        public ProdottoController(SchoolContext context, ILogger<ProdottoController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Prodotto> Entities => Context.Prodotto;

        protected override Func<Prodotto, int, bool> FilterById => (e, id) => e.CdProdotto == id;

        //passa alla view la lista di tutte le entites del controller (Context.Prodotto)
        //public async Task<IActionResult> List() => View(await Entities.ToListAsync());

        public IActionResult List(int cdprodotto)
        {
            var query = from prodotti in Context.Prodotto
                        where prodotti.CdProdotto.Equals(cdprodotto)
                        select prodotti;

            return View(query.ToList());
        }

        [HttpPost]
        public IActionResult Find(string input)
        {
            var query = (from prodotti in Context.Prodotto
                        where prodotti.Titolo.Contains(input) || prodotti.Descrizione.Contains(input)
                        select prodotti).OrderByDescending(x => x.Prezzo);

            return View(query.ToList());
        }

        public async Task<IActionResult> Index() => View(await Entities.ToListAsync());

    }
}
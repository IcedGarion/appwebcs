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
        public async Task<IActionResult> Index() => View(await Entities.ToListAsync());

    }
}
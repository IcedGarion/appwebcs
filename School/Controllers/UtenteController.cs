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
    public class UtenteController : CrudController<SchoolContext, int, Utente>
    {
        public UtenteController(SchoolContext context, ILogger<UtenteController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Utente> Entities => Context.Utente;

        protected override Func<Utente, int, bool> FilterById => (e, id) => e.CdUtente == id;

        //passa alla view la lista di tutte le entites del controller (Context.Utente)
        //Utente/Index: in @Model si trova la lista
        public async Task<IActionResult> Index() => View(await Entities.ToListAsync());

    }
}
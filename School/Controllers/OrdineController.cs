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
    public class OrdineController : CrudController<SchoolContext, int, Ordine>
    {
        public OrdineController(SchoolContext context, ILogger<OrdineController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Ordine> Entities => Context.Ordine;

        protected override Func<Ordine, int, bool> FilterById => (e, id) => e.CdOrdine == id;

        //passa alla view la lista di tutte le entites del controller (Context.Ordine)
        public IActionResult List()
        {
            SchoolContext context = new SchoolContext();

            //query separata dal suo controller per restituire elenco utenti (formattati nel loro datasource)
            //perche' in View va specificato il @Model IEnumerable<UtentiDataSource>
            //var query = from utenti in context.Utente select new UtentiDataSource { Username = utenti.Username, Password = utenti.Password };


            var query = from ordini in context.Ordine
                        join utenti in context.Utente on ordini.CdUtente equals utenti.CdUtente
                        join ordineProdotto in context.OrdineProdotto on ordini.CdOrdine equals ordineProdotto.CdOrdine
                        join prodotti in context.Prodotto on ordineProdotto.CdProdotto equals prodotti.CdProdotto
                        select new OrdiniDataSource { CdOrdine = ordini.CdOrdine, Username = utenti.Username, Titolo = prodotti.Titolo, Totale = ordini.Totale };

            return View(query.ToList());
        }

        public IActionResult Index() => Redirect("/Ordine/List");

    }
}
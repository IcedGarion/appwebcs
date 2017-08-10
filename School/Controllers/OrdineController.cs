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
using Microsoft.AspNetCore.Http;

namespace School.Controllers
{
    public class OrdineController : CrudController<SchoolContext, int, Ordine>
    {
        public OrdineController(SchoolContext context, ILogger<OrdineController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Ordine> Entities => Context.Ordine;

        protected override Func<Ordine, int, bool> FilterById => (e, id) => e.CdOrdine == id;

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Ordine());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Ordine ord)
        {
            await base.Create(ord);

            return Redirect("/Ordine");
        }

        //passa alla view la lista di tutte le entites del controller (Context.Ordine)
        public IActionResult List()
        {
            SchoolContext context = new SchoolContext();

            //invece di restituire solo gli ordini, fa una join per aggiungere altre informazioni
            var query = from ordini in context.Ordine
                        join utenti in context.Utente on ordini.CdUtente equals utenti.CdUtente
                        join ordineProdotto in context.OrdineProdotto on ordini.CdOrdine equals ordineProdotto.CdOrdine
                        join prodotti in context.Prodotto on ordineProdotto.CdProdotto equals prodotti.CdProdotto
                        select new OrdiniJoinDataSource { CdOrdine = ordini.CdOrdine, Username = utenti.Username, Titolo = prodotti.Titolo, Totale = ordini.Totale };

            return View(query.ToList());
        }

        public IActionResult Add(string prodotto, int qta)
        {
            List<OrdineProdotto> carrello;

            Int32.TryParse(prodotto, out int cdprodotto);

            //controlla se c'e' gia' qualche prodotto nel carrello in session:
            var exCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            if (exCart == null)
            {
                //crea nuovo carrello aggiungendo il primo prodotto
                carrello = new List<OrdineProdotto>();
            }
            //se invece esiste gia', accoda un nuovo prodotto alla lista
            else
            {
                //aggiunge al carrello (in session) il prodotto
                carrello = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");    
            }

            carrello.Add(new OrdineProdotto { CdProdotto = cdprodotto, Quantita = qta });
            HttpContext.Session.SetObjectAsJson("Cart", carrello);

            //controllo
            //var session = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");

            return Redirect("/Home/Index");//redir to cart page
        }

        public IActionResult Index() => Redirect("/Ordine/List");

    }
}
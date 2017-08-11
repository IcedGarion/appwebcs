using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> Create(Ordine ord)
        {
            SchoolContext context = new SchoolContext();

            //legge il carrello
            var SessionCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            if (SessionCart == null)
            {
                return Redirect("/Carrello/Index");
            }

            //legge cdUtente
#warning nullable int
            int utente = 0;
            if (HttpContext.Session.GetInt32("CdUtente") != null)
                utente = (int)HttpContext.Session.GetInt32("CdUtente");

            //cerca nel db tutti i prodotti da inserire, per calcolare il totale
            var AddProductsDb = from prodotti in context.Prodotto
                              join carrello in SessionCart on prodotti.CdProdotto equals carrello.CdProdotto
                              select new { Prezzo = prodotti.Prezzo, Sconto = prodotti.Sconto, Quantita = carrello.Quantita };

            double totale = AddProductsDb.Sum(x => (x.Prezzo - x.Sconto) * x.Quantita);

            //crea elenco ordineProdotto
            List<OrdineProdotto> ordProd = new List<OrdineProdotto>();

            foreach (var prod in SessionCart)
            {
                ordProd.Add(new OrdineProdotto
                {
                    CdProdotto = prod.CdProdotto,
                    Quantita = prod.Quantita
                });
            }

            //crea un nuovo ordine collegato all'utente e all'elenco di prodotti sopra
            Ordine ordine = new Ordine
            {
                CdUtente = utente,
                Stato = "Sent",
                DtInserimento = DateTime.Now,
                Totale = totale,
                OrdineProdotto = ordProd
            };

            //salva sul db
            await base.Create(ordine);

            //rimuove carrello in session
            HttpContext.Session.Remove("Cart");

            return Redirect("/Ordine/List");
        }

        //passa alla view la lista di tutte le entites del controller (Context.Ordine)
        public IActionResult List()
        {
            SchoolContext context = new SchoolContext();

            //prende cdUtente da session
            var tmp = HttpContext.Session.GetInt32("CdUtente");
            var ruolo = HttpContext.Session.GetString("Ruolo");

#warning da togliere dopo autenticazione
            if (tmp == null)
            {
                //se non sei loggato, lista vuota
                return View(new List<OrdiniJoinDataSource>());
            }

            int CdUtente = (int)tmp;

            if (ruolo.Equals("user"))
            {
                //invece di restituire solo gli ordini, fa una join per aggiungere altre informazioni
                var query = from ordini in context.Ordine
                            join utenti in context.Utente on ordini.CdUtente equals utenti.CdUtente
                            join ordineProdotto in context.OrdineProdotto on ordini.CdOrdine equals ordineProdotto.CdOrdine
                            join prodotti in context.Prodotto on ordineProdotto.CdProdotto equals prodotti.CdProdotto
                            where utenti.CdUtente.Equals(CdUtente)
                            select new OrdiniJoinDataSource
                            {
                                CdOrdine = ordini.CdOrdine,
                                Username = utenti.Username,
                                Titolo = prodotti.Titolo,
                                DtInserimento = ordini.DtInserimento,
                                Quantita = ordineProdotto.Quantita,
                                Totale = ordini.Totale
                            };

                return View(query.ToList());
            }
            else
            {
                return Redirect("/Ordine/WholeList");
            }
        }

        //espone tutti gli ordini di tutti gli utenti
        public IActionResult WholeList()
        {
            SchoolContext context = new SchoolContext();

            //fa una join per aggiungere altre informazioni
            var query = from ordini in context.Ordine
                        join utenti in context.Utente on ordini.CdUtente equals utenti.CdUtente
                        join ordineProdotto in context.OrdineProdotto on ordini.CdOrdine equals ordineProdotto.CdOrdine
                        join prodotti in context.Prodotto on ordineProdotto.CdProdotto equals prodotti.CdProdotto
                        select new OrdiniJoinDataSource
                        {
                            CdOrdine = ordini.CdOrdine,
                            Stato = ordini.Stato,
                            Username = utenti.Username,
                            Titolo = prodotti.Titolo,
                            DtInserimento = ordini.DtInserimento,
                            Quantita = ordineProdotto.Quantita,
                            Totale = ordini.Totale
                        };

            return View(query.ToList());
        }

        public IActionResult Index() => Redirect("/Ordine/List");

    }
}
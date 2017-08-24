using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Upo.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Upo.Model;
using Microsoft.AspNetCore.Http;

namespace Upo.Controllers
{
    public class OrdineController : CrudController<UpoECommerceContext, int, Ordine>
    {
        public OrdineController(UpoECommerceContext context, ILogger<OrdineController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Ordine> Entities => Context.Ordine;

        protected override Func<Ordine, int, bool> FilterById => (e, id) => e.CdOrdine == id;

        /*
         * Crea un nuovo ordine (CHECKOUT)
         */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            UpoECommerceContext context = new UpoECommerceContext();

            //Se non si e' loggati redirige alla login, quando si tenta di acquistare
            if(HttpContext.Session.GetInt32("CdUtente") == null)
            {
                return Redirect("/Utente/Login");
            }

            //legge il carrello da session
            var SessionCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            if (SessionCart == null)
            {
                //se il carrello e' vuoto non si puo' eseguire l'acquisto: rimanda alla pagina del carrello
                return Redirect("/Carrello/Index");
            }

            //legge cdUtente da session
            int utente = 0;
            var tmp = HttpContext.Session.GetInt32("CdUtente");
            if (tmp != null)
                utente = (int)tmp;

            //JOIN dei prodotti in carrello con quelli nel db, per ritrovarne tutte le informazioni (es, per calcolo totale)
            var AddProductsDb = from prodotti in context.Prodotto
                              join carrello in SessionCart on prodotti.CdProdotto equals carrello.CdProdotto
                              select new { Prezzo = prodotti.Prezzo, Sconto = prodotti.Sconto, Quantita = carrello.Quantita };

            //calcola il totale sommando tutti i prezzi scontati dei prodotti nel carrello
            double totale = AddProductsDb.Sum(x => (x.Prezzo - x.Sconto) * x.Quantita);

            //crea tabella di link fra ordini e prodotti
            List<OrdineProdotto> ordProd = new List<OrdineProdotto>();

            //aggiunge tutti i prodotti da acquistare con relativa quantita'
            foreach (var prod in SessionCart)
            {
                ordProd.Add(new OrdineProdotto
                {
                    CdProdotto = prod.CdProdotto,
                    Quantita = prod.Quantita
                });
            }

            //crea un nuovo ordine collegato all'utente e collega l'elenco di prodotti OrdineProdotto
            Ordine ordine = new Ordine
            {
                CdUtente = utente,
                Stato = "Sent",                 //di default un nuovo ordine assume stato "sent"
                DtInserimento = DateTime.Now,   //inserisce con la data corrente
                Totale = totale,
                OrdineProdotto = ordProd
            };

            //rende persistenti le modifiche chiamando il metodo Create del CrudController
            await base.Create(ordine);

            //rimuove carrello in session
            HttpContext.Session.Remove("Cart");

            return Redirect("/Ordine/Index");
        }

        /*
         * Modifica proprieta' di un ordine (ADMIN)
         */
        [HttpPost]
        public async Task<IActionResult> Update(string ordine, string stato)
        {
            //riceve parametri dal form
            Int32.TryParse(ordine, out int CdOrdine);
            Ordine ToUpdate;

            //cerca nel db l'ordine con codice corrispondente a quello passato dal form
            var query = from ordini in Context.Ordine
                        where ordini.CdOrdine.Equals(CdOrdine)
                        select ordini;

            //prende il primo elemento (l'unico) della query
            ToUpdate = query.First();

            //modifica stato solo se diverso dal precedente
            if (!ToUpdate.Stato.Equals(stato))
            {
                ToUpdate.Stato = stato;

                //rende persistente chiamando il metodo Update del CrudController
                await base.Update(ToUpdate);
            }

            return Redirect("/Ordine/List");
        }


        /*
         * Espone tutti gli ordini di un utente (SOLO USER). Possibilita' di filtrare gli ordini: 
         * se chiamato con parametri (HTTP GET con parametri nell'URL) allora usa i parametri per filtrare
         */
        public async Task<IActionResult> Index(string clear, string start, string end,
            string titolo, string qtaoperator, string qta, string totoperator, string tot, string stato)
        {
            int CdUtente = 0;
            //prende cdUtente da session
            var tmp = HttpContext.Session.GetInt32("CdUtente");
            if(tmp != null)
                CdUtente = (int)tmp;
            //query: tutti gli ordini di un certo utente
            var Query = UserQuery(CdUtente);
            bool filtered = false;

            //FILTRO: custom IQueryable extension method
            Query = Query.FilterOrder(ref filtered, clear, start, end, titolo, qtaoperator, qta, totoperator, tot, stato);

            //view deve sapere se e' stato applicato un filtro o no: salva in Request scope
            TempData["OrdineFilter"] = filtered.ToString();

            return View(await Query.ToListAsync());
        }

        /*
         * Espone tutti gli ordini di tutti gli utenti (SOLO ADMIN). Possibilita' di filtrare, logica di prima
         */
        [HttpGet]
        public async Task<IActionResult> List(string clear, string start, string end,
            string titolo, string qtaoperator, string qta, string totoperator, string tot, string stato)
        {
            //prende tutti gli ordini di tutti gli utenti
            var Query = AdminQuery();
            bool filtered = false;

            //FILTRO: custom IQueryable extension method
            Query = Query.FilterOrder(ref filtered, clear, start, end, titolo, qtaoperator, qta, totoperator, tot, stato);

            TempData["OrdineFilter"] = filtered.ToString();

            return View(await Query.ToListAsync());
        }


        /*
         * Usata da List: restituisce tutti gli ordini di tutti gli utenti, in JOIN con i prodotti che contiene
         *     e username di chi ha acquistato
         */
        private IQueryable<OrdiniJoinDataSource> AdminQuery()
        {
            var q = from ordini in Context.Ordine
                    join utenti in Context.Utente on ordini.CdUtente equals utenti.CdUtente
                    join ordineProdotto in Context.OrdineProdotto on ordini.CdOrdine equals ordineProdotto.CdOrdine
                    join prodotti in Context.Prodotto on ordineProdotto.CdProdotto equals prodotti.CdProdotto
                    select new OrdiniJoinDataSource
                    {
                        CdOrdine = ordini.CdOrdine,
                        Stato = ordini.Stato,
                        Username = utenti.Username,
                        Titolo = prodotti.Titolo,
                        CdProdotto = prodotti.CdProdotto,
                        DtInserimento = ordini.DtInserimento,
                        Quantita = ordineProdotto.Quantita,
                        Totale = ordini.Totale
                    };

            return q;
        }

        /*
         * Usata da Index: restituisce tutti gli ordini effettuati da un utente specifico,
         *     con anche tutti i prodotti acquistati
         */
        private IQueryable<OrdiniJoinDataSource> UserQuery(int CdUtente)
        {
            //invece di restituire solo gli ordini, fa una join per aggiungere altre informazioni
            var q = from ordini in Context.Ordine
                    join utenti in Context.Utente on ordini.CdUtente equals utenti.CdUtente
                    join ordineProdotto in Context.OrdineProdotto on ordini.CdOrdine equals ordineProdotto.CdOrdine
                    join prodotti in Context.Prodotto on ordineProdotto.CdProdotto equals prodotti.CdProdotto
                    where utenti.CdUtente.Equals(CdUtente)
                    select new OrdiniJoinDataSource
                    {
                        CdOrdine = ordini.CdOrdine,
                        Stato = ordini.Stato,
                        Username = utenti.Username,
                        Titolo = prodotti.Titolo,
                        CdProdotto = prodotti.CdProdotto,
                        DtInserimento = ordini.DtInserimento,
                        Quantita = ordineProdotto.Quantita,
                        Totale = ordini.Totale
                    };

            return q;
        }
    }
}
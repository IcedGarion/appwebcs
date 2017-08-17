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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            UpoECommerceContext context = new UpoECommerceContext();

            //Se non si e' loggati redirige alla login, quando si tenta di acquistare
            if(HttpContext.Session.GetInt32("CdUtente") == null)
            {
                return Redirect("/Utente/Login");
            }

            //legge il carrello
            var SessionCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            if (SessionCart == null)
            {
                return Redirect("/Carrello/Index");
            }

            //legge cdUtente
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

            return Redirect("/Ordine/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string ordine, string stato)
        {
            //riceve parametri dal form
            Int32.TryParse(ordine, out int CdOrdine);
            Ordine ToUpdate;

            //cerca nel db quell'ordine
            var query = from ordini in Context.Ordine
                        where ordini.CdOrdine.Equals(CdOrdine)
                        select ordini;

            //prende il primo elemento (l'unico) della query
            ToUpdate = query.First();

            //modifica stato solo se diverso!
            if (!ToUpdate.Stato.Equals(stato))
            {
                ToUpdate.Stato = stato;

                //salva su db
                await base.Update(ToUpdate);
            }

            return Redirect("/Ordine/List");
        }


        //FILTRA
        public async Task<IActionResult> Index(string clear, string start, string end,
            string titolo, string qtaoperator, string qta, string totoperator, string tot, string stato)
        {
            //prende cdUtente da session
            var tmp = HttpContext.Session.GetInt32("CdUtente");
            var ruolo = HttpContext.Session.GetString("Ruolo");
            int CdUtente = (int)tmp;
            var Query = UserQuery(CdUtente);
            bool filtered = false;

            filtered = Filter(ref Query, clear, start, end, titolo, qtaoperator, qta, totoperator, tot, stato);

            TempData["OrdineFilter"] = filtered.ToString();

            return View(await Query.ToListAsync());
        }

        /***
         * GET senza parametri: lista normale; GET con parametro: filtra secondo i parametri
         ***/
        [HttpGet]
        public async Task<IActionResult> List(string clear, string start, string end,
            string titolo, string qtaoperator, string qta, string totoperator, string tot, string stato)
        {
            var Query = AdminQuery();
            bool filtered = false;

            filtered = Filter(ref Query, clear, start, end, titolo, qtaoperator, qta, totoperator, tot, stato);

            TempData["OrdineFilter"] = filtered.ToString();

            return View(await Query.ToListAsync());
        }

        private bool Filter(ref IQueryable<OrdiniJoinDataSource> Query, string clear, string start, string end,
            string titolo, string qtaoperator, string qta, string totoperator, string tot, string stato)
        {
            DateTime.TryParse("1/1/1754", out DateTime MIN);
            DateTime.TryParse("12/31/9998", out DateTime MAX);
            bool filtered = false;

            //se c'e' clear, non fa niente
            if (clear == null)
            {
                if (start != null && end != null)
                {
                    try
                    {
                        DateTime Start = DateTime.Parse(start);
                        DateTime End = DateTime.Parse(end);
       
                        if((Start.CompareTo(MIN) > 0 && Start.CompareTo(MAX) < 0)
                            && (End.CompareTo(MIN) > 0 && End.CompareTo(MAX) < 0))
                        {
                            Query = Query.Where(ordine => ordine.DtInserimento >= Start && ordine.DtInserimento <= End);
                        }
                    }
                    catch(Exception)
                    { }

                    filtered = true;
                }

                if (titolo != null && !titolo.Equals(""))
                {
                    Query = Query.Where(ordine => ordine.Titolo.Contains(titolo));
                    filtered = true;
                }

                if (qtaoperator != null && qta != null)
                {
                    double.TryParse(qta, out double Qta);

                    switch (qtaoperator)
                    {
                        case "<":
                            Query = Query.Where(ordine => ordine.Quantita < Qta);
                            break;
                        case "<=":
                            Query = Query.Where(ordine => ordine.Quantita <= Qta);
                            break;
                        case ">":
                            Query = Query.Where(ordine => ordine.Quantita > Qta);
                            break;
                        case ">=":
                            Query = Query.Where(ordine => ordine.Quantita >= Qta);
                            break;
                        case "=":
                            Query = Query.Where(ordine => ordine.Quantita == Qta);
                            break;
                        default:
                            break;
                    }
                    filtered = true;
                }

                if (totoperator != null && tot != null)
                {
                    double.TryParse(tot, out double Tot);

                    switch (totoperator)
                    {
                        case "<":
                            Query = Query.Where(ordine => ordine.Totale < Tot);
                            break;
                        case "<=":
                            Query = Query.Where(ordine => ordine.Totale <= Tot);
                            break;
                        case ">":
                            Query = Query.Where(ordine => ordine.Totale > Tot);
                            break;
                        case ">=":
                            Query = Query.Where(ordine => ordine.Totale >= Tot);
                            break;
                        case "=":
                            Query = Query.Where(ordine => ordine.Totale == Tot);
                            break;
                        default:
                            break;
                    }
                    filtered = true;
                }

                if (stato != null && !stato.Equals(""))
                {
                    Query = Query.Where(ordine => ordine.Stato.Equals(stato));
                    filtered = true;
                }
            }

            return filtered;
        }

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
                        DtInserimento = ordini.DtInserimento,
                        Quantita = ordineProdotto.Quantita,
                        Totale = ordini.Totale
                    };

            return q;
        }

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
                        DtInserimento = ordini.DtInserimento,
                        Quantita = ordineProdotto.Quantita,
                        Totale = ordini.Totale
                    };
            return q;
        }

    }
}
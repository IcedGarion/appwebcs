using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Upo.Controllers;
using Upo.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Upo.Model;

namespace Upo.Controllers
{
    public class ProdottoController : CrudController<UpoECommerceContext, int, Prodotto>
    {
        public ProdottoController(UpoECommerceContext context, ILogger<ProdottoController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Prodotto> Entities => Context.Prodotto;

        protected override Func<Prodotto, int, bool> FilterById => (e, id) => e.CdProdotto == id;

        //solo per admin
        [HttpPost]
        public async Task<IActionResult> Update(string previousUrl, string prodotto, string prezzo, string sconto, string disponibile)
        {
            Prodotto ToUpdate;
            string PreviousURL = previousUrl;

            //riceve parametri dal form
            Int32.TryParse(prodotto, out int CdProdotto);
            double.TryParse(prezzo, out double Prezzo);
            double.TryParse(sconto, out double Sconto);

            //cerca nel db quel prodotto
            var query = from prodotti in Context.Prodotto
                        where prodotti.CdProdotto.Equals(CdProdotto)
                        select prodotti;

            //prende il primo elemento (l'unico) della query
            ToUpdate = query.First();

            //modifica tutto (solo se ci sono stati cambiamenti)
            if ((!(ToUpdate.Prezzo == Prezzo)) || (!(ToUpdate.Sconto == Sconto)) || (!(ToUpdate.Disponibile.Equals(disponibile))))
            {
                ToUpdate.Prezzo = Prezzo;
                ToUpdate.Sconto = Sconto;
                ToUpdate.Disponibile = disponibile;

                //salva su db
                await base.Update(ToUpdate);
            }

            return Redirect(PreviousURL);
        }

        //solo per admin: pagina con elenco di tutti i prodotti ma solo ADMIN: pulsanti MODIFICA PRODOTTO
        public async Task<IActionResult> List() => View(await Entities.ToListAsync());

        //pagina con elenco di tutti i prodotti: pulsante aggiungi per USER, no pulsanti per ADMIN
        public async Task<IActionResult> Index() => View(await Entities.ToListAsync());


        public async Task<IActionResult> Detail(int cdprodotto)
        {
            var query = from prodotti in Context.Prodotto
                        where prodotti.CdProdotto.Equals(cdprodotto)
                        select prodotti;

            return View(await query.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Find(string input)
        {
            var query = (from prodotti in Context.Prodotto
                        where prodotti.Titolo.Contains(input) || prodotti.Descrizione.Contains(input)
                        select prodotti).OrderByDescending(x => x.Prezzo);

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Advanced(string apply, string clear, string titolo, string prezzooperator, string prezzo,
            string sconto, string disp)
        {
            var Query = from prodotti in Context.Prodotto
                        select prodotti;
        
            bool filtered = false;

            if(apply != null)
            {
                filtered = true;
            }

            //se c'e' clear, non fa niente
            if (clear == null)
            {
                if (titolo != null && !titolo.Equals(""))
                {
                    Query = Query.Where(prod => prod.Titolo.Contains(titolo));
                    filtered = true;
                }

                if (prezzooperator != null && prezzo != null)
                {
                    double.TryParse(prezzo, out double Prezzo);

                    switch (prezzooperator)
                    {
                        case "<":
                            Query = Query.Where(prod => prod.Prezzo < Prezzo);
                            break;
                        case "<=":
                            Query = Query.Where(prod => prod.Prezzo <= Prezzo);
                            break;
                        case ">":
                            Query = Query.Where(prod => prod.Prezzo > Prezzo);
                            break;
                        case ">=":
                            Query = Query.Where(prod => prod.Prezzo >= Prezzo);
                            break;
                        case "=":
                            Query = Query.Where(prod => prod.Prezzo == Prezzo);
                            break;
                        default:
                            break;
                    }

                    filtered = true;
                }

                if(sconto != null && !sconto.Equals(""))
                {

                    if (sconto.Equals("si"))
                    {
                        Query = Query.Where(prod => prod.Sconto > 0);
                    }
                    else
                    {
                        Query = Query.Where(prod => prod.Sconto == 0);
                    }

                    filtered = true;
                }

                if (disp != null && !disp.Equals(""))
                {
                    Query = Query.Where(ordine => ordine.Disponibile.Equals(disp));
                    filtered = true;
                }
            }
            
            TempData["AdvancedFilter"] = filtered.ToString();

            return View(await Query.ToListAsync());
        }
    }
}
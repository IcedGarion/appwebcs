using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        /*
         * Modifica informazioni sul prodotto (SOLO ADMIN)
         */
        [HttpPost]
        public async Task<IActionResult> Update(string previousUrl, string prodotto, string prezzo, string sconto, string disponibile)
        {
            Prodotto ToUpdate;

            //riceve parametri dal form: codice prodotto, nuovo prezzo nuovo sconto, nuova disponibilita'
            Int32.TryParse(prodotto, out int CdProdotto);
            double.TryParse(prezzo, out double Prezzo);
            double.TryParse(sconto, out double Sconto);

            //cerca nel db il prodotto da modificare
            var query = from prodotti in Context.Prodotto
                        where prodotti.CdProdotto.Equals(CdProdotto)
                        select prodotti;

            //prende il primo elemento (l'unico) della query
            ToUpdate = query.First();

            //modifica tutte le informazioni (solo se ci sono stati cambiamenti)
            //i campi del form non possono essere lasciati vuoti e i campi numerici devono contenere numeri, pertanto
            //si suppone che, a questo punto, i dati inseriti siano validi
            if ((!(ToUpdate.Prezzo == Prezzo)) || (!(ToUpdate.Sconto == Sconto)) || (!(ToUpdate.Disponibile.Equals(disponibile))))
            {
                ToUpdate.Prezzo = Prezzo;
                ToUpdate.Sconto = Sconto;
                ToUpdate.Disponibile = disponibile;

                //rende persistenti le modifiche
                await base.Update(ToUpdate);
            }

            return Redirect("/Prodotto/List");
        }

        /*
         * Redirigono nelle pagine di elenco di tutti i prodotti
         */

        //solo per admin: pagina con i prodotti e pulsanti MODIFICA PRODOTTO
        public async Task<IActionResult> List() => View(await Entities.ToListAsync());

        //pagina con i prodotti e pulsante aggiungi per USER, no pulsanti per ADMIN
        public async Task<IActionResult> Index() => View(await Entities.ToListAsync());

        /*
         * Seleziona uno specifico prodotto e tutte le sue informazioni (codice prodotto nell'URL)
         */
        public async Task<IActionResult> Detail(int cdprodotto)
        {
            //prende il prodotto che ha codice corrispondente al parametro 
            var query = from prodotti in Context.Prodotto
                        where prodotti.CdProdotto.Equals(cdprodotto)
                        select prodotti;

            return View(await query.ToListAsync());
        }

        /*
         * Seleziona l'insieme di prodotti il cui titolo o la cui descrizione contiene la stringa in input
         */
        [HttpPost]
        public async Task<IActionResult> Find(string input)
        {
            var query = (from prodotti in Context.Prodotto
                        where prodotti.Titolo.Contains(input) || prodotti.Descrizione.Contains(input)
                        select prodotti).OrderByDescending(x => x.Prezzo);

            return View(await query.ToListAsync());
        }

        /*
         * Ricerca avanzata: usa lo stesso filtro di ordini e utenti, applicato ai prodotti.
         * Filtra, fra tutti i prodotti, quelli che rispecchiano le caratteristiche indicate in input (titolo, prezzo, sconto, disponibilita')
         */
        public async Task<IActionResult> Advanced(string apply, string clear, string titolo, string prezzooperator, string prezzo,
            string sconto, string disp)
        {
            //seleziona tutti i prodotti
            var Query = from prodotti in Context.Prodotto
                        select prodotti;
        
            bool filtered = false;

            if(apply != null)
            {
                filtered = true;
            }

            //FILTRO: custom IQueryable extension method
            Query = Query.FilterProd(ref filtered, clear, titolo, disp, prezzooperator, prezzo, sconto);
            
            TempData["AdvancedFilter"] = filtered.ToString();

            return View(await Query.ToListAsync());
        }
    }
}
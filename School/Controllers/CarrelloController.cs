using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using School.Model;
using School.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace School.Controllers
{
    public class CarrelloController : Controller
    {
        //mostra lo stato del carrello
        public IActionResult Index()
        {
            SchoolContext context = new SchoolContext();

            //legge codice prodotti in session e li recupera dal db
            var SessionCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");

            if (SessionCart == null)
                return View(new List<CarrelloDataSource>());

            //join fra prodotti in db e quelli nel carrello
            var query = from prodotti in context.Prodotto
                        join carrello in SessionCart on prodotti.CdProdotto equals carrello.CdProdotto
                        select new CarrelloDataSource
                        {
                            CdProdotto = prodotti.CdProdotto,
                            Titolo = prodotti.Titolo,
                            Descrizione = prodotti.Descrizione,
                            Prezzo = prodotti.Prezzo,
                            Sconto = prodotti.Sconto,
                            Immagine = prodotti.Immagine,
                            Quantita = carrello.Quantita
                        };

            return View(query.ToList());
        }

        //aggiunge un prodotto al carrello
        [HttpPost]
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

            return Redirect("/Carrello/Index");
        }

        [HttpPost]
        public IActionResult Remove(string prodotto)
        {
            Int32.TryParse(prodotto, out int cdprodotto);

            //legge il carrello
            var SessionCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            
            //rimuove il prodotto
            SessionCart.RemoveAll(x => x.CdProdotto == cdprodotto);

            //se ora il carrello e' vuoto, elimina oggetto in session
            if (SessionCart.Count() == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetObjectAsJson("Cart", SessionCart);
            }

            return Redirect("/Carrello/Index");
        }

        [HttpPost]
        public IActionResult Buy()
        {
            //legge il carrello
            var SessionCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            if(SessionCart == null)
            {
                return Redirect("/Carrello/Index");
            }

            //inserisce un nuovo ordine e un nuovo ordineprodotto


            return Redirect("/Ordini/List");
        }
    }
}
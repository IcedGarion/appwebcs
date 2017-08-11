using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
            List<OrdineProdotto> newCart;

            Int32.TryParse(prodotto, out int cdprodotto);

            //controlla se c'e' gia' qualche prodotto nel carrello in session:
            var exCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            if (exCart == null)
            {
                //se non esiste, crea nuovo carrello aggiungendo il primo prodotto
                newCart = new List<OrdineProdotto>();
                newCart.Add(new OrdineProdotto { CdProdotto = cdprodotto, Quantita = qta });
            }
            //se invece esiste gia' un carrello:
            else
            {
                newCart = exCart;

                //cerca se esiste gia' il prodotto nella lista
                var prod = newCart.FirstOrDefault(p => p.CdProdotto == cdprodotto);
                if (prod == null)
                {
                    newCart.Add(new OrdineProdotto { CdProdotto = cdprodotto, Quantita = qta });
                }
                //se esiste, incrementa la quantita'
                else
                {
                    newCart.Remove(prod);
                    newCart.Add(new OrdineProdotto { CdProdotto = cdprodotto, Quantita = prod.Quantita + qta });
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", newCart);

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

        [HttpGet]
        public IActionResult Empty()
        {
            HttpContext.Session.Remove("Cart");

            return Redirect("/Carrello/Index");
        }

        [HttpPost]
        public IActionResult Update(string prodotto, int qta)
        {
            Int32.TryParse(prodotto, out int cdprodotto);

            //prende il carrello in session:
            var SessionCart = HttpContext.Session.GetObjectFromJson<List<OrdineProdotto>>("Cart");
            if (SessionCart != null)
            {
                //cerca il prodotto nella lista
                var prod = SessionCart.FirstOrDefault(p => p.CdProdotto == cdprodotto);
                if (prod != null)
                //modifica la quantita'
                {
                    SessionCart.Remove(prod);

                    if (qta != 0)
                    {
                        SessionCart.Add(new OrdineProdotto { CdProdotto = cdprodotto, Quantita = qta });
                        HttpContext.Session.SetObjectAsJson("Cart", SessionCart);
                    }
                    //se quantita == 0 non reinserisce il prodotto
                    else
                    {
                        //se ora il carrello e' vuoto, elimina oggetto in session
                        if (SessionCart.Count() == 0)
                        {
                            HttpContext.Session.Remove("Cart");
                        }
                        else
                        {
                            HttpContext.Session.SetObjectAsJson("Cart", SessionCart);
                        }
                    }
                }
            }

            return Redirect("/Carrello/Index");
        }
    }
}
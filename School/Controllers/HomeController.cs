using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using School.Model;
using School.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace School.Controllers
{
    public class HomeController : Controller
    {
        //possibile redirect
        //public IActionResult Index() => Redirect(Url.Action("Index", "Utente"));

        public IActionResult Index()
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
    }
}
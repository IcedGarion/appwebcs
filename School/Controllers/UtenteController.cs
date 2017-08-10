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
    public class UtenteController : CrudController<SchoolContext, int, Utente>
    {
        public UtenteController(SchoolContext context, ILogger<UtenteController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Utente> Entities => Context.Utente;

        protected override Func<Utente, int, bool> FilterById => (e, id) => e.CdUtente == id;

        /* PATTERN MVC PER I FORM:
         * la pagina Utente/Create crea un nuovo utente partendo da un form nella View.
         * per prima cosa la richiesta passa per il controller, che passa alla view il Model, in modo che la view possa collegare il form ai campi giusti
         * quando l'esecuzione passa alla view e il form viene riempito, viene richiamato lo stesso metodo Create precedente,
         * ma viene scelto l'override che prende un Model come parametro, e accetta HTTP POST (secondo metodo qua sotto)
         * questo metodo usa i dati ricavati dal form e mappati nell'oggetto Model (Utente) e richiama il metodo del CrudController Create
         * che salva su db. 
         * 
         * Si possono fare diverse operazioni nella seconda Create, per esempio modificare o validare i dati prima di salvare
         **/
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Utente());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Utente usr)
        {
            //controlla se le password sono uguali e se non esiste gia' username
            await base.Create(usr);

            return Redirect("/Utente");
        }

        [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        public IActionResult Login(string user, string pass)
        {
            Utente loggato;
            //controlla se esiste utente con quella password
            var query = from utenti in Context.Utente
                         where utenti.Username.Equals(user) && utenti.Password.Equals(pass)
                         select utenti;

            //se trova un risultato, salva in session codice e ruolo
            if(query.Count() == 1)
            {
                loggato = query.ToList()[0];

                //SALVA IN SESSION DATI LOGIN
                HttpContext.Session.SetInt32("CdUtente", loggato.CdUtente);
                HttpContext.Session.SetString("Ruolo", loggato.Stato == null? "none" : loggato.Stato);

                return Redirect("/Home");
            }
            //altrimenti rimanda alla login con messaggio
            else
            {

                HttpContext.Session.SetString("LoginMsg", "Username o Password non corretti");

                return Redirect("/Utente/Login");
            }
            
        }

        [HttpGet]
        public IActionResult Logout(string user, string pass)
        {
            HttpContext.Session.Remove("CdUtente");
            HttpContext.Session.Remove("Ruolo");

            return Redirect("/Home");
        }

        //passa alla view la lista di tutte le entites del controller (Context.Utente)
        //Utente/Index: in @Model si trova la lista
        public async Task<IActionResult> List() => View(await Entities.ToListAsync());

        public IActionResult Index() => Redirect("/Utente/List");

    }
}
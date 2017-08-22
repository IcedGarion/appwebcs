using System;
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
    public class UtenteController : CrudController<UpoECommerceContext, int, Utente>
    {
        public UtenteController(UpoECommerceContext context, ILogger<UtenteController> logger) : base(context, logger)
        {
        }

        protected override DbSet<Utente> Entities => Context.Utente;

        protected override Func<Utente, int, bool> FilterById => (e, id) => e.CdUtente == id;

        //GET Utente/Create -> pagina col form di registrazione
        //POST Utente/Create -> legge il form di registrazione

        /*
         * Passa alla pagina contenente il form di registrazione
         */
        [HttpGet]
        public IActionResult Create() => View();

        /*
         * Processa le informazioni provenienti dal form di registrazione
         */
        [HttpPost]
        public async Task<IActionResult> Create(string user, string pass)
        {
            //controlla che non esista gia' lo stesso username desiderato
            var query = from utenti in Context.Utente
                        where utenti.Username.Equals(user)
                        select utenti;

            //se username e' gia' presente nel db allora rimanda alla view con un messaggio
            if(query.Count() != 0)
            {
                TempData["LoginMsg"] = "Username inserito esiste gia'! Prova con un altro username";

                return View();
            }

            //se username non esiste gia':
            //assume password gia' controllate in javascript (wwwroot/js/valdate.js):
            //rende persistente il nuovo utente chiamando il metodo Create del CrudController
            await base.Create(new Utente { Username = user, Password = pass, Ruolo = "user" });

            //recupera dal db l'utente con CdUtente appena inserito
            query = from utenti in Context.Utente
                        where utenti.Username.Equals(user)
                        select utenti;

            //utente appena inserito:
            Utente New = query.First();

            //SALVA IN SESSION DATI LOGIN dell'utente appena inserito (Effettua la login)
            HttpContext.Session.SetInt32("CdUtente", New.CdUtente);
            HttpContext.Session.SetString("Username", New.Username);
            HttpContext.Session.SetString("Ruolo", New.Ruolo);
            TempData["LoginMsg"] = "Registrazione Completata!";

            return Redirect("/Home/Index");
        }

        //GET Utente/Login -> pagina col form di login
        //POST Utente/Login -> legge il form di login

        /*
         * Rimanda alla pagina di login
         */
        [HttpGet]
        public IActionResult Login() => View();

        /*
         * Processa i dati del form di login
         */
        [HttpPost]
        public IActionResult Login(string user, string pass)
        {
            Utente loggato;

            //controlla se esiste utente con quella password nel db, per sicurezza
            var query = from utenti in Context.Utente
                         where utenti.Username.Equals(user) && utenti.Password.Equals(pass)
                         select utenti;

            //se trova un risultato, salva in session codice e ruolo
            if(query.Count() == 1)
            {
                //unico elemento della lista: l'utente con CdUtente
                loggato = query.First();

                //SALVA IN SESSION DATI LOGIN (Effettua login)
                HttpContext.Session.SetInt32("CdUtente", loggato.CdUtente);
                HttpContext.Session.SetString("Username", loggato.Username);
                HttpContext.Session.SetString("Ruolo", loggato.Ruolo == null? "none" : loggato.Ruolo);

                return Redirect("/Home");
            }
            //altrimenti rimanda alla login con messaggio
            else
            {
                TempData["LoginMsg"] = "Username o Password non corretti";

                return Redirect("/Utente/Login");
            }   
        }

        /*
         * Effettua il logout
         */
        [HttpGet]
        public IActionResult Logout(string user, string pass)
        {
            //rimuove da session tutti i dati di login
            HttpContext.Session.Remove("CdUtente");
            HttpContext.Session.Remove("Ruolo");
            HttpContext.Session.Remove("LoginMsg");
            HttpContext.Session.Remove("Cart");

            return Redirect("/Home");
        }

        /*
         * Aggiorna i dati relativi ad un utente (SOLO ADMIN)
         */
        [HttpPost]
        public async Task<IActionResult> Update(string user, string ruolo)
        {
            //riceve parametri dal form
            Int32.TryParse(user, out int CdUtente);
            Utente ToUpdate;

            //cerca nel db l'utente con cdUtente corrispondente a quello ricevuto dal form
            var query = from utenti in Context.Utente
                        where utenti.CdUtente.Equals(CdUtente)
                        select utenti;

            //prende il primo elemento (l'unico) della query
            ToUpdate = query.First();

            //modifica ruolo solo se diverso
            if(!ToUpdate.Ruolo.Equals(ruolo))
            {
                ToUpdate.Ruolo = ruolo;

                //rende le modifiche persistenti chiamando il metodo Update del CrudController
                await base.Update(ToUpdate);
            }

            return Redirect("/Utente/List");
        }

        /*
         * Espone la lista di tutti gli utenti registrati (SOLO ADMIN), con possibilita' di filtrare
         */
        public IActionResult List(string clear, string username, string ruolo)
        {
            //seleziona dal db tutti  gli utenti
            var Query = from utenti in Context.Utente
                        select utenti;
            bool filtered = false;

            //FILTRO: custom IQueryable extension method
            Query = Query.FilterUser(ref filtered, clear, username, ruolo);

            TempData["UtenteFilter"] = filtered.ToString();
            return View(Query.ToList());
        }

        public IActionResult Index() => Redirect("/Utente/List");

    }
}
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

        //la view
        [HttpGet]
        public IActionResult Create() => View();

        //il form per creare 
        [HttpPost]
        public async Task<IActionResult> Create(string user, string pass)
        {
            //controlla se non esiste gia' lo stesso username
            var query = from utenti in Context.Utente
                        where utenti.Username.Equals(user)
                        select utenti;

            //username gia' esistente!
            if(query.Count() != 0)
            {
                TempData["LoginMsg"] = "Username inserito esiste gia'! Prova con un altro username";

                return View();
            }


            //assume password gia' controllate in js
            await base.Create(new Utente { Username = user, Password = pass, Ruolo = "user" });

            //recupera dal db il CdUtente appena inserito
            query = from utenti in Context.Utente
                        where utenti.Username.Equals(user)
                        select utenti;

            //utente appena inserito:
            Utente New = query.First();

            //SALVA IN SESSION DATI LOGIN
            HttpContext.Session.SetInt32("CdUtente", New.CdUtente);
            HttpContext.Session.SetString("Username", New.Username);
            HttpContext.Session.SetString("Ruolo", New.Ruolo);
            TempData["LoginMsg"] = "Registrazione Completata!";

            return Redirect("/Home/Index");
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
                //unico elemento della lista: l'utente con CdUtente
                loggato = query.First();

                //SALVA IN SESSION DATI LOGIN
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

        [HttpGet]
        public IActionResult Logout(string user, string pass)
        {
            HttpContext.Session.Remove("CdUtente");
            HttpContext.Session.Remove("Ruolo");
            HttpContext.Session.Remove("LoginMsg");
            HttpContext.Session.Remove("Cart");

            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string user, string ruolo)
        {
            //riceve parametri dal form
            Int32.TryParse(user, out int CdUtente);
            Utente ToUpdate;

            //cerca nel db quell'utente
            var query = from utenti in Context.Utente
                        where utenti.CdUtente.Equals(CdUtente)
                        select utenti;

            //prende il primo elemento (l'unico) della query
            ToUpdate = query.First();

            //modifica ruolo solo se diverso!
            if(!ToUpdate.Ruolo.Equals(ruolo))
            {
                ToUpdate.Ruolo = ruolo;

                //salva su db
                await base.Update(ToUpdate);
            }

            return Redirect("/Utente/List");
        }

        //passa alla view la lista di tutte le entites del controller (Context.Utente)
        //Utente/Index: in @Model si trova la lista
        public IActionResult List(string clear, string username, string ruolo)
        {
            var Query = from utenti in Context.Utente
                        select utenti;

            //FILTRI
            bool filtered = Query.FilterUser(clear, username, ruolo);

            TempData["UtenteFilter"] = filtered.ToString();
            return View(Query.ToList());
        }

        public IActionResult Index() => Redirect("/Utente/List");

    }
}
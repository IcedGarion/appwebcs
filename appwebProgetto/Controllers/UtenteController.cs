using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using School.Controllers;
using appwebProgetto.Data;
using appwebProgetto.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace appwebProgetto.Controllers
{
    public class UtenteController : CrudController<ProjectContext, int, Utente>
    {
        public UtenteController(ProjectContext context, ILogger<UtenteController> logger) : base(context, logger)
        { }

        protected override DbSet<Utente> Entities => throw new NotImplementedException();

        protected override Func<Utente, int, bool> FilterById => throw new NotImplementedException();

   
        public IActionResult Index()
        {
            return View();
        }
    }
}
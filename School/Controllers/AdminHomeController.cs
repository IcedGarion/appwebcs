using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using School.Model;
using School.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace School.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
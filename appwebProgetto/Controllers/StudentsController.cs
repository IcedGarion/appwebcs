using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using School.Data;
using School.Models;
using System;

namespace School.Controllers
{
    //[Authorize]
    public class StudentsController : CrudController<SchoolContext, int, Student>
    {
        public StudentsController(SchoolContext context, ILogger<StudentsController> logger)
            : base(context, logger)
        { }

        protected override DbSet<Student> Entities => Context.Students;

        protected override Func<Student, int, bool> FilterById => (e, id) => e.ID == id;
    }
}
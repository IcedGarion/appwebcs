using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using School.Data;
using School.Models;
using Microsoft.AspNetCore.Authorization;

namespace School.Controllers
{
    [Authorize]
    public class CoursesController : CrudController<SchoolContext, int, Course>
    {
        public CoursesController(SchoolContext context, ILogger<CoursesController> logger) 
            : base(context, logger)
        { }

        protected override DbSet<Course> Entities => Context.Courses;

        protected override Func<Course, int, bool> FilterById => (e, id) => e.CourseID == id;

        [HttpPost]
        public override Task<IActionResult> Delete(int courseID) => base.Delete(courseID);
    }
}
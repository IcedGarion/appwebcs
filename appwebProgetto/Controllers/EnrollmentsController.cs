using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using School.Data;
using School.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace School.Controllers
{
    [Authorize]
    public class EnrollmentsController : CrudController<SchoolContext, int, Enrollment>
    {
        public EnrollmentsController(SchoolContext context, ILogger<Enrollment> logger) : base(context, logger)
        { }

        protected override DbSet<Enrollment> Entities => Context.Enrollments;

        protected override Func<Enrollment, int, bool> FilterById => (e, id) => e.EnrollmentID == id;

        public override IActionResult Index()
        {
            ViewData[nameof(Student)] = Context.Students.ToSelectList(nameof(Student.ID), nameof(Student.FullName));
            ViewData[nameof(Course)] = Context.Courses.ToSelectList(nameof(Course.CourseID), nameof(Course.Title));

            return base.Index();
        }

        public override Task<IActionResult> Delete(int enrollmentID) => base.Delete(enrollmentID);
    }
}
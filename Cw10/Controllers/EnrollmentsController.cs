using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cw10.Models;
using Cw10.DTO;

namespace Cw10.Controllers
{
    [Route("api/enrollmentscontroller")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly s16859Context _context;

        public EnrollmentsController(s16859Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Enrollment>> EnrollStudent(StudentRequest studentRequest)
        {
            int idStudy;

            if (studentRequest.IndexNumber == null || studentRequest.FirstName == null || studentRequest.LastName == null || studentRequest.Birthdate == null || studentRequest.Studies == null)
            {
                return NotFound("Nie pełne dane");
            }

            var result = await _context.Studies.Where(e => e.Name == studentRequest.Studies).ToListAsync();

            if (result.Count == 0)
            {
                return BadRequest("Nie znaleziono kierunku");
            }
            else
            {
                idStudy = result[0].IdStudy;
            }

            var result2 = await _context.Enrollment.Where(e=>e.IdEnrollment==1)
                .Join(
                _context.Student,
                e=>e.IdEnrollment,
                s=>s.IdEnrollment,
                (e,s) => new
                {
                    IdEnrollment=e.IdEnrollment,
                    Semester = e.Semester,
                    IdStudy = e.IdStudy,
                    StartDate = e.StartDate

                })
                .ToListAsync();
           

            return Ok(result2);
        }


        // GET: api/Enrollments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollment()
        {
            return await _context.Enrollment.ToListAsync();
        }

        // GET: api/Enrollments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return enrollment;
        }

        // PUT: api/Enrollments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollment(int id, Enrollment enrollment)
        {
            if (id != enrollment.IdEnrollment)
            {
                return BadRequest();
            }

            _context.Entry(enrollment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnrollmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Enrollments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /*
        [HttpPost]
        public async Task<ActionResult<Enrollment>> PostEnrollment(Enrollment enrollment)
        {
            _context.Enrollment.Add(enrollment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EnrollmentExists(enrollment.IdEnrollment))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEnrollment", new { id = enrollment.IdEnrollment }, enrollment);
        }*/

        // DELETE: api/Enrollments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Enrollment>> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();

            return enrollment;
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.IdEnrollment == id);
        }
    }
}

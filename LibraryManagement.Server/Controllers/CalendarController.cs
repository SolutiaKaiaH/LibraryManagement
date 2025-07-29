using LibraryManagement.Server.Data;
using LibraryManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Schedule;

namespace LibraryManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarObjsController : ControllerBase
    {
        private readonly LibraryContext _db;
        public CalendarObjsController(LibraryContext db) => _db = db;
    

        // GET: api/ScheduleCalendarObjs
        [HttpGet]
        public async Task<ActionResult<List<CalendarObj>>> GetAll()
        {
            return await _db.Calendar
                            .Select(e => new CalendarObj
                            {
                                Id = e.Id,
                                Subject = e.Subject,
                                StartTime = e.StartTime,
                                EndTime = e.EndTime,
                                IsAllDay = e.IsAllDay,
                                Location = e.Location,
                                Description = e.Description,
                                CalendarId = e.CalendarId,
                                OwnerId = e.OwnerId,
                                IsPlanned = e.IsPlanned,
                                RecurrenceRule = e.RecurrenceRule,
                                RecurrenceID = e.RecurrenceID,
                                RecurrenceException = e.RecurrenceException
                            })
                            .ToListAsync();
        }

        // GET api/ScheduleCalendarObjs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CalendarObj>> GetOne(int id)
        {
            var ev = await _db.Calendar.FindAsync(id);
            if (ev == null) return NotFound();
            return Ok(new CalendarObj
            {
                Id = ev.Id,
                Subject = ev.Subject,
                StartTime = ev.StartTime,
                EndTime = ev.EndTime,
                IsAllDay = ev.IsAllDay,
                Location = ev.Location,
                Description = ev.Description,
                CalendarId = ev.CalendarId,
                OwnerId = ev.OwnerId,
                IsPlanned = ev.IsPlanned,
                RecurrenceRule = ev.RecurrenceRule,
                RecurrenceID = ev.RecurrenceID,
                RecurrenceException = ev.RecurrenceException
            });
        }

        // POST api/ScheduleCalendarObjs
        [HttpPost]
        public async Task<ActionResult<CalendarObj>> Create(CalendarObj model)
        {
            var ev = new CalendarObj
            {
                Subject = model.Subject,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                IsAllDay = model.IsAllDay,
                Location = model.Location,
                Description = model.Description,
                CalendarId = model.CalendarId,
                OwnerId = model.OwnerId,
                IsPlanned = model.IsPlanned,
                RecurrenceRule = model.RecurrenceRule,
                RecurrenceID = model.RecurrenceID,
                RecurrenceException = model.RecurrenceException
            };
            _db.Calendar.Add(ev);
            await _db.SaveChangesAsync();
            model.Id = ev.Id;
            return CreatedAtAction(nameof(GetOne), new { id = ev.Id }, model);
        }

        // PUT api/ScheduleCalendarObjs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CalendarObj model)
        {
            if (id != model.Id) return BadRequest();
            var ev = await _db.Calendar.FindAsync(id);
            if (ev == null) return NotFound();

            // copy fields back
            ev.Subject = model.Subject;
            ev.StartTime = model.StartTime;
            ev.EndTime = model.EndTime;
            ev.IsAllDay = model.IsAllDay;
            ev.Location = model.Location;
            ev.Description = model.Description;
            ev.CalendarId = model.CalendarId;
            ev.OwnerId = model.OwnerId;
            ev.IsPlanned = model.IsPlanned;
            ev.RecurrenceRule = model.RecurrenceRule;
            ev.RecurrenceID = model.RecurrenceID;
            ev.RecurrenceException = model.RecurrenceException;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/ScheduleCalendarObjs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _db.Calendar.FindAsync(id);
            if (ev == null) return NotFound();
            _db.Calendar.Remove(ev);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
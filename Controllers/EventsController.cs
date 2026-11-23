using EventManagerApi.Data;
using EventManagerApi.DTOs;
using EventManagerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagerApi.Controllers;

[ApiController]
[Route("api/events")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/events
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
    {
        var events = await _context.Events
            .AsNoTracking()
            .Include(e => e.Attendees)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return Ok(events.Select(ToDto));
    }

    // GET /api/events/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetEvent(int id)
    {
        var ev = await _context.Events
            .AsNoTracking()
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
        {
            return NotFound();
        }

        return Ok(ToDto(ev));
    }

    // POST /api/events
    [HttpPost]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventDto>> CreateEvent(CreateEventDto dto)
    {
        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Date = dto.Date,
            Location = dto.Location
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = ev.Id }, ToDto(ev));
    }

    // PUT /api/events/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto dto)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
        {
            return NotFound();
        }

        ev.Title = dto.Title;
        ev.Description = dto.Description;
        ev.Date = dto.Date;
        ev.Location = dto.Location;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/events/{id}  (cascades to attendees)
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
        {
            return NotFound();
        }

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST /api/events/{eventId}/attendees  (Register)
    [HttpPost("{eventId:int}/attendees")]
    [ProducesResponseType(typeof(AttendeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttendeeDto>> RegisterAttendee(int eventId, RegisterAttendeeDto dto)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
        if (!eventExists)
        {
            return NotFound();
        }

        var attendee = new Attendee
        {
            Name = dto.Name,
            Email = dto.Email,
            EventId = eventId
        };

        _context.Attendees.Add(attendee);
        await _context.SaveChangesAsync();

        // Location header points at the parent event, which includes the new attendee.
        return CreatedAtAction(nameof(GetEvent), new { id = eventId }, ToDto(attendee));
    }

    // DELETE /api/events/{eventId}/attendees/{attendeeId}  (Unregister)
    [HttpDelete("{eventId:int}/attendees/{attendeeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnregisterAttendee(int eventId, string attendeeId)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
        if (!eventExists)
        {
            return NotFound();
        }

        var attendee = await _context.Attendees
            .FirstOrDefaultAsync(a => a.Id == attendeeId && a.EventId == eventId);
        if (attendee == null)
        {
            return NotFound();
        }

        _context.Attendees.Remove(attendee);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static EventDto ToDto(Event ev) => new()
    {
        Id = ev.Id,
        Title = ev.Title,
        Description = ev.Description,
        Date = ev.Date,
        Location = ev.Location,
        Attendees = ev.Attendees.Select(ToDto).ToList()
    };

    private static AttendeeDto ToDto(Attendee a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Email = a.Email
    };
}

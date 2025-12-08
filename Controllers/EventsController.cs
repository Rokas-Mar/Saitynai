using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _db;
    public EventsController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpGet]
    public ActionResult<IEnumerable<Event>> GetAll() => Ok(_db.Events.ToList());

    [HttpGet("{id}")]
    public ActionResult<Event> Get(int id)
    {
        var evt = _db.Events.Find(id);
        return evt == null ? NotFound() : Ok(evt);
    }

    [Authorize]
    [HttpPost]
    public ActionResult<Event> Create(Event evt)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Events.Add(evt);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = evt.Id }, evt);
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Event evt)
    {
        var existing = _db.Events.Find(id);
        if (existing == null) return NotFound();

        existing.Name = evt.Name;
        existing.Date = evt.Date;
        existing.Location = evt.Location;
        existing.UserId = evt.UserId;

        _db.SaveChanges();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var evt = _db.Events.Find(id);
        if (evt == null) return NotFound();
        _db.Events.Remove(evt);
        _db.SaveChanges();
        return NoContent();
    }
}

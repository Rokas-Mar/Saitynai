using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _db;
    public EventsController(AppDbContext db) => _db = db;

    [HttpGet]
    public ActionResult<IEnumerable<Event>> GetAll() => Ok(_db.Events.ToList());

    [HttpGet("{id}")]
    public ActionResult<Event> Get(int id)
    {
        var evt = _db.Events.Find(id);
        return evt == null ? NotFound() : Ok(evt);
    }

    [HttpPost]
    public ActionResult<Event> Create(Event evt)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Events.Add(evt);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = evt.Id }, evt);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Event evt)
    {
        if (id != evt.Id) return BadRequest();
        var existing = _db.Events.Find(id);
        if (existing == null) return NotFound();
        _db.Entry(existing).CurrentValues.SetValues(evt);
        _db.SaveChanges();
        return NoContent();
    }

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

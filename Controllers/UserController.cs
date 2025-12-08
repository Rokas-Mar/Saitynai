using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// Put id body neturi buti

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpGet]
    public ActionResult<IEnumerable<Event>> GetAll() => Ok(_db.Users.ToList());

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<Event> Get(int id)
    {
        var org = _db.Users.Find(id);
        return org == null ? NotFound() : Ok(org);
    }

    [Authorize]
    [HttpPost]
    public ActionResult<User> Create(User user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Users.Add(user);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] User user)
    {
        if (user == null)
            return BadRequest("User data is required.");

        var existing = _db.Users.Find(id);
        if (existing == null)
            return NotFound($"User with id {id} not found.");

        existing.Name = user.Name;
        existing.Surname = user.Surname;
        existing.Email = user.Email;
        existing.Number = user.Number;
        existing.Role = user.Role;
        existing.OrganisationId = user.OrganisationId;

        _db.SaveChanges();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        _db.Users.Remove(user);
        _db.SaveChanges();
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id}/events")]
    public ActionResult<IEnumerable<Event>> GetUserEvents(int id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound($"User with ID {id} not found.");

        var events = _db.Events.Where(e => e.UserId == id).ToList();
        return Ok(events);
    }
}

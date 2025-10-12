using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [HttpGet]
    public ActionResult<IEnumerable<Event>> GetAll() => Ok(_db.Users.ToList());

    [HttpGet("{id}")]
    public ActionResult<Event> Get(int id)
    {
        var org = _db.Users.Find(id);
        return org == null ? NotFound() : Ok(org);
    }

    [HttpPost]
    public ActionResult<User> Create(User user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Users.Add(user);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, User user)
    {
        if (id != user.Id) return BadRequest();
        var existing = _db.Users.Find(id);
        if (existing == null) return NotFound();
        _db.Entry(existing).CurrentValues.SetValues(user);
        _db.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        _db.Users.Remove(user);
        _db.SaveChanges();
        return NoContent();
    }
}

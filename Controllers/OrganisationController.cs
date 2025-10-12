using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrganisationsController : ControllerBase
{
    private readonly AppDbContext _db;
    public OrganisationsController(AppDbContext db) => _db = db;

    [HttpGet]
    public ActionResult<IEnumerable<Organisation>> GetAll() => Ok(_db.Organisations.ToList());

    [HttpGet("{id}")]
    public ActionResult<Organisation> Get(int id)
    {
        var org = _db.Organisations.Find(id);
        return org == null ? NotFound() : Ok(org);
    }

    [HttpPost]
    public ActionResult<Organisation> Create(Organisation org)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Organisations.Add(org);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = org.Id }, org);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Organisation org)
    {
        if (id != org.Id) return BadRequest();
        var existing = _db.Organisations.Find(id);
        if (existing == null) return NotFound();
        _db.Entry(existing).CurrentValues.SetValues(org);
        _db.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var org = _db.Organisations.Find(id);
        if (org == null) return NotFound();
        _db.Organisations.Remove(org);
        _db.SaveChanges();
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;



[ApiController]
[Route("api/[controller]")]
public class OrganisationsController : ControllerBase
{

    private readonly AppDbContext _db;
    public OrganisationsController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpGet]
    public ActionResult<IEnumerable<Organisation>> GetAll() => Ok(_db.Organisations.ToList());

    [HttpGet("{id}")]
    public ActionResult<Organisation> Get(int id)
    {
        var org = _db.Organisations.Find(id);
        return org == null ? NotFound() : Ok(org);
    }

    [Authorize]
    [HttpPost]
    public ActionResult<Organisation> Create(Organisation org)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Organisations.Add(org);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = org.Id }, org);
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Organisation org)
    {
        var existing = _db.Organisations.Find(id);
        if (existing == null) return NotFound();

        existing.Name = org.Name;
        existing.Email = org.Email;
        existing.Address = org.Address;
        existing.PostalCode = org.PostalCode;
        existing.IBAN = org.IBAN;
        existing.Number = org.Number;
        existing.CompanyCode = org.CompanyCode;

        _db.SaveChanges();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var org = _db.Organisations.Find(id);
        if (org == null) return NotFound();
        _db.Organisations.Remove(org);
        _db.SaveChanges();
        return NoContent();
    }

    [Authorize]
    [HttpGet("{organisationId}/events")]
    public async Task<IActionResult> GetOrganisationEvents(int organisationId)
    {
        var organisation = await _db.Organisations
            .Where(o => o.Id == organisationId)
            .Select(o => new
            {
                o.Id,
                o.Name,
                Users = o.Users.Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Surname,
                    Events = u.Events.Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.Date,
                        e.Location
                    })
                })
            })
            .FirstOrDefaultAsync();

        if (organisation == null)
            return NotFound("Organisation not found.");

        return Ok(organisation);
    }
}

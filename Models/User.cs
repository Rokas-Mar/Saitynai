using Microsoft.Extensions.Logging;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Role { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }

    public int OrganisationId { get; set; }
    public Organisation Organisation { get; set; }

    public List<Event> Events { get; set; }
}
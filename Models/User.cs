using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Role { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }

    public int OrganisationId { get; set; }
    [JsonIgnore]
    public Organisation? Organisation { get; set; }
    [JsonIgnore]
    public List<Event>? Events { get; set; }
}
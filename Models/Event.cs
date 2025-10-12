using System.Text.Json.Serialization;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }

    public int UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
}
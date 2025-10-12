using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = "server=localhost;port=3306;database=organisationdb;user=root;password=";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Organisations.Any())
    {
        var org1 = new Organisation { Name = "TechCorp", Email = "contact@techcorp.com", Address = "123 Tech Street", PostalCode = "10001", IBAN = "LT123456789012345678", Number = "123456789", CompanyCode = "TC-001" };
        var org2 = new Organisation { Name = "HealthSolutions", Email = "info@healthsolutions.com", Address = "45 Wellness Ave", PostalCode = "20002", IBAN = "LT987654321098765432", Number = "987654321", CompanyCode = "HS-002" };
        db.Organisations.AddRange(org1, org2);
        db.SaveChanges();

        var user1 = new User { Name = "John", Surname = "Doe", Role = "Admin", Number = "555-1234", Email = "john.doe@techcorp.com", OrganisationId = org1.Id };
        var user2 = new User { Name = "Jane", Surname = "Smith", Role = "Manager", Number = "555-5678", Email = "jane.smith@techcorp.com", OrganisationId = org1.Id };
        var user3 = new User { Name = "Alice", Surname = "Brown", Role = "Employee", Number = "555-8765", Email = "alice.brown@healthsolutions.com", OrganisationId = org2.Id };
        db.Users.AddRange(user1, user2, user3);
        db.SaveChanges();

        var event1 = new Event { Name = "Tech Launch", Date = DateTime.Now.AddDays(7), Location = "TechCorp HQ", UserId = user1.Id };
        var event2 = new Event { Name = "Annual Meeting", Date = DateTime.Now.AddDays(30), Location = "Conference Center", UserId = user2.Id };
        var event3 = new Event { Name = "Health Workshop", Date = DateTime.Now.AddDays(14), Location = "Wellness Hall", UserId = user3.Id };
        db.Events.AddRange(event1, event2, event3);
        db.SaveChanges();
    }
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
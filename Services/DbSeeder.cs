using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
	public static void Seed(AppDbContext db)
	{

		Console.WriteLine("🔍 Checking if database needs seeding...");

		if (db.Users.Any())
		{
			Console.WriteLine("➡️ Database already seeded. Skipping.");
			return;
		}

		Console.WriteLine("🌱 Seeding initial data...");

		if (db.Organisations.Any()) return; // Prevent reseed

		var rnd = new Random();

		// -------- ORGANISATIONS --------
		var organisations = new List<Organisation>
		{
			new Organisation { Name="TechCorp LT", Email="info@techcorp.lt", Address="Vilniaus g. 12, Vilnius", PostalCode="LT-01119", IBAN="LT157300010147715229", Number="+37060012345", CompanyCode="302515123" },
			new Organisation { Name="Baltic Logistics", Email="info@balog.lt", Address="Taikos pr. 78, Klaipėda", PostalCode="LT-91148", IBAN="LT567044060005556613", Number="+37069032111", CompanyCode="305487951" },
			new Organisation { Name="Digitalis Group", Email="contact@digitalis.lt", Address="K. Donelaičio g. 15, Kaunas", PostalCode="LT-44240", IBAN="LT087300010006987444", Number="+37068414775", CompanyCode="302654987" },
			new Organisation { Name="Vilmed Pharma", Email="office@vilmed.lt", Address="Žirmūnų g. 50, Vilnius", PostalCode="LT-09226", IBAN="LT087044060002569977", Number="+37063055888", CompanyCode="305122478" },
			new Organisation { Name="AmberTech Solutions", Email="labas@ambertech.lt", Address="Aušros al. 5, Panevėžys", PostalCode="LT-35199", IBAN="LT637044060008877455", Number="+37065811477", CompanyCode="302114785" }
		};

		db.Organisations.AddRange(organisations);
		db.SaveChanges();

		// -------- USERS --------
		string[] names = { "Rokas", "Mantas", "Karolis", "Austėja", "Ieva", "Gabija", "Pijus", "Dominykas", "Justė", "Lukas", "Eglė", "Saulius", "Monika", "Airidas", "Jonas" };
		string[] surnames = { "Kazlauskas", "Briedis", "Bakšys", "Žemaitis", "Petrauskaitė", "Balčiūnaitė", "Sabaliauskas", "Valaitis", "Mažeikaitė", "Dambrauskas", "Mikalauskas" };

		var users = new List<User>();

		foreach (var org in organisations)
		{
			// One admin per organisation
			users.Add(new User
			{
				Name = $"{names[rnd.Next(names.Length)]}",
				Surname = "Administratorius",
				Email = $"admin@{org.Name.Replace(" ", "").ToLower()}.lt",
				Number = $"+3706{rnd.Next(1000000, 9999999)}",
				Password = "admin123",
				Role = "Admin",
				OrganisationId = org.Id
			});

			// 3–5 employees
			int count = rnd.Next(3, 6);
			for (int i = 0; i < count; i++)
			{
				var name = names[rnd.Next(names.Length)];
				var surname = surnames[rnd.Next(surnames.Length)];

				users.Add(new User
				{
					Name = name,
					Surname = surname,
					Email = $"{name.ToLower()}.{surname.ToLower()}@{org.Name.Replace(" ", "").ToLower()}.lt",
					Number = $"+3706{rnd.Next(1000000, 9999999)}",
					Password = "user123",
					Role = "Employee",
					OrganisationId = org.Id
				});
			}
		}

		db.Users.AddRange(users);
		db.SaveChanges();

		// -------- EVENTS --------
		string[] eventNames = { "Meeting", "Conference", "Training", "Workshop", "Presentation", "Audit", "Planning" };
		string[] cities = { "Vilnius", "Kaunas", "Klaipėda", "Šiauliai", "Panevėžys" };

		var events = new List<Event>();
		foreach (var user in users)
		{
			int count = rnd.Next(2, 5);
			for (int i = 0; i < count; i++)
			{
				events.Add(new Event
				{
					Name = $"{eventNames[rnd.Next(eventNames.Length)]} {rnd.Next(100, 999)}",
					Location = cities[rnd.Next(cities.Length)],
					Date = DateTime.Now.AddDays(rnd.Next(1, 60)),
					UserId = user.Id
				});
			}
		}

		db.Events.AddRange(events);
		db.SaveChanges();
		Console.WriteLine("✅ Seeding complete.");

	}
}
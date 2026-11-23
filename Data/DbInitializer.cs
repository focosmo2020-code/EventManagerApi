using EventManagerApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventManagerApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext db)
        {
            // Ensure the database exists
            var creator = db.GetService<IRelationalDatabaseCreator>();
            if (!creator.Exists()) creator.Create();

            // Create our tables if they're missing (works even if the database isn't empty)
            bool tablesExist;
            try { _ = db.Events.Any(); tablesExist = true; }
            catch (SqlException ex) when (ex.Number == 208) { tablesExist = false; } // invalid object name
            if (!tablesExist) creator.CreateTables();

            if (db.Events.Any()) return; // already seeded

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Career Fair", Description = "Meet employers hiring co-op students and graduates.",
                    Date = new DateTime(2026, 12, 1, 10, 0, 0), Location = "Gym",
                    Attendees =
                    {
                        new Attendee { Name = "Alice Smith", Email = "alice@example.com" },
                        new Attendee { Name = "Bob Jones",   Email = "bob@example.com" }
                    }
                },
                new Event
                {
                    Title = "Tech Talk", Description = "Industry speakers on cloud and AI.",
                    Date = new DateTime(2026, 12, 8, 14, 0, 0), Location = "Auditorium",
                    Attendees =
                    {
                        new Attendee { Name = "Carol White", Email = "carol@example.com" },
                        new Attendee { Name = "Dan Brown",   Email = "dan@example.com" }
                    }
                },
                new Event
                {
                    Title = "Hack Night", Description = "Overnight coding challenge with prizes.",
                    Date = new DateTime(2026, 12, 15, 18, 0, 0), Location = "Library",
                    Attendees =
                    {
                        new Attendee { Name = "Eve Green",  Email = "eve@example.com" },
                        new Attendee { Name = "Frank Black", Email = "frank@example.com" }
                    }
                }
            };

            db.Events.AddRange(events);
            db.SaveChanges();
        }
    }
}

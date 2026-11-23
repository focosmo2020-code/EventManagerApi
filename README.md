# Event Manager Web API (Lab 5)

RESTful Web API built with ASP.NET Core (.NET 8), Entity Framework Core and SQL Server LocalDB.

## Run

1. Open the folder in Visual Studio (or run `dotnet run`).
2. On startup the app creates the `EventManagerApiDb` database in LocalDB and seeds 3 events with 2 attendees each.
3. Swagger UI opens at `/swagger`.

## Endpoints

| Method | Route | Success | Errors |
|---|---|---|---|
| GET | `/api/events` | 200 | |
| GET | `/api/events/{id}` | 200 | 404 |
| POST | `/api/events` | 201 + Location | 400 |
| PUT | `/api/events/{id}` | 204 | 400, 404 |
| DELETE | `/api/events/{id}` | 204 (cascades attendees) | 404 |
| POST | `/api/events/{eventId}/attendees` | 201 | 400, 404 |
| DELETE | `/api/events/{eventId}/attendees/{attendeeId}` (attendeeId is a GUID string) | 204 | 404 |

## Structure

- `Models/` – `Event`, `Attendee`
- `Data/` – `AppDbContext` (Fluent API, cascade delete), `DbInitializer` (seed data)
- `DTOs/` – request/response DTOs with validation attributes
- `Controllers/EventsController.cs` – all endpoints

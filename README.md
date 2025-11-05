# Hotel Reservation System API

A comprehensive hotel booking system built with .NET 8, following Clean Architecture principles.

## Setup

### First Time Setup

1. Copy `appsettings.Development.json.example` to `appsettings.Development.json` in the API project
2. Update the connection string with your PostgreSQL credentials
3. Never commit `appsettings.Development.json` (it's in .gitignore)

## Features

- **User Authentication & Authorization** - JWT-based authentication
- **Room Management** - Search and manage hotel rooms
- **Reservation System** - Create, view, and cancel reservations
- **Guest Management** - User profile management
- **Clean Architecture** - Domain-driven design with separation of concerns
- **Entity Framework Core** - PostgreSQL database with migrations
- **Docker Support** - Containerized application and database

## Architecture

The solution follows Clean Architecture principles with the following layers:

- **Domain** - Core business entities, enums, and domain services
- **Application** - Business logic, interfaces, and DTOs
- **Infrastructure** - Data access, external services, and repository implementations
- **API** - REST API controllers and configuration

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- AutoMapper
- FluentValidation

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL (installed locally)

### Running Locally

1. Ensure PostgreSQL is running locally
2. Run the following commands:

```bash
# Restore packages
dotnet restore

# Run database migrations
dotnet ef database update --project src/HotelReservation.Infrastructure --startup-project src/HotelReservation.API

# Run the API
dotnet run --project src/HotelReservation.API
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token

### Rooms
- `GET /api/rooms/search` - Search available rooms
- `GET /api/rooms/{id}` - Get room details
- `GET /api/rooms/types` - Get room types
- `GET /api/rooms/{id}/availability` - Check room availability

### Reservations (Requires Authentication)
- `POST /api/reservations` - Create a new reservation
- `GET /api/reservations/{id}` - Get reservation details
- `GET /api/reservations/guest/{guestId}` - Get guest's reservations
- `PUT /api/reservations/{id}/cancel` - Cancel a reservation
- `PUT /api/reservations/{id}/checkin` - Check in a reservation
- `PUT /api/reservations/{id}/checkout` - Check out a reservation

### Guests (Requires Authentication)
- `GET /api/guests/user/{userId}` - Get guest by user ID
- `POST /api/guests` - Create guest profile
- `PUT /api/guests/{id}` - Update guest profile
- `DELETE /api/guests/{id}` - Delete guest profile

## Database Schema

The system includes the following main entities:

- **AppUser** - User accounts with authentication
- **Guest** - Guest profiles linked to users
- **RoomType** - Room categories (Standard, Deluxe, Suite)
- **Room** - Individual hotel rooms
- **RoomFeature** - Room amenities
- **Reservation** - Booking records

## Configuration

Key configuration settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=hotel_reservation_db;Username=postgres;Password=password"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "HotelReservationAPI",
    "Audience": "HotelReservationClients",
    "ExpiryMinutes": 60
  }
}
```

## Testing

Run the unit tests:

```bash
dotnet test
```

## Development

### Adding New Features

1. Define entities in the Domain layer
2. Create interfaces in the Application layer
3. Implement repositories in the Infrastructure layer
4. Add services in the Application layer
5. Create controllers in the API layer

### Database Migrations

To create a new migration:

```bash
dotnet ef migrations add MigrationName --project src/HotelReservation.Infrastructure --startup-project src/HotelReservation.API
```

To update the database:

```bash
dotnet ef database update --project src/HotelReservation.Infrastructure --startup-project src/HotelReservation.API
```

## License

This project is licensed under the MIT License.

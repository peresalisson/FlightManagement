# ✈️ Flight Management System

A professional full-stack ASP.NET MVC Core application for managing flights with automatic distance and fuel consumption calculations.

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![C#](https://img.shields.io/badge/C%23-12.0-green)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%208.0-orange)

## 🌟 Features

- ✈️ **Complete Flight Management** - Full CRUD operations
- 🗺️ **GPS-Based Distance Calculation** - Haversine formula implementation
- ⛽ **Automatic Fuel Calculation** - Based on distance and aircraft consumption
- 📊 **Comprehensive Reporting** - Statistics and summaries
- 💾 **Data Persistence** - SQL Server with Entity Framework Core
- 🎨 **Modern UI** - Responsive Bootstrap 5 design
- 🏗️ **Clean Architecture** - Repository and Service patterns
- ⚡ **Async Operations** - Full async/await implementation

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, or Docker)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YOUR-USERNAME/FlightManagement.git
   cd FlightManagement
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update connection string** (optional)
   
   Edit `appsettings.json` if not using LocalDB:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "YOUR_CONNECTION_STRING"
   }
   ```

4. **Create database**
   
   The database will be created automatically on first run, or use migrations:
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

## 🏗️ Architecture

```
FlightManagement/
├── Controllers/          # MVC Controllers
├── Models/              # Domain entities
├── ViewModels/          # Data transfer objects
├── Data/                # DbContext and database configuration
├── Repositories/        # Data access layer
├── Services/            # Business logic layer
└── Views/               # Razor views
    └── Flights/         # Flight-related views
```

## 🧮 Technical Highlights

### Distance Calculation
Uses the **Haversine formula** to calculate great-circle distance between GPS coordinates:

```csharp
Distance = 2 × R × arcsin(√(sin²(Δφ/2) + cos φ₁ × cos φ₂ × sin²(Δλ/2)))
```

Where:
- R = Earth's radius (6,371 km)
- φ = latitude
- λ = longitude

### Fuel Calculation
```
Required Fuel = (Distance × Fuel Consumption per km) + Takeoff Fuel
```

## 📦 Technologies Used

- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# 12.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server
- **Frontend**: Bootstrap 5, Bootstrap Icons
- **Patterns**: Repository Pattern, Service Layer, Dependency Injection

## 🗃️ Database Schema

### Airports
- Pre-seeded with 6 major international airports
- Stores IATA codes and GPS coordinates

### Flights
- Flight number, route, departure time
- Aircraft fuel consumption parameters
- Auto-calculated distance and fuel requirements

## 📊 API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/Flights` | GET | List all flights |
| `/Flights/Create` | GET/POST | Create new flight |
| `/Flights/Edit/{id}` | GET/POST | Edit flight |
| `/Flights/Details/{id}` | GET | View flight details |
| `/Flights/Delete/{id}` | GET/POST | Delete flight |
| `/Flights/Report` | GET | View statistics report |

## 🧪 Testing

```bash
dotnet test
```

## 📝 Sample Data

The system comes pre-loaded with these airports:

| Code | Airport | Location | Coordinates |
|------|---------|----------|-------------|
| JFK | John F. Kennedy | New York, USA | 40.6413°N, 73.7781°W |
| LAX | Los Angeles Intl | Los Angeles, USA | 33.9416°N, 118.4085°W |
| LHR | London Heathrow | London, UK | 51.4700°N, 0.4543°W |
| CDG | Charles de Gaulle | Paris, France | 49.0097°N, 2.5479°E |
| DXB | Dubai Intl | Dubai, UAE | 25.2532°N, 55.3657°E |
| NRT | Narita Intl | Tokyo, Japan | 35.7720°N, 140.3929°E |

## 🔧 Configuration

### Connection Strings

**LocalDB (default)**:
```json
"Server=(localdb)\\mssqllocaldb;Database=FlightManagementDb;Trusted_Connection=True"
```



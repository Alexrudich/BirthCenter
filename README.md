# BirthCenter API

REST API service for managing newborn patients with FHIR-compliant date search.

## 🚀 Technologies

- .NET 6
- PostgreSQL
- Entity Framework Core
- Docker & Docker Compose
- Swagger / OpenAPI

## 📋 Features

- Full CRUD operations for Patient entity
- FHIR-compliant date search (`eq`, `gt`, `lt`, `ge`, `le`, partial dates)
- Global exception handling
- Clean Architecture (Domain, Application, Infrastructure, API)
- Swagger documentation with XML comments
- Test data seeding endpoint
- Docker Compose orchestration

## 🖥️ Run from Visual Studio

1. Open `BirthCenter.sln`
2. Select `docker-compose` as startup project
3. Press `F5`

The application will start with:
- API on http://localhost:7000
- Swagger UI automatically opens
- PostgreSQL in Docker container

## 🐳 Quick Start with Docker Compose

### Prerequisites
- Docker Desktop
- Git

### Run the application

```bash
# Clone the repository
git clone <your-repo-url>
cd BirthCenter

# Start all services
docker-compose up --build
```

After startup:
- API: http://localhost:7000
- Swagger UI: http://localhost:7000/swagger
- PostgreSQL: `localhost:5432` (user: `postgres`, password: `postgres`)

## 🧪 Test Data

The API includes a `SeedController` for managing test data:

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/Seed/generate` | Creates 100 test patients |
| `GET` | `/api/Seed/count` | Shows current patient count |
| `DELETE` | `/api/Seed/clear` | Removes all patients |

## 📬 API Endpoints

### Patients

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Patients` | Get all patients |
| `GET` | `/api/Patients/{id}` | Get patient by ID |
| `POST` | `/api/Patients` | Create new patient |
| `PUT` | `/api/Patients/{id}` | Update patient |
| `DELETE` | `/api/Patients/{id}` | Delete patient |

## 🔍 Date Search Examples

| Query | Description |
|-------|-------------|
| `?birthDate=2024` | Patients born in 2024 |
| `?birthDate=2024-01` | Patients born in January 2024 |
| `?birthDate=2024-01-13` | Patients born on Jan 13, 2024 |
| `?birthDate=gt2024-01-01` | Patients born after Jan 1, 2024 |
| `?birthDate=lt2024-12-31` | Patients born before Dec 31, 2024 |
| `?birthDate=ge2024-06-01` | Patients born on or after Jun 1, 2024 |
| `?birthDate=le2024-06-30` | Patients born on or before Jun 30, 2024 |

## 📮 Postman Collection

Import `BirthCenter.postman_collection.json` from the repository root.

Collection includes:
- Test data management (generate, count, clear)
- CRUD operations
- Various date search scenarios

## 🏗️ Project Structure

```
BirthCenter/
├── BirthCenter.API/          # Web API layer
├── BirthCenter.Application/   # Use cases and DTOs
├── BirthCenter.Domain/        # Entities and enums
├── BirthCenter.Infrastructure/# Data access and repositories
├── BirthCenter.Shared/         # Shared utilities
├── BirthCenter.sln
├── docker-compose.yml
├── .dockerignore
├── README.md
├── TASK.md
└── BirthCenter.postman_collection.json
```

## 📝 API Request Examples

### Create a patient
```json
POST /api/Patients
{
  "name": {
    "use": "official",
    "family": "Ivanov",
    "given": ["Ivan", "Ivanovich"]
  },
  "gender": "male",
  "birthDate": "2024-01-13T18:25:43",
  "active": true
}
```

### Update a patient
```json
PUT /api/Patients/{id}
{
  "name": {
    "family": "Petrov",
    "given": ["Petr", "Petrovich"],
    "use": "official"
  },
  "active": false
}
```

## 🔧 Development Setup

### Run with .NET CLI
```bash
cd BirthCenter.API
dotnet run
```

### Run tests (if any)
```bash
dotnet test
```

## 📝 License

This project is created for evaluation purposes as a test assignment.

## 👨‍💻 Author

[Your Name]

---

## ⚠️ Note

Instead of a separate console application for generating test data, a `SeedController` is implemented. This provides the same functionality through the API and eliminates connectivity issues.
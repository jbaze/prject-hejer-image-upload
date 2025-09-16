# Customer/Lead Image Upload API

This is a .NET 9 Web API that allows users to upload and manage images for customers and leads, with JWT authentication and a maximum limit of 10 images per customer/lead.

## Features

- 🔐 **JWT Authentication** with login/logout and token refresh
- 👤 Customer CRUD operations
- 📋 Lead CRUD operations  
- 📸 **Single & Multiple image upload** for customers and leads
- 🔢 10-image limit enforcement per customer/lead
- 🚀 **Intelligent bulk upload** with partial success handling
- 💾 Base64 image storage
- 📚 Swagger documentation
- 🗄️ Entity Framework Core with SQL Server
- 🏗️ Clean Architecture (Domain, Application, Infrastructure, API)

## Prerequisites

Before running the application, ensure you have the following installed:

### Required Software
- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server LocalDB** or **SQL Server** - [Download SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Visual Studio 2022** (recommended) or **Visual Studio Code**

### Database Requirements
The application uses **Microsoft SQL Server** with the following default configuration:
- **Server**: `(localdb)\mssqllocaldb` (SQL Server LocalDB)
- **Database**: `CustomerImageApiDb`
- **Authentication**: Windows Authentication (Trusted Connection)

## Getting Started

### 1. Clone the Repositorygit clone <repository-url>
cd CustomerImageSolution
### 2. Install Dependenciesdotnet restore
### 3. Configure Database Connection (Optional)
The application is pre-configured to use SQL Server LocalDB. If you need to use a different SQL Server instance, update the connection string in `src/CustomerImageApi.API/appsettings.json`:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CustomerImageApiDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
### 4. Setup Database
The application will automatically create and migrate the database on first run. Alternatively, you can manually run:
dotnet ef database update --project src\CustomerImageApi.Infrastructure --startup-project src\CustomerImageApi.API
### 5. Run the Application dotnet run --project src\CustomerImageApi.API --launch-profile https
### 6. Access the Application
- **API Base URL**: `https://localhost:7195` (or the port shown in console)
- **Swagger Documentation**: `https://localhost:7195/swagger`

## Default Login Credentials

The system comes with a pre-configured administrator account:

- **Email**: `administrator@localhost`
- **Password**: `Administrator1!`

### Authentication Flow
1. Use the `/api/auth/login` endpoint with the above credentials
2. The API will return a JWT access token and refresh token
3. Include the access token in subsequent requests using the `Authorization` header:Authorization: Bearer <your-access-token>
## Project Structuresrc/
├── CustomerImageApi.API/          # Web API controllers and configuration
├── CustomerImageApi.Application/  # Business logic and services
├── CustomerImageApi.Domain/       # Entities and domain interfaces
├── CustomerImageApi.DTOs/         # Data Transfer Objects
└── CustomerImageApi.Infrastructure/ # Data access and repositories
## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login with email and password |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Logout and revoke refresh token |
| GET | `/api/auth/validate` | Validate current token |
| GET | `/api/auth/me` | Get currently logged user details |

### Customers
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/customers` | Get all customers |
| GET | `/api/customers/{id}` | Get customer by ID |
| POST | `/api/customers` | Create new customer |
| PUT | `/api/customers/{id}` | Update customer |
| DELETE | `/api/customers/{id}` | Delete customer |

### Leads
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/leads` | Get all leads |
| GET | `/api/leads/{id}` | Get lead by ID |
| POST | `/api/leads` | Create new lead |
| PUT | `/api/leads/{id}` | Update lead |
| DELETE | `/api/leads/{id}` | Delete lead |

### Customer Images
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/customers/{customerId}/images` | Get all images for customer |
| POST | `/api/customers/{customerId}/images` | Upload single image to customer |
| **POST** | **`/api/customers/{customerId}/images/bulk`** | **Upload multiple images to customer** |
| DELETE | `/api/customers/{customerId}/images/{imageId}` | Delete customer image |

### Lead Images
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/leads/{leadId}/images` | Get all images for lead |
| POST | `/api/leads/{leadId}/images` | Upload single image to lead |
| **POST** | **`/api/leads/{leadId}/images/bulk`** | **Upload multiple images to lead** |
| DELETE | `/api/leads/{leadId}/images/{imageId}` | Delete lead image |
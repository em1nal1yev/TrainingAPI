👨‍💻 Maintainer

Development, architecture and design by Emin Alnaliyev
Email : eminal1y3@gmail.com




✨ Features

RESTful API endpoints for Users, Trainings, and Sessions

JWT-based authentication with role-based authorization

Swagger documentation

Clean/Onion Architecture (Controllers, Services, Repositories, Domain)

Email service for notifications (registration, password reset, etc.)

Environment-based configuration (Development, Production)





🛠 Tech Stack

Language: C#

Framework: ASP.NET Core 8

Database: MS SQL Server

ORM: Entity Framework Core

Authentication: JWT Bearer Tokens

Documentation: Swagger / OpenAPI





🏗 Architecture
TrainingAPI
├── Core/          # Domain Entities, Interfaces
├── Infrastructure/# Repositories, DbContext, Migrations
├── Application/   # Services, Business Logic, DTOs
├── API/           # Controllers, Endpoints, Program.cs


Separation of concerns ensures maintainability, testability, and scalability.





🚀 Getting Started
Prerequisites

.NET 8 SDK

SQL Server 

IDE: Visual Studio 2022 





Steps

1. Clone repository:

git clone https://github.com/em1nal1yev/TrainingAPI.git
cd TrainingAPI


2. Restore packages:

dotnet restore


3. Build project:

dotnet build


4. Apply migrations to your database:

dotnet ef database update


5. Run API:

dotnet run --project API/TrainingAPI.API.csproj


6. Open Swagger at:

https://localhost:5001/swagger/index.html





⚙ Configuration

Environment-based settings:

appsettings.Development.json → Local development

appsettings.Production.json → Production

Sensitive data such as connection strings, JWT secrets, and SMTP credentials should be stored in Environment Variables.

Example:

setx JWT__Key "YourSecretKeyHere"
setx ConnectionStrings__DefaultConnection "Server=.;Database=TrainingDb;Trusted_Connection=True;"
setx SMTP__Host "smtp.your-email.com"
setx SMTP__Port "587"
setx SMTP__Username "your-email@domain.com"
setx SMTP__Password "your-email-password"




🔑 Authentication & Authorization

JWT Bearer tokens

Role-based access control:

Admin – Full access

User – Limited access





🧪 Testing

Unit tests for services using xUnit

Integration tests for controllers and DB layer

Run tests:

dotnet test


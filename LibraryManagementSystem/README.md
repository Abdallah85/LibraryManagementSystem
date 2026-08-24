# Library Management System API

A comprehensive RESTful API for managing library operations, built with ASP.NET Core, Entity Framework Core, and PostgreSQL. The system provides user authentication, book management, borrowing transactions, and comprehensive access control.

## Quick Start

### Clone & Setup
```bash
git clone https://github.com/Abdallah85/LibraryManagementSystem.git
cd LibraryManagementSystem
dotnet restore
```

### Configure Database
Edit `LibraryManagementSystem/appsettings.json`:
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Host=localhost;Port=5432;Database=mydatabase;Username=admin;Password=admin"
  },
  "Jwt": {
	"Secret": "[Your-32-char-Base64-secret]",
	"Issuer": "LibrarySystem",
	"Audience": "LibrarySystemClient"
  }
}
```

### Run the API
```bash
dotnet run --project LibraryManagementSystem
```

**API Endpoints**:
- **HTTP**: `http://localhost:5167`
- **HTTPS**: `https://localhost:7152`
- **Swagger**: `https://localhost:7152/swagger/index.html`

---

## Project Overview

### Purpose
Backend service for managing library operations: user authentication, book cataloging, borrowing transactions, and access control.

### Main Features
- ✅ JWT-based authentication with refresh tokens
- ✅ User registration and login with secure password hashing
- ✅ Book catalog management (Categories, Languages, Publishers, Authors)
- ✅ Borrowing and return transaction tracking
- ✅ Role-based access control (Admin, Librarian, Staff, Member)
- ✅ Activity logging for audit trails
- ✅ Soft delete support
- ✅ Comprehensive error handling

### Tech Stack
- **Framework**: ASP.NET Core 10
- **Database**: PostgreSQL 12+
- **ORM**: Entity Framework Core
- **Authentication**: JWT with refresh tokens
- **API Docs**: Swagger/OpenAPI
- **Container**: Docker & Docker Compose

---

## Architecture

### Layered Architecture Pattern

```
┌─────────────────────────────────┐
│   Presentation Layer            │  Controllers (AuthController, etc.)
│                                 │
├─────────────────────────────────┤
│   Application Services Layer    │  Services (AuthService, CategoryService, etc.)
│                                 │
├─────────────────────────────────┤
│   Domain Layer                  │  Entities, Enums, Custom Exceptions
│                                 │
├─────────────────────────────────┤
│   Persistence Layer             │  DbContext, Repositories, UnitOfWork
│                                 │
└──────────────┬──────────────────┘
			   │
		PostgreSQL Database
```

### Projects

| Project | Purpose |
|---------|---------|
| **LibraryManagementSystemApi** | Main API, middleware, startup |
| **Presentation** | HTTP Controllers |
| **Services** | Business logic (CRUD, validation) |
| **ServicesAbstractions** | Service interfaces |
| **Domain** | Entities, enums, exceptions |
| **Persistence** | EF Core DbContext, repositories |
| **Shared** | DTOs, response models, configs |

---

## Database Setup

### Auto-Initialization
Database is **automatically created and seeded** on first run:
```bash
dotnet run  # Creates DB + tables + default roles
```

### Manual Migration (if needed)
```bash
# Create migration after model changes
dotnet ef migrations add [MigrationName] --project Persistence --startup-project LibraryManagementSystem

# Apply migrations
dotnet ef database update --startup-project LibraryManagementSystem
```

### Default Seeded Roles
- **Administrator** - Full access
- **Librarian** - Library staff
- **Staff** - General operations
- **Member** - Default user role

---

## API Endpoints

### Authentication (No token required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register new user → gets JWT + refresh token |
| `POST` | `/api/auth/login` | Login with email/username → gets JWT + refresh token |
| `POST` | `/api/auth/refresh-token` | Use refresh token → get new access token |

### Categories (Requires authentication)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/categories` | Create category |
| `GET` | `/api/categories` | Get all categories (pageable) |
| `GET` | `/api/categories/{id}` | Get category by ID |
| `PUT` | `/api/categories/{id}` | Update category |
| `DELETE` | `/api/categories/{id}` | Delete category |

### Languages (Requires authentication)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/languages` | Create language |
| `GET` | `/api/languages` | Get all languages (pageable) |
| `GET` | `/api/languages/{id}` | Get language by ID |
| `PUT` | `/api/languages/{id}` | Update language |
| `DELETE` | `/api/languages/{id}` | Delete language |

### Publishers (Requires authentication)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/publishers` | Create publisher |
| `GET` | `/api/publishers` | Get all publishers (pageable) |
| `GET` | `/api/publishers/{id}` | Get publisher by ID |
| `PUT` | `/api/publishers/{id}` | Update publisher |
| `DELETE` | `/api/publishers/{id}` | Delete publisher |

---

## Authentication & JWT

### Registration Example
```bash
curl -X POST https://localhost:7152/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
	"username": "john_doe",
	"email": "john@example.com",
	"password": "SecurePass123!"
  }'
```

**Response**:
```json
{
  "success": true,
  "message": "Registration successful.",
  "data": {
	"accessToken": "eyJhbGciOiJIUzI1NiIs...",
	"accessTokenExpiresAt": "2026-01-20T12:45:30Z",
	"refreshToken": "base64_encoded_token",
	"refreshTokenExpiresAt": "2026-01-27T12:45:30Z",
	"userId": "user-guid",
	"username": "john_doe",
	"role": "Member"
  }
}
```

### Using Token in Protected Endpoints
```bash
curl -X GET https://localhost:7152/api/categories \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### Token Details
- **Access Token**: Expires in 15 minutes
- **Refresh Token**: Expires in 7 days, hashed in DB
- **Algorithm**: HMAC SHA256
- **Issuer**: LibrarySystem
- **Audience**: LibrarySystemClient

### Swagger Authorization
1. Click **Authorize** button (top-right)
2. Enter: `Bearer YOUR_ACCESS_TOKEN`
3. All subsequent requests auto-include the token

---

## Configuration

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Host=localhost;Port=5432;Database=library;Username=admin;Password=admin"
  },
  "Jwt": {
	"Secret": "32-char-base64-encoded-secret",
	"Issuer": "LibrarySystem",
	"Audience": "LibrarySystemClient",
	"AccessTokenExpirationMinutes": 15,
	"RefreshTokenExpirationDays": 7
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  }
}
```

### Password Requirements
- Minimum 8 characters
- At least 1 uppercase letter (A-Z)
- At least 1 lowercase letter (a-z)
- At least 1 digit (0-9)
- At least 1 special character (!@#$%^&*)

---

## Error Handling

### Error Response Format
```json
{
  "errorCode": "NOT_FOUND",
  "message": "Category with id 999 not found",
  "details": null,
  "traceId": "0HN8LGIT5A9BU:00000001",
  "timestamp": "2026-01-20T12:30:45Z"
}
```

### HTTP Status Codes
| Status | Error Code | Scenario |
|--------|-----------|----------|
| **400** | BAD_REQUEST | Invalid input or business rule |
| **401** | UNAUTHORIZED | Missing/invalid JWT or credentials |
| **403** | FORBIDDEN | Insufficient permissions |
| **404** | NOT_FOUND | Resource doesn't exist |
| **409** | CONFLICT | Duplicate email/username/name |
| **500** | INTERNAL_SERVER_ERROR | Unexpected server error |

---

## Database Schema (Key Entities)

### User (ASP.NET Identity)
- Id (GUID)
- UserName, Email
- PasswordHash (hashed)
- IsActive, IsMember
- MembershipStatus

### Book
- Id (int)
- ISBN, Title, Edition
- PublicationYear
- Status (InLibrary, CheckedOut)
- LanguageId FK → Language
- PublisherId FK → Publisher

### Category
- Id (int)
- Name, Description

### Language
- Id (int)
- Name, Code (e.g., "en", "ar")

### Publisher
- Id (int)
- Name, Address, ContactEmail, Website

### Author
- Id (int)
- FullName, Bio

### Many-to-Many Junctions
- **BookAuthor**: Book ↔ Author
- **BookCategory**: Book ↔ Category

### Transactions & Audit
- **BorrowingTransaction**: Track checkouts/returns
- **ActivityLog**: Audit trail
- **RefreshToken**: Token management (hashed)

---

## Docker

### Build Image
```bash
docker build -t library-management-api:latest -f LibraryManagementSystem/Dockerfile .
```

### Run Container
```bash
docker run -d \
  --name library-api \
  -p 8080:8080 \
  -p 8081:8081 \
  -e "ConnectionStrings:DefaultConnection=Host=postgres;Port=5432;Database=library;Username=admin;Password=admin" \
  -e "Jwt:Secret=YOUR_JWT_SECRET" \
  library-management-api:latest
```

### Ports
- **HTTP**: 8080
- **HTTPS**: 8081

---

## Validation Rules

### RegisterRequest
- Username: 3-100 chars, required
- Email: Valid email format, required
- Password: 8+ chars, mixed case, digit, special char

### CreateCategoryDto
- Name: Required, Max 255 chars
- Description: Optional

### CreateLanguageDto
- Name: Required
- Code: Required (e.g., "en", "ar", "fr")

### CreatePublisherDto
- Name: Required
- Address, ContactEmail, Website: Optional

---

## Development

### Key Design Patterns
- **Repository Pattern**: Generic CRUD via `IGenericRepository<T>`
- **Unit of Work**: Transactional consistency
- **Specification Pattern**: Dynamic query building
- **Dependency Injection**: Loose coupling via interfaces
- **Service Layer**: Business logic encapsulation

### Important Implementation Details

1. **Refresh Token Security**
   - Tokens are hashed (SHA256) before storage
   - Only hash persists in DB (not raw token)
   - Auto-revoked after use

2. **Audit Trail**
   - CreatedBy, UpdatedBy track user IDs
   - CreatedAt, UpdatedAt auto-populated
   - Soft delete via IsDeleted flag

3. **JWT Claims**
   - sub: User ID
   - name: Username
   - email: Email
   - jti: Unique token ID
   - role: User roles (repeatable)

4. **Middleware Pipeline**
   - Swagger → HTTPS → Authentication → Authorization → Routing → Controllers → Exception Handler

### Known Limitations

⚠️ **Missing Features**:
- No pagination implementation (query params exist but ignored)
- No role-based endpoint authorization (all authenticated users have access)
- No Book/Author/BorrowingTransaction API endpoints
- Single configuration file (use env vars for secrets)

✅ **Recommendations**:
1. Implement `.Skip().Take()` for pagination
2. Add `[Authorize(Roles = "Administrator")]` to sensitive endpoints
3. Create full CRUD endpoints for all entities
4. Use Azure Key Vault or HashiCorp Vault for secrets
5. Add request logging middleware
6. Implement caching (Redis) for frequently accessed data

---

## Running in Development

### Prerequisites
- .NET SDK 10.0+
- PostgreSQL 12+
- Visual Studio/VS Code (optional)

### Steps
1. Clone: `git clone https://github.com/Abdallah85/LibraryManagementSystem.git`
2. Configure: Edit `appsettings.json` with DB connection
3. Restore: `dotnet restore`
4. Build: `dotnet build`
5. Run: `dotnet run --project LibraryManagementSystem`

### Verify Running
```bash
curl https://localhost:7152/swagger/index.html
# Should load Swagger UI
```

---

## API Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { /* actual data */ }
}
```

### Error Response
```json
{
  "errorCode": "NOT_FOUND",
  "message": "Resource not found",
  "details": null,
  "traceId": "unique-id",
  "timestamp": "2026-01-20T12:30:45Z"
}
```

---

## Security Considerations

✅ **Implemented**:
- Password hashing via ASP.NET Identity (PBKDF2)
- JWT signature verification
- Token expiration validation
- Refresh token revocation
- HTTPS enforcement
- SQL injection protection (EF Core parameterized queries)
- Soft deletes preserve data integrity

⚠️ **For Production**:
- Store secrets in Azure Key Vault or HashiCorp Vault
- Enable CORS only for trusted domains
- Implement rate limiting
- Add request logging/auditing
- Use HSTS headers
- Consider OAuth2/OpenID Connect for complex auth
- Regular security audits

---

## License

See LICENSE file in repository.

---

## Support

Issues or questions? Create a GitHub issue or contact the repository maintainer.

**Repository**: https://github.com/Abdallah85/LibraryManagementSystem  
**Last Updated**: January 2026  
**Framework Version**: .NET 10

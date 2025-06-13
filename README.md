# robot-controller-api
dotnet project
Here is a detailed `README.md` for your **robot-controller-api** project, reflecting all aspects of **Basic Authentication**, **User Management**, **Authorization**, and integration with **Entity Framework / PostgreSQL**:

---

```markdown
# 🤖 Robot Controller API

## Overview

The **Robot Controller API** is a secure backend service built with **ASP.NET Core Web API** that manages robot commands and maps while supporting user registration, authentication, and authorization. It uses **Basic Authentication**, **Role-based Authorization**, and **secure password hashing** with **EF Core** and **PostgreSQL** as the database backend.

---

## ✅ Features

- 🔐 Basic HTTP Authentication
- 🧑‍💼 User Registration, Login, and Management
- 🔑 Password hashing using `PasswordHasher<T>` (from `Microsoft.AspNetCore.Identity`)
- 🧾 Role-based access control with Authorization Policies
- 🗂️ Robot Command and Map Management
- 🐘 PostgreSQL integration using Entity Framework Core
- ⚙️ Swagger UI for API testing
- ✅ Postman Collection for automated testing

---

## 📁 Project Structure

```

robot-controller-api/
│
├── Controllers/
│   ├── UsersController.cs
│   ├── RobotCommandsController.cs
│   └── MapsController.cs
│
├── Persistence/
│   ├── UserDataAccess.cs
│   ├── RobotCommandEF.cs
│   ├── MapEF.cs
│   └── RobotContext.cs
│
├── Security/
│   └── BasicAuthenticationHandler.cs
│
├── Models/
│   ├── UserModel.cs
│   ├── LoginModel.cs
│   └── RobotCommand.cs
│
├── Program.cs
├── appsettings.json
├── db-schema.sql
└── README.md

````

---

## 🔧 Setup Instructions

### 1. 🐘 Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio](https://visualstudio.microsoft.com/) or VS Code
- (Optional) [Postman](https://www.postman.com/) for testing

---

### 2. ⚙️ Configure Database
Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=robotdb;Username=postgres;Password=yourpassword"
}
````

Apply the schema manually or via migration:

```sql
-- db-schema.sql
CREATE TABLE "user" (
  id SERIAL PRIMARY KEY,
  email TEXT UNIQUE NOT NULL,
  firstname TEXT NOT NULL,
  lastname TEXT NOT NULL,
  passwordhash TEXT NOT NULL,
  role TEXT NOT NULL,
  description TEXT,
  createddate TIMESTAMP NOT NULL DEFAULT NOW(),
  modifieddate TIMESTAMP NOT NULL DEFAULT NOW()
);
```

---

### 3. 🚀 Run the API

```bash
dotnet run
```

Access the Swagger UI:

```
https://localhost:<port>/swagger
```

---

## 🔐 Authentication

This project uses **Basic Authentication**. Supply credentials as Base64 encoded email and password.

Example header:

```
Authorization: Basic dXNlcjFAZXhhbXBsZS5jb206c2l0MzMxcGFzc3dvcmQ=
```

The `BasicAuthenticationHandler` validates the user from the database using a hashed password and issues a ClaimsPrincipal.

---

## 🛡️ Authorization Policies

Defined in `Program.cs`:

```csharp
options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
options.AddPolicy("UserOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
```

Apply policies via `[Authorize(Policy = "...")]` on endpoints.

---

## 📡 API Endpoints

### 🔐 User Management (via `UsersController.cs`)

| Method | Endpoint     | Description               | Access         |
| ------ | ------------ | ------------------------- | -------------- |
| GET    | /users       | Get all users             | AdminOnly      |
| GET    | /users/admin | Get Admins only           | AdminOnly      |
| GET    | /users/{id}  | Get user by ID            | UserOnly       |
| POST   | /users       | Register new user         | AllowAnonymous |
| PUT    | /users/{id}  | Update user (no email/pw) | UserOnly       |
| PATCH  | /users/{id}  | Change email/password     | UserOnly       |
| DELETE | /users/{id}  | Delete user               | AdminOnly      |

### 🧠 Robot Command Management

| Method | Endpoint        | Description       | Access    |
| ------ | --------------- | ----------------- | --------- |
| POST   | /robot-commands | Add new command   | AdminOnly |
| GET    | /robot-commands | List all commands | UserOnly  |

### 🗺️ Map Management

| Method | Endpoint | Description  | Access    |
| ------ | -------- | ------------ | --------- |
| GET    | /maps    | Get all maps | UserOnly  |
| POST   | /maps    | Add new map  | AdminOnly |

---

## 🔑 Password Hashing

We use `PasswordHasher<UserModel>`:

```csharp
var hasher = new PasswordHasher<UserModel>();
var pwHash = hasher.HashPassword(user, password);
var result = hasher.VerifyHashedPassword(user, pwHash, password);
```

Only the hash is stored in the DB for security.

---

## 🧪 Testing

Use **Swagger UI** or **Postman Collection** for API testing.

> You can also run Postman Collection with Newman CLI:

```bash
newman run postman-collection.json -e postman-environment.json --insecure
```

---

---

## 🎓 Author & Unit

* Unit: **SIT331 Full Stack Development**
* Task: **Practical Task 6.1 – Authentication & Authorization**
* Trimester: T1 2025
* Technologies: ASP.NET Core, Entity Framework Core, PostgreSQL

---

## 📚 Additional Resources

* [Microsoft Docs – Authentication in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
* [PasswordHasher<T> Usage Guide](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1)



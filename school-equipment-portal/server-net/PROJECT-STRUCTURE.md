# Project Structure

## Overview
The School Equipment Portal server project is now organized with a clean, maintainable folder structure following .NET best practices.

## 📁 Folder Structure

```
server-net/
├── Controllers/                  # API Controllers
│   ├── UsersController.cs       # Authentication endpoints
│   ├── EquipmentController.cs   # Equipment CRUD operations
│   ├── RequestsController.cs    # Request management
│   └── README.md               # Controllers documentation
├── DTOs/                        # Data Transfer Objects
│   ├── Models.cs               # Entity models (User, Equipment, etc.)
│   ├── RequestDtos.cs          # API input/output DTOs
│   ├── Data.cs                 # Data container class
│   ├── Helpers.cs              # Utility methods
│   └── README.md               # DTOs documentation
├── Modules/                     # Data Storage Modules
│   ├── IDataStore.cs           # Data storage interface
│   ├── FileDataStore.cs        # File-based storage implementation
│   ├── MongoDataStore.cs       # MongoDB storage implementation
│   └── README.md               # Modules documentation
├── Properties/                  # Project properties
├── bin/                        # Build output
├── obj/                        # Build cache
├── DataStore.cs                # Legacy DataStore (backward compatibility)
├── Program.cs                  # Application entry point
├── appsettings.json            # Configuration
├── appsettings.Development.json # Development configuration
├── SchoolEquipmentApi.csproj   # Project file
└── README-MongoDB.md           # MongoDB integration docs
```

## 🏗️ Architecture Benefits

### Clean Separation of Concerns
- **Controllers**: Handle HTTP requests/responses
- **DTOs**: Define data contracts and models
- **Modules**: Abstract data storage implementations
- **Configuration**: Environment-specific settings

### Scalability
- Easy to add new controllers
- Simple to extend DTOs
- Pluggable storage implementations
- Modular component design

### Maintainability
- Each file has single responsibility
- Consistent namespace organization
- Comprehensive documentation
- Follows .NET conventions

## 🔧 Technology Stack

- **.NET 7**: Web API framework
- **ASP.NET Core MVC**: Controller-based architecture
- **MongoDB**: Database integration (optional)
- **Dependency Injection**: Built-in DI container
- **Bearer Authentication**: Token-based security

## 🚀 Getting Started

1. **Install Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build Project**:
   ```bash
   dotnet build
   ```

3. **Run Application**:
   ```bash
   dotnet run --urls="http://localhost:5010"
   ```

## 📚 Documentation

- **Controllers/README.md**: API endpoint documentation
- **DTOs/README.md**: Data models and DTOs explanation
- **Modules/README.md**: Data storage implementations guide
- **README-MongoDB.md**: MongoDB integration guide

This structure provides a solid foundation for a production-ready .NET Web API application!
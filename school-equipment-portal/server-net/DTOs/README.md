# DTOs (Data Transfer Objects) Organization

This folder contains all the data models, DTOs, and utility classes used throughout the application.

## File Structure

### Models.cs
Contains the main entity models with MongoDB attributes:
- `User` - User account information
- `Session` - User authentication sessions  
- `Equipment` - Equipment inventory items
- `BorrowRequest` - Equipment borrowing requests

### RequestDtos.cs
Contains input/output DTOs for API endpoints:
- `SignupDto` - User registration data
- `LoginDto` - User login credentials
- `EquipmentInput` - Equipment creation/update data
- `RequestInput` - Request creation data

### Data.cs
Contains the data container class:
- `Data` - Main data structure for file-based storage compatibility

### Helpers.cs
Contains utility helper methods:
- `Helpers.GetBearer()` - Extract bearer token from HTTP request
- `Helpers.Overlaps()` - Check for date range overlaps

## Usage

All classes are in the `SchoolEquipmentApi.DTOs` namespace. Import them in your controllers and services:

```csharp
using SchoolEquipmentApi.DTOs;
```

## MongoDB Integration

The models include MongoDB BSON attributes for proper serialization:
- `[BsonId]` - Marks the primary key field
- `[BsonRepresentation(BsonType.ObjectId)]` - For ObjectId representation

This organization provides:
- ✅ Clean separation of concerns
- ✅ Easy maintenance and updates
- ✅ Consistent namespace organization
- ✅ Reusable across controllers and services
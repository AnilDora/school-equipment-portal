# Controllers

This folder contains all the API controllers for the School Equipment Portal application.

## Controller Files

### UsersController.cs
- **Route**: `/api/auth`
- **Purpose**: Handles user authentication and authorization
- **Endpoints**:
  - `POST /api/auth/signup` - User registration
  - `POST /api/auth/login` - User login
  - `GET /api/auth/me` - Get current user information

### EquipmentController.cs
- **Route**: `/api/equipment`
- **Purpose**: Manages equipment inventory (CRUD operations)
- **Endpoints**:
  - `GET /api/equipment` - Get all equipment
  - `GET /api/equipment/{id}` - Get specific equipment
  - `POST /api/equipment` - Create new equipment (admin only)
  - `PUT /api/equipment/{id}` - Update equipment (admin only)
  - `DELETE /api/equipment/{id}` - Delete equipment (admin only)

### RequestsController.cs
- **Route**: `/api/requests`
- **Purpose**: Handles equipment borrowing requests
- **Endpoints**:
  - `POST /api/requests` - Create new request
  - `GET /api/requests` - Get requests (all for admin/staff, own for students)
  - `PUT /api/requests/{id}/approve` - Approve request (admin/staff only)
  - `PUT /api/requests/{id}/reject` - Reject request (admin/staff only)
  - `PUT /api/requests/{id}/return` - Mark as returned

## Architecture

All controllers follow the ASP.NET Core MVC pattern:
- Inherit from `ControllerBase`
- Use dependency injection for `IDataStore`
- Include proper route attributes
- Handle authentication and authorization
- Return appropriate HTTP status codes

## Dependencies

- **DTOs**: All controllers use `SchoolEquipmentApi.DTOs` namespace
- **DataStore**: Injected `IDataStore` for data persistence
- **Authentication**: Token-based using `Helpers.GetBearer()`

## Security

- All endpoints require authentication (valid bearer token)
- Role-based authorization for admin/staff operations
- Input validation and error handling
- Conflict detection for overlapping requests
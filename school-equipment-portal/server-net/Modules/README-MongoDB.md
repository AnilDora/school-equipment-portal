# School Equipment Portal - MongoDB Integration

## Overview

The application now supports both file-based storage (JSON) and MongoDB for data persistence. The system automatically chooses the storage method based on configuration.

## Storage Options

### 1. File-based Storage (Default)
- Uses local JSON file (`data.json`)
- No external dependencies required
- Suitable for development and small deployments

### 2. MongoDB Storage
- Uses MongoDB database for persistence
- Better performance and scalability
- Suitable for production deployments

## Configuration

The application checks for a MongoDB connection string in the configuration. If found, it uses MongoDB; otherwise, it falls back to file-based storage.

### MongoDB Configuration

1. **appsettings.json** (Production):
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://your-mongodb-server:27017"
  }
}
```

2. **appsettings.Development.json** (Development):
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  }
}
```

3. **Environment Variable**:
```bash
set ConnectionStrings__MongoDB=mongodb://localhost:27017
```

## MongoDB Setup

### Local Development with MongoDB

1. **Install MongoDB locally**:
   - Download from: https://www.mongodb.com/try/download/community
   - Or use Docker: `docker run -d -p 27017:27017 --name mongodb mongo:latest`

2. **Configure connection string**:
   - Update `appsettings.Development.json` with your MongoDB connection string
   - Or leave empty to use file-based storage

### MongoDB Atlas (Cloud)

1. Create account at https://www.mongodb.com/atlas
2. Create a free cluster
3. Get connection string and update configuration
4. Whitelist your IP address

## Features

### Database Structure

The MongoDB implementation creates the following collections:
- `users` - User accounts and authentication
- `sessions` - User sessions and tokens
- `equipment` - Equipment inventory
- `requests` - Equipment borrow requests

### Indexes

The system automatically creates optimized indexes for:
- User lookups by username (unique)
- Session expiration cleanup
- Equipment filtering by category and availability
- Request queries by equipment, status, and requester

### Session Management

- Automatic cleanup of expired sessions
- Configurable session expiration (default: 7 days)
- Token-based authentication

## Data Migration

### From File to MongoDB

The current implementation maintains compatibility with the existing interface. To migrate:

1. Start with file-based storage (existing data.json)
2. Configure MongoDB connection string
3. Restart the application
4. The system will use MongoDB going forward

**Note**: Currently, there's no automatic migration tool. You'll need to recreate users and data in the new MongoDB setup.

## Development Notes

### Interface Compatibility

The `IDataStore` interface remains unchanged, ensuring compatibility with existing controllers and business logic.

### Performance Considerations

- MongoDB implementation includes optimized queries
- Automatic index creation for common lookup patterns
- Session cleanup runs on each data read operation

### Production Recommendations

1. Use MongoDB for production deployments
2. Configure proper MongoDB replica sets for high availability
3. Implement backup strategies
4. Monitor database performance and storage usage
5. Consider implementing connection pooling for high-traffic scenarios

## Troubleshooting

### Connection Issues

1. Verify MongoDB is running
2. Check connection string format
3. Ensure network connectivity
4. Verify authentication credentials if using secured MongoDB

### Fallback to File Storage

If MongoDB connection fails, the application will log an error and should be configured with fallback logic (this can be enhanced in future versions).

## Running the Application

```bash
# File-based storage (default)
dotnet run --urls="http://localhost:5010"

# MongoDB storage (with connection string configured)
dotnet run --urls="http://localhost:5010"
```

The application will log which storage method is being used on startup.
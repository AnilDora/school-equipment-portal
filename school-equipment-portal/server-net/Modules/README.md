# Modules

This folder contains the data storage implementations and core interfaces for the School Equipment Portal.

## Files Overview

### IDataStore.cs
- **Purpose**: Core interface for data storage abstraction
- **Methods**:
  - `Data Read()` - Read all data from storage
  - `void Write(Data data)` - Write all data to storage
- **Benefits**: Enables switching between different storage implementations

### FileDataStore.cs
- **Purpose**: File-based data storage implementation
- **Features**:
  - JSON file persistence
  - Thread-safe operations with locking
  - Atomic file operations (temp file + move)
  - Backup and restore functionality
  - Error handling and recovery
- **Use Case**: Development, small deployments, simple setups

### MongoDataStore.cs
- **Purpose**: MongoDB-based data storage implementation
- **Features**:
  - MongoDB driver integration
  - Automatic index creation for performance
  - Session cleanup and management
  - Database statistics monitoring
  - Comprehensive error handling
- **Use Case**: Production deployments, scalable applications

## Architecture Benefits

### 1. Separation of Concerns
- Data storage logic is isolated from business logic
- Controllers only depend on the IDataStore interface
- Easy to test with mock implementations

### 2. Storage Flexibility
- Switch between file and MongoDB storage via configuration
- No code changes needed in controllers
- Support for future storage implementations (Redis, SQL Server, etc.)

### 3. Enhanced Features

#### FileDataStore Enhancements:
- **Atomic Operations**: Prevents data corruption during writes
- **Backup System**: Create and restore data backups
- **Error Recovery**: Handles file corruption gracefully
- **Directory Management**: Auto-creates directories as needed

#### MongoDataStore Enhancements:
- **Performance Indexes**: Optimized queries for all collections
- **Session Management**: Automatic cleanup of expired sessions
- **Statistics**: Database monitoring capabilities
- **Error Handling**: Graceful degradation on connection issues

## Configuration

The application automatically selects the storage implementation:

```csharp
// In Program.cs
if (useMongoDb)
    builder.Services.AddSingleton<IDataStore>(_ => new MongoDataStore(connectionString));
else
    builder.Services.AddSingleton<IDataStore>(_ => new FileDataStore(dataFile));
```

## Usage Examples

### Basic Operations (Same for both implementations)
```csharp
// Inject in controller
public class MyController : ControllerBase
{
    private readonly IDataStore _dataStore;
    
    public MyController(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }
    
    // Read data
    var data = _dataStore.Read();
    
    // Modify data
    data.Users.Add(newUser);
    
    // Write data
    _dataStore.Write(data);
}
```

### File-Specific Operations
```csharp
if (_dataStore is FileDataStore fileStore)
{
    var backupPath = fileStore.CreateBackup();
    var fileInfo = fileStore.GetFileInfo();
}
```

### MongoDB-Specific Operations
```csharp
if (_dataStore is MongoDataStore mongoStore)
{
    var user = await mongoStore.GetUserByUsernameAsync("john");
    var stats = await mongoStore.GetDatabaseStatsAsync();
}
```

## Testing

The interface-based design makes testing easier:

```csharp
// Mock implementation for testing
public class MockDataStore : IDataStore
{
    private Data _data = new Data();
    
    public Data Read() => _data;
    public void Write(Data data) => _data = data;
}
```

## Migration Path

The old `DataStore` class is marked as obsolete but still works for backward compatibility:
- `DataStore` → `FileDataStore` (same functionality, enhanced features)
- New projects should use `FileDataStore` or `MongoDataStore` directly

This modular approach provides a solid foundation for scalable, maintainable data storage!
using MongoDB.Driver;
using MongoDB.Bson;
using SchoolEquipmentApi.DTOs;

namespace SchoolEquipmentApi.Modules
{
    /// <summary>
    /// MongoDB-based implementation of IDataStore
    /// Provides efficient database operations with proper indexing and session management
    /// </summary>
    public class MongoDataStore : IDataStore
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Session> _sessions;
        private readonly IMongoCollection<Equipment> _equipment;
        private readonly IMongoCollection<BorrowRequest> _requests;

        public MongoDataStore(string connectionString, string databaseName = "SchoolEquipmentDB")
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            
            _users = _database.GetCollection<User>("users");
            _sessions = _database.GetCollection<Session>("sessions");
            _equipment = _database.GetCollection<Equipment>("equipment");
            _requests = _database.GetCollection<BorrowRequest>("requests");
            
            // Create indexes for better performance
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            try
            {
                // User indexes
                var userIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.Username);
                var userIndexOptions = new CreateIndexOptions { Unique = true };
                _users.Indexes.CreateOne(new CreateIndexModel<User>(userIndexKeys, userIndexOptions));

                // Session indexes
                var sessionIndexKeys = Builders<Session>.IndexKeys.Ascending(s => s.ExpiresAt);
                _sessions.Indexes.CreateOne(new CreateIndexModel<Session>(sessionIndexKeys));

                // Equipment indexes
                var equipmentIndexKeys = Builders<Equipment>.IndexKeys
                    .Ascending(e => e.Category)
                    .Ascending(e => e.Available);
                _equipment.Indexes.CreateOne(new CreateIndexModel<Equipment>(equipmentIndexKeys));

                // Request indexes
                var requestIndexKeys = Builders<BorrowRequest>.IndexKeys
                    .Ascending(r => r.EquipmentId)
                    .Ascending(r => r.Status)
                    .Ascending(r => r.Requester);
                _requests.Indexes.CreateOne(new CreateIndexModel<BorrowRequest>(requestIndexKeys));
            }
            catch (Exception ex)
            {
                // Log index creation failures but don't stop the application
                Console.WriteLine($"Warning: Failed to create some MongoDB indexes: {ex.Message}");
            }
        }

        public Data Read()
        {
            // Clean up expired sessions
            CleanupExpiredSessions();

            var users = _users.Find(_ => true).ToList();
            var sessions = _sessions.Find(_ => true).ToList();
            var equipment = _equipment.Find(_ => true).ToList();
            var requests = _requests.Find(_ => true).ToList();

            // Convert sessions back to dictionary format for compatibility
            var sessionDict = sessions.ToDictionary(s => s.Token, s => new Session 
            { 
                Token = s.Token,
                Username = s.Username, 
                Role = s.Role,
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt
            });

            return new Data
            {
                Users = users,
                Sessions = sessionDict,
                Equipment = equipment,
                Requests = requests
            };
        }

        public void Write(Data data)
        {
            // This method maintains compatibility with the existing interface
            // Note: This is a simplified implementation. In production, you'd want
            // to implement more sophisticated synchronization logic
            
            // Clear existing data (not recommended for production)
            _users.DeleteMany(_ => true);
            _equipment.DeleteMany(_ => true);
            _requests.DeleteMany(_ => true);
            
            // Insert new data
            if (data.Users.Any())
                _users.InsertMany(data.Users);
                
            if (data.Equipment.Any())
                _equipment.InsertMany(data.Equipment);
                
            if (data.Requests.Any())
                _requests.InsertMany(data.Requests);
                
            // Handle sessions separately due to dictionary format
            _sessions.DeleteMany(_ => true);
            var sessions = data.Sessions.Select(kvp => new Session
            {
                Token = kvp.Key,
                Username = kvp.Value.Username,
                Role = kvp.Value.Role,
                CreatedAt = kvp.Value.CreatedAt,
                ExpiresAt = kvp.Value.ExpiresAt
            }).ToList();
            
            if (sessions.Any())
                _sessions.InsertMany(sessions);
        }

        private void CleanupExpiredSessions()
        {
            try
            {
                var expiredFilter = Builders<Session>.Filter.Lt(s => s.ExpiresAt, DateTime.UtcNow);
                _sessions.DeleteMany(expiredFilter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to cleanup expired sessions: {ex.Message}");
            }
        }

        // Additional methods for direct MongoDB operations (optional, for better performance)
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Id))
                user.Id = ObjectId.GenerateNewId().ToString();
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<Session> CreateSessionAsync(string token, string username, string role)
        {
            var session = new Session
            {
                Token = token,
                Username = username,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _sessions.InsertOneAsync(session);
            return session;
        }

        public async Task<Session?> GetSessionAsync(string token)
        {
            return await _sessions.Find(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Equipment>> GetEquipmentAsync()
        {
            return await _equipment.Find(_ => true).ToListAsync();
        }

        public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
        {
            if (string.IsNullOrEmpty(equipment.Id))
                equipment.Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x");
            await _equipment.InsertOneAsync(equipment);
            return equipment;
        }

        public async Task<Equipment?> UpdateEquipmentAsync(string id, Equipment equipment)
        {
            equipment.Id = id;
            var result = await _equipment.ReplaceOneAsync(e => e.Id == id, equipment);
            return result.ModifiedCount > 0 ? equipment : null;
        }

        public async Task<bool> DeleteEquipmentAsync(string id)
        {
            var result = await _equipment.DeleteOneAsync(e => e.Id == id);
            return result.DeletedCount > 0;
        }

        /// <summary>
        /// Get database statistics for monitoring
        /// </summary>
        public async Task<object> GetDatabaseStatsAsync()
        {
            try
            {
                var stats = await _database.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("dbStats", 1));
                return new
                {
                    DatabaseName = _database.DatabaseNamespace.DatabaseName,
                    Collections = stats.GetValue("collections", 0).ToInt32(),
                    Objects = stats.GetValue("objects", 0).ToInt64(),
                    DataSize = stats.GetValue("dataSize", 0).ToInt64(),
                    StorageSize = stats.GetValue("storageSize", 0).ToInt64()
                };
            }
            catch
            {
                return new { Error = "Unable to retrieve database statistics" };
            }
        }
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SchoolEquipmentApi.DTOs
{
    public class User 
    { 
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Username { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty; 
        public string Role { get; set; } = "student"; 
    }

    public class Session 
    { 
        [BsonId]
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty; 
        public string Role { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
    }

    public class Equipment 
    { 
        [BsonId]
        public string Id { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty; 
        public string Category { get; set; } = string.Empty; 
        public string Condition { get; set; } = string.Empty; 
        public int Quantity { get; set; } = 0; 
        public bool Available { get; set; } = true; 
    }

    public class BorrowRequest 
    { 
        [BsonId]
        public string Id { get; set; } = string.Empty; 
        public string EquipmentId { get; set; } = string.Empty; 
        public string Requester { get; set; } = string.Empty; 
        public string StartDate { get; set; } = string.Empty; 
        public string EndDate { get; set; } = string.Empty; 
        public string Status { get; set; } = "pending"; 
        public string? CreatedAt { get; set; }
        public string? Approver { get; set; } 
        public string? ApprovedAt { get; set; } 
        public string? RejectedAt { get; set; } 
        public string? ReturnedAt { get; set; } 
    }
}
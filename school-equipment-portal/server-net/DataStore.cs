using System.Text.Json;

// Data store and models used by the API. Kept minimal for the demo.
public class DataStore
{
    private readonly string _file;
    private readonly object _lock = new();
    public DataStore(string file) => _file = file;

    public Data Read()
    {
        lock (_lock)
        {
            if (!File.Exists(_file))
            {
                var init = new Data();
                var opts = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                File.WriteAllText(_file, JsonSerializer.Serialize(init, opts));
                return init;
            }
            var txt = File.ReadAllText(_file);
            var ropts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Data>(txt, ropts) ?? new Data();
        }
    }

    public void Write(Data data)
    {
        lock (_lock)
        {
            var opts = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            File.WriteAllText(_file, JsonSerializer.Serialize(data, opts));
        }
    }
}

public class Data
{
    public List<User> Users { get; set; } = new();
    public Dictionary<string, Session> Sessions { get; set; } = new();
    public List<Equipment> Equipment { get; set; } = new();
    public List<BorrowRequest> Requests { get; set; } = new();
}

public class User { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; public string Role { get; set; } = "student"; }
public class Session { public string Username { get; set; } = string.Empty; public string Role { get; set; } = string.Empty; }
public class Equipment { public string Id { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public string Category { get; set; } = string.Empty; public string Condition { get; set; } = string.Empty; public int Quantity { get; set; } = 0; public bool Available { get; set; } = true; }
public class BorrowRequest { public string Id { get; set; } = string.Empty; public string EquipmentId { get; set; } = string.Empty; public string Requester { get; set; } = string.Empty; public string StartDate { get; set; } = string.Empty; public string EndDate { get; set; } = string.Empty; public string Status { get; set; } = "pending"; public string? CreatedAt { get; set; } public string? Approver { get; set; } public string? ApprovedAt { get; set; } public string? RejectedAt { get; set; } public string? ReturnedAt { get; set; } }

public record SignupDto(string Username, string Password, string? Role);
public record LoginDto(string Username, string Password);
public record EquipmentInput(string Name, string? Category, string? Condition, int Quantity, bool Available);
public record RequestInput(string EquipmentId, string StartDate, string EndDate);

public static class Helpers
{
    public static string? GetBearer(Microsoft.AspNetCore.Http.HttpRequest req)
    {
        if (!req.Headers.TryGetValue("Authorization", out var v)) return null;
        var s = v.ToString();
        if (s.StartsWith("Bearer ")) return s.Substring(7);
        return s;
    }

    public static bool Overlaps(string s1, string e1, string s2, string e2)
    {
        if (!DateTimeOffset.TryParse(s1, out var a)) return false;
        if (!DateTimeOffset.TryParse(e1, out var b)) return false;
        if (!DateTimeOffset.TryParse(s2, out var c)) return false;
        if (!DateTimeOffset.TryParse(e2, out var d)) return false;
        return a <= d && c <= b;
    }
}

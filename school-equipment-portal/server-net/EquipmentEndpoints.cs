using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class EquipmentEndpoints
{
    public static void Map(WebApplication app, DataStore store)
    {
        app.MapGet("/api/equipment", (HttpRequest req) =>
        {
            var data = store.Read();
            return Results.Ok(data.Equipment);
        });

        app.MapPost("/api/equipment", (HttpRequest req, EquipmentInput input) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            if (session.Role != "admin") return Results.StatusCode(403);
            if (string.IsNullOrWhiteSpace(input.Name)) return Results.BadRequest(new { message = "Name required" });
            var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x");
            var item = new Equipment { Id = id, Name = input.Name, Category = input.Category ?? string.Empty, Condition = input.Condition ?? "Good", Quantity = input.Quantity, Available = input.Available };
            data.Equipment.Add(item);
            store.Write(data);
            return Results.Ok(item);
        });

        app.MapPut("/api/equipment/{id}", (HttpRequest req, string id, EquipmentInput input) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            if (session.Role != "admin") return Results.StatusCode(403);
            var idx = data.Equipment.FindIndex(e => e.Id == id);
            if (idx == -1) return Results.NotFound();
            var updated = data.Equipment[idx];
            updated.Name = input.Name ?? updated.Name;
            updated.Category = input.Category ?? updated.Category;
            updated.Condition = input.Condition ?? updated.Condition;
            updated.Quantity = input.Quantity;
            updated.Available = input.Available;
            data.Equipment[idx] = updated;
            store.Write(data);
            return Results.Ok(updated);
        });

        app.MapDelete("/api/equipment/{id}", (HttpRequest req, string id) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            if (session.Role != "admin") return Results.StatusCode(403);
            var idx = data.Equipment.FindIndex(e => e.Id == id);
            if (idx == -1) return Results.NotFound();
            data.Equipment.RemoveAt(idx);
            store.Write(data);
            return Results.Ok(new { success = true });
        });
    }
}

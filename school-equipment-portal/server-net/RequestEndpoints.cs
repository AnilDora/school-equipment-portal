using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class RequestEndpoints
{
    public static void Map(WebApplication app, DataStore store)
    {
        app.MapPost("/api/requests", (HttpRequest req, RequestInput input) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            if (string.IsNullOrWhiteSpace(input.EquipmentId) || string.IsNullOrWhiteSpace(input.StartDate) || string.IsNullOrWhiteSpace(input.EndDate))
                return Results.BadRequest(new { message = "equipmentId, startDate and endDate required" });
            var eq = data.Equipment.FirstOrDefault(x => x.Id == input.EquipmentId);
            if (eq == null) return Results.NotFound(new { message = "Equipment not found" });
            var approved = data.Requests.Where(r => r.EquipmentId == input.EquipmentId && r.Status == "approved").ToList();
            foreach (var a in approved)
            {
                if (Helpers.Overlaps(input.StartDate, input.EndDate, a.StartDate, a.EndDate)) return Results.Conflict(new { message = "Requested period overlaps with existing approved booking" });
            }
            var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x");
            var reqItem = new BorrowRequest { Id = id, EquipmentId = input.EquipmentId, Requester = session.Username, StartDate = input.StartDate, EndDate = input.EndDate, Status = "pending", CreatedAt = DateTimeOffset.UtcNow.ToString("o") };
            data.Requests.Add(reqItem);
            store.Write(data);
            return Results.Ok(reqItem);
        });

        app.MapGet("/api/requests", (HttpRequest req) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            if (session.Role == "admin" || session.Role == "staff") return Results.Ok(data.Requests);
            var mine = data.Requests.Where(r => r.Requester == session.Username).ToList();
            return Results.Ok(mine);
        });

        app.MapPut("/api/requests/{id}/approve", (HttpRequest req, string id) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            if (session.Role != "admin" && session.Role != "staff") return Results.StatusCode(403);
            var idx = data.Requests.FindIndex(r => r.Id == id);
            if (idx == -1) return Results.NotFound();
            var r = data.Requests[idx];
            var approved = data.Requests.Where(x => x.EquipmentId == r.EquipmentId && x.Status == "approved" && x.Id != r.Id).ToList();
            foreach (var a in approved)
            {
                if (Helpers.Overlaps(r.StartDate, r.EndDate, a.StartDate, a.EndDate)) return Results.Conflict(new { message = "Requested period overlaps with existing approved booking" });
            }
            r.Status = "approved";
            r.Approver = session.Username;
            r.ApprovedAt = DateTimeOffset.UtcNow.ToString("o");
            // decrement equipment quantity when approved
            var eqIdx = data.Equipment.FindIndex(e => e.Id == r.EquipmentId);
            if (eqIdx == -1) return Results.NotFound(new { message = "Equipment not found" });
            var eq = data.Equipment[eqIdx];
            if (eq.Quantity <= 0) return Results.Conflict(new { message = "No items available to approve this request" });
            eq.Quantity -= 1;
            if (eq.Quantity == 0) eq.Available = false;
            data.Equipment[eqIdx] = eq;

            data.Requests[idx] = r;
            store.Write(data);
            return Results.Ok(r);
        });

        app.MapPut("/api/requests/{id}/reject", (HttpRequest req, string id) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            if (session.Role != "admin" && session.Role != "staff") return Results.StatusCode(403);
            var idx = data.Requests.FindIndex(r => r.Id == id);
            if (idx == -1) return Results.NotFound();
            var r = data.Requests[idx];
            r.Status = "rejected";
            r.Approver = session.Username;
            r.RejectedAt = DateTimeOffset.UtcNow.ToString("o");
            data.Requests[idx] = r;
            store.Write(data);
            return Results.Ok(r);
        });

        app.MapPut("/api/requests/{id}/return", (HttpRequest req, string id) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            var idx = data.Requests.FindIndex(r => r.Id == id);
            if (idx == -1) return Results.NotFound();
            var r = data.Requests[idx];
            if (session.Role != "admin" && session.Role != "staff" && session.Username != r.Requester) return Results.StatusCode(403);
            r.Status = "returned";
            r.ReturnedAt = DateTimeOffset.UtcNow.ToString("o");
            // increment equipment quantity when returned
            var eqIdxR = data.Equipment.FindIndex(e => e.Id == r.EquipmentId);
            if (eqIdxR != -1)
            {
                var eqR = data.Equipment[eqIdxR];
                eqR.Quantity += 1;
                if (eqR.Quantity > 0) eqR.Available = true;
                data.Equipment[eqIdxR] = eqR;
            }

            data.Requests[idx] = r;
            store.Write(data);
            return Results.Ok(r);
        });
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class AuthEndpoints
{
    public static void Map(WebApplication app, DataStore store)
    {
        app.MapPost("/api/auth/signup", (SignupDto dto) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return Results.BadRequest(new { message = "username and password required" });

            var data = store.Read();
            if (data.Users.Any(u => u.Username == dto.Username))
                return Results.Conflict(new { message = "User exists" });

            var user = new User { Username = dto.Username, Password = dto.Password, Role = string.IsNullOrWhiteSpace(dto.Role) ? "student" : dto.Role };
            data.Users.Add(user);
            var token = Guid.NewGuid().ToString("N");
            data.Sessions[token] = new Session { Username = user.Username, Role = user.Role };
            store.Write(data);
            return Results.Ok(new { token, user = new { user.Username, user.Role } });
        });

        app.MapPost("/api/auth/login", (LoginDto dto) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return Results.BadRequest(new { message = "username and password required" });
            var data = store.Read();
            var found = data.Users.FirstOrDefault(u => u.Username == dto.Username && u.Password == dto.Password);
            if (found == null) return Results.Unauthorized();
            var token = Guid.NewGuid().ToString("N");
            data.Sessions[token] = new Session { Username = found.Username, Role = found.Role };
            store.Write(data);
            return Results.Ok(new { token, user = new { found.Username, found.Role } });
        });

        app.MapGet("/api/auth/me", (HttpRequest req) =>
        {
            var token = Helpers.GetBearer(req);
            var data = store.Read();
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) return Results.Unauthorized();
            return Results.Ok(new { user = session });
        });
    }
}

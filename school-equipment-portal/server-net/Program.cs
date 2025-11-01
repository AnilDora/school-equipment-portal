using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Allow browser clients (development) to call this API. Enables preflight (OPTIONS) handling.
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        // In development allow any origin/headers/methods. For production lock this down.
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Use camelCase for JSON responses so the React client (which expects camelCase) works correctly.
builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    opts.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS middleware so browser preflight (OPTIONS) requests succeed.
app.UseCors("LocalDev");

// Data file stored next to the running assembly
var dataFile = Path.Combine(AppContext.BaseDirectory, "data.json");
var store = new DataStore(dataFile);

// Map endpoints from separate classes
AuthEndpoints.Map(app, store);
EquipmentEndpoints.Map(app, store);
RequestEndpoints.Map(app, store);

app.Run();

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SchoolEquipmentApi.Modules;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
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
        // Configure data storage - MongoDB or File-based
        var connectionString = builder.Configuration.GetConnectionString("MongoDB");
        var useMongoDb = !string.IsNullOrEmpty(connectionString);
        
        if (useMongoDb)
        {
            Console.WriteLine("Using MongoDB for data storage");
            builder.Services.AddSingleton<IDataStore>(_ => new MongoDataStore(connectionString!));
        }
        else
        {
            Console.WriteLine("Using file-based storage for data");
            var dataFile = Path.Combine(AppContext.BaseDirectory, "data.json");
            builder.Services.AddSingleton<IDataStore>(_ => new FileDataStore(dataFile));
        }

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();

        // Enable CORS middleware so browser preflight (OPTIONS) requests succeed.
        app.UseCors("LocalDev");

        // Map controllers
        app.MapControllers();

        app.Run();
    }
}

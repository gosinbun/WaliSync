using Microsoft.EntityFrameworkCore;
using WaliSync.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup Database Context (SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=AcademicDb.db";

builder.Services.AddDbContext<AcademicDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// 2. Add Controllers
builder.Services.AddControllers();

// 3. Configure CORS for cross-device demo & local hybrid client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Initialize Database & Auto-Seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AcademicDbContext>();
    await SeedData.InitializeAsync(context);
}

// 5. Middleware Pipeline
app.UseCors("AllowAll");

app.MapControllers();

app.Run();


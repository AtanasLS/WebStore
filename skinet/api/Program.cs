using api.Extensions;
using api.Middleware;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

// Create a new scope for application services.
using var scope = app.Services.CreateScope();

// Get the service provider from the created scope.
var services = scope.ServiceProvider;

// Get the application's database context service.
var context = services.GetRequiredService<StoreContext>();

// Get a logger service for error handling.
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    // Attempt to asynchronously apply pending database migrations.
    await context.Database.MigrateAsync();
    // Attempt to asynchronously fill the database with seed data.
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    // Log any exceptions that occur during migration.
    logger.LogError(ex, "An error occurred during migration.");
}

app.Run();



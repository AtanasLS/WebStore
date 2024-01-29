using api.Extensions;
using api.Middleware;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Data.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseSwaggerDocumenatiton();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

// Create a new scope for application services.
using var scope = app.Services.CreateScope();

// Get the service provider from the created scope.
var services = scope.ServiceProvider;

// Get the application's database context service.
var context = services.GetRequiredService<StoreContext>();
var identityContext = services.GetRequiredService<AppIdentityDbContext>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();

// Get a logger service for error handling.
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    // Attempt to asynchronously apply pending database migrations.
    await context.Database.MigrateAsync();
    await identityContext.Database.MigrateAsync();
    // Attempt to asynchronously fill the database with seed data.
    await StoreContextSeed.SeedAsync(context);
    await AppIdentityDbContextSeed.SeedUserAsync(userManager);
}
catch (Exception ex)
{
    // Log any exceptions that occur during migration.
    logger.LogError(ex, "An error occurred during migration.");
}

app.Run();



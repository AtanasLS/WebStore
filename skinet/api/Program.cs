using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddDbContext<StoreContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


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



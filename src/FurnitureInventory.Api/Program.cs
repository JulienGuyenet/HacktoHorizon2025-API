using Microsoft.EntityFrameworkCore;
using FurnitureInventory.Core.Interfaces;
using FurnitureInventory.Infrastructure.Data;
using FurnitureInventory.Infrastructure.Repositories;
using FurnitureInventory.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuration de la base de données SQLite
builder.Services.AddDbContext<FurnitureInventoryContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=furnitureinventory.db"));

// Enregistrement des repositories
builder.Services.AddScoped<IFurnitureRepository, FurnitureRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IRfidTagRepository, RfidTagRepository>();
builder.Services.AddScoped<IRfidReaderRepository, RfidReaderRepository>();

// Enregistrement des services
builder.Services.AddScoped<IFurnitureService, FurnitureService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IRfidService, RfidService>();

builder.Services.AddControllers();

// Configuration de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Furniture Inventory API", 
        Version = "v1",
        Description = "API modulaire pour la gestion d'inventaire de meubles avec intégration RFID"
    });
});

var app = builder.Build();

// Création automatique de la base de données au démarrage
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FurnitureInventoryContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Furniture Inventory API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

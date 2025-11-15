using Microsoft.EntityFrameworkCore;
using FurnitureInventory.Core.Interfaces;
using FurnitureInventory.Infrastructure.Data;
using FurnitureInventory.Infrastructure.Repositories;
using FurnitureInventory.Infrastructure.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

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
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();

// Configuration de la localisation
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure supported cultures for localization
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("fr")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    
    // Request culture providers order:
    // 1. Query string (?culture=fr)
    // 2. Cookie
    // 3. Accept-Language header
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:9000")   // ton front
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // si besoin
    });
});

var app = builder.Build();

// Configure localization middleware
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

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

// MUST be before app.MapControllers()
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

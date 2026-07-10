using Microsoft.EntityFrameworkCore;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")
                  ?? "Data Source=tanitydzien.db"));

builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<MenuGenerator>();
builder.Services.AddScoped<ShoppingListService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string CorsPolicy = "web";
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p => p
    .WithOrigins("http://localhost:4200", "http://localhost:4201")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// baza + dane startowe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    SeedData.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(CorsPolicy);
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

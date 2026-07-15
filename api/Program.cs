using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaniTydzien.Api.Data;
using TaniTydzien.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")
                  ?? "Data Source=tanitydzien.db"));

builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<MenuGenerator>();
builder.Services.AddScoped<ShoppingListService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<IEmailSender, DevEmailSender>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Brak klucza Jwt:Key w konfiguracji."))),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

// Stripe — klucz może być pusty (płatności zwrócą wtedy czytelny błąd konfiguracji)
var stripeKey = builder.Configuration["Stripe:SecretKey"];
if (!string.IsNullOrWhiteSpace(stripeKey))
    Stripe.StripeConfiguration.ApiKey = stripeKey;

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o =>
    {
        // Frontend oczekuje błędów w formacie { message } — spłaszczamy ModelState.
        o.InvalidModelStateResponseFactory = ctx =>
        {
            var firstError = ctx.ModelState
                .Where(kv => kv.Value?.Errors.Count > 0)
                .Select(kv => kv.Value!.Errors[0].ErrorMessage)
                .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "Nieprawidłowe dane wejściowe.";
            return new BadRequestObjectResult(new { message = firstError });
        };
    });
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

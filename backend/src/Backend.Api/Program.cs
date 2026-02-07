using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using Backend.Api.Data;
using Backend.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task4 API", Version = "v1" });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Log connection string (masked) for debugging
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("CRITICAL: Connection string 'DefaultConnection' is null or empty!");
}
else
{
    Console.WriteLine($"Connection string found (length: {connectionString.Length}). Starts with: '{connectionString.Substring(0, Math.Min(connectionString.Length, 15))}...'");
}

// Convert PostgreSql URI to connection string if needed
// Render uses "postgres://" or "postgresql://"
if (!string.IsNullOrEmpty(connectionString) && (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://")))
{
    try 
    {
        var databaseUri = new Uri(connectionString);
        var userInfo = databaseUri.UserInfo.Split(':');
        var builderDb = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = Npgsql.SslMode.Require,
            TrustServerCertificate = true // Often needed for hosted DBs
        };
        connectionString = builderDb.ToString();
        Console.WriteLine("Successfully parsed Postgres URI format.");
    }
    catch (Exception ex)
    {
         Console.WriteLine($"Failed to parse Postgres URI: {ex.Message}");
    }
}

builder.Services.AddDbContext<Backend.Api.Data.AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<Backend.Api.Data.Repositories.IUserRepository, Backend.Api.Data.Repositories.UserRepository>();

// Services
builder.Services.AddScoped<Backend.Api.Services.IEmailService, Backend.Api.Services.EmailService>();
builder.Services.AddScoped<Backend.Api.Services.IAuthService, Backend.Api.Services.AuthService>();
builder.Services.AddScoped<Backend.Api.Services.IUserService, Backend.Api.Services.UserService>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // Lax for local dev with different ports
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Required for SameSite=None if we used that, but handy generally
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var frontendUrl = builder.Configuration["App:FrontendUrl"];
        if (string.IsNullOrEmpty(frontendUrl)) throw new InvalidOperationException("App:FrontendUrl is not configured");

        if (frontendUrl == "*")
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(frontendUrl)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Register UserStatusMiddleware
app.UseMiddleware<Backend.Api.Middleware.UserStatusMiddleware>();

app.MapControllers();

// Auto-migration on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeVault.Data;
using SafeVault.Models;
using SafeVault.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework with In-Memory database for demo
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                        options.UseInMemoryDatabase("SafeVaultDb"));

// Configure ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                                                            {
                                                                  options.Password.RequiredLength = 8;
                                                                  options.Password.RequireNonAlphanumeric = true;
                                                                  options.Password.RequireUppercase = true;
                                                                  options.Password.RequireLowercase = true;
                                                                  options.Password.RequireDigit = true;
                                                                  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                                                                  options.Lockout.MaxFailedAccessAttempts = 5;
                                                                  options.User.RequireUniqueEmail = true;
                                                            })
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "SafeVaultDefaultSecretKey!2024@#$";

builder.Services.AddAuthentication(options =>
                                   {
                                         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                   })
  .AddJwtBearer(options =>
                {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = jwtSettings["Issuer"] ?? "SafeVault",
                                ValidAudience = jwtSettings["Audience"] ?? "SafeVaultUsers",
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                                ClockSkew = TimeSpan.Zero
                        };
                });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
                                  {
                                        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                                        options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
                                        options.AddPolicy("RequireVerifiedEmail", policy =>
                                                                  policy.RequireClaim("email_verified", "true"));
                                  });

// Register application services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IInputValidationService, InputValidationService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

// Add CORS
builder.Services.AddCors(options =>
                         {
                               options.AddPolicy("AllowedOrigins", policy =>
                                                 {
                                                           policy.WithOrigins("https://localhost:3000", "https://safevault.example.com")
                                                                           .AllowAnyHeader()
                                                                           .AllowAnyMethod();
                                                 });
                         });

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
      await SeedData.Initialize(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
      app.UseSwagger();
      app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

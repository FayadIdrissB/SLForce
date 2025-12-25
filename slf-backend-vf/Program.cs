using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using slf_backend.Data;
using slf_backend.Entities;
using slf_backend.Middlewares;
using slf_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// ======================================
//  LOGGING (Serilog)
// ======================================
builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console()
      .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// ======================================
//  SERVICES
// ======================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ======================================
//  SWAGGER / OPENAPI
// ======================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SLForce API",
        Version = "v1",
        Description = "API backend pour SLForce"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ======================================
//  DATABASE (SQL Server)
// ======================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================
//  PASSWORD HASHER (Identity)
// ======================================
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ======================================
//  JWT AUTHENTICATION
// ======================================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("Configuration JWT manquante dans appsettings.json");
}

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ======================================
//  AUTHORIZATION & POLICIES
// ======================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
        policy.RequireClaim("IsAdmin", "True"));

    options.AddPolicy("IsCoach", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Coach"));

    options.AddPolicy("IsAthlete", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Athlete"));
});

// ======================================
//  RATE LIMITING
// ======================================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

// ======================================
//  CORS
// ======================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApp", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

// ======================================
//  APPLICATION SERVICES
// ======================================
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ITinodeService, TinodeService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IModerationService, ModerationService>();

// ======================================
//  BUILD APP
// ======================================
var app = builder.Build();

// ======================================
//  MIDDLEWARE PIPELINE
// ======================================

// Exception Handling (doit être en premier)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SLForce API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowApp");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


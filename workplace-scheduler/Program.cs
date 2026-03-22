using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Text;
using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using workplace_scheduler.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Swagger + JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //IMP: Swagger automatically add bearer word 
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

// PostgreSQL DbContext
var conn = builder.Configuration.GetConnectionString("postgresql")
          ?? builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<workplace_scheduler.Data.AppDbContext>(opt =>
    opt.UseNpgsql(conn));

// JWT Config
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection.GetValue<string>("Key") ?? throw new Exception("JWT Key missing");
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");

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
        ValidIssuer = issuer,

        ValidateAudience = true,
        ValidAudience = audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // optional but recommended
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var authHeader = ctx.Request.Headers["Authorization"].ToString();
            Console.WriteLine("Raw Header: " + authHeader);

            Console.WriteLine("Extracted Token: " + ctx.Token);
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("JWT auth failed: " + ctx.Exception?.Message);
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            Console.WriteLine("JWT challenge: " + ctx.Error + " - " + ctx.ErrorDescription);
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            Console.WriteLine("JWT validated for: " + ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);
            return Task.CompletedTask;
        }
    };
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoomService, RoomService>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
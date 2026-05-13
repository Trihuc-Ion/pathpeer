using System.Text;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PathPeer.Application.Common;
using PathPeer.Application.Features.Auth;
using PathPeer.Application.Features.Courses;
using PathPeer.Application.Features.Courses.Services;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Infrastructure.Persistence;
using PathPeer.Infrastructure.Persistence.Repositories;
using PathPeer.Infrastructure.Services;
using Serilog;

//Setup Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DbContext (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwt = builder.Configuration.GetSection("Jwt");

var key = jwt["Key"];
var issuer = jwt["Issuer"];
var audience = jwt["Audience"];

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key ??
                throw new InvalidOperationException("JWT Key lipsește din appsettings.json")))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["authToken"];
                return Task.CompletedTask;
            }
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Hangfire
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }));
builder.Services.AddHangfireServer();

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Enrollment
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Course
builder.Services.AddScoped<ICourseVoteRepository, CourseVoteRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ILessonBlockRepository, LessonBlockRepository>();

builder.Services.AddScoped<ICourseVoteService, CourseVoteService>();
builder.Services.AddScoped<ICourseService, CourseService>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();

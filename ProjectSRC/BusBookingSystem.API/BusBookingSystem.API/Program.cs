using System.Text;
using BusBookingSystem.Application;
using BusBookingSystem.Domain.AutoMapper;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryImplementation;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
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

// JWT configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });


/// Auto Mapper 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Database connection (Postgres)
var connectionString = builder.Configuration.GetConnectionString("DBConnString")
    ?? throw new InvalidOperationException("Database connection string 'DBConnString' not found.");

builder.Services.AddDbContext<DbBbsContext>(options =>
    options.UseNpgsql(connectionString));

// Register dependencies
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddTransient<IBBSAuth, BBSAuth>();
builder.Services.AddTransient<IBBSUser, BBSUser>();
builder.Services.AddTransient<IBBSBus, BBSBus>();
builder.Services.AddTransient<IBBSRoutes, BBSRoutes>();
builder.Services.AddTransient<IBBSSeats, BBSseats>();
builder.Services.AddTransient<IBBSbooking, BBSbooking>();
builder.Services.AddTransient<Iconfirmbooking, ConfirmBooking>();
builder.Services.AddTransient<BBSHashCode>();
builder.Services.AddSingleton<ErrorHandler>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logPath = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "logs");
    return new ErrorHandler(logPath);
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication(); // authentication before authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Security.Claims;
using System.Text;
using CbeMeetingAttendance.Data;
using CbeMeetingAttendance.Models;
using CbeMeetingAttendance.Repositories;
using CbeMeetingAttendance.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:80", "http://localhost", "https://twiddling-blissful-traverse.ngrok-free.dev")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            // This tells EF Core to automatically retry if the DB is still warming up
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

builder.Services
    .AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        // Password
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;

        // User
        options.User.RequireUniqueEmail = true;

        // Lockout
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT ERROR: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],

            ValidAudiences = jwtSettings
                .GetSection("Audiences")
                .Get<string[]>(),

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            ),

            ClockSkew = TimeSpan.Zero,

            RoleClaimType = ClaimTypes.Role
        };
    });

// Application Repositories & Services
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<ProfileRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RefreshTokenRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<AttendanceRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ExportService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Swagger / OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Explicitly qualify Microsoft.OpenApi types to resolve namespace collision
    var securityScheme = new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your JWT token directly in the input box below.",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    // Apply security globally using Swashbuckle 10 delegate syntax
    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CbeMeetingAttendance API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowFrontend");

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();

    // Apply pending migrations
    db.Database.Migrate();

    // Seed roles and admin user
    await IdentitySeeder.SeedAsync(services);
}

app.Run();
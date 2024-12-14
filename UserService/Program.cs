using Microsoft.EntityFrameworkCore;
using MassTransit;
using UserService.Data;
using UserService.Models;
using System.Threading;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.Services;
using UserService.Events;

// Configure the builder and services
var builder = WebApplication.CreateBuilder(args);

// Set up DbContext for MariaDB
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MariaDb"),
    new MariaDbServerVersion(new Version(10, 5, 9))));
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    var assem = typeof(Program).Assembly;
    x.AddConsumers(assem);
    x.AddSagaStateMachines(assem);
    x.AddSagas(assem);
    x.AddActivities(assem);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h =>
        {
            h.Username("user");
            h.Password("password");
        });

        
        
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ProductService API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer prefix",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Key"]);

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
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "admin"));
});

var app = builder.Build();

// Check MariaDB container readiness, apply migrations, and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<UserDbContext>();

    // Wait for MariaDB to be ready
    var isDatabaseReady = false;
    var retries = 5;
    while (!isDatabaseReady && retries > 0)
    {
        try
        {
            // Try connecting to the database to verify readiness
            context.Database.ExecuteSqlRaw("SELECT 1");
            isDatabaseReady = true;
        }
        catch
        {
            retries--;
            Thread.Sleep(2000); // Wait 2 seconds before retrying
        }
    }

    if (!isDatabaseReady)
    {
        throw new Exception("Unable to connect to MariaDB after multiple retries.");
    }

    // Apply pending migrations and seed the admin user
    context.Database.Migrate();
    SeedAdminUser(context);
}

// Swagger setup for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

// Seed admin user with hashed password
void SeedAdminUser(UserDbContext context)
{
    // Check if any users exist
    if (!context.Users.Any())
    {
        // Hash the admin password
        var password = "AdminPassword123"; // Change this password for production!
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // Create the initial admin user
        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = passwordHash,
            Role = "admin"
        };

        // Add and save the admin user to the database
        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

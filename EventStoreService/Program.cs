using Microsoft.EntityFrameworkCore;
using EventStoreService.Data;
using EventStoreService.Models;
using MassTransit;
using EventStoreService.Consumers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext for MariaDB
builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MariaDb"), new MariaDbServerVersion(new Version(10, 5, 9))));
builder.Services.AddScoped<UserCreatedConsumer>();
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

        
        cfg.ReceiveEndpoint("usercreated", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
        });
        cfg.ReceiveEndpoint("productcreated", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(context);
        });
    });
});
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
builder.Services.AddControllers();

var app = builder.Build();

// Apply migrations at startup to create EventStore tables
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
    dbContext.Database.Migrate();
}
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();

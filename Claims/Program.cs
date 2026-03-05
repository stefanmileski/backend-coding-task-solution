using Claims.Core.Clock;
using Claims.Infrastructure;
using Claims.Infrastructure.Auditing;
using Claims.Infrastructure.Auditing.Interfaces;
using Claims.Infrastructure.Interfaces;
using Claims.Services.Claim;
using Claims.Services.Claim.Interfaces;
using Claims.Services.Cover;
using Claims.Services.Cover.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

var builder = WebApplication.CreateBuilder(args);

string? sqlConnectionString;
string? mongoConnectionString;

if (builder.Configuration.GetValue<bool>("UseTestContainers"))
{
    // Start Testcontainers for SQL Server and MongoDB
    var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            : new MsSqlBuilder()
        ).Build();

    var mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .Build();

    await sqlContainer.StartAsync();
    await mongoContainer.StartAsync();

    sqlConnectionString = sqlContainer.GetConnectionString();
    mongoConnectionString = mongoContainer.GetConnectionString();
}
else
{
    sqlConnectionString = builder.Configuration.GetConnectionString("SqlServer");
    mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
}

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(sqlConnectionString));

builder.Services.AddDbContext<ClaimsContext>(options =>
{
    var client = new MongoClient(mongoConnectionString);
    var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]); // Use a default/test database name
    options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddScoped<ICoversService, CoversService>();
builder.Services.AddScoped<IClaimsContext, ClaimsContext>();
builder.Services.AddScoped<IAuditer, Auditer>();

builder.Services.AddSingleton<IAuditQueue, AuditQueue>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddHostedService<AuditWorker>();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }

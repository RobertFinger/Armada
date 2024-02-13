using RabbitMQ.Client;
using DataManager.Services;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using YourNamespace.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();


builder.Services.AddSingleton<IAsyncConnectionFactory, ConnectionFactory>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddTransient<IAsyncConnectionFactory, ConnectionFactory>();
builder.Services.AddHostedService<DataManagerReceiver>();
builder.Services.AddScoped<DataManagerSender>();


var dbConnection = builder.Configuration[AuthConstants.DatabaseConnectionString];

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

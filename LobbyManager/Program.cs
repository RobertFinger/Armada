using LobbyManager.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddTransient<IAsyncConnectionFactory, ConnectionFactory>();
builder.Services.AddSingleton<LobbyReceiver>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var rabbitMQReceiver = app.Services.GetService<LobbyReceiver>();
rabbitMQReceiver?.StartListening(); 

app.MapControllers();

app.Run();

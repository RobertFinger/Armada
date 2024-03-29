using RabbitMQ.Client;
using PaymentManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddTransient<IAsyncConnectionFactory, ConnectionFactory>();
builder.Services.AddSingleton<PaymentsReceiver>();
builder.Services.AddSingleton<PaymentSender>();
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

var rabbitMQReceiver = app.Services.GetService<PaymentsReceiver>();
rabbitMQReceiver?.StartListening();

app.MapControllers();

app.Run();

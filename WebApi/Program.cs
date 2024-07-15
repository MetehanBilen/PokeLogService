using Business;
using Core;
using DataAccess.Concrete;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//---
builder.Services.AddSingleton(sp => new ConnectionFactory()
{
    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")),
    DispatchConsumersAsync = true
});

builder.Services.AddSingleton<IModel>(serviceProvider =>
{
    var rabbitMQClientService = serviceProvider.GetRequiredService<RabbitMQClientService>();
    return rabbitMQClientService.Connect();
});
builder.Services.AddElastic(builder.Configuration);
builder.Services.AddScoped<ESLogDal>();
builder.Services.AddScoped<LogService>();

builder.Services.AddSingleton<RabbitMQClientService>();

builder.Services.AddHostedService<LogSubsService>();




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

app.Run();

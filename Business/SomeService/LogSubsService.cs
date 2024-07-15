using Business;
using Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Services;

public class LogSubsService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public LogSubsService(RabbitMQClientService rabbitMQClientService, IModel channel, IServiceProvider serviceProvider)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _channel = channel;
        _serviceProvider = serviceProvider;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMQClientService.Connect();
        _channel.BasicQos(0, 1, false);

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            LogModel logModel;
            try
            {
                logModel = JsonSerializer.Deserialize<LogModel>(message);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize message: {ex.Message}");
                _channel.BasicNack(ea.DeliveryTag, false, false);
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var logService = scope.ServiceProvider.GetRequiredService<LogService>();
                var success = await logService.SaveAsync(logModel);

                if (success)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            }

            Console.WriteLine($"Processed message: {message}");

            await Task.Yield();
        };

        _channel.BasicConsume(queue: RabbitMQClientService.QueueName, autoAck: false, consumer: consumer);

        Console.WriteLine("Listening for logs. Press any key to exit.");

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _rabbitMQClientService.Dispose();
        return base.StopAsync(cancellationToken);
    }
}



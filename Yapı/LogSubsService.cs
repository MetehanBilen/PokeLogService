using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Services;

public class LogSubsService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly RabbitMQClientService _rabbitMQClientService;
    private IModel _channel;

    public LogSubsService(RabbitMQClientService rabbitMQClientService, IModel channel)
    {
        _rabbitMQClientService = rabbitMQClientService;
        _channel = channel;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMQClientService.Connect();
        _channel.BasicQos(0, 1, false);

        return base.StartAsync(cancellationToken);
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine(message);
        };

        _channel.BasicConsume(queue: RabbitMQClientService.QueueName, autoAck: true, consumer: consumer);

        Console.WriteLine("Listening for logs. Press any key to exit.");


        return Task.CompletedTask;
    }
}

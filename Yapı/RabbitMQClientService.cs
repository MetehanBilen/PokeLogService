using Elastic.Clients.Elasticsearch.Graph;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Services;

public class RabbitMQClientService : IDisposable
{


    private readonly ConnectionFactory _connectionFactory;
    private  IConnection _connection;
    private  IModel _channel;


    public static string ExchangeName = "log-Exchange-0807";
    public static string routingKey = "PokeLog";
    public static string QueueName = "queue-watermark-image";

    public RabbitMQClientService(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IModel Connect()
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(ExchangeName, "direct", true, false);

        _channel.QueueDeclare(QueueName, true, false, false, null);

        _channel.QueueBind(QueueName, ExchangeName, routingKey, null);

        Console.WriteLine("RabbitMQ ile baglantı kuruldu.");

        return _channel;

    }
    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();


        _connection?.Close();
        _connection?.Dispose();
        Console.WriteLine("RabbitMQ ile baglantı koptu.");
    }
}

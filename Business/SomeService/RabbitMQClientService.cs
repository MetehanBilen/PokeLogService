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
    private IConnection _connection;
    private IModel _channel;

    public static string ExchangeName = "log-Exchange-0807";
    public static string RoutingKey = "PokeLog";
    public static string QueueName = "pokeQueue";

    public RabbitMQClientService(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _connection = _connectionFactory.CreateConnection(); // Ensure the connection is created here
    }

    public IModel Connect()
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        _channel = _connection.CreateModel(); // Now _connection is guaranteed to be non-null
        _channel.ExchangeDeclare(ExchangeName, "direct", true, false);
        _channel.QueueDeclare(QueueName, true, false, false, null);
        _channel.QueueBind(QueueName, ExchangeName, RoutingKey, null);

        Console.WriteLine("RabbitMQ ile bağlantı kuruldu.");

        return _channel;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();

        _connection?.Close();
        _connection?.Dispose();
        Console.WriteLine("RabbitMQ ile bağlantı koptu.");
    }
}

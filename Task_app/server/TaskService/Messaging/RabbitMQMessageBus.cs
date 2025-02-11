// RabbitMQMessageBus.cs
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace TaskService.Messaging
{
public interface IMessageBus
{
    void PublishTaskEvent(string eventType, object eventData);
}

public class RabbitMQMessageBus : IMessageBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQMessageBus(IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "task.events", type: ExchangeType.Fanout);
    }

    public void PublishTaskEvent(string eventType, object eventData)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventData));

        _channel.BasicPublish(
            exchange: "task.events",
            routingKey: "",
            basicProperties: null,
            body: body
        );
    }
}
}

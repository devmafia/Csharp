// RabbitMQMessageBus.cs
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;

namespace UserService.Messaging
{
public interface IMessageBus
{
    void PublishUserEvent(string eventType, object eventData);
}

public class RabbitMQMessageBus : IMessageBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQMessageBus(IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "user.events", type: ExchangeType.Fanout);
    }

    public void PublishUserEvent(string eventType, object eventData)
    {
        var message = new { EventType = eventType, Data = eventData, Timestamp = DateTime.UtcNow };
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        _channel.BasicPublish(
            exchange: "task.events",
            routingKey: "",
            basicProperties: null,
            body: body
        );
    }
}
}

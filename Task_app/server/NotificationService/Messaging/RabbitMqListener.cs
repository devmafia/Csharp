using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
// using FirebaseAdmin.Auth;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NotificationService.Hubs;
using Microsoft.AspNetCore.Connections;
using System.Text.Json.Serialization;

namespace NotificationService.Messaging
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly NotificationRepository _repository;

        public NotificationController(IHubContext<NotificationHub> hubContext, NotificationRepository repository)
        {
            _hubContext = hubContext;
            _repository = repository;
        }

        // [Authorize]
        // [HttpGet]
        // public async Task<IActionResult> GetUserNotifications()
        // {
        //     var userId = User.FindFirst("user_id")?.Value;
        //     if (string.IsNullOrEmpty(userId)) return Unauthorized();
        //     var notifications = await _repository.GetUserNotificationsAsync(userId);
        //     return Ok(notifications);
        // }
        // [HttpGet]
        // public async Task<IActionResult> GetUserNotifications()
        // {
        //     var userId = User.FindFirst("user_id")?.Value;
        //     if (string.IsNullOrEmpty(userId)) return Unauthorized();
        //     var notifications = await _repository.GetNotificationsAsync(userId);
        //     return Ok(notifications);
        // }
    }

    public class RabbitMqListener : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly NotificationRepository _repository;

        public RabbitMqListener(RabbitMQ.Client.IConnectionFactory connectionFactory,IHubContext<NotificationHub> hubContext, NotificationRepository repository)
        {
            _hubContext = hubContext;
            _repository = repository;
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "task.events", type: "fanout");
            _channel.ExchangeDeclare(exchange: "user.events", type: "fanout");

            _channel.QueueDeclare(queue: "notification_tasks", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "notification_users", durable: true, exclusive: false, autoDelete: false, arguments: null);

            _channel.QueueBind("notification_tasks", "task.events", "");
            _channel.QueueBind("notification_users", "user.events", "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var taskConsumer = new AsyncEventingBasicConsumer(_channel);
            taskConsumer.Received += async (model, ea) => await ProcessEvent(ea, "task");
            _channel.BasicConsume(queue: "notification_tasks", autoAck: true, consumer: taskConsumer);

            var userConsumer = new AsyncEventingBasicConsumer(_channel);
            userConsumer.Received += async (model, ea) => await ProcessEvent(ea, "user");
            _channel.BasicConsume(queue: "notification_users", autoAck: true, consumer: userConsumer);

            return Task.CompletedTask;
        }

        private async Task ProcessEvent(BasicDeliverEventArgs ea, string eventType)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var taskEvent = JsonSerializer.Deserialize<TaskNotificationEvent>(message);
            if (taskEvent == null) return;

            await _repository.SaveNotificationAsync(taskEvent);

            switch (taskEvent.EventType)
            {
                case "TaskCreated":
                    await _hubContext.Clients.User(taskEvent.UserId).SendAsync("TaskCreated", taskEvent);
                    break;
                case "TaskUpdated":
                    await _hubContext.Clients.User(taskEvent.UserId).SendAsync("TaskUpdated", taskEvent);
                    break;
                case "TaskDeleted":
                    await _hubContext.Clients.User(taskEvent.UserId).SendAsync("TaskDeleted", taskEvent.Id);
                    break;
                default:
                    await _hubContext.Clients.User(taskEvent.UserId).SendAsync("ReceiveNotification", taskEvent);
                    break;
            }
        }
    }

    public class NotificationRepository
    {
        private readonly List<TaskNotificationEvent> _notifications = new();

        public Task SaveNotificationAsync(TaskNotificationEvent notification)
        {
            _notifications.Add(notification);
            return Task.CompletedTask;
        }

        public Task<List<TaskNotificationEvent>> GetNotificationsAsync()
        {
            return Task.FromResult(_notifications.ToList());
        }
    }

    public class NotificationEvent
    {
        public string EventType { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Timestamp { get; set; }
    }

    public class TaskNotificationEvent
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }

}

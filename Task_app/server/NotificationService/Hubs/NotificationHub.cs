using Microsoft.AspNetCore.SignalR;
using NotificationService.Messaging;

namespace NotificationService.Hubs;
public class NotificationHub : Hub
{  public async Task SendNotification(TaskNotificationEvent eventData)
{
    await Clients.User(eventData.UserId).SendAsync("ReceiveNotification", eventData);


}

}

// CRUD Controller
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskService.Data;
using TaskService.Models;
using TaskService.Dtos;
using TaskService.Messaging;
using Microsoft.AspNetCore.Authorization;

namespace TaskService.Controllers;

[Route("api/tasks")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly TaskDbContext _context;
    private readonly IMessageBus _messageBus;

    public TaskController(IMessageBus messageBus,TaskDbContext context)
    {
        _context = context;
        _messageBus = messageBus;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTasks()
    {
        var userId = User.FindFirst("user_id")?.Value;
        Console.WriteLine("WWWW"+ userId);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var userTasks = await _context.Tasks
            .Where(t => t.UserId == userId)
            .ToListAsync();
        return Ok(userTasks);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        var userId = User.FindFirst("user_id")?.Value;
        if (task.UserId != userId)
            return Forbid();

        return Ok(task);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTask(CreateTaskDto task)
    {
        var userId = User.FindFirst("user_id")?.Value;
        var new_task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            UserId = userId
        };
        _context.Tasks.Add(new_task);
        await _context.SaveChangesAsync();
        var eventData = new TaskNotificationEvent
            {
                Id = new_task.Id,
                EventType = "TaskCreated",
                Title = new_task.Title,
                Description = new_task.Description,
                Timestamp = DateTime.UtcNow.ToString("o"), // Or use newTask.CreatedAt if available
                UserId = userId
            };

    _messageBus.PublishTaskEvent("TASK_CREATED", eventData);

        return CreatedAtAction(nameof(GetTask), new { id = new_task.Id }, new_task);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;

        await _context.SaveChangesAsync();
        var userId = User.FindFirst("user_id")?.Value;
        var eventData = new TaskNotificationEvent
          {
              Id = task.Id,
              EventType = "TaskUpdated",
              Title = task.Title,
              Description = task.Description,
              Timestamp = DateTime.UtcNow.ToString("o"), // Or use newTask.CreatedAt if available
              UserId = userId
          };

        _messageBus.PublishTaskEvent("TASK_UPDATED", eventData);
        return NoContent();
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        var userId = User.FindFirst("user_id")?.Value;
        var eventData = new TaskNotificationEvent
            {
                Id = task.Id,
                EventType = "TaskUpdated",
                Title = task.Title,
                Description = task.Description,
                Timestamp = DateTime.UtcNow.ToString("o"),
                UserId = userId // Or use newTask.CreatedAt if available
            };
        _messageBus.PublishTaskEvent("TASK_DELETED", eventData);

        return NoContent();
    }
}

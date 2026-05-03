using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = "Admin")]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _db;
    public NotificationController(AppDbContext db) => _db = db;

    [HttpPost("delay")]
    public async Task<IActionResult> TriggerDelayAlert(NotificationDto dto)
    {
        var notification = new Notification { RecipientType = dto.RecipientType, RecipientId = dto.RecipientId, NotificationType = "Delay", Message = dto.Message, Channel = dto.Channel, DeliveryStatus = "Sent" };
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
        return Ok(new { notification.Id, notification.NotificationType, notification.DeliveryStatus });
    }

    [HttpPost("route-change")]
    public async Task<IActionResult> TriggerRouteChangeAlert(NotificationDto dto)
    {
        var notification = new Notification { RecipientType = dto.RecipientType, RecipientId = dto.RecipientId, NotificationType = "RouteChange", Message = dto.Message, Channel = dto.Channel, DeliveryStatus = "Sent" };
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
        return Ok(new { notification.Id, notification.NotificationType, notification.DeliveryStatus });
    }

    [HttpPost("payment-confirmation")]
    public async Task<IActionResult> SendPaymentConfirmation(NotificationDto dto)
    {
        var notification = new Notification { RecipientType = dto.RecipientType, RecipientId = dto.RecipientId, NotificationType = "Payment", Message = dto.Message, Channel = dto.Channel, DeliveryStatus = "Sent" };
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
        return Ok(new { notification.Id, notification.NotificationType, notification.DeliveryStatus });
    }

    [HttpGet("{recipientType}/{recipientId}")]
    public async Task<IActionResult> GetUserNotifications(string recipientType, int recipientId)
    {
        var notifications = await _db.Notifications
            .Where(n => n.RecipientType == recipientType && n.RecipientId == recipientId)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
        return Ok(notifications);
    }
}

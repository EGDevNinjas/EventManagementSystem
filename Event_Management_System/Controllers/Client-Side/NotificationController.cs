using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventManagementSystem.BLL.Services;

namespace EventManagementSystem.API.Controllers.Client_Side
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // 🧠 Helper: Get current user ID from claims
        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // ✅ Get all notifications for user
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            int userId = GetUserId();
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            return Ok(notifications);
        }

        // ✅ Get unread notification count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            int userId = GetUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { unreadCount = count });
        }

        // ✅ Mark a specific notification as read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var success = await _notificationService.MarkAsReadAsync(id);
            return success ? NoContent() : NotFound();
        }

        // ✅ Mark all as read
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            int userId = GetUserId();
            var updatedCount = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { updatedCount });
        }

        // ✅ Delete single notification
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var success = await _notificationService.DeleteNotificationAsync(id);
            return success ? NoContent() : NotFound();
        }

        // ✅ Delete all notifications
        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            int userId = GetUserId();
            await _notificationService.DeleteAllNotificationsAsync(userId);
            return NoContent();
        }
    }
}

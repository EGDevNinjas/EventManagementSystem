using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.DAL.Contexts;
using EventManagementSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        var notif = await _context.Notifications.FindAsync(notificationId);
        if (notif == null)
            return false;

        notif.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notif in notifications)
        {
            notif.IsRead = true;
        }

        await _context.SaveChangesAsync();
        return notifications.Count;
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId)
    {
        var notif = await _context.Notifications.FindAsync(notificationId);
        if (notif == null)
            return false;

        _context.Notifications.Remove(notif);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAllNotificationsAsync(int userId)
    {
        var notifs = _context.Notifications.Where(n => n.UserId == userId);
        _context.Notifications.RemoveRange(notifs);
        await _context.SaveChangesAsync();
    }
}

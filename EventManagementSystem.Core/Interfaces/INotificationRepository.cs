using EventManagementSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<int> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task DeleteAllNotificationsAsync(int userId);
    }
}

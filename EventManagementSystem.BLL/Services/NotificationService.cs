using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementSystem.BLL.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        // ✅ رجع كل الإشعارات لمستخدم
        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
        {
            return await _notificationRepository.GetNotificationsForUserAsync(userId);
        }

        // ✅ عدد الإشعارات غير المقروءة
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        // ✅ تعليم إشعار كمقروء
        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId);
        }

        // ✅ تعليم كل الإشعارات كمقروءة
        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        // ✅ حذف إشعار واحد
        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            return await _notificationRepository.DeleteNotificationAsync(notificationId);
        }

        // ✅ حذف كل الإشعارات
        public async Task DeleteAllNotificationsAsync(int userId)
        {
            await _notificationRepository.DeleteAllNotificationsAsync(userId);
        }
    }
}

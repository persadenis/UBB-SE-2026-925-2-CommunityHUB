using System;
using System.Collections.Generic;

using Boards_WP.Data.Models;

namespace Boards_WP.Data.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        int AddNotification(Notification notification);
        void MarkNotificationAsRead(int notificationId);
        List<Notification> GetNotificationsByUserId(int userID, int offset, int limit);
    }
}
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

using Boards_WP.Data.Models;
using Boards_WP.Data.Repositories.Interfaces;

namespace Boards_WP.Data.Services.Interfaces
{
    public interface INotificationsService
    {
        void AddNotification(Notification notification);
        void ReadNotification(Notification notification);
        List<Notification> GetNotificationsByUserId(int userID, int offset, int limit);
        string GetNotificationMessage(Notification notification);
    }
}
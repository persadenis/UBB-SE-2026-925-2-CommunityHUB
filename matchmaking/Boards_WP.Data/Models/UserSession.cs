using System;
using System.Collections.Generic;
using System.Text;

namespace Boards_WP.Data.Models;

public class UserSession
{
    public User CurrentUser { get; set; }

    public UserSession()
    {
        CurrentUser = new User
        {
            UserID = 0,
            Username = "Guest",
            Email = string.Empty,
            PasswordHash = string.Empty,
            Bio = string.Empty,
            Status = "Offline",
            AvatarUrl = "ms-appx:///Assets/DefaultAvatar.png"
        };
    }
}

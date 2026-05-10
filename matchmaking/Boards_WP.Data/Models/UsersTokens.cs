using System;

namespace Boards_WP.Data.Models
{
    public class UsersTokens
    {
        public required User CurrentUser { get; set; }
        public int TokensNumber { get; set; }
        public DateTime LastSeen { get; set; }
    }
}

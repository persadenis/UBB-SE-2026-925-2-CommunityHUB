using System;

namespace Boards_WP.Data.Models
{
    public class Community
    {
        public int CommunityID { get; set; }
        public String Name { get; set; } = String.Empty;
        public String Description { get; set; } = String.Empty;
        public byte[]? Picture { get; set; }
        public byte[]? Banner { get; set; }
        public int MembersNumber { get; set; }
        public User Admin { get; set; }
    }
}

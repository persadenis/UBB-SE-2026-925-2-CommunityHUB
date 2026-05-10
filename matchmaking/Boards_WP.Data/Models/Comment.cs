using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Boards_WP.Data.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public Post ParentPost { get; set; }
        public Comment? ParentComment { get; set; }
        public User Owner { get; set; }
        public String Description { get; set; } = String.Empty;
        public int Score { get; set; }
        public DateTime CreationTime { get; set; }
        public int Indentation { get; set; }
        public bool IsDeleted { get; set; }
        public VoteType UserCurrentVote { get; set; } = VoteType.None;
        public String GetShareLink => $"boards://post/{ParentPost.PostID}";

    }
}

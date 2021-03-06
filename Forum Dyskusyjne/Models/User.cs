using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class User : IdentityUser
    {
        public string AvatarPath { get; set; }

        public int MessagePaging { get; set; }

        public int MessageNumber { get; set; }

        public int RankId { get; set; }

        public Rank Rank { get; set; }

        public ICollection<ForumUser> Forums { get; set; }

        /*public ICollection<Announcement> Announcements { get; set; }

        public ICollection<Thread> Threads { get; set; }

        public ICollection<Message> Messages { get; set; }

        public ICollection<PrivateMessage> PrivateMessages { get; set; }*/
    }
}

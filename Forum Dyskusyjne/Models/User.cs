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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class ForumUser
    {
        public int Id { get; set; }

        public int ForumId { get; set; }

        public string UserId { get; set; }

        public Forum Forum { get; set; }

        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class Forum
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Category Category { get; set; }

        public ICollection<Thread> Threads { get; set; }

        public ICollection<User> Moderators { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class Thread
    {
        public int Id { get; set; }

        public bool Sticky { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public int Views { get; set; }

        public User Author { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}

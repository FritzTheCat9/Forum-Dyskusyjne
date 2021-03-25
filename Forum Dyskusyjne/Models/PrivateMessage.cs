using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public User Author { get; set; }

        public User Recerver { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class Message
    {
        public int Id { get; set; }

        public bool Reported { get; set; }

        public bool Visible { get; set; }

        [Required]
        public string Text { get; set; }
        
        [Required]
        public int ThreadId { get; set; }

        public Thread Thread { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
    }
}

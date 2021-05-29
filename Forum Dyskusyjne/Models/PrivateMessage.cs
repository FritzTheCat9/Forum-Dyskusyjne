using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        public bool ReceiverVisible { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public User Receiver { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
    }
}
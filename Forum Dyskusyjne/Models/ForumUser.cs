using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class ForumUser
    {
        public int Id { get; set; }

        [Required]
        public int ForumId { get; set; }

        [Required]
        public string UserId { get; set; }

        public Forum Forum { get; set; }

        public User User { get; set; }
    }
}

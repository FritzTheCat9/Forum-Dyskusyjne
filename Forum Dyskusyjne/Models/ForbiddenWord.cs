﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Models
{
    public class ForbiddenWord
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

using Forum_Dyskusyjne.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum_Dyskusyjne.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public new DbSet<User> Users { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<ForbiddenWord> ForbiddenWords{ get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ForumUser> ForumUsers { get; set; }
        public DbSet<HtmlTag> HtmlTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Dodanie danych do bazy danych

            builder.Entity<Rank>().HasData(
                new Rank { Id = 1, Name = "New", MessagesNumber = 0 },
                new Rank { Id = 2, Name = "Begginer", MessagesNumber = 10 },
                new Rank { Id = 3, Name = "More experienced", MessagesNumber = 50 }
            );
            // Reszta danych dodana jest w pliku DataInitializer
        }
    }
}

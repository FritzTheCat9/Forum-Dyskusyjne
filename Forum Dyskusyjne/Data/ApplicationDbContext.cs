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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public new DbSet<User> Users { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<ForbiddenWords> ForbiddenWords{ get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Dodanie danych do bazy danych

            /*builder.Entity<Announcement>().HasData(
                new Announcement { Id = 1, Author = new User(), Title = "Ogłoszenie 1", Text = "Witajcie na forum dyskusyjnym!" }
            );*/
        }
    }
}

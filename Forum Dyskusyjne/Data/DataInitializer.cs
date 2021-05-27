using Forum_Dyskusyjne.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Data
{
    public static class DataInitializer
    {
        public static void SeedData(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(context, userManager);
            SeedData(context, userManager);
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Administrator";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("NormalUser").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "NormalUser";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }

        public static void SeedUsers(ApplicationDbContext context, UserManager<User> userManager)
        {
            if (userManager.FindByEmailAsync("bartlomiejuminski1999@gmail.com").Result == null)
            {
                User user = new User();
                user.UserName = "bartlomiejuminski1999@gmail.com";
                user.Email = "bartlomiejuminski1999@gmail.com";

                user.AvatarPath = Directory.GetCurrentDirectory() + @"\Resources\Avatars\DefaultAvatar.png";
                user.MessagePaging = 10;
                user.MessageNumber = 0;
                user.Rank = context.Ranks.Where(x => x.Id == 1).FirstOrDefault();

                IdentityResult result = userManager.CreateAsync(user, "Uminski123!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }

            if (userManager.FindByEmailAsync("user@gmail.com").Result == null)
            {
                User user = new User();
                user.UserName = "user@gmail.com";
                user.Email = "user@gmail.com";

                user.AvatarPath = Directory.GetCurrentDirectory() + @"\Resources\Avatars\DefaultAvatar.png";
                user.MessagePaging = 10;
                user.MessageNumber = 0;
                user.Rank = context.Ranks.Where(x => x.Id == 1).FirstOrDefault();

                IdentityResult result = userManager.CreateAsync(user, "User123!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "NormalUser").Wait();
                }
            }
        }

        private static void SeedData(ApplicationDbContext context, UserManager<User> userManager)
        {
            /* Users */
            var user1 = userManager.FindByNameAsync("bartlomiejuminski1999@gmail.com").Result;
            var user2 = userManager.FindByNameAsync("user@gmail.com").Result;

            /* Announcements */
            if (context.Announcements.Any()) { return; }
            var announcements = new Announcement[]
            {
                new Announcement { Author = user1, Title = "Ogłoszenie 1", Text = "Zawartość ogłoszenia" },
                new Announcement { Author = user1, Title = "Ogłoszenie 2", Text = "Zawartość ogłoszenia" },
                new Announcement { Author = user1, Title = "Ogłoszenie 3", Text = "Zawartość ogłoszenia" },
                new Announcement { Author = user1, Title = "Ogłoszenie 4", Text = "Zawartość ogłoszenia" },
                new Announcement { Author = user1, Title = "Ogłoszenie 5", Text = "Zawartość ogłoszenia" }
            };
            foreach (var announcement in announcements)
            {
                context.Announcements.Add(announcement);
            }
            context.SaveChanges();

            /* Attachments */
            if (context.Attachments.Any()) { return; }
            var attachments = new Attachment[]
            {
                new Attachment { Path = Directory.GetCurrentDirectory() + @"\Resources\Attachments\DefaultAttachment1.txt" },
                new Attachment { Path = Directory.GetCurrentDirectory() + @"\Resources\Attachments\DefaultAttachment2.txt" },
                new Attachment { Path = Directory.GetCurrentDirectory() + @"\Resources\Attachments\DefaultAttachment3.txt" },
                new Attachment { Path = Directory.GetCurrentDirectory() + @"\Resources\Attachments\DefaultAttachment4.txt" },
                new Attachment { Path = Directory.GetCurrentDirectory() + @"\Resources\Attachments\DefaultAttachment5.txt" }
            };
            foreach (var attachment in attachments.Reverse())
            {
                context.Attachments.Add(attachment);
            }
            context.SaveChanges();

            /* PrivateMessages */
            if (context.PrivateMessages.Any()) { return; }
            var privateMessages = new PrivateMessage[]
            {
                new PrivateMessage { Author = user1, Receiver = user2, Title = "Prwywatna wiadomość 1", Text = "Zawartość prywatnej wiadomości",
                    Attachments = new List<Attachment>()
                    {
                        context.Attachments.Where(x => x.Id == 1).First(),
                        context.Attachments.Where(x => x.Id == 2).First()
                    }
                },
                new PrivateMessage { Author = user2, Receiver = user1, Title = "Prwywatna wiadomość 2", Text = "Zawartość prywatnej wiadomości",
                Attachments = new List<Attachment>()
                    {
                        context.Attachments.Where(x => x.Id == 3).First()
                    }},
                new PrivateMessage { Author = user1, Receiver = user2, Title = "Prwywatna wiadomość 3", Text = "Zawartość prywatnej wiadomości" },
            };
            foreach (var privateMessage in privateMessages.Reverse())
            {
                context.PrivateMessages.Add(privateMessage);
            }
            context.SaveChanges();

            /* Messages */
           /* if (context.Messages.Any()) { return; }
            var messages = new Message[]
            {
                new Message { Author = user1, Text = "Zawartość wiadomości", Visible = true, Reported = false,
                    Attachments = new List<Attachment>()
                    {
                        context.Attachments.Where(x => x.Id == 4).First()
                    }
                },
                new Message { Author = user2, Text = "Zawartość wiadomości", Visible = true, Reported = false },
                new Message { Author = user1, Text = "Zawartość wiadomości" , Visible = true, Reported = false },
                new Message { Author = user2, Text = "Zawartość wiadomości", Visible = true, Reported = false },
                new Message { Author = user1, Text = "Zawartość wiadomości" , Visible = true, Reported = false }
            };
            foreach (var message in messages.Reverse())
            {
                context.Messages.Add(message);
            }
            context.SaveChanges();*/

            /* Threads */
            /*if (context.Threads.Any()) { return; }
            var threads = new Thread[]
            {
                new Thread { Author = user1, Title = "Tytuł wątku 3", Text = "Zawartość wątku", Sticky = true, Views = 0,
                    Attachments = new List<Attachment>()
                    {
                        context.Attachments.Where(x => x.Id == 5).First()
                    },
                    Messages = new List<Message>()
                    {
                        context.Messages.Where(x => x.Id == 1).First(),
                        context.Messages.Where(x => x.Id == 2).First()
                    }
                },
                new Thread { Author = user2, Title = "Tytuł wątku 2", Text = "Zawartość wątku", Sticky = false, Views = 0,
                    Messages = new List<Message>()
                    {
                        context.Messages.Where(x => x.Id == 3).First()
                    }
                },
                new Thread { Author = user1, Title = "Tytuł wątku 1", Text = "Zawartość wątku", Sticky = false, Views = 0,
                    Messages = new List<Message>()
                    {
                        context.Messages.Where(x => x.Id == 4).First(),
                        context.Messages.Where(x => x.Id == 5).First()
                    }
                },
            };
            foreach (var thread in threads.Reverse())
            {
                context.Threads.Add(thread);
            }
            context.SaveChanges();*/

            /* ForbiddenWords */
            if (context.ForbiddenWords.Any()) { return; }
            var forbiddenWords = new ForbiddenWord[]
            {
                new ForbiddenWord { Name = "Dupa" },
                new ForbiddenWord { Name = "Cycki" },
                new ForbiddenWord { Name = "PIS" },
            };
            foreach (var forbiddenWord in forbiddenWords.Reverse())
            {
                context.ForbiddenWords.Add(forbiddenWord);
            }
            context.SaveChanges();

            /* Categories */
            if (context.Categories.Any()) { return; }
            var categories = new Category[]
            {
                new Category { Name = "Gry" },
                new Category { Name = "Filmy" },
                new Category { Name = "Informatyka" }
            };
            foreach (var category in categories.Reverse())
            {
                context.Categories.Add(category);
            }
            context.SaveChanges();

            /* Forums */
            if (context.Forums.Any()) { return; }
            var forums = new Forum[]
            {
                new Forum { Name = "Minecraft", Category = context.Categories.Where(x => x.Id == 1).First(),
                    Threads = new List<Thread>()
                    {
                         new Thread { Author = user1, Title = "Tytuł wątku 3", Text = "Zawartość wątku",  Sticky = true, Views = 0,
                             Attachments = new List<Attachment>()
                             {
                                 context.Attachments.Where(x => x.Id == 5).First()
                             },
                             Messages = new List<Message>()
                             {
                                 /*context.Messages.Where(x => x.Id == 1).First(),
                                 context.Messages.Where(x => x.Id == 2).First()*/

                                  new Message { Author = user1, Text = "Zawartość wiadomości :( :) XD xD poop sad", Visible = true, Reported = false,
                                    Attachments = new List<Attachment>()
                                    {
                                        context.Attachments.Where(x => x.Id == 4).First()
                                    }
                                    },
                                    new Message { Author = user2, Text = "Zawartość wiadomości", Visible = true, Reported = false },
                
                             }
                         },
                        new Thread { Author = user2, Title = "Tytuł wątku 2", Text = "Zawartość wątku", Sticky = false, Views = 0,
                            Messages = new List<Message>()
                            {
                                //context.Messages.Where(x => x.Id == 3).First()
                                new Message { Author = user1, Text = "Zawartość wiadomości" , Visible = true, Reported = false },
               
                            }
                        }
                    }/*,
                    Users = new List<ForumUser>()
                    {
                        new ForumUser { User = userManager.FindByEmailAsync("bartlomiejuminski1999@gmail.com").Result, Forum = }

                        userManager.FindByEmailAsync("bartlomiejuminski1999@gmail.com").Result,
                        userManager.FindByEmailAsync("user@gmail.com").Result
                    }*/
                },
                new Forum { Name = "Brickleberry", Category = context.Categories.Where(x => x.Id == 2).First(),
                    Threads = new List<Thread>()
                    {
                        new Thread { Author = user1, Title = "Tytuł wątku 1", Text = "Zawartość wątku", Sticky = false, Views = 0,
                            Messages = new List<Message>()
                            {
                                /*context.Messages.Where(x => x.Id == 4).First(),
                                context.Messages.Where(x => x.Id == 5).First()*/
                                new Message { Author = user2, Text = "Zawartość wiadomości", Visible = true, Reported = false },
                                new Message { Author = user1, Text = "Zawartość wiadomości" , Visible = true, Reported = false }
                            }
                        }
                    }/*,
                    Users = new List<ForumUser>()
                    {
                        userManager.FindByEmailAsync("bartlomiejuminski1999@gmail.com").Result
                    }*/
                }
            };
            foreach (var forum in forums.Reverse())
            {
                foreach (var thread in forum.Threads)
                {
                    thread.Forum = forum;
                    foreach (var message in thread.Messages)
                    {
                        message.Thread = thread;
                    }
                }
                context.Forums.Add(forum);
            }

            /* ForumUsers = Moderators */
            var forumUsers = new ForumUser[] {
                new ForumUser { Forum = forums[0], User = userManager.FindByEmailAsync("bartlomiejuminski1999@gmail.com").Result },
                new ForumUser { Forum = forums[0], User = userManager.FindByEmailAsync("user@gmail.com").Result },
                new ForumUser { Forum = forums[1], User = userManager.FindByEmailAsync("bartlomiejuminski1999@gmail.com").Result }
            };
            context.ForumUsers.AddRange(forumUsers);
            context.SaveChanges();
        }
    }
}

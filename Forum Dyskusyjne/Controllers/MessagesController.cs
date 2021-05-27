using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum_Dyskusyjne.Data;
using Forum_Dyskusyjne.Models;
using Microsoft.AspNetCore.Authorization;

namespace Forum_Dyskusyjne
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //------------------------------------------------------------------------------
        // GET: Fora/ShowForaThreads/:id
        public async Task<IActionResult> ShowThreadMessages(int id)
        {
            var applicationDbContext = _context.Messages.Include(m => m.Author).Include(m => m.Thread).Where(x => x.ThreadId == id);
            return View("Index", await applicationDbContext.ToListAsync());
        }

        /// <summary>
        /// Sprawdzenie czy user o podanym id jest moderatorem forum o podanym id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="forumId"></param>
        /// <returns>Null - nie jest moderatorem, obiekt ForumUser - jest moderatorem</returns>
        public bool CheckIsForumModerator(int forumId)
        {
            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                 .Where(x => x.Email == LoggedUserEmail)
                 .FirstOrDefault();

            if (user != null)
            {
                ForumUser forumUser = _context.ForumUsers
                .Where(x => x.UserId == user.Id)
                .Where(x => x.ForumId == forumId)
                .FirstOrDefault();

                if(forumUser != null)
                    return true;
            }

            return false;
        }

        public bool IsMessageAuthor(int messageId)
        {
            string LoggedUserEmail = User.Identity.Name;
            Message message = _context.Messages
                .Include(x => x.Author)
                .Where(x => x.Id == messageId)
                .FirstOrDefault();
            User user = _context.Users
                .Where(x => x.Email == LoggedUserEmail)
                .FirstOrDefault();

            if (message != null && user != null)
            {
                if (message.Author.Id == user.Id)
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool CheckIsAdmin()
        {
            if (User.IsInRole("Administrator"))
            {
                return true;
            }

            return false;
        }

        public bool ContainsForbiddenWords(Message message)
        {
            var forbiddenWords = _context.ForbiddenWords.ToList();

            string[] words = message.Text.ToLower().Split(' ');

            foreach (var word in words)
            {
                foreach (var forbidden in forbiddenWords)
                {
                    if(word == forbidden.Name.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AktualizujRange()
        {
            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                .Include(x => x.Rank)
                .Where(x => x.Email == LoggedUserEmail)
                .FirstOrDefault();

            user.MessageNumber++;

            var maxMessageNumber = user.Rank.MessagesNumber;
            List<Rank> ranks = _context.Ranks.ToList();
            foreach (var rank in ranks)
            {
                if(user.MessageNumber >= rank.MessagesNumber)
                {
                    if(rank.MessagesNumber > maxMessageNumber)
                    {
                        user.Rank = rank;
                    }
                }
            }
        }
        //------------------------------------------------------------------------------


        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Messages.Include(m => m.Author).Include(m => m.Thread);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Author)
                .Include(m => m.Thread)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        [Authorize(Roles = "Administrator,NormalUser")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Title");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Create([Bind("Id,Reported,Visible,Text,ThreadId,AuthorId")] Message message)
        {
            if (ModelState.IsValid)
            {
                if(!ContainsForbiddenWords(message))
                {
                    AktualizujRange();
                    _context.Add(message);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", message.AuthorId);
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Title", message.ThreadId);
            return View(message);
        }

        // GET: Messages/Edit/5
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(x => x.Thread)
                .ThenInclude(x => x.Forum)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (message == null)
            {
                return NotFound();
            }

            int forumId = message.Thread.Forum.Id;
            if (CheckIsForumModerator(forumId) || IsMessageAuthor(message.Id) || CheckIsAdmin())
            {
                ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", message.AuthorId);
                ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Title", message.ThreadId);
                return View(message);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Reported,Visible,Text,ThreadId,AuthorId")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!ContainsForbiddenWords(message))
                    {
                        _context.Update(message);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", message.AuthorId);
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Title", message.ThreadId);
            return View(message);
        }

        // GET: Messages/Delete/5
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Author)
                .Include(m => m.Thread)
                .ThenInclude(x => x.Forum)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            int forumId = message.Thread.Forum.Id;
            if (CheckIsForumModerator(forumId) || IsMessageAuthor(message.Id) || CheckIsAdmin())
            {
                return View(message);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}

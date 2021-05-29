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
        public async Task<IActionResult> ShowThreadMessages(int id, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var messages = _context.Messages.Include(m => m.Author).Include(m => m.Thread).Where(x => x.ThreadId == id);

            var thread = _context.Threads.FirstOrDefault(x => x.Id == id);          // zliczanie odsłon wątku
            if(thread != null)
            {
                thread.Views++;
                _context.SaveChanges();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                messages = messages.Where(p => p.Text.ToUpper().Contains(searchString.ToUpper()));
            }

            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                .Where(x => x.Email == LoggedUserEmail)
                .FirstOrDefault();

            int pageSize = 10;
            if (user != null) pageSize = user.MessagePaging;

            return View(await PaginatedList<Message>.CreateAsync(messages.AsNoTracking(), pageNumber ?? 1, pageSize));
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
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var messages = _context.Messages.Include(m => m.Author).Include(m => m.Thread).Where(x => 1 == 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                messages = messages.Where(p => p.Text.ToUpper().Contains(searchString.ToUpper()));
            }

            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                .Where(x => x.Email == LoggedUserEmail)
                .FirstOrDefault();

            int pageSize = 10;
            if (user != null) pageSize = user.MessagePaging;

            return View(await PaginatedList<Message>.CreateAsync(messages.AsNoTracking(), pageNumber ?? 1, pageSize));


           // return View(await applicationDbContext.ToListAsync());
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
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Title");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Create([Bind("Id,Reported,Visible,Text,ThreadId")] Message message)
        {
            if (ModelState.IsValid)
            {
                if (!ContainsForbiddenWords(message))
                {
                    string LoggedUserEmail = User.Identity.Name;
                    User user = _context.Users
                         .Where(x => x.Email == LoggedUserEmail)
                         .FirstOrDefault();
                    message.AuthorId = user.Id;

                    AktualizujRange();
                    _context.Add(message);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
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
                ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Title", message.ThreadId);
                return View(message);
            }

            int threadId = message.Thread.Id;
            return RedirectToAction("ShowThreadMessages", new { id = threadId });
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Reported,Visible,Text,ThreadId")] Message message)
        {
            var messageToUpdate = await _context.Messages
                .Include(x => x.Author)
                .Include(x => x.Thread)
                .ThenInclude(x => x.Forum)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            messageToUpdate.Id = message.Id;
            messageToUpdate.Reported = message.Reported;
            messageToUpdate.Visible = message.Visible;
            messageToUpdate.Text = message.Text;
            messageToUpdate.ThreadId = message.ThreadId;

            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!ContainsForbiddenWords(messageToUpdate))
                    {
                        _context.Update(messageToUpdate);
                        await _context.SaveChangesAsync(); 
                        int threadId = messageToUpdate.Thread.Id;
                        return RedirectToAction("ShowThreadMessages", new { id = threadId });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(messageToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["ThreadId"] = new SelectList(_context.Threads, "Id", "Title", messageToUpdate.ThreadId);
            return View(messageToUpdate);
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

            int threadId = message.Thread.Id;
            return RedirectToAction("ShowThreadMessages", new { id = threadId });
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

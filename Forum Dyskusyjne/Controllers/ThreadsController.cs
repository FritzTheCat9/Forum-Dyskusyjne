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
    public class ThreadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThreadsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------------------------------
        // GET: Fora/ShowForaThreads/:id
        public async Task<IActionResult> ShowForaThreads(int id)
        {
            var threads = _context.Threads.Include(t => t.Author).Include(t => t.Forum).Include(x => x.Messages).Where(x => x.ForumId == id);
            threads = threads.OrderByDescending(x => x.Sticky);

            return View("Index", await threads.ToListAsync());
        }

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

                if (forumUser != null)
                    return true;
            }

            return false;
        }

        public bool IsThreadAuthor(int threadId)
        {
            string LoggedUserEmail = User.Identity.Name;
            Thread thread = _context.Threads
                .Include(x => x.Author)
                .Where(x => x.Id == threadId)
                .FirstOrDefault();
            User user = _context.Users
                .Where(x => x.Email == LoggedUserEmail)
                .FirstOrDefault();

            if (thread != null && user != null)
            {
                if (thread.Author.Id == user.Id)
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
        //------------------------------------------------------------------------------

        // GET: Threads
        public async Task<IActionResult> Index()
        {
            var threads = _context.Threads.Include(t => t.Author).Include(t => t.Forum).OrderByDescending(x => x.Sticky);
            return View(await threads.ToListAsync());
        }

        // GET: Threads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thread = await _context.Threads
                .Include(t => t.Author)
                .Include(t => t.Forum)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thread == null)
            {
                return NotFound();
            }

             thread.Views++;                // zliczanie odsłon wątku
            _context.SaveChanges();

            return View(thread);
        }

        // GET: Threads/Create
        [Authorize(Roles = "Administrator,NormalUser")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name");
            return View();
        }

        // POST: Threads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Create([Bind("Id,Sticky,Title,Text,Views,ForumId,AuthorId")] Thread thread)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("NormalUser"))
                {
                    thread.Sticky = false;
                }

                 _context.Add(thread);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", thread.AuthorId);
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name", thread.ForumId);
            return View(thread);
        }

        // GET: Threads/Edit/5
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thread = await _context.Threads
                .Include(x => x.Forum)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (thread == null)
            {
                return NotFound();
            }
            int forumId = thread.Forum.Id;
            if (CheckIsForumModerator(forumId) || IsThreadAuthor(thread.Id) || CheckIsAdmin())
            {
                ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", thread.AuthorId);
                ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name", thread.ForumId);
                return View(thread);
            }

            return RedirectToAction("ShowForaThreads", new { id = forumId });
        }

        // POST: Threads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sticky,Title,Text,Views,ForumId,AuthorId")] Thread thread)
        {
            if (id != thread.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.IsInRole("NormalUser"))
                    {
                        thread.Sticky = false;
                    }

                    _context.Update(thread);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThreadExists(thread.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", thread.AuthorId);
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name", thread.ForumId);
            return View(thread);
        }

        // GET: Threads/Delete/5
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thread = await _context.Threads
                .Include(x => x.Forum)
                .Include(m => m.Author)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (thread == null)
            {
                return NotFound();
            }

            int forumId = thread.Forum.Id;
            if (CheckIsForumModerator(forumId) || IsThreadAuthor(thread.Id) || CheckIsAdmin())
            {
                return View(thread);
            }

            return RedirectToAction("ShowForaThreads", new { id = forumId });
        }

        // POST: Threads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thread = await _context.Threads.FindAsync(id);
            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThreadExists(int id)
        {
            return _context.Threads.Any(e => e.Id == id);
        }
    }
}

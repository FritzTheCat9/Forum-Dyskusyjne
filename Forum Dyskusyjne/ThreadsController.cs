using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum_Dyskusyjne.Data;
using Forum_Dyskusyjne.Models;

namespace Forum_Dyskusyjne
{
    public class ThreadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThreadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Threads
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Threads.Include(t => t.Author).Include(t => t.Forum);
            return View(await applicationDbContext.ToListAsync());
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

            return View(thread);
        }

        // GET: Threads/Create
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
        public async Task<IActionResult> Create([Bind("Id,Sticky,Title,Text,Views,ForumId,AuthorId")] Thread thread)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thread);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", thread.AuthorId);
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name", thread.ForumId);
            return View(thread);
        }

        // GET: Threads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thread = await _context.Threads.FindAsync(id);
            if (thread == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "UserName", thread.AuthorId);
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name", thread.ForumId);
            return View(thread);
        }

        // POST: Threads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> Delete(int? id)
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

            return View(thread);
        }

        // POST: Threads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

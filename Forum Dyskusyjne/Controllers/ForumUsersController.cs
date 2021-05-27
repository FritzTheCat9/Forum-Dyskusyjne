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
    public class ForumUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ForumUsers
        public async Task<IActionResult> Index()
        {
           

            var applicationDbContext = _context.ForumUsers.Include(f => f.Forum).Include(f => f.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var text = "siema \x263A";
            var coded = "";
            foreach (string word in text.Split(" "))
            {
                if (word.Contains("\x263A"))
                {
                    char a = (char.Parse(word));
                }
                else
                {
                    coded += word + " ";
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var forumUser = await _context.ForumUsers
                .Include(f => f.Forum)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forumUser == null)
            {
                return NotFound();
            }

            return View(forumUser);
        }

        // GET: ForumUsers/Create
        public IActionResult Create()
        {
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: ForumUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ForumId,UserId")] ForumUser forumUser)
        {
            if (ModelState.IsValid)
            {
                var forumUserCheck = _context.ForumUsers
                    .Where(x => x.UserId == forumUser.UserId)
                    .Where(x => x.ForumId == forumUser.ForumId)
                    .FirstOrDefault();

                if(forumUserCheck == null)
                {
                    _context.Add(forumUser);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name", forumUser.ForumId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", forumUser.UserId);
            return View(forumUser);
        }

        // GET: ForumUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumUser = await _context.ForumUsers
                .Include(f => f.Forum)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forumUser == null)
            {
                return NotFound();
            }

            return View(forumUser);
        }

        // POST: ForumUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumUser = await _context.ForumUsers.FindAsync(id);
            _context.ForumUsers.Remove(forumUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumUserExists(int id)
        {
            return _context.ForumUsers.Any(e => e.Id == id);
        }
    }
}

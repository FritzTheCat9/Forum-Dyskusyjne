using Forum_Dyskusyjne.Data;
using Forum_Dyskusyjne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var users = _context.Users.Include(u => u.Rank).Include(u => u.Forums);
            return View(await users.ToListAsync());
        }

        // GET: User/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(u => u.Rank).Include(u => u.Forums)
                .ThenInclude(f => f.Forum)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["RankId"] = new SelectList(_context.Ranks, "Id", "Name");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(string id, [Bind("Id,RankId")] User USER)
        {
            var user = await _context.Users.Include(u => u.Rank).Include(u => u.Forums)
                .ThenInclude(f => f.Forum)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.Rank = await _context.Ranks.Where(x => x.Id == USER.RankId).FirstOrDefaultAsync();             // change user rank

            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            ViewData["RankId"] = new SelectList(_context.Ranks, "Id", "Name");

            return View(user);
        }
    }
}

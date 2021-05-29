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

namespace Forum_Dyskusyjne.Controllers
{
    public class UserPrivateMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserPrivateMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserPrivateMessages
        [Authorize(Roles = "Administrator,NormalUser")]
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

            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                .Where(x => x.Email == LoggedUserEmail)
                .FirstOrDefault();

            var privateMessages = _context.PrivateMessages
                .Include(p => p.Author)
                .Include(p => p.Receiver)
                .Where(p => p.ReceiverId == user.Id)
                .Where(p => p.ReceiverVisible == true);

            if (!String.IsNullOrEmpty(searchString))
            {
                privateMessages = privateMessages.Where(p => p.Text.ToUpper().Contains(searchString.ToUpper()) || p.Title.ToUpper().Contains(searchString.ToUpper()));
            }

            int pageSize = user.MessagePaging;
            return View(await PaginatedList<PrivateMessage>.CreateAsync(privateMessages.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: UserPrivateMessages/Details/5
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateMessage = await _context.PrivateMessages
                .Include(p => p.Author)
                .Include(p => p.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (privateMessage == null)
            {
                return NotFound();
            }

            return View(privateMessage);
        }

        // GET: UserPrivateMessages/Create
        [Authorize(Roles = "Administrator,NormalUser")]
        public IActionResult Create()
        {
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: UserPrivateMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Create([Bind("Id,Title,Text,ReceiverId")] PrivateMessage privateMessage)
        {
            if (ModelState.IsValid)
            {
                string LoggedUserEmail = User.Identity.Name;
                User user = _context.Users
                     .Where(x => x.Email == LoggedUserEmail)
                     .FirstOrDefault();

                privateMessage.AuthorId = user.Id;              // autor = zalogowany user
                privateMessage.ReceiverVisible = true;

                _context.Add(privateMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Email", privateMessage.ReceiverId);
            return View(privateMessage);
        }

        // GET: UserPrivateMessages/Delete/5
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateMessage = await _context.PrivateMessages
                .Include(p => p.Author)
                .Include(p => p.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (privateMessage == null)
            {
                return NotFound();
            }

            return View(privateMessage);
        }

        // POST: UserPrivateMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,NormalUser")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var privateMessage = await _context.PrivateMessages.FindAsync(id);
            privateMessage.ReceiverVisible = false;                                     // ukrycie wiadomości
            _context.PrivateMessages.Update(privateMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrivateMessageExists(int id)
        {
            return _context.PrivateMessages.Any(e => e.Id == id);
        }
    }
}

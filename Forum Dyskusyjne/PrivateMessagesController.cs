﻿using System;
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
    public class PrivateMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrivateMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrivateMessages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PrivateMessages.Include(p => p.Author).Include(p => p.Recerver);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PrivateMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateMessage = await _context.PrivateMessages
                .Include(p => p.Author)
                .Include(p => p.Recerver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (privateMessage == null)
            {
                return NotFound();
            }

            return View(privateMessage);
        }

        // GET: PrivateMessages/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["RecerverId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: PrivateMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Text,AuthorId,RecerverId")] PrivateMessage privateMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(privateMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", privateMessage.AuthorId);
            ViewData["RecerverId"] = new SelectList(_context.Users, "Id", "Id", privateMessage.RecerverId);
            return View(privateMessage);
        }

        // GET: PrivateMessages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateMessage = await _context.PrivateMessages.FindAsync(id);
            if (privateMessage == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", privateMessage.AuthorId);
            ViewData["RecerverId"] = new SelectList(_context.Users, "Id", "Id", privateMessage.RecerverId);
            return View(privateMessage);
        }

        // POST: PrivateMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Text,AuthorId,RecerverId")] PrivateMessage privateMessage)
        {
            if (id != privateMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(privateMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrivateMessageExists(privateMessage.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", privateMessage.AuthorId);
            ViewData["RecerverId"] = new SelectList(_context.Users, "Id", "Id", privateMessage.RecerverId);
            return View(privateMessage);
        }

        // GET: PrivateMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var privateMessage = await _context.PrivateMessages
                .Include(p => p.Author)
                .Include(p => p.Recerver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (privateMessage == null)
            {
                return NotFound();
            }

            return View(privateMessage);
        }

        // POST: PrivateMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var privateMessage = await _context.PrivateMessages.FindAsync(id);
            _context.PrivateMessages.Remove(privateMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrivateMessageExists(int id)
        {
            return _context.PrivateMessages.Any(e => e.Id == id);
        }
    }
}
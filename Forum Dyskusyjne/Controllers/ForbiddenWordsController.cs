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
    public class ForbiddenWordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForbiddenWordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ForbiddenWords
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ForbiddenWords.ToListAsync());
        }

        // GET: ForbiddenWords/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forbiddenWord = await _context.ForbiddenWords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forbiddenWord == null)
            {
                return NotFound();
            }

            return View(forbiddenWord);
        }

        // GET: ForbiddenWords/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ForbiddenWords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name")] ForbiddenWord forbiddenWord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(forbiddenWord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(forbiddenWord);
        }

        // GET: ForbiddenWords/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forbiddenWord = await _context.ForbiddenWords.FindAsync(id);
            if (forbiddenWord == null)
            {
                return NotFound();
            }
            return View(forbiddenWord);
        }

        // POST: ForbiddenWords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ForbiddenWord forbiddenWord)
        {
            if (id != forbiddenWord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forbiddenWord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForbiddenWordExists(forbiddenWord.Id))
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
            return View(forbiddenWord);
        }

        // GET: ForbiddenWords/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forbiddenWord = await _context.ForbiddenWords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (forbiddenWord == null)
            {
                return NotFound();
            }

            return View(forbiddenWord);
        }

        // POST: ForbiddenWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forbiddenWord = await _context.ForbiddenWords.FindAsync(id);
            _context.ForbiddenWords.Remove(forbiddenWord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForbiddenWordExists(int id)
        {
            return _context.ForbiddenWords.Any(e => e.Id == id);
        }
    }
}

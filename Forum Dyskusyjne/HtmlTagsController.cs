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
    public class HtmlTagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HtmlTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HtmlTags
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.HtmlTags.ToListAsync());
        }

        // GET: HtmlTags/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var htmlTag = await _context.HtmlTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (htmlTag == null)
            {
                return NotFound();
            }

            return View(htmlTag);
        }

        // GET: HtmlTags/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: HtmlTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name")] HtmlTag htmlTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(htmlTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(htmlTag);
        }

        // GET: HtmlTags/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var htmlTag = await _context.HtmlTags.FindAsync(id);
            if (htmlTag == null)
            {
                return NotFound();
            }
            return View(htmlTag);
        }

        // POST: HtmlTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] HtmlTag htmlTag)
        {
            if (id != htmlTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(htmlTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HtmlTagExists(htmlTag.Id))
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
            return View(htmlTag);
        }

        // GET: HtmlTags/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var htmlTag = await _context.HtmlTags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (htmlTag == null)
            {
                return NotFound();
            }

            return View(htmlTag);
        }

        // POST: HtmlTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var htmlTag = await _context.HtmlTags.FindAsync(id);
            _context.HtmlTags.Remove(htmlTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HtmlTagExists(int id)
        {
            return _context.HtmlTags.Any(e => e.Id == id);
        }
    }
}

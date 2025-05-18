using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2_DB.Models;

namespace Lab2_DB.Controllers
{
    public class ReadersController : Controller
    {
        private readonly LibraryContext _context;

        public ReadersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Readers.ToListAsync());
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // GET: Readers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Readers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullNameReader,PhoneNumberReader,EmailReader,RoleReader,PlaceStudyOrWorkReader")] Reader reader)
        {
            // Перевірка унікальності телефону
            if (await _context.Readers.AnyAsync(r => r.PhoneNumberReader == reader.PhoneNumberReader))
            {
                ModelState.AddModelError("PhoneNumberReader", "Цей номер телефону вже використовується.");
            }

            // Перевірка унікальності електронної пошти
            if (await _context.Readers.AnyAsync(r => r.EmailReader == reader.EmailReader))
            {
                ModelState.AddModelError("EmailReader", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reader);
        }

        // GET: Readers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
            {
                return NotFound();
            }
            return View(reader);
        }

        // POST: Readers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FullNameReader,PhoneNumberReader,EmailReader,RoleReader,PlaceStudyOrWorkReader")] Reader reader)
        {
            if (id != reader.Id)
            {
                return NotFound();
            }

            // Перевірка унікальності телефону (крім поточного запису)
            if (await _context.Readers.AnyAsync(r => r.PhoneNumberReader == reader.PhoneNumberReader && r.Id != reader.Id))
            {
                ModelState.AddModelError("PhoneNumberReader", "Цей номер телефону вже використовується.");
            }

            // Перевірка унікальності електронної пошти (крім поточного запису)
            if (await _context.Readers.AnyAsync(r => r.EmailReader == reader.EmailReader && r.Id != reader.Id))
            {
                ModelState.AddModelError("EmailReader", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderExists(reader.Id))
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
            return View(reader);
        }


        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var reader = await _context.Readers
                .Include(r => r.Requests)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reader == null)
            {
                return NotFound();
            }

            if (reader.Requests.Any())
            {
                TempData["ErrorMessage"] = "Неможливо видалити читача, оскільки він має запити.";
                return RedirectToAction(nameof(Index));
            }

            _context.Readers.Remove(reader);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReaderExists(long id)
        {
            return _context.Readers.Any(e => e.Id == id);
        }
    }
}

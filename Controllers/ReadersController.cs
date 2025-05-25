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

            reader.Id = 0;
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
                TempData["Error"] = "ID читача не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
            {
                TempData["Error"] = "Читача не знайдено.";
                return RedirectToAction(nameof(Index));
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
                TempData["Error"] = "ID читача не співпадає.";
                return RedirectToAction(nameof(Index));
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

                    TempData["Message"] = $"Читача {reader.FullNameReader} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderExists(reader.Id))
                    {
                        TempData["Error"] = "Читача не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні читача: {ex.Message}";
                }
            }

            return View(reader);
        }

        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID читача не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var reader = await _context.Readers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reader == null)
            {
                TempData["Error"] = "Читача не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var reader = await _context.Readers
                    .Include(r => r.Requests)
                    .ThenInclude(req => req.IssuedBooks)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reader == null)
                {
                    TempData["Error"] = "Читача не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var request in reader.Requests)
                {
                    foreach (var issuedBook in request.IssuedBooks)
                    {
                        _context.IssuedBooks.Remove(issuedBook);
                    }
                    _context.Requests.Remove(request);
                }

                _context.Readers.Remove(reader);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Читача {reader.FullNameReader} успішно видалено разом із пов’язаними запитами та видачами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні читача: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ReaderExists(long id)
        {
            return _context.Readers.Any(e => e.Id == id);
        }
    }
}

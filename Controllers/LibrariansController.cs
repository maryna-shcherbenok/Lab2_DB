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
    public class LibrariansController : Controller
    {
        private readonly LibraryContext _context;

        public LibrariansController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Librarians
        public async Task<IActionResult> Index()
        {
            return View(await _context.Librarians.ToListAsync());
        }

        // GET: Librarians/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.Id == id);
            if (librarian == null)
            {
                return NotFound();
            }

            return View(librarian);
        }

        // GET: Librarians/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Librarians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullNameLibrarian,PhoneNumberLibrarian,EmailLibrarian")] Librarian librarian)
        {
            // Перевірка унікальності номера телефону
            if (await _context.Librarians.AnyAsync(l => l.PhoneNumberLibrarian == librarian.PhoneNumberLibrarian))
            {
                ModelState.AddModelError("PhoneNumberLibrarian", "Цей номер телефону вже використовується.");
            }

            // Перевірка унікальності email
            if (await _context.Librarians.AnyAsync(l => l.EmailLibrarian == librarian.EmailLibrarian))
            {
                ModelState.AddModelError("EmailLibrarian", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(librarian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(librarian);
        }

        // GET: Librarians/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID бібліотекаря не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null)
            {
                TempData["Error"] = "Бібліотекаря не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(librarian);
        }

        // POST: Librarians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FullNameLibrarian,PhoneNumberLibrarian,EmailLibrarian")] Librarian librarian)
        {
            if (id != librarian.Id)
            {
                TempData["Error"] = "ID бібліотекаря не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            // Перевірка унікальності номера телефону (окрім поточного бібліотекаря)
            if (await _context.Librarians.AnyAsync(l => l.PhoneNumberLibrarian == librarian.PhoneNumberLibrarian && l.Id != librarian.Id))
            {
                ModelState.AddModelError("PhoneNumberLibrarian", "Цей номер телефону вже використовується.");
            }

            // Перевірка унікальності email (окрім поточного бібліотекаря)
            if (await _context.Librarians.AnyAsync(l => l.EmailLibrarian == librarian.EmailLibrarian && l.Id != librarian.Id))
            {
                ModelState.AddModelError("EmailLibrarian", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(librarian);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Бібліотекаря {librarian.FullNameLibrarian} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrarianExists(librarian.Id))
                    {
                        TempData["Error"] = "Бібліотекаря не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні бібліотекаря: {ex.Message}";
                }
            }

            return View(librarian);
        }

        // GET: Librarians/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID бібліотекаря не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.Id == id);

            if (librarian == null)
            {
                TempData["Error"] = "Бібліотекаря не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(librarian);
        }

        // POST: Librarians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var librarian = await _context.Librarians
                    .Include(l => l.Requests)
                    .ThenInclude(req => req.IssuedBooks)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (librarian == null)
                {
                    TempData["Error"] = "Бібліотекаря не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var request in librarian.Requests)
                {
                    foreach (var issuedBook in request.IssuedBooks)
                    {
                        _context.IssuedBooks.Remove(issuedBook);
                    }
                    _context.Requests.Remove(request);
                }

                _context.Librarians.Remove(librarian);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Бібліотекаря {librarian.FullNameLibrarian} успішно видалено разом із пов’язаними запитами та видачами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні бібліотекаря: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool LibrarianExists(long id)
        {
            return _context.Librarians.Any(e => e.Id == id);
        }
    }
}

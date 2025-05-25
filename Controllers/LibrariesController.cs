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
    public class LibrariesController : Controller
    {
        private readonly LibraryContext _context;

        public LibrariesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Libraries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Libraries.ToListAsync());
        }

        // GET: Libraries/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (library == null)
            {
                return NotFound();
            }

            return View(library);
        }

        // GET: Libraries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Libraries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TitleLibrary,PhoneNumberLibrary,EmailLibrary,LibraryAddress,LeaderLibrary")] Library library)
        {
            // Перевірка унікальності телефону
            if (await _context.Libraries.AnyAsync(l => l.PhoneNumberLibrary == library.PhoneNumberLibrary))
            {
                ModelState.AddModelError("PhoneNumberLibrary", "Цей номер телефону вже використовується.");
            }

            // Перевірка унікальності email
            if (await _context.Libraries.AnyAsync(l => l.EmailLibrary == library.EmailLibrary))
            {
                ModelState.AddModelError("EmailLibrary", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(library);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(library);
        }

        // GET: Libraries/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID бібліотеки не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var library = await _context.Libraries.FindAsync(id);
            if (library == null)
            {
                TempData["Error"] = "Бібліотеку не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(library);
        }

        // POST: Libraries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TitleLibrary,PhoneNumberLibrary,EmailLibrary,LibraryAddress,LeaderLibrary")] Library library)
        {
            if (id != library.Id)
            {
                TempData["Error"] = "ID бібліотеки не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            // Унікальність номера телефону, окрім поточного запису
            if (await _context.Libraries.AnyAsync(l => l.PhoneNumberLibrary == library.PhoneNumberLibrary && l.Id != library.Id))
            {
                ModelState.AddModelError("PhoneNumberLibrary", "Цей номер телефону вже використовується.");
            }

            // Унікальність email, окрім поточного запису
            if (await _context.Libraries.AnyAsync(l => l.EmailLibrary == library.EmailLibrary && l.Id != library.Id))
            {
                ModelState.AddModelError("EmailLibrary", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(library);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Бібліотеку {library.TitleLibrary} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryExists(library.Id))
                    {
                        TempData["Error"] = "Бібліотеку не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні бібліотеки: {ex.Message}";
                }
            }

            return View(library);
        }

        // GET: Libraries/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID бібліотеки не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var library = await _context.Libraries
                .FirstOrDefaultAsync(m => m.Id == id);

            if (library == null)
            {
                TempData["Error"] = "Бібліотеку не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(library);
        }

        // POST: Libraries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var library = await _context.Libraries
                    .Include(l => l.Departments)
                    .ThenInclude(d => d.Funds)
                    .ThenInclude(f => f.Books)
                    .ThenInclude(b => b.IssuedBooks)
                    .ThenInclude(ib => ib.Request)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (library == null)
                {
                    TempData["Error"] = "Бібліотеку не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

                // Вручну видаляємо пов’язані записи (ех...)
                foreach (var department in library.Departments)
                {
                    foreach (var fund in department.Funds)
                    {
                        foreach (var book in fund.Books)
                        {
                            foreach (var issuedBook in book.IssuedBooks)
                            {
                                if (issuedBook.Request != null)
                                {
                                    _context.Requests.Remove(issuedBook.Request);
                                }
                                _context.IssuedBooks.Remove(issuedBook);
                            }
                            _context.Books.Remove(book);
                        }
                        _context.Funds.Remove(fund);
                    }
                    _context.Departments.Remove(department);
                }

                _context.Libraries.Remove(library);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Бібліотеку {library.TitleLibrary} успішно видалено разом із пов’язаними відділами, фондами та книгами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні бібліотеки: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool LibraryExists(long id)
        {
            return _context.Libraries.Any(e => e.Id == id);
        }
    }
}

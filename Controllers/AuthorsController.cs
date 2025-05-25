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
    public class AuthorsController : Controller
    {
        private readonly LibraryContext _context;

        public AuthorsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authors.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullNameAuthor,PhoneNumberAuthor,EmailAuthor")] Author author)
        {
            ModelState.Remove("Books");

            // Унікальність номеру телефону
            if (await _context.Authors.AnyAsync(a => a.PhoneNumberAuthor == author.PhoneNumberAuthor))
            {
                ModelState.AddModelError("PhoneNumberAuthor", "Цей номер телефону вже використовується.");
            }

            // Унікальність email
            if (await _context.Authors.AnyAsync(a => a.EmailAuthor == author.EmailAuthor))
            {
                ModelState.AddModelError("EmailAuthor", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID автора не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                TempData["Error"] = "Автора не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FullNameAuthor,PhoneNumberAuthor,EmailAuthor")] Author author)
        {
            if (id != author.Id)
            {
                TempData["Error"] = "ID автора не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("Books");

            // Перевірка унікальності номеру телефону, виключаючи поточного автора
            if (await _context.Authors.AnyAsync(a => a.PhoneNumberAuthor == author.PhoneNumberAuthor && a.Id != author.Id))
            {
                ModelState.AddModelError("PhoneNumberAuthor", "Цей номер телефону вже використовується.");
            }

            // Перевірка унікальності email, виключаючи поточного автора
            if (await _context.Authors.AnyAsync(a => a.EmailAuthor == author.EmailAuthor && a.Id != author.Id))
            {
                ModelState.AddModelError("EmailAuthor", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Автора {author.FullNameAuthor} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
                    {
                        TempData["Error"] = "Автора не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні автора: {ex.Message}";
                }
            }

            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID автора не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                TempData["Error"] = "Автора не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .ThenInclude(b => b.IssuedBooks)
                    .ThenInclude(ib => ib.Request)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    TempData["Error"] = "Автора не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var book in author.Books)
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

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Автора {author.FullNameAuthor} успішно видалено разом із пов’язаними книгами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні автора: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool AuthorExists(long id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}

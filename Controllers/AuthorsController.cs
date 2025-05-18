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
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
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
                return NotFound();
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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

            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(long? id)
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

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var author = await _context.Authors
                .Include(a => a.Books) // завантажити пов’язані книги
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            if (author.Books.Any())
            {
                // Створюємо повідомлення в TempData, щоб передати його у View
                TempData["ErrorMessage"] = "Неможливо видалити автора, оскільки він пов’язаний з існуючими книгами.";
                return RedirectToAction(nameof(Index));
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(long id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}

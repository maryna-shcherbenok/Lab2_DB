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
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Books.Include(b => b.AuthorBookNavigation).Include(b => b.FundBookNavigation).Include(b => b.PublisherBookNavigation);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.AuthorBookNavigation)
                .Include(b => b.FundBookNavigation)
                .Include(b => b.PublisherBookNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorBook"] = new SelectList(_context.Authors, "Id", "FullNameAuthor");
            ViewData["FundBook"] = new SelectList(_context.Funds, "Id", "TitleFund");
            ViewData["PublisherBook"] = new SelectList(_context.PublishingHouses, "Id", "TitlePh");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Isbn,TitleBook,AuthorBook,GenreBook,AvailabilityStatusBook,NumberPages,FundBook,PublisherBook")] Book book)
        {
            ModelState.Remove("AuthorBookNavigation");
            ModelState.Remove("FundBookNavigation");
            ModelState.Remove("PublisherBookNavigation");

            if (_context.Books.Any(b => b.Isbn == book.Isbn))
            {
                ModelState.AddModelError("Isbn", "Цей ISBN вже використовується.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorBook"] = new SelectList(_context.Authors, "Id", "FullNameAuthor", book.AuthorBook);
            ViewData["FundBook"] = new SelectList(_context.Funds, "Id", "TitleFund", book.FundBook);
            ViewData["PublisherBook"] = new SelectList(_context.PublishingHouses, "Id", "TitlePh", book.PublisherBook);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID книги не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var book = await _context.Books
                .Include(b => b.AuthorBookNavigation)
                .Include(b => b.FundBookNavigation)
                .Include(b => b.PublisherBookNavigation)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                TempData["Error"] = "Книгу не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorBook"] = new SelectList(_context.Authors, "Id", "FullNameAuthor", book.AuthorBook);
            ViewData["FundBook"] = new SelectList(_context.Funds, "Id", "TitleFund", book.FundBook);
            ViewData["PublisherBook"] = new SelectList(_context.PublishingHouses, "Id", "TitlePh", book.PublisherBook);

            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Isbn,TitleBook,AuthorBook,GenreBook,AvailabilityStatusBook,NumberPages,FundBook,PublisherBook")] Book book)
        {
            if (id != book.Id)
            {
                TempData["Error"] = "ID книги не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("AuthorBookNavigation");
            ModelState.Remove("FundBookNavigation");
            ModelState.Remove("PublisherBookNavigation");

            if (_context.Books.Any(b => b.Isbn == book.Isbn && b.Id != book.Id))
            {
                ModelState.AddModelError("Isbn", "Цей ISBN вже використовується для іншої книги.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Завантажуємо існуючу книгу
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.Id == id);

                    if (existingBook == null)
                    {
                        TempData["Error"] = "Книгу не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Зберігаємо старий ISBN для оновлення в Requests
                    var oldISBN = existingBook.Isbn;

                    // Оновлюємо дані книги
                    existingBook.TitleBook = book.TitleBook;
                    existingBook.Isbn = book.Isbn;
                    existingBook.GenreBook = book.GenreBook;
                    existingBook.NumberPages = book.NumberPages;
                    existingBook.AvailabilityStatusBook = book.AvailabilityStatusBook;
                    existingBook.AuthorBook = book.AuthorBook;
                    existingBook.FundBook = book.FundBook;
                    existingBook.PublisherBook = book.PublisherBook;

                    // Оновлюємо ISBN у пов’язаних записах Requests
                    if (oldISBN != book.Isbn)
                    {
                        var relatedRequests = await _context.Requests
                            .Where(r => r.Isbn == oldISBN)
                            .ToListAsync();

                        foreach (var request in relatedRequests)
                        {
                            request.Isbn = book.Isbn;
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Книгу {book.TitleBook} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        TempData["Error"] = "Книгу не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні книги: {ex.Message}";
                }
            }

            // Якщо ModelState не валідний, повертаємо форму з помилками
            ViewData["AuthorBook"] = new SelectList(_context.Authors, "Id", "FullNameAuthor", book.AuthorBook);
            ViewData["FundBook"] = new SelectList(_context.Funds, "Id", "TitleFund", book.FundBook);
            ViewData["PublisherBook"] = new SelectList(_context.PublishingHouses, "Id", "TitlePh", book.PublisherBook);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID книги не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var book = await _context.Books
                .Include(b => b.AuthorBookNavigation)
                .Include(b => b.FundBookNavigation)
                .Include(b => b.PublisherBookNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                TempData["Error"] = "Книгу не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    TempData["Error"] = "Книгу не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

                // Знаходимо всі запити, пов’язані з книгою через ISBN
                var relatedRequests = await _context.Requests
                    .Include(r => r.IssuedBooks)
                    .Where(r => r.Isbn == book.Isbn)
                    .ToListAsync();

                // Видаляємо пов’язані записи
                foreach (var request in relatedRequests)
                {
                    // Видаляємо IssuedBooks, пов’язані з Request
                    foreach (var issuedBook in request.IssuedBooks)
                    {
                        _context.IssuedBooks.Remove(issuedBook);
                    }
                    // Видаляємо сам Request
                    _context.Requests.Remove(request);
                }

                // Видаляємо книгу
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Книгу {book.TitleBook} успішно видалено разом із пов’язаними запитами та видачами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні книги: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        private bool BookExists(long id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}

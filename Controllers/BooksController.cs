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
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
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
                return NotFound();
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
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var book = await _context.Books
                .Include(b => b.IssuedBooks)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            if (book.IssuedBooks.Any())
            {
                TempData["ErrorMessage"] = "Неможливо видалити книгу, оскільки вона пов’язана з виданими запитами.";
                return RedirectToAction(nameof(Index));
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(long id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}

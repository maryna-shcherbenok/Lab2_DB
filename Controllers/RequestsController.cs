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
    public class RequestsController : Controller
    {
        private readonly LibraryContext _context;

        public RequestsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Requests.Include(r => r.CardNumberReaderNavigation).Include(r => r.PassNumberLibrarianNavigation);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.CardNumberReaderNavigation)
                .Include(r => r.PassNumberLibrarianNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            ViewData["CardNumberReader"] = new SelectList(_context.Readers, "Id", "FullNameReader");
            ViewData["PassNumberLibrarian"] = new SelectList(_context.Librarians, "Id", "FullNameLibrarian");
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PassNumberLibrarian,CardNumberReader,CreationDateRequest,RequestType,RequestStatus,Isbn")] Request request)
        {
            ModelState.Remove("CardNumberReaderNavigation");
            ModelState.Remove("PassNumberLibrarianNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync(); 

                var book = await _context.Books.FirstOrDefaultAsync(b => b.Isbn == request.Isbn);
                if (book != null)
                {
                    // Створюємо запис у IssuedBooks
                    var issuedBook = new IssuedBook
                    {
                        RequestId = request.Id,
                        BookId = book.Id
                    };

                    _context.IssuedBooks.Add(issuedBook);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CardNumberReader"] = new SelectList(_context.Readers, "Id", "FullNameReader", request.CardNumberReader);
            ViewData["PassNumberLibrarian"] = new SelectList(_context.Librarians, "Id", "FullNameLibrarian", request.PassNumberLibrarian);
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID запиту не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                TempData["Error"] = "Запит не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CardNumberReader"] = new SelectList(_context.Readers, "Id", "FullNameReader", request.CardNumberReader);
            ViewData["PassNumberLibrarian"] = new SelectList(_context.Librarians, "Id", "FullNameLibrarian", request.PassNumberLibrarian);
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,PassNumberLibrarian,CardNumberReader,CreationDateRequest,RequestType,RequestStatus,Isbn")] Request request)
        {
            if (id != request.Id)
            {
                TempData["Error"] = "ID запиту не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("CardNumberReaderNavigation");
            ModelState.Remove("PassNumberLibrarianNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    // Завантажуємо існуючий запит
                    var existingRequest = await _context.Requests
                        .FirstOrDefaultAsync(r => r.Id == id);

                    if (existingRequest == null)
                    {
                        TempData["Error"] = "Запит не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Оновлюємо дані запиту
                    existingRequest.PassNumberLibrarian = request.PassNumberLibrarian;
                    existingRequest.CardNumberReader = request.CardNumberReader;
                    existingRequest.CreationDateRequest = request.CreationDateRequest;
                    existingRequest.RequestType = request.RequestType;
                    existingRequest.RequestStatus = request.RequestStatus;
                    existingRequest.Isbn = request.Isbn;

                    // Оновлення IssuedBooks
                    var book = await _context.Books.FirstOrDefaultAsync(b => b.Isbn == request.Isbn);
                    if (book != null)
                    {
                        var issued = await _context.IssuedBooks
                            .FirstOrDefaultAsync(ib => ib.RequestId == request.Id);

                        if (issued != null)
                        {
                            issued.BookId = book.Id;
                            _context.Update(issued);
                        }
                        else
                        {
                            _context.IssuedBooks.Add(new IssuedBook
                            {
                                RequestId = request.Id,
                                BookId = book.Id
                            });
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Запит успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
                    {
                        TempData["Error"] = "Запит не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні запиту: {ex.Message}";
                }
            }

            ViewData["CardNumberReader"] = new SelectList(_context.Readers, "Id", "FullNameReader", request.CardNumberReader);
            ViewData["PassNumberLibrarian"] = new SelectList(_context.Librarians, "Id", "FullNameLibrarian", request.PassNumberLibrarian);
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            return View(request);
        }

        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID запиту не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var request = await _context.Requests
                .Include(r => r.CardNumberReaderNavigation)
                .Include(r => r.PassNumberLibrarianNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (request == null)
            {
                TempData["Error"] = "Запит не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var request = await _context.Requests
                    .Include(r => r.IssuedBooks)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (request == null)
                {
                    TempData["Error"] = "Запит не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var issuedBook in request.IssuedBooks)
                {
                    _context.IssuedBooks.Remove(issuedBook);
                }

                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Запит успішно видалено разом із пов’язаними видачами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні запиту: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool RequestExists(long id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}

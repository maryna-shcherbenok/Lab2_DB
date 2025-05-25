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
    public class FundsController : Controller
    {
        private readonly LibraryContext _context;

        public FundsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Funds
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Funds.Include(f => f.DepartmentFundNavigation);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Funds/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _context.Funds
                .Include(f => f.DepartmentFundNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fund == null)
            {
                return NotFound();
            }

            return View(fund);
        }

        // GET: Funds/Create
        public IActionResult Create()
        {
            ViewData["DepartmentFund"] = new SelectList(_context.Departments, "Id", "TitleDepartment");
            return View();
        }

        // POST: Funds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TitleFund,TypeFund,TotalNumberCopies,DepartmentFund")] Fund fund)
        {
            ModelState.Remove("DepartmentFundNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(fund);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentFund"] = new SelectList(_context.Departments, "Id", "TitleDepartment", fund.DepartmentFund);
            return View(fund);
        }

        // GET: Funds/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID фонду не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var fund = await _context.Funds.FindAsync(id);
            if (fund == null)
            {
                TempData["Error"] = "Фонд не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentFund"] = new SelectList(_context.Departments, "Id", "TitleDepartment", fund.DepartmentFund);
            return View(fund);
        }

        // POST: Funds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TitleFund,TypeFund,TotalNumberCopies,DepartmentFund")] Fund fund)
        {
            if (id != fund.Id)
            {
                TempData["Error"] = "ID фонду не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("DepartmentFundNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fund);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Фонд {fund.TitleFund} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FundExists(fund.Id))
                    {
                        TempData["Error"] = "Фонд не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні фонду: {ex.Message}";
                }
            }

            ViewData["DepartmentFund"] = new SelectList(_context.Departments, "Id", "TitleDepartment", fund.DepartmentFund);
            return View(fund);
        }

        // GET: Funds/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID фонду не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var fund = await _context.Funds
                .Include(f => f.DepartmentFundNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fund == null)
            {
                TempData["Error"] = "Фонд не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(fund);
        }

        // POST: Funds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var fund = await _context.Funds
                    .Include(f => f.Books)
                    .ThenInclude(b => b.IssuedBooks)
                    .ThenInclude(ib => ib.Request)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (fund == null)
                {
                    TempData["Error"] = "Фонд не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

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
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Фонд {fund.TitleFund} успішно видалено разом із пов’язаними книгами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні фонду: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool FundExists(long id)
        {
            return _context.Funds.Any(e => e.Id == id);
        }
    }
}

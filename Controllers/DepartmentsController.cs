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
    public class DepartmentsController : Controller
    {
        private readonly LibraryContext _context;

        public DepartmentsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Departments.Include(d => d.LibraryNavigation);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.LibraryNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["Library"] = new SelectList(_context.Libraries, "Id", "TitleLibrary");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TitleDepartment,LeaderDepartment,Library")] Department department)
        {
            ModelState.Remove("LibraryNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Library"] = new SelectList(_context.Libraries, "Id", "TitleLibrary", department.Library);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID відділу не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                TempData["Error"] = "Відділ не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Library"] = new SelectList(_context.Libraries, "Id", "TitleLibrary", department.Library);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TitleDepartment,LeaderDepartment,Library")] Department department)
        {
            if (id != department.Id)
            {
                TempData["Error"] = "ID відділу не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("LibraryNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Відділ {department.TitleDepartment} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        TempData["Error"] = "Відділ не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні відділу: {ex.Message}";
                }
            }

            ViewData["Library"] = new SelectList(_context.Libraries, "Id", "TitleLibrary", department.Library);
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID відділу не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var department = await _context.Departments
                .Include(d => d.LibraryNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                TempData["Error"] = "Відділ не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var department = await _context.Departments
                    .Include(d => d.Funds)
                    .ThenInclude(f => f.Books)
                    .ThenInclude(b => b.IssuedBooks)
                    .ThenInclude(ib => ib.Request)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (department == null)
                {
                    TempData["Error"] = "Відділ не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

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
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Відділ {department.TitleDepartment} успішно видалено разом із пов’язаними фондами та книгами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні відділу: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool DepartmentExists(long id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}

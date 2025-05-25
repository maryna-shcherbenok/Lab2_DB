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
    public class PublishingHousesController : Controller
    {
        private readonly LibraryContext _context;

        public PublishingHousesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: PublishingHouses
        public async Task<IActionResult> Index()
        {
            return View(await _context.PublishingHouses.ToListAsync());
        }

        // GET: PublishingHouses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publishingHouse = await _context.PublishingHouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publishingHouse == null)
            {
                return NotFound();
            }

            return View(publishingHouse);
        }

        // GET: PublishingHouses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PublishingHouses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TitlePh,PhoneNumberPh,AddressPh,EmailPh")] PublishingHouse publishingHouse)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(publishingHouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(publishingHouse);
        }

        // GET: PublishingHouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID видавництва не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var publishingHouse = await _context.PublishingHouses.FindAsync(id);
            if (publishingHouse == null)
            {
                TempData["Error"] = "Видавництво не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(publishingHouse);
        }

        // POST: PublishingHouses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TitlePh,PhoneNumberPh,AddressPh,EmailPh")] PublishingHouse publishingHouse)
        {
            if (id != publishingHouse.Id)
            {
                TempData["Error"] = "ID видавництва не співпадає.";
                return RedirectToAction(nameof(Index));
            }

            // Перевірка унікальності номера телефону (окрім поточного видавництва)
            if (await _context.PublishingHouses.AnyAsync(p => p.PhoneNumberPh == publishingHouse.PhoneNumberPh && p.Id != publishingHouse.Id))
            {
                ModelState.AddModelError("PhoneNumberPh", "Цей номер телефону вже використовується.");
            }

            // Перевірка унікальності email (окрім поточного видавництва)
            if (await _context.PublishingHouses.AnyAsync(p => p.EmailPh == publishingHouse.EmailPh && p.Id != publishingHouse.Id))
            {
                ModelState.AddModelError("EmailPh", "Ця електронна пошта вже використовується.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publishingHouse);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Видавництво {publishingHouse.TitlePh} успішно оновлено.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublishingHouseExists(publishingHouse.Id))
                    {
                        TempData["Error"] = "Видавництво не знайдено.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Помилка при оновленні видавництва: {ex.Message}";
                }
            }

            return View(publishingHouse);
        }

        // GET: PublishingHouses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID видавництва не вказано.";
                return RedirectToAction(nameof(Index));
            }

            var publishingHouse = await _context.PublishingHouses
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publishingHouse == null)
            {
                TempData["Error"] = "Видавництво не знайдено.";
                return RedirectToAction(nameof(Index));
            }

            return View(publishingHouse);
        }

        // POST: PublishingHouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var publishingHouse = await _context.PublishingHouses
                    .Include(p => p.Books)
                    .ThenInclude(b => b.IssuedBooks)
                    .ThenInclude(ib => ib.Request)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (publishingHouse == null)
                {
                    TempData["Error"] = "Видавництво не знайдено.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var book in publishingHouse.Books)
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

                _context.PublishingHouses.Remove(publishingHouse);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Видавництво {publishingHouse.TitlePh} успішно видалено разом із пов’язаними книгами.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка при видаленні видавництва: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool PublishingHouseExists(int id)
        {
            return _context.PublishingHouses.Any(e => e.Id == id);
        }
    }
}

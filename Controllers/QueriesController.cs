using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab2_DB.Models;

namespace Lab2_DB.Controllers
{
    public class QueriesController : Controller
    {
        private readonly LibraryContext _context;

        public QueriesController(LibraryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Funds = _context.Funds.ToList();
            ViewBag.Publishers = _context.PublishingHouses.ToList();
            ViewBag.Books = _context.Books.ToList();
            ViewBag.Readers = _context.Readers.ToList();
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Libraries = _context.Libraries.ToList();
            ViewBag.Librarians = _context.Librarians.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Books.Select(b => b.GenreBook).Distinct().ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Query1(string TitleFund)
        {
            var books = _context.Books
                .FromSqlInterpolated($@"
                    SELECT b.*
                    FROM Books b
                    JOIN Funds f ON b.FundBook = f.ID
                    WHERE f.TitleFund = {TitleFund}")
                .ToList();
            return View("Results1", books);
        }

        [HttpPost]
        public IActionResult Query2(string PublisherName)
        {
            var authors = _context.Authors
                .FromSqlInterpolated($@"
                    SELECT DISTINCT a.*
                    FROM Authors a
                    JOIN Books b ON a.ID = b.AuthorBook
                    JOIN PublishingHouses p ON b.PublisherBook = p.ID
                    WHERE p.TitlePH = {PublisherName}")
                .ToList();
            return View("Results2", authors);
        }

        [HttpPost]
        public IActionResult Query3(long ISBN)
        {
            var readers = _context.Readers
                .FromSqlInterpolated($@"
                    SELECT DISTINCT r.*
                    FROM Readers r
                    JOIN Requests req ON r.ID = req.CardNumberReader
                    JOIN IssuedBooks ib ON req.ID = ib.RequestID
                    WHERE req.ISBN = {ISBN}")
                .ToList();
            return View("Results3", readers);
        }

        [HttpPost]
        public IActionResult Query4(string DeptName)
        {
            var department = _context.Departments
                .Where(d => d.TitleDepartment == DeptName)
                .FirstOrDefault();

            var totalCopies = _context.Funds
                .Where(f => f.DepartmentFundNavigation.TitleDepartment == DeptName)
                .Sum(f => f.TotalNumberCopies);

            ViewBag.TotalCopies = totalCopies;
            return View("Results4", new[] { department });
        }

        [HttpPost]
        public IActionResult Query5(string LibraryTitle)
        {
            var result = _context.Libraries
                .Where(l => l.TitleLibrary == LibraryTitle)
                .ToList();
            return View("Results5", result);
        }

        [HttpPost] // Запит з використанням суми
        public IActionResult Query6(string ReaderName)
        {
            var reader = _context.Readers.FirstOrDefault(r => r.FullNameReader == ReaderName);

            if (reader == null)
            {
                ViewBag.ReaderName = ReaderName;
                ViewBag.PageSum = 0;
                return View("Results6");
            }

            var issuedBookIds = _context.IssuedBooks
                .Where(ib => ib.Request.CardNumberReader == reader.Id)
                .Select(ib => ib.BookId)
                .Distinct();

            var pageSum = _context.Books
                .Where(b => !issuedBookIds.Contains(b.Id))
                .Sum(b => (int?)b.NumberPages) ?? 0;

            ViewBag.ReaderName = ReaderName;
            ViewBag.PageSum = pageSum;

            return View("Results6");
        }

        [HttpPost]
        public IActionResult Query7(string TitleFund)
        {
            var readers = _context.Readers
                .FromSqlInterpolated($@"
                    SELECT r.*
                    FROM Readers r
                    WHERE EXISTS (
                        SELECT 1 FROM Funds f WHERE f.TitleFund = {TitleFund}
                    )
                    AND NOT EXISTS (
                        SELECT b.ID
                        FROM Books b
                        JOIN Funds f ON b.FundBook = f.ID
                        WHERE f.TitleFund = {TitleFund}
                        AND NOT EXISTS (
                            SELECT 1
                            FROM Requests req
                            JOIN IssuedBooks ib ON req.ID = ib.RequestID
                            WHERE req.CardNumberReader = r.ID
                            AND ib.BookID = b.ID
                        )
                    )")
                .ToList();
            return View("Results7", readers);
        }

        [HttpPost]
        public IActionResult Query8(string DeptName)
        {
            var books = _context.Books
                .FromSqlInterpolated($@"
                    SELECT b.*
                    FROM Books b
                    WHERE NOT EXISTS (
                        SELECT 1 FROM IssuedBooks ib WHERE ib.BookID = b.ID
                    )
                    AND b.FundBook IN (
                        SELECT f.ID
                        FROM Funds f
                        JOIN Departments d ON f.DepartmentFund = d.ID
                        WHERE d.TitleDepartment = {DeptName}
                    )")
                .ToList();
            return View("Results8", books);
        }

        [HttpPost]
        public IActionResult Query9(string AuthorName)
        {
            var readers = _context.Readers
                .FromSqlInterpolated($@"
                    SELECT r.*
                    FROM Readers r
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM Requests req
                        JOIN IssuedBooks ib ON req.ID = ib.RequestID
                        JOIN Books b ON ib.BookID = b.ID
                        JOIN Authors a ON b.AuthorBook = a.ID
                        WHERE req.CardNumberReader = r.ID
                        AND a.FullNameAuthor <> {AuthorName}
                    )
                    AND EXISTS (
                        SELECT 1
                        FROM Requests req2
                        JOIN IssuedBooks ib2 ON req2.ID = ib2.RequestID
                        JOIN Books b2 ON ib2.BookID = b2.ID
                        JOIN Authors a2 ON b2.AuthorBook = a2.ID
                        WHERE req2.CardNumberReader = r.ID
                        AND a2.FullNameAuthor = {AuthorName}
                    )")
                .ToList();
            return View("Results9", readers);
        }

        [HttpPost]
        public IActionResult Query10(int LibrarianID)
        {
            var librarians = _context.Librarians
                .FromSqlInterpolated($@"
                    SELECT l2.*
                    FROM Librarians l2
                    WHERE l2.ID <> {LibrarianID}
                    AND NOT EXISTS (
                        SELECT ib.BookID
                        FROM Requests req1
                        JOIN IssuedBooks ib ON req1.ID = ib.RequestID
                        WHERE req1.PassNumberLibrarian = {LibrarianID}
                        AND NOT EXISTS (
                            SELECT 1
                            FROM Requests req2
                            JOIN IssuedBooks ib2 ON req2.ID = ib2.RequestID
                            WHERE req2.PassNumberLibrarian = l2.ID
                            AND ib2.BookID = ib.BookID
                        )
                    )
                    AND NOT EXISTS (
                        SELECT ib2.BookID
                        FROM Requests req3
                        JOIN IssuedBooks ib2 ON req3.ID = ib2.RequestID
                        WHERE req3.PassNumberLibrarian = l2.ID
                        AND NOT EXISTS (
                            SELECT 1
                            FROM Requests req4
                            JOIN IssuedBooks ib3 ON req4.ID = ib3.RequestID
                            WHERE req4.PassNumberLibrarian = {LibrarianID}
                            AND ib3.BookID = ib2.BookID
                        )
                    )")
                .ToList();
            return View("Results10", librarians);
        }

        [HttpPost]
        public IActionResult Query11(string GenreName)
        {
            var publishers = _context.PublishingHouses
                .FromSqlInterpolated($@"
                    SELECT p.*
                    FROM PublishingHouses p
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM Books b
                        WHERE b.GenreBook = {GenreName}
                        AND b.PublisherBook <> p.ID
                    )
                    AND EXISTS (
                        SELECT 1
                        FROM Books b2
                        WHERE b2.GenreBook = {GenreName}
                        AND b2.PublisherBook = p.ID
                    )")
                .ToList();
            return View("Results11", publishers);
        }

        [HttpPost]
        public IActionResult Query12(long ReaderId)
        {
            // Спочатку перевіримо, чи читач взагалі має якісь книги
            bool hasBooks = _context.Requests
                .Include(r => r.IssuedBooks)
                .Any(r => r.CardNumberReader == ReaderId && r.IssuedBooks.Any());

            if (!hasBooks)
            {
                // ViewBag.ErrorMessage = "Обраний читач не має жодної виданої книги.";
                return View("Results12", new List<Reader>()); // Порожній список
            }

            var result = _context.Readers
                .FromSqlInterpolated($@"
            SELECT DISTINCT r2.*
            FROM Readers r2
            WHERE r2.ID <> {ReaderId}
            AND NOT EXISTS (
                SELECT ib1.BookID
                FROM Requests req1
                JOIN IssuedBooks ib1 ON req1.ID = ib1.RequestID
                WHERE req1.CardNumberReader = {ReaderId}
                AND NOT EXISTS (
                    SELECT 1
                    FROM Requests req2
                    JOIN IssuedBooks ib2 ON req2.ID = ib2.RequestID
                    WHERE req2.CardNumberReader = r2.ID
                    AND ib2.BookID = ib1.BookID
                )
            )
            AND EXISTS (
                SELECT 1
                FROM Requests req3
                JOIN IssuedBooks ib3 ON req3.ID = ib3.RequestID
                WHERE req3.CardNumberReader = r2.ID
                AND NOT EXISTS (
                    SELECT 1
                    FROM Requests req4
                    JOIN IssuedBooks ib4 ON req4.ID = ib4.RequestID
                    WHERE req4.CardNumberReader = {ReaderId}
                    AND ib4.BookID = ib3.BookID
                )
            )
        ")
                .ToList();

            return View("Results12", result);
        }
    }
}

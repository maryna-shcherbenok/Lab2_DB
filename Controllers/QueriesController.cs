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
            var books = (from b in _context.Books
                         join f in _context.Funds on b.FundBook equals f.Id
                         join p in _context.PublishingHouses on b.PublisherBook equals p.Id
                         join a in _context.Authors on b.AuthorBook equals a.Id
                         where !_context.Funds.Any(f2 => f2.Id == f.Id && f2.TitleFund == TitleFund)
                         select new
                         {
                             b.TitleBook,
                             PublisherName = p.TitlePh,
                             AuthorName = a.FullNameAuthor,
                             IssueCount = _context.IssuedBooks
                                 .Join(_context.Requests,
                                     ib => ib.RequestId,
                                     r => r.Id,
                                     (ib, r) => new { ib, r })
                                 .Count(x => x.ib.BookId == b.Id)
                         }).ToList<object>();

            return View("Results1", books);
        }

        [HttpPost]
        public IActionResult Query2(string PublisherName)
        {
            var authors = (from a in _context.Authors
                           join b in _context.Books on a.Id equals b.AuthorBook
                           join p in _context.PublishingHouses on b.PublisherBook equals p.Id
                           where p.TitlePh == PublisherName
                           group new { a, b } by a.FullNameAuthor into g
                           select new
                           {
                               FullNameAuthor = g.Key,
                               TotalBooks = g.Count()
                           }).ToList<object>();

            return View("Results2", authors);
        }

        [HttpPost]
        public IActionResult Query3(long ISBN)
        {
            var readers = (from r in _context.Readers
                           join req in _context.Requests on r.Id equals req.CardNumberReader
                           join ib in _context.IssuedBooks on req.Id equals ib.RequestId
                           join b in _context.Books on ib.BookId equals b.Id
                           where req.Isbn == ISBN && b.NumberPages > 400
                           select new
                           {
                               r.FullNameReader,
                               TotalRequests = _context.Requests
                                   .Count(req2 => req2.CardNumberReader == r.Id)
                           }).Distinct().ToList<object>();

            return View("Results3", readers);
        }

        [HttpPost]
        public IActionResult Query4(string DeptName)
        {
            var departments = (from d in _context.Departments
                               join f in _context.Funds on d.Id equals f.DepartmentFund
                               join lib in _context.Libraries on d.Library equals lib.Id
                               where d.TitleDepartment == DeptName
                               group new { d, f, lib } by new { d.TitleDepartment, lib.TitleLibrary } into g
                               select new
                               {
                                   g.Key.TitleDepartment,
                                   g.Key.TitleLibrary,
                                   TotalCopies = g.Sum(x => x.f.TotalNumberCopies)
                               }).ToList<object>();

            return View("Results4", departments);
        }

        [HttpPost]
        public IActionResult Query5(string LibraryTitle)
        {
            var result = (from lib in _context.Libraries
                          join l in _context.Librarians on lib.LeaderLibrary equals l.FullNameLibrarian
                          where lib.TitleLibrary == LibraryTitle
                          select new
                          {
                              LibraryName = lib.TitleLibrary,
                              LeaderName = lib.LeaderLibrary,
                              LeaderPhone = l.PhoneNumberLibrarian,
                              LeaderEmail = l.EmailLibrarian
                          }).ToList<object>();

            return View("Results5", result);
        }

        [HttpPost]
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
                .Distinct()
                .ToList();

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
            var fundBookIds = _context.Books
                .Where(b => b.FundBookNavigation.TitleFund == TitleFund)
                .Select(b => b.Id)
                .ToList();

            if (!fundBookIds.Any())
            {
                ViewBag.NoBooksInFund = true;
                ViewBag.FundName = TitleFund;
                return View("Results7", new List<object>());
            }

            var issuedBookIds = _context.IssuedBooks
                .Where(ib => fundBookIds.Contains(ib.BookId))
                .Select(ib => ib.BookId)
                .Distinct()
                .ToList();

            var readers = new List<object>();
            if (!issuedBookIds.Any())
            {
                readers = new List<object>();
            }
            else
            {
                readers = _context.Readers
                    .Where(r => !issuedBookIds.Except(
                        _context.IssuedBooks
                            .Where(ib => ib.Request.CardNumberReader == r.Id)
                            .Select(ib => ib.BookId)
                    ).Any())
                    .Select(r => new { r.FullNameReader })
                    .ToList<object>();
            }

            ViewBag.FundName = TitleFund;
            return View("Results7", readers);
        }

        [HttpPost]
        public IActionResult Query8(long ReaderId)
        {
            var readerBookIds = _context.IssuedBooks
                .Where(ib => ib.Request.CardNumberReader == ReaderId)
                .Select(ib => ib.BookId)
                .Distinct()
                .ToList();

            if (!readerBookIds.Any())
            {
                ViewBag.NoBooks = true;
                return View("Results8", new List<object>());
            }

            var readers = _context.Readers
                .Where(r => r.Id != ReaderId)
                .Select(r => new
                {
                    r.FullNameReader,
                    ReaderBooks = _context.IssuedBooks
                        .Where(ib => ib.Request.CardNumberReader == r.Id)
                        .Select(ib => ib.BookId)
                        .Distinct()
                        .ToList()
                })
                .Where(r => readerBookIds.All(bookId => r.ReaderBooks.Contains(bookId)) 
                    && r.ReaderBooks.Except(readerBookIds).Any())
                .Select(r => new { r.FullNameReader })
                .ToList<object>();

            if (!readers.Any())
            {
                ViewBag.NoResults = true;
            }

            ViewBag.ReaderId = ReaderId;
            return View("Results8", readers);
        }

        [HttpPost]
        public IActionResult Query9(string AuthorName)
        {
            var readers = _context.Readers
                .Where(r => !_context.Requests
                    .Join(_context.IssuedBooks,
                        req => req.Id,
                        ib => ib.RequestId,
                        (req, ib) => new { req, ib })
                    .Join(_context.Books,
                        x => x.ib.BookId,
                        b => b.Id,
                        (x, b) => new { x.req, b })
                    .Join(_context.Authors,
                        x => x.b.AuthorBook,
                        a => a.Id,
                        (x, a) => new { x.req, a })
                    .Where(x => x.req.CardNumberReader == r.Id && x.a.FullNameAuthor != AuthorName)
                    .Any()
                    &&
                    _context.Requests
                        .Join(_context.IssuedBooks,
                            req => req.Id,
                            ib => ib.RequestId,
                            (req, ib) => new { req, ib })
                        .Join(_context.Books,
                            x => x.ib.BookId,
                            b => b.Id,
                            (x, b) => new { x.req, b })
                        .Join(_context.Authors,
                            x => x.b.AuthorBook,
                            a => a.Id,
                            (x, a) => new { x.req, a })
                        .Where(x => x.req.CardNumberReader == r.Id && x.a.FullNameAuthor == AuthorName)
                        .Any())
                .ToList();

            return View("Results9", readers);
        }

        [HttpPost]
        public IActionResult Query10(long ReaderId)
        {
            var readerBookIds = _context.IssuedBooks
                .Where(ib => ib.Request.CardNumberReader == ReaderId)
                .Select(ib => ib.BookId)
                .Distinct()
                .ToList();

            if (!readerBookIds.Any())
            {
                ViewBag.NoBooks = true;
                return View("Results10", new List<object>());
            }

            var readers = _context.Readers
                .Where(r => r.Id != ReaderId)
                .Select(r => new
                {
                    r.FullNameReader,
                    ReaderBooks = _context.IssuedBooks
                        .Where(ib => ib.Request.CardNumberReader == r.Id)
                        .Select(ib => ib.BookId)
                        .Distinct()
                        .ToList()
                })
                .Where(r => !readerBookIds.All(bookId => !r.ReaderBooks.Contains(bookId))) 
                .Select(r => new { r.FullNameReader })
                .Distinct()
                .ToList<object>();

            if (!readers.Any())
            {
                ViewBag.NoResults = true;
            }

            return View("Results10", readers);
        }

        [HttpPost]
        public IActionResult Query11(long LibraryId)
        {
            // Книги заданої бібліотеки за ключовими полями
            var referenceBooks = _context.Books
                .Include(b => b.FundBookNavigation)
                    .ThenInclude(f => f.DepartmentFundNavigation)
                .Where(b => b.FundBookNavigation.DepartmentFundNavigation.Library == LibraryId)
                .Select(b => new { b.Isbn, b.TitleBook, b.AuthorBook, b.PublisherBook, b.NumberPages })
                .Distinct()
                .ToList();

            if (!referenceBooks.Any())
            {
                ViewBag.NoBooks = true;
                return View("Results11", new List<object>());
            }

            var otherLibraries = _context.Libraries
                .Where(l => l.Id != LibraryId)
                .ToList(); // Завантажуємо повністю список бібліотек перед використанням AsEnumerable

            var connection = _context.Database.GetDbConnection();
            var libraries = new List<object>();

            foreach (var l in otherLibraries)
            {
                using var scope = new LibraryContext(new DbContextOptionsBuilder<LibraryContext>()
                    .UseSqlServer(connection.ConnectionString)
                    .Options);

                var bookSet = scope.Books
                    .Include(b => b.FundBookNavigation)
                        .ThenInclude(f => f.DepartmentFundNavigation)
                    .Where(b => b.FundBookNavigation.DepartmentFundNavigation.Library == l.Id)
                    .Select(b => new { b.Isbn, b.TitleBook, b.AuthorBook, b.PublisherBook, b.NumberPages })
                    .Distinct()
                    .ToList();

                if (referenceBooks.All(rb => bookSet.Contains(rb)))
                {
                    libraries.Add(new { l.TitleLibrary });
                }
            }

            if (!libraries.Any())
            {
                ViewBag.NoResults = true;
            }

            ViewBag.LibraryId = LibraryId;
            return View("Results11", libraries);
        }
    }
}
#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Models;

namespace MyLibraryApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string bookname, string IsAvailable)
        {
            if (String.IsNullOrEmpty(bookname) && String.IsNullOrEmpty(IsAvailable))
            {
                var applicationDbContext2 = _context.Books.Include(b => b.Publisher);
                return View(await applicationDbContext2.ToListAsync());

            }
            else if (String.IsNullOrEmpty(bookname) && !String.IsNullOrEmpty(IsAvailable))
            {
                if (IsAvailable.Equals("yes"))
                {
                    var searchedBooks = await _context.Books.Include(b => b.Publisher).Where(a => a.IsAvaiable == true).ToListAsync();
                    return View(searchedBooks);
                }
                else if (IsAvailable.Equals("no"))
                {
                    var searchedBooks = await _context.Books.Include(b => b.Publisher).Where(a => a.IsAvaiable == false).ToListAsync();
                    return View(searchedBooks);
                }

                var applicationDbContext2 = _context.Books.Include(b => b.Publisher);
                return View(await applicationDbContext2.ToListAsync());
            }
            else 
            {
          

                if (IsAvailable.Equals("yes"))
                {
                    var searchByTitle = await _context.Books.Include(b => b.Publisher).Where(t => t.Title.Contains(bookname)).Where(a => a.IsAvaiable == true).ToListAsync();
                    var searchByGenre = await _context.Books.Include(b => b.Publisher).Where(t => t.Genre.Contains(bookname)).Where(a => a.IsAvaiable == true).ToListAsync();
                    var searchByAuthor = await _context.Books.Include(b => b.Publisher).Where(t => t.Author.Contains(bookname)).Where(a => a.IsAvaiable == true).ToListAsync();
                    if (searchByTitle.Count > 0)
                    {
                    return View(searchByTitle);
                    }
                    else if(searchByGenre.Count > 0)
                    {
                        return View( searchByGenre);
                    }
                    else if(searchByAuthor.Count > 0)
                    {
                        return View( searchByAuthor);
                    }

                    
                    return View();

                }
                else if (IsAvailable.Equals("no"))
                {
                    var searchByTitle = await _context.Books.Include(b => b.Publisher).Where(t => t.Title.Contains(bookname)).Where(a => a.IsAvaiable == false).ToListAsync();
                    var searchByGenre = await _context.Books.Include(b => b.Publisher).Where(t => t.Genre.Contains(bookname)).Where(a => a.IsAvaiable == false).ToListAsync();
                    var searchByAuthor = await _context.Books.Include(b => b.Publisher).Where(t => t.Author.Contains(bookname)).Where(a => a.IsAvaiable == false).ToListAsync();
                    if (searchByTitle.Count > 0)
                    {
                        return View(searchByTitle);
                    }
                    else if (searchByGenre.Count > 0)
                    {
                        return View(searchByGenre);
                    }
                    else if (searchByAuthor.Count > 0)
                    {
                        return View(searchByAuthor);
                    }

                    return View();
                }
                else {

                var searchByTitle = await _context.Books.Include(b => b.Publisher).Where(t => t.Title.Contains(bookname)).ToListAsync();
                var searchByGenre = await _context.Books.Include(b => b.Publisher).Where(t => t.Genre.Contains(bookname)).ToListAsync();
                var searchByAuthor = await _context.Books.Include(b => b.Publisher).Where(t => t.Author.Contains(bookname)).ToListAsync();
                if (searchByTitle.Count > 0)
                {
                    return View(searchByTitle);
                }
                else if (searchByGenre.Count > 0)
                {
                    return View(searchByGenre);
                }
                else if (searchByAuthor.Count > 0)
                {
                    return View(searchByAuthor);
                }
                }
         
                return View();
            }
        }
        // GET: Books/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PubName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Author,Genre,PublisherId,IsAvaiable,Annotation")] Book book)
        {
            Book book1= _context.Books.Find(book.Isbn);
            Regex notDigits = new Regex(@"[\D]");
            MatchCollection matchAuthor = notDigits.Matches(book.Author.ToString());
            MatchCollection matchGenre = notDigits.Matches(book.Genre.ToString());
            if (!matchAuthor.Any())
            {
                ModelState.AddModelError("authorError", "Author name cannot contain digits");
            }
            if (!matchGenre.Any())
            {
                ModelState.AddModelError("genreError", "Genre name cannot contain digits");
            }
            if (book1 != null)
            {
                ModelState.AddModelError("sameIsbn", "There is already a book with this ISBN!");
            }
            if(book.Isbn.ToString().Length != 9)
            {
                ModelState.AddModelError("sizeErr", "Size of ISBN must be 9 digits!");
            }
            if (ModelState.IsValid)
            {   
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PubName", book.PublisherId);
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PubName", book.PublisherId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Isbn,Title,Author,Genre,PublisherId,IsAvaiable,Annotation")] Book book)
        {
            if (id != book.Isbn)
            {
                return NotFound();
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
                    if (!BookExists(book.Isbn))
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
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PubName", book.PublisherId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Isbn == id);
        }
    }
}

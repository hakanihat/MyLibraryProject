#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Models;

namespace MyLibraryApp.Controllers
{
    public class BorrowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Borrows
        public async Task<IActionResult> Index(string readerID, string oldBorrows)
        {
            if (String.IsNullOrEmpty(readerID) && String.IsNullOrEmpty(oldBorrows)) { 
            var applicationDbContext = _context.Borrows.Include(b => b.Reader).Include(c => c.Book).Where(p => p.ReturnedDate > DateTime.Now).OrderByDescending(c => c.ReturnedDate);
            return View(await applicationDbContext.ToListAsync());
            }
            else if(String.IsNullOrEmpty(readerID) && !String.IsNullOrEmpty(oldBorrows)) 
            {
                var applicationDbContext = await _context.Borrows.Include(c => c.Book).Include(b => b.Reader).OrderByDescending(a => a.ReturnedDate).ToListAsync();
           
                    return View(applicationDbContext);
                
            }
            else if (!String.IsNullOrEmpty(readerID) && String.IsNullOrEmpty(oldBorrows) && readerID.All(char.IsDigit))
            {
                var searchByReader = await _context.Borrows.Include(c => c.Book).Include(b => b.Reader).Where(r => r.ReaderId == Int32.Parse(readerID) && r.ReturnedDate > DateTime.Now).OrderByDescending(a => a.ReturnedDate).ToListAsync();
                var searchedByBook = await _context.Borrows.Include(c => c.Book).Include(b => b.Reader).Where(q => q.Isbn== Int32.Parse(readerID) && q.ReturnedDate > DateTime.Now).OrderByDescending(a => a.ReturnedDate).ToListAsync();
                if (searchByReader.Any())
                {
                    return View(searchByReader);
                }
                else if (searchedByBook.Any())
                {
                    return View(searchedByBook);
                }
                else return View();
            }
            else if(!String.IsNullOrEmpty(readerID) && !String.IsNullOrEmpty(oldBorrows) && readerID.All(char.IsDigit))
            {
                var searchByReader = await _context.Borrows.Include(c => c.Book).Include(b => b.Reader).Where(r => r.ReaderId == Int32.Parse(readerID)).OrderByDescending(a => a.ReturnedDate).ToListAsync();
                var searchedByBook = await _context.Borrows.Include(c => c.Book).Include(b => b.Reader).Where(q => q.Isbn == Int32.Parse(readerID)).OrderByDescending(a => a.ReturnedDate).ToListAsync();
                if (searchByReader.Any())
                {
                    return View(searchByReader);
                }
                else if (searchedByBook.Any())
                {
                    return View(searchedByBook);
                }
                else return View();
            }
            else
            {
                return View();
            }

        }

       

        // GET: Borrows/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "ReaderId");
            return View();
        }

        // POST: Borrows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("IdBorrow,Isbn,ReaderId,BorrowedDate,ReturnedDate")] Borrow borrow)
        {
            if(borrow.BorrowedDate > borrow.ReturnedDate)
            {
                ModelState.AddModelError("dateError", "Returned date cannot be earlier than borrowed date!");
            }
            Book book = _context.Books.Find(borrow.Isbn);
            if (ModelState.IsValid && book.IsAvaiable==true)
            {
                book.IsAvaiable = false;
                _context.Update(book);
                _context.Add(borrow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn",borrow.Isbn);
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "ReaderId", borrow.ReaderId);
            return View(borrow);
        }

        // GET: Borrows/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrows.FindAsync(id);
            if (borrow == null)
            {
                return NotFound();
            }
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn", borrow.Isbn);
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "ReaderId", borrow.ReaderId);
            return View(borrow);
        }

        // POST: Borrows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("IdBorrow,Isbn,ReaderId,BorrowedDate,ReturnedDate")] Borrow borrow)
        {
            if (id != borrow.IdBorrow)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Book book = _context.Books.Find(borrow.Isbn);
                try
                {
                    _context.Update(borrow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowExists(borrow.IdBorrow))
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
            ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn", borrow.Isbn);
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "ReaderId", borrow.ReaderId);
            return View(borrow);
        }
        [Authorize]
        public async Task<IActionResult> GiveBack(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrows.FindAsync(id);
            if (borrow == null)
            {
                return NotFound();
            }
            Book book = _context.Books.Find(borrow.Isbn);
            if (book == null)
            {
                throw new Exception();
            }
            book.IsAvaiable = true;
            borrow.ReturnedDate = DateTime.Now;
            try
            {
                _context.Update(book);
                _context.Update(borrow);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowExists(borrow.IdBorrow))
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


        // GET: Borrows/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrows
                .Include(r => r.Book)
                .Include(b => b.Reader)
                .FirstOrDefaultAsync(m => m.IdBorrow == id);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // POST: Borrows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrow = await _context.Borrows.FindAsync(id);
            Book book = _context.Books.Find(borrow.Isbn);
            book.IsAvaiable = true;
            _context.Books.Update(book);
            _context.Borrows.Remove(borrow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowExists(int id)
        {
            return _context.Borrows.Any(e => e.IdBorrow == id);
        }
    }
}

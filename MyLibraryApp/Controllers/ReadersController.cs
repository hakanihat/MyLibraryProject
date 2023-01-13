#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Models;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace MyLibraryApp.Controllers
{
    public class ReadersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index(string readerID)
        {
            if (String.IsNullOrEmpty(readerID))
            {
                return View(await _context.Readers.ToListAsync());
            }
            else
            {


                if (readerID.All(char.IsDigit)) { 
                var searchedById = await _context.Readers.Where(r=>r.ReaderId== Int32.Parse(readerID)).OrderBy(a => a.ReaderId).ToListAsync();
                    if (searchedById.Any())
                    {
                        return View(searchedById);
                    }
                }
                var searchedByNum = await _context.Readers.Where(o => o.TelNum.Equals(readerID)).OrderBy(a => a.ReaderId).ToListAsync();
               
                 if(searchedByNum.Any())
                {
                    return View(searchedByNum);
                }
                else
                return View();
            }
        }


        // GET: Readers/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Readers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ReaderId,FirstName,LastName,TelNum,Address")] Reader reader)
        {
            Regex regex = new Regex(@"^08[0-9]*$");
            Regex notDigits = new Regex(@"[\D]");
            MatchCollection matchFName = notDigits.Matches(reader.FirstName);
            MatchCollection matchLName = notDigits.Matches(reader.LastName);
            MatchCollection match = regex.Matches(reader.TelNum.ToString());
            if (!match.Any() || reader.TelNum.Length!=10)
            {
                ModelState.AddModelError("phoneErr", "The phone number must contain 10 digits and start with 08");
            }

            if (!matchFName.Any() || !matchLName.Any())
            {
                ModelState.AddModelError("fNameErr", "Name cannot contain numbers");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reader);
        }

        // GET: Readers/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
            {
                return NotFound();
            }
            return View(reader);
        }

        // POST: Readers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("ReaderId,FirstName,LastName,TelNum,Address")] Reader reader)
        {
            if (id != reader.ReaderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderExists(reader.ReaderId))
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
            return View(reader);
        }

        // GET: Readers/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers
                .FirstOrDefaultAsync(m => m.ReaderId == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            _context.Readers.Remove(reader);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReaderExists(int id)
        {
            return _context.Readers.Any(e => e.ReaderId == id);
        }
    }
}

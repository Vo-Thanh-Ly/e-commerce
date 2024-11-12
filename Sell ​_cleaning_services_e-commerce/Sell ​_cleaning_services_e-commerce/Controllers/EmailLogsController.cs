using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Data;
using Sell__cleaning_services_e_commerce.Models;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{
    public class EmailLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmailLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmailLogs.Include(e => e.Sender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EmailLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailLog = await _context.EmailLogs
                .Include(e => e.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailLog == null)
            {
                return NotFound();
            }

            return View(emailLog);
        }

        // GET: EmailLogs/Create
        public IActionResult Create()
        {
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: EmailLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ToEmail,Subject,Message,SentDate,SenderId")] EmailLog emailLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", emailLog.SenderId);
            return View(emailLog);
        }

        // GET: EmailLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailLog = await _context.EmailLogs.FindAsync(id);
            if (emailLog == null)
            {
                return NotFound();
            }
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", emailLog.SenderId);
            return View(emailLog);
        }

        // POST: EmailLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ToEmail,Subject,Message,SentDate,SenderId")] EmailLog emailLog)
        {
            if (id != emailLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailLogExists(emailLog.Id))
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
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", emailLog.SenderId);
            return View(emailLog);
        }

        // GET: EmailLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailLog = await _context.EmailLogs
                .Include(e => e.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailLog == null)
            {
                return NotFound();
            }

            return View(emailLog);
        }

        // POST: EmailLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailLog = await _context.EmailLogs.FindAsync(id);
            if (emailLog != null)
            {
                _context.EmailLogs.Remove(emailLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailLogExists(int id)
        {
            return _context.EmailLogs.Any(e => e.Id == id);
        }
    }
}

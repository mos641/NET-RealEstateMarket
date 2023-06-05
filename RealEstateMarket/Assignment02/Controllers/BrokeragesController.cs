using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealEstateMarket.Data;
using RealEstateMarket.Models;
using RealEstateMarket.Models.ViewModels;

namespace RealEstateMarket.Controllers
{
    public class BrokeragesController : Controller
    {
        private readonly MarketDbContext _context;

        public BrokeragesController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Brokerages
        public async Task<IActionResult> Index(string? Id)
        {
            var viewModel = new BrokeragesViewModel
            {
                Brokerages = await _context.Brokerages
                .Include(i => i.Subscriptions)
                .ThenInclude(Subscription => Subscription.Client)
                .AsNoTracking()
                .OrderBy(i => i.Id)
                .ToListAsync()
            };

            if (Id != null)
            {
                viewModel.Subscriptions = viewModel.Brokerages.Where(
                    x => x.Id == Id).Single().Subscriptions;
            }

            return View(viewModel);
        }

        // GET: Brokerages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Brokerages == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brokerage == null)
            {
                return NotFound();
            }

            return View(brokerage);
        }

        // GET: Brokerages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brokerages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brokerage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brokerage);
        }

        // GET: Brokerages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Brokerages == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null)
            {
                return NotFound();
            }
            return View(brokerage);
        }

        // POST: Brokerages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] Brokerage brokerage)
        {
            if (id != brokerage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brokerage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrokerageExists(brokerage.Id))
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
            return View(brokerage);
        }

        // GET: Brokerages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Brokerages == null)
            {
                return NotFound();
            }

            var brokerage = await _context.Brokerages
                .Include(x => x.Advertisements)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brokerage == null)
            {
                return NotFound();
            }

            return View(brokerage);
        }

        // POST: Brokerages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            if (_context.Brokerages == null)
            {
                return Problem("Entity set 'MarketDbContext.Brokerages'  is null.");
            }

            var brokerage = await _context.Brokerages
                .Include(x => x.Advertisements)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (brokerage != null)
            {
                if (brokerage.Advertisements.Count() > 0)
                {
                    throw new Exception("Can not delete a brokerage with existing advertisements");
                }
                _context.Brokerages.Remove(brokerage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrokerageExists(string id)
        {
          return _context.Brokerages.Any(e => e.Id == id);
        }
    }
}

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
using Microsoft.AspNetCore.Mvc.Routing;

namespace RealEstateMarket.Controllers
{
    public class ClientsController : Controller
    {
        private readonly MarketDbContext _context;

        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new BrokeragesViewModel
            {
                Clients = await _context.Clients
                .Include(Client => Client.Subscriptions)
                .ThenInclude(Subscription => Subscription.Brokerage)
                .AsNoTracking()
                .OrderBy(Client => Client.LastName)
                .ToListAsync()
            };

            if (id != null)
            {
                viewModel.Subscriptions = viewModel.Clients
                    .Where(Client => Client.Id == id).Single().Subscriptions;
            }

            return View(viewModel);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstName,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } else
            {
                throw new Exception("Invalid Client model");
            }
            // return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'MarketDbContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> EditSubscriptions(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var viewModel = new ClientSubscriptionsViewModel
            {
                Client = await _context.Clients
                .Include(Client => Client.Subscriptions)
                .ThenInclude(Subscription => Subscription.Brokerage)
                .ThenInclude(Brokerage => Brokerage.Subscriptions)
                .SingleAsync(Client => Client.Id == id),
                NonSubscribed = await _context.Brokerages
                .Include(Brokerage=> Brokerage.Subscriptions)
                .AsNoTracking()
                .OrderBy(Brokerage => Brokerage.Title)
                .ToListAsync()
             };

            if (viewModel.Client == null)
            {
                return NotFound();
            }

            // split brokerages into subscribed and unsubscribed
            viewModel.Subscribed = viewModel.Client.Subscriptions.Select(Subscription => Subscription.Brokerage).OrderBy(Brokerage => Brokerage.Title);
            viewModel.NonSubscribed = viewModel.NonSubscribed.Except(viewModel.Subscribed);

            return View(viewModel);
        }

        public async Task<ActionResult> ChangeRegistration(int? clientId, string? brokerageId, bool? register)
        {
            if (clientId == null || brokerageId == null || register == null)
            {
                return NotFound();
            }

            if (register == false)
            {
                var subscription = await _context.Subscriptions
                    .FindAsync(clientId, brokerageId);

                if (subscription == null)
                {
                    return NotFound();
                }

                _context.Subscriptions.Remove(subscription);
            }
            else
            {
                // find client and brokerage
                var brokerage = await _context.Brokerages.FindAsync(brokerageId);
                var client = await _context.Clients.FindAsync(clientId);

                if (brokerage == null || client == null)
                {
                    return NotFound();
                }

                var subscription = new Subscription();
                subscription.ClientId = client.Id;
                subscription.BrokerageId = brokerage.Id;
                subscription.Client = client;
                subscription.Brokerage = brokerage;

                // add to context
                _context.Subscriptions.Add(subscription);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("EditSubscriptions", "Clients", new { id = clientId });
        }

        private bool ClientExists(int id)
        {
          return _context.Clients.Any(e => e.Id == id);
        }
    }
}

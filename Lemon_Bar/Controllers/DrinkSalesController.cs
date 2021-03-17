using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lemon_Bar.Models;

namespace Lemon_Bar.Controllers
{
    public class DrinkSalesController : Controller
    {
        private readonly Lemon_BarContext _context;

        public DrinkSalesController(Lemon_BarContext context)
        {
            _context = context;
        }

        // GET: DrinkSales
        public async Task<IActionResult> Index()
        {
            var lemon_BarContext = _context.DrinkSales.Include(d => d.UserNavigation);
            return View(await lemon_BarContext.ToListAsync());
        }

        // GET: DrinkSales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drinkSale = await _context.DrinkSales
                .Include(d => d.UserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drinkSale == null)
            {
                return NotFound();
            }

            return View(drinkSale);
        }

        // GET: DrinkSales/Create
        public IActionResult Create()
        {
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: DrinkSales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DrinkId,NetCost,SalePrice,SaleDate,User")] DrinkSale drinkSale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drinkSale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", drinkSale.User);
            return View(drinkSale);
        }

        // GET: DrinkSales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drinkSale = await _context.DrinkSales.FindAsync(id);
            if (drinkSale == null)
            {
                return NotFound();
            }
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", drinkSale.User);
            return View(drinkSale);
        }

        // POST: DrinkSales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DrinkId,NetCost,SalePrice,SaleDate,User")] DrinkSale drinkSale)
        {
            if (id != drinkSale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drinkSale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrinkSaleExists(drinkSale.Id))
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
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", drinkSale.User);
            return View(drinkSale);
        }

        // GET: DrinkSales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drinkSale = await _context.DrinkSales
                .Include(d => d.UserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drinkSale == null)
            {
                return NotFound();
            }

            return View(drinkSale);
        }

        // POST: DrinkSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drinkSale = await _context.DrinkSales.FindAsync(id);
            _context.DrinkSales.Remove(drinkSale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrinkSaleExists(int id)
        {
            return _context.DrinkSales.Any(e => e.Id == id);
        }
    }
}

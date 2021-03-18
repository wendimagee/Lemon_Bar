﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lemon_Bar.Models;
using System.Security.Claims;

namespace Lemon_Bar.Controllers
{
    public class DrinkSalesController : Controller
    {
        private readonly Lemon_BarContext _context;
        private readonly CocktailDAL cocktailDAL = new CocktailDAL();

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

        public async Task<IActionResult> Create(int id)
        {
            DrinkSale drinkSale = new DrinkSale();
            //this is where we can take in cocktailDAL.drink and convert it to a DrinkSales object
            Rootobject d = cocktailDAL.GetIdDataString(id);
            Drink drink = d.drinks[0];
            drinkSale.DrinkId = id.ToString();
            drinkSale.User = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            drinkSale.NetCost = GetNetCost(drink);
            drinkSale.SalePrice = drinkSale.NetCost * 3;
            if (ModelState.IsValid)
            {
                _context.Add(drinkSale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", drinkSale.User);
            return RedirectToAction("Index");
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

        public decimal GetNetCost(Drink drink)
        {
            List<Item> userItems = _context.Items.Where(u => u.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            foreach (Item item in userItems)
            {
                if (drink.strIngredient1 == item.ItemName)
                {
                    if (drink.strMeasure1.ToLower().Contains("oz"))
                    {
                        decimal? ingredient1cost = item.UnitCost * ConvertFromOz(drink.strMeasure1);
                    }
                    else if (drink.strMeasure1.ToLower().Contains("ml"))
                    {
                        decimal? ingredient1cost = item.UnitCost * ConvertFromMl(drink.strMeasure1);
                    }
                    //net cost = each ingredient.UnitCost * quantity of units needed for drink(30ml vodka)
                }

            }
            decimal netCost = 0;
            return netCost;
        }
        public decimal ConvertFromOz(string measurement)
        {
            decimal measure1 = 0;
            string[] measures = measurement.Split(" ");
            foreach(string measure in measures)
            {
                if (measure.Contains("/"))
                {
                    //code from Nate
                    decimal fraction = 0.5m;
                    measure1 = (fraction + Int32.Parse(measures[0])) * 30;
                }
                else
                {
                    measure1 = Int32.Parse(measures[0]) * 30;
                }
            }
            
            return measure1;
        }
        public decimal ConvertFromMl(string measurement)
        {
            decimal measure1 = 0;
            string[] measures = measurement.Split(" ");
            foreach (string measure in measures)
            {
                if (measure.Contains("/"))
                {
                    //code from Nate
                    decimal fraction = 0.5m;
                    measure1 = (fraction + Int32.Parse(measures[0])) * 30;
                }
                else
                {
                    measure1 = Int32.Parse(measures[0]) * 30;
                }
            }

            return measure1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lemon_Bar.Models;
using System.Security.Claims;
using System.Globalization;

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
            drinkSale.SalePrice = drinkSale.NetCost * 5;
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

        public decimal? GetNetCost(Drink drink)
        {
            List<Item> userItems = _context.Items.Where(u => u.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            decimal? netCost = 0;
            foreach (Item item in userItems)
            {
                if (drink.strIngredient1.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure1.ToLower().Contains("oz"))
                    {
                        netCost += item.UnitCost * ConvertFromOz(drink.strMeasure1);
                    }
                    else if (drink.strMeasure1.ToLower().Contains("ml"))
                    {
                        netCost += item.UnitCost * ConvertFromMl(drink.strMeasure1);
                    }
                    else if (item.Garnish == true)
                    {
                        netCost += item.UnitCost * ConvertFromGarnish(drink.strMeasure1);
                    }
                }
                else if (drink.strIngredient2.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure2.ToLower().Contains("oz"))
                    {
                        netCost += item.UnitCost * ConvertFromOz(drink.strMeasure2);
                    }
                    else if (drink.strMeasure3.ToLower().Contains("ml"))
                    {
                        netCost += item.UnitCost * ConvertFromMl(drink.strMeasure1);
                    }
                    else if (item.Garnish == true)
                    {
                        netCost += item.UnitCost * ConvertFromGarnish(drink.strMeasure2);
                    }
                }
                else if (drink.strIngredient3 != null && drink.strIngredient3.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure3.ToLower().Contains("oz"))
                    {
                        netCost += item.UnitCost * ConvertFromOz(drink.strMeasure3);
                    }
                    else if (drink.strMeasure3.ToLower().Contains("ml"))
                    {
                        netCost += item.UnitCost * ConvertFromMl(drink.strMeasure3);
                    }
                    else if (item.Garnish == true)
                    {
                        netCost += item.UnitCost * ConvertFromGarnish(drink.strMeasure3);
                    }
                }
                else if (drink.strIngredient4 != null && drink.strIngredient4.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure4.ToLower().Contains("oz"))
                    {
                        netCost += item.UnitCost * ConvertFromOz(drink.strMeasure4);
                    }
                    else if (drink.strMeasure4.ToLower().Contains("ml"))
                    {
                        netCost += item.UnitCost * ConvertFromMl(drink.strMeasure4);
                    }
                    else if (item.Garnish == true)
                    {
                        netCost += item.UnitCost * ConvertFromGarnish(drink.strMeasure4);
                    }
                }
                else if (drink.strIngredient5 != null && drink.strIngredient5.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure5.ToLower().Contains("oz"))
                    {
                        netCost += item.UnitCost * ConvertFromOz(drink.strMeasure5);
                    }
                    else if (drink.strMeasure5.ToLower().Contains("ml"))
                    {
                        netCost += item.UnitCost * ConvertFromMl(drink.strMeasure5);
                    }
                    else if (item.Garnish == true)
                    {
                        netCost += item.UnitCost * ConvertFromGarnish(drink.strMeasure5);
                    }
                }
                else if (drink.strIngredient6 != null && drink.strIngredient6.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure6.ToLower().Contains("oz"))
                    {
                        netCost += item.UnitCost * ConvertFromOz(drink.strMeasure6);
                    }
                    else if (drink.strMeasure6.ToLower().Contains("ml"))
                    {
                        netCost += item.UnitCost * ConvertFromMl(drink.strMeasure6);
                    }
                    else if (item.Garnish == true)
                    {
                        netCost += item.UnitCost * ConvertFromGarnish(drink.strMeasure6);
                    }
                }
            }
            return netCost;
        }
        public decimal? ConvertFromOz(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ");
            foreach(string measure in measures)
            {
                if (measure.Contains("/"))
                {
                    if (measures[0].Length > 2)
                    {
                        decimal firstDigit = decimal.Parse(measure[0].ToString());
                        decimal secondDigit = decimal.Parse(measure[2].ToString());
                        decimal fraction = firstDigit / secondDigit;
                        decimal notFraction = decimal.Parse(measures[0]);
                        measure1 = (fraction + notFraction);
                        return measure1;
                    }
                    else
                    {
                        decimal firstDigit = decimal.Parse(measure[0].ToString());
                        decimal secondDigit = decimal.Parse(measure[2].ToString());
                        decimal fraction = firstDigit / secondDigit;
                        return fraction;
                    }

                }
                else
                {
                    measure1 = decimal.Parse(measures[0]);
                    return measure1;
                }
            }

            return measure1;
        }
        public decimal? ConvertFromMl(string measurement)
        {
            decimal measure1 = 0;
            string[] measures = measurement.Split(" ");
            foreach (string measure in measures)
            {
                if (measure.Contains("/"))
                {
                    decimal firstDigit = decimal.Parse(measure[0].ToString());
                    decimal secondDigit = decimal.Parse(measure[2].ToString());
                    decimal fraction = firstDigit / secondDigit;
                    measure1 = (fraction + decimal.Parse(measures[0].ToString())) * 0.03m;
                }
                else
                {
                    measure1 = decimal.Parse(measures[0].ToString()) * 0.03m;
                }
            }

            return measure1;
        }
        public decimal? ConvertFromGarnish(string measurement)
        {
            decimal measure1 = 0;
            string[] measures = measurement.Split(" ");
            foreach (string measure in measures)
            {
                if (measure.Contains("/"))
                {
                    decimal firstDigit = decimal.Parse(measure[0].ToString());
                    decimal secondDigit = decimal.Parse(measure[2].ToString());
                    decimal fraction = firstDigit/secondDigit;
                    measure1 = (fraction + decimal.Parse(measures[0]));
                }
                else
                {
                    measure1 = decimal.Parse(measures[0]);
                }
            }

            return measure1;
        }
    }

}

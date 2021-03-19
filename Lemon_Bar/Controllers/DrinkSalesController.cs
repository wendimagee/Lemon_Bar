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

        public async Task<IActionResult> Create(string id)
        {
            DrinkSale drinkSale = new DrinkSale();
            //this is where we can take in cocktailDAL.drink and convert it to a DrinkSales object
            Rootobject d = cocktailDAL.GetIdDataString(id);

            //Check to see if we have ingredients, store needed ingredients as list then reroute to Inventory Create


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
                        decimal? measurement = ConvertFromOz(drink.strMeasure1);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure1.ToLower().Contains("ml"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure1);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (item.Garnish == true)
                    {
                        decimal? measurement = ConvertFromGarnish(drink.strMeasure1);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure1.ToLower().Contains("shot") || drink.strMeasure1.ToLower().Contains("jigger"))
                    {
                        decimal? measurement = ConvertFromShot(drink.strMeasure1);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure1.ToLower().Contains("cl"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure1);
                        netCost += item.UnitCost * (measurement * 100);
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure1.ToLower().Contains("Top")|| drink.strMeasure1.ToLower().Contains("Fill"))
                    {
                        decimal? measurement = 2.0m;
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                }
                else if (drink.strIngredient2.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure2.ToLower().Contains("oz"))
                    {
                        decimal? measurement = ConvertFromOz(drink.strMeasure2);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure2.ToLower().Contains("ml"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure2);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (item.Garnish == true)
                    {
                        decimal? measurement = ConvertFromGarnish(drink.strMeasure2);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure2.ToLower().Contains("shot") || drink.strMeasure2.ToLower().Contains("jigger"))
                    {
                        decimal? measurement = ConvertFromShot(drink.strMeasure2);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure2.ToLower().Contains("cl"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure2);
                        netCost += item.UnitCost * (measurement * 100);
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure2.ToLower().Contains("Top") || drink.strMeasure2.ToLower().Contains("Fill"))
                    {
                        decimal? measurement = 2.0m;
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                }
                else if (drink.strIngredient3 != null && drink.strIngredient3.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure3.ToLower().Contains("oz"))
                    {
                        decimal? measurement = ConvertFromOz(drink.strMeasure3);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure3.ToLower().Contains("ml"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure3);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (item.Garnish == true)
                    {
                        decimal? measurement = ConvertFromGarnish(drink.strMeasure3);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure3.ToLower().Contains("shot") || drink.strMeasure3.ToLower().Contains("jigger"))
                    {
                        decimal? measurement = ConvertFromShot(drink.strMeasure3);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure3.ToLower().Contains("cl"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure3);
                        netCost += item.UnitCost * (measurement * 100);
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure3.ToLower().Contains("Top") || drink.strMeasure3.ToLower().Contains("Fill"))
                    {
                        decimal? measurement = 2.0m;
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                }
                else if (drink.strIngredient4 != null && drink.strIngredient4.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure4.ToLower().Contains("oz"))
                    {
                        decimal? measurement = ConvertFromOz(drink.strMeasure4);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure4.ToLower().Contains("ml"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure4);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (item.Garnish == true)
                    {
                        decimal? measurement = ConvertFromGarnish(drink.strMeasure4);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure4.ToLower().Contains("shot") || drink.strMeasure4.ToLower().Contains("jigger"))
                    {
                        decimal? measurement = ConvertFromShot(drink.strMeasure4);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure4.ToLower().Contains("cl"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure4);
                        netCost += item.UnitCost * (measurement * 100);
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure4.ToLower().Contains("Top") || drink.strMeasure4.ToLower().Contains("Fill"))
                    {
                        decimal? measurement = 2.0m;
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                }
                else if (drink.strIngredient5 != null && drink.strIngredient5.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure5.ToLower().Contains("oz"))
                    {
                        decimal? measurement = ConvertFromOz(drink.strMeasure5);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure5.ToLower().Contains("ml"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure5);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (item.Garnish == true)
                    {
                        decimal? measurement = ConvertFromGarnish(drink.strMeasure5);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure5.ToLower().Contains("shot") || drink.strMeasure5.ToLower().Contains("jigger"))
                    {
                        decimal? measurement = ConvertFromShot(drink.strMeasure5);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure5.ToLower().Contains("cl"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure5);
                        netCost += item.UnitCost * (measurement * 100);
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure5.ToLower().Contains("Top") || drink.strMeasure5.ToLower().Contains("Fill"))
                    {
                        decimal? measurement = 2.0m;
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                }
                else if (drink.strIngredient6 != null && drink.strIngredient6.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure6.ToLower().Contains("oz"))
                    {
                        decimal? measurement = ConvertFromOz(drink.strMeasure6);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure6.ToLower().Contains("ml"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure6);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (item.Garnish == true)
                    {
                        decimal? measurement = ConvertFromGarnish(drink.strMeasure6);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure6.ToLower().Contains("shot") || drink.strMeasure6.ToLower().Contains("jigger"))
                    {
                        decimal? measurement = ConvertFromShot(drink.strMeasure6);
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure6.ToLower().Contains("cl"))
                    {
                        decimal? measurement = ConvertFromMl(drink.strMeasure6);
                        netCost += item.UnitCost * (measurement * 100);
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                    else if (drink.strMeasure6.ToLower().Contains("Top") || drink.strMeasure6.ToLower().Contains("Fill"))
                    {
                        decimal? measurement = 2.0m;
                        netCost += item.UnitCost * measurement;
                        item.Quantity -= (double)(measurement);
                        _context.Items.Update(item);
                    }
                }
            }
            return netCost;
        }
        public decimal? ConvertFromOz(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ");

            if (measures[0].Contains("/") && measures[0].Length > 2)
            {
                string fractionFirst = measures[0];
                decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                decimal fraction = firstDigit / secondDigit;
                return fraction;
            }
            else if (measures[1].Contains("/"))
            {
                string fractionFirst = measures[1];
                decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                decimal fraction = firstDigit / secondDigit;
                decimal notFraction = decimal.Parse(measures[0]);
                measure1 = (fraction + notFraction);
                return measure1;
            }
            else
            {
                measure1 = decimal.Parse(measures[0]);
                return measure1;
            }
        }
        public decimal? ConvertFromMl(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ");

            if (measures[0].Contains("/") && measures[0].Length > 2)
            {
                string fractionFirst = measures[0];
                decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                decimal fraction = firstDigit / secondDigit;
                return fraction * 0.03m;
            }
            else if (measures[1].Contains("/"))
            {
                string fractionFirst = measures[1];
                decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                decimal fraction = firstDigit / secondDigit;
                decimal notFraction = decimal.Parse(measures[0]);
                measure1 = (fraction + notFraction);
                return measure1 * 0.03m;
            }
            else
            {
                measure1 = decimal.Parse(measures[0]);
                return measure1 * 0.03m;
            }
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
                    decimal fraction = firstDigit / secondDigit;
                    measure1 = (fraction + decimal.Parse(measures[0]));
                }
                else if (measure.Contains("whole") || measure.Contains("sprig"))
                {
                    measure1 = 1.0m;
                }
                else
                {
                    measure1 = decimal.Parse(measures[0]);
                }
            }

            return measure1;
        }
        public decimal? ConvertFromShot(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ");

            if (measures[0].Contains("/") && measures[0].Length > 2)
            {
                string fractionFirst = measures[0];
                decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                decimal fraction = firstDigit / secondDigit;
                return fraction * 1.5m;
            }
            else if (measures[1].Contains("/"))
            {
                string fractionFirst = measures[1];
                decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                decimal fraction = firstDigit / secondDigit;
                decimal notFraction = decimal.Parse(measures[0]);
                measure1 = (fraction + notFraction);
                return measure1 * 1.5m;
            }
            else
            {
                measure1 = decimal.Parse(measures[0]);
                return measure1 * 1.5m;
            }
        }
        
        //public bool MissingIng(Drink drink)
        //{
        //    int index = 0;



        //    List<string> ingredients = new List<string>();
        //    if (!String.IsNullOrEmpty(drink.strIngredient1)) { ingredients.Add(drink.strIngredient1); }
        //    if (!String.IsNullOrEmpty(drink.strIngredient2)) { ingredients.Add(drink.strIngredient2); }
        //    if (!String.IsNullOrEmpty(drink.strIngredient3)) { ingredients.Add(drink.strIngredient3); }
        //    if (!String.IsNullOrEmpty(drink.strIngredient4)) { ingredients.Add(drink.strIngredient4); }
        //    if (!String.IsNullOrEmpty(drink.strIngredient5)) { ingredients.Add(drink.strIngredient5); }
        //    if (!String.IsNullOrEmpty(drink.strIngredient6)) { ingredients.Add(drink.strIngredient6); }

        //    List<string> measurement = new List<string>();
        //    if (!String.IsNullOrEmpty(drink.strMeasure1)) { measurement.Add(drink.strMeasure1); }
        //    if (!String.IsNullOrEmpty(drink.strMeasure2)) { measurement.Add(drink.strMeasure2); }
        //    if (!String.IsNullOrEmpty(drink.strMeasure3)) { measurement.Add(drink.strMeasure3); }
        //    if (!String.IsNullOrEmpty(drink.strMeasure4)) { measurement.Add(drink.strMeasure4); }
        //    if (!String.IsNullOrEmpty(drink.strMeasure5)) { measurement.Add(drink.strMeasure5); }
        //    if (!String.IsNullOrEmpty(drink.strMeasure6)) { measurement.Add(drink.strMeasure6); }

        //    //if counts don't match do validation on whereever the list with the current drink is coming from

        //    List<Item> userInv = _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
        //    int count = 0;
        //    List<string> temp = new List<string>();


        //    foreach (string x in ingredients)
        //    {
        //        for (int i = 0; i < userInv.Count; i++)
        //        {
        //            if (userInv[i].ItemName.Contains(x))
        //            {
        //                count++;
        //                break;
        //            }
        //            else
        //            {
        //                temp.Add(x);
        //            }
        //        }
        //    }

        //    if (count == ingredients.Count)
        //    {
        //        validDrink = true;
        //    }


        //    if (validDrink)
        //    {

        //        filtered.Add(drink);
        //    }

        //    index++;
        //    returnList.drinks = filtered;
            
        //return returnList;
        //}

    }

}

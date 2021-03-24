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
            return View(await _context.DrinkSales.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).OrderByDescending(e => e.SaleDate).ToListAsync());
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

        public async Task<IActionResult> Create(string id, int quantity)
        {
            DrinkSale drinkSale = new DrinkSale();
            //this is where we can take in cocktailDAL.drink and convert it to a DrinkSales object
            Rootobject d = cocktailDAL.GetIdDataString(id);
            Drink drink = d.drinks[0];
            //Check to see if we have ingredients, store needed ingredients as list then reroute to Inventory Create
            bool validDrink = MissingIng(drink);
            TempData.Remove("Low");
            TempData.Remove("partial");
            TempData.Remove("partialAlt");

            if (validDrink)
            {
                List<string> ingredients = new List<string>();
                if (!String.IsNullOrEmpty(drink.strIngredient1)) { ingredients.Add(drink.strIngredient1.ToLower()); }
                if (!String.IsNullOrEmpty(drink.strIngredient2)) { ingredients.Add(drink.strIngredient2.ToLower()); }
                if (!String.IsNullOrEmpty(drink.strIngredient3)) { ingredients.Add(drink.strIngredient3.ToLower()); }
                if (!String.IsNullOrEmpty(drink.strIngredient4)) { ingredients.Add(drink.strIngredient4.ToLower()); }
                if (!String.IsNullOrEmpty(drink.strIngredient5)) { ingredients.Add(drink.strIngredient5.ToLower()); }
                if (!String.IsNullOrEmpty(drink.strIngredient6)) { ingredients.Add(drink.strIngredient6.ToLower()); }
                if (drink.strIngredient7 != null) { ingredients.Add(drink.strIngredient7.ToString()); }

                List<DrinkSale> sales = new List<DrinkSale>();
                for (int i = 0; i < quantity; i++)
                {
                    if (_context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value && ingredients.Contains(x.ItemName)).Any(x => x.Quantity < 10))
                    {
                        List<string> low = _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value &&
                        ingredients.Contains(x.ItemName) && x.Quantity < 10).Select(x => x.ItemName).ToList();

                        string inggies = "";

                        foreach (string item in low)
                        {
                            inggies += " " + item + ",";
                        }

                        inggies = inggies.Substring(0, inggies.Length - 1);
                        string lowQty = "You are running low on" + inggies + "," + " please add more!";
                        TempData["Low"] = lowQty;
                        if(i == 0)
                        {
                            TempData["partialAlt"] = "any";
                        }
                        else
                        {
                            TempData["partial"] = i.ToString();
                        }
                        break;

                    }
                    else
                    {
                        drinkSale = new DrinkSale();
                        drinkSale.DrinkId = id.ToString();
                        drinkSale.User = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        drinkSale.NetCost = GetNetCost(drink);
                        drinkSale.SalePrice = drinkSale.NetCost * 5;

                        if (ModelState.IsValid)
                        {
                            //sales.Add(new DrinkSale
                            //{
                            //    DrinkId = drinkSale.DrinkId,
                            //    NetCost = drinkSale.NetCost,
                            //    SalePrice = drinkSale.SalePrice,
                            //    SaleDate = drinkSale.SaleDate,
                            //    User = User.FindFirst(ClaimTypes.NameIdentifier).Value
                            //});
                            _context.DrinkSales.Add(drinkSale);
                            _context.SaveChanges();
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }

                }

                //if(sales.Count > 0)
                //{
                //    _context.DrinkSales.AddRange(sales);
                //    await _context.SaveChangesAsync();
                //}


                ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", drinkSale.User);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Items");
            }


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
                    if (drink.strMeasure1 != null)
                    {
                        if (!drink.strMeasure1.Contains(" "))
                        {
                            string num = "";
                            string unit = "";
                            foreach (char letter in drink.strMeasure1)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                                else
                                {
                                    unit += letter;
                                }
                            }

                            drink.strMeasure1 = num + " " + unit;

                        }

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
                            netCost += item.UnitCost * (measurement * 10);
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure1.ToLower().Contains("top") || drink.strMeasure1.ToLower().Contains("fill"))
                        {
                            decimal? measurement = 2.0m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure1.ToLower().Contains("dash"))
                        {
                            string[] measures = drink.strMeasure1.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (measures.Length > 1)
                            {
                                decimal? measurement = ConvertFromDash(drink.strMeasure1);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                            else
                            {
                                netCost += item.UnitCost * 0.021m;
                                item.Quantity -= 0.021;
                                _context.Items.Update(item);
                            }

                        }
                        else if (drink.strMeasure1.ToLower().Contains("tsp"))
                        {
                            if (drink.strMeasure1.ToLower().Contains("/"))
                            {
                                decimal? measure1 = 0;
                                string[] measures = drink.strMeasure1.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                if (measures[0].Contains("/"))
                                {
                                    string fractionFirst = measures[0];
                                    decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                                    decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                                    decimal fraction = firstDigit / secondDigit;
                                    netCost += item.UnitCost * fraction;
                                    item.Quantity -= (double)(fraction);
                                    _context.Items.Update(item);
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
                                    netCost += item.UnitCost * measure1;
                                    item.Quantity -= (double)(measure1);
                                    _context.Items.Update(item);
                                    return measure1;
                                }
                            }
                            else
                            {
                                decimal? measurement = ConvertFromTsp(drink.strMeasure1);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure1.ToLower().Contains("tbsp"))
                        {
                            decimal? measurement = ConvertFromTbsp(drink.strMeasure1);
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure1.ToLower().Contains("splash"))
                        {
                            decimal? measurement = 0.5m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else
                        {
                            string num = "";
                            foreach (char letter in drink.strMeasure1)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                            }

                            if (String.IsNullOrEmpty(num))
                            {
                                netCost += item.UnitCost * 1.00m;
                                item.Quantity -= 1.00;
                                _context.Items.Update(item);
                            }
                            else
                            {
                                decimal measure1 = decimal.Parse(num);
                                netCost += item.UnitCost * measure1;
                                item.Quantity -= (double)(measure1);
                                _context.Items.Update(item);
                            }
                        }
                    }
                }
                else if (drink.strIngredient2.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure2 != null)
                    {
                        if (!drink.strMeasure2.Contains(" "))
                        {
                            string num = "";
                            string unit = "";
                            foreach (char letter in drink.strMeasure2)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                                else
                                {
                                    unit += letter;
                                }
                            }

                            drink.strMeasure2 = num + " " + unit;

                        }

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
                            netCost += item.UnitCost * (measurement * 10);
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure2.ToLower().Contains("top") || drink.strMeasure2.ToLower().Contains("fill"))
                        {
                            decimal? measurement = 2.0m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure2.ToLower().Contains("dash"))
                        {
                            string[] measures = drink.strMeasure2.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (measures.Length > 1)
                            {
                                decimal? measurement = ConvertFromDash(drink.strMeasure2);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                            else
                            {
                                netCost += item.UnitCost * 0.021m;
                                item.Quantity -= 0.021;
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure2.ToLower().Contains("tsp"))
                        {
                            if (drink.strMeasure2.ToLower().Contains("/"))
                            {
                                decimal? measure1 = 0;
                                string[] measures = drink.strMeasure2.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                if (measures[0].Contains("/"))
                                {
                                    string fractionFirst = measures[0];
                                    decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                                    decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                                    decimal fraction = firstDigit / secondDigit;
                                    netCost += item.UnitCost * fraction;
                                    item.Quantity -= (double)(fraction);
                                    _context.Items.Update(item);
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
                                    netCost += item.UnitCost * measure1;
                                    item.Quantity -= (double)(measure1);
                                    _context.Items.Update(item);
                                    return measure1;
                                }
                            }
                            else
                            {
                                decimal? measurement = ConvertFromTsp(drink.strMeasure2);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure2.ToLower().Contains("tbsp"))
                        {
                            decimal? measurement = ConvertFromTbsp(drink.strMeasure2);
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure2.ToLower().Contains("splash"))
                        {
                            decimal? measurement = 0.5m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else
                        {
                            string num = "";
                            foreach (char letter in drink.strMeasure2)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                            }

                            if (String.IsNullOrEmpty(num))
                            {
                                netCost += item.UnitCost * 1.00m;
                                item.Quantity -= 1.00;
                                _context.Items.Update(item);
                            }
                            else
                            {
                                decimal measure1 = decimal.Parse(num);
                                netCost += item.UnitCost * measure1;
                                item.Quantity -= (double)(measure1);
                                _context.Items.Update(item);
                            }
                        }
                    }
                }
                else if (drink.strIngredient3 != null && drink.strIngredient3.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure3 != null)
                    {
                        if (!drink.strMeasure3.Contains(" "))
                        {
                            string num = "";
                            string unit = "";
                            foreach (char letter in drink.strMeasure3)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                                else
                                {
                                    unit += letter;
                                }
                            }

                            drink.strMeasure3 = num + " " + unit;

                        }

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
                            netCost += item.UnitCost * (measurement * 10);
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure3.ToLower().Contains("top") || drink.strMeasure3.ToLower().Contains("fill"))
                        {
                            decimal? measurement = 2.0m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure3.ToLower().Contains("dash"))
                        {
                            string[] measures = drink.strMeasure3.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (measures.Length > 1)
                            {
                                decimal? measurement = ConvertFromDash(drink.strMeasure3);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                            else
                            {
                                netCost += item.UnitCost * 0.021m;
                                item.Quantity -= 0.021;
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure3.ToLower().Contains("tsp"))
                        {
                            if (drink.strMeasure3.ToLower().Contains("/"))
                            {
                                decimal? measure1 = 0;
                                string[] measures = drink.strMeasure3.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                if (measures[0].Contains("/"))
                                {
                                    string fractionFirst = measures[0];
                                    decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                                    decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                                    decimal fraction = firstDigit / secondDigit;
                                    netCost += item.UnitCost * fraction;
                                    item.Quantity -= (double)(fraction);
                                    _context.Items.Update(item);
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
                                    netCost += item.UnitCost * measure1;
                                    item.Quantity -= (double)(measure1);
                                    _context.Items.Update(item);
                                    return measure1;
                                }
                            }
                            else
                            {
                                decimal? measurement = ConvertFromTsp(drink.strMeasure3);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure3.ToLower().Contains("tbsp"))
                        {
                            decimal? measurement = ConvertFromTbsp(drink.strMeasure3);
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure3.ToLower().Contains("splash"))
                        {
                            decimal? measurement = 0.5m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else
                        {
                            string num = "";
                            foreach (char letter in drink.strMeasure3)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                            }

                            if (String.IsNullOrEmpty(num))
                            {
                                netCost += item.UnitCost * 1.00m;
                                item.Quantity -= 1.00;
                                _context.Items.Update(item);
                            }
                            else
                            {
                                decimal measure1 = decimal.Parse(num);
                                netCost += item.UnitCost * measure1;
                                item.Quantity -= (double)(measure1);
                                _context.Items.Update(item);
                            }
                        }
                    }

                }
                else if (drink.strIngredient4 != null && drink.strIngredient4.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure4 != null)
                    {
                        if (!drink.strMeasure4.Contains(" "))
                        {
                            string num = "";
                            string unit = "";
                            foreach (char letter in drink.strMeasure4)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                                else
                                {
                                    unit += letter;
                                }
                            }

                            drink.strMeasure4 = num + " " + unit;

                        }

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
                            netCost += item.UnitCost * (measurement * 10);
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure4.ToLower().Contains("top") || drink.strMeasure4.ToLower().Contains("fill"))
                        {
                            decimal? measurement = 2.0m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure4.ToLower().Contains("dash"))
                        {
                            string[] measures = drink.strMeasure4.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (measures.Length > 1)
                            {
                                decimal? measurement = ConvertFromDash(drink.strMeasure4);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                            else
                            {
                                netCost += item.UnitCost * 0.021m;
                                item.Quantity -= 0.021;
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure4.ToLower().Contains("tsp"))
                        {
                            if (drink.strMeasure4.ToLower().Contains("/"))
                            {
                                decimal? measure1 = 0;
                                string[] measures = drink.strMeasure4.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                if (measures[0].Contains("/"))
                                {
                                    string fractionFirst = measures[0];
                                    decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                                    decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                                    decimal fraction = firstDigit / secondDigit;
                                    netCost += item.UnitCost * fraction;
                                    item.Quantity -= (double)(fraction);
                                    _context.Items.Update(item);
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
                                    netCost += item.UnitCost * measure1;
                                    item.Quantity -= (double)(measure1);
                                    _context.Items.Update(item);
                                    return measure1;
                                }
                            }
                            else
                            {
                                decimal? measurement = ConvertFromTsp(drink.strMeasure4);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure4.ToLower().Contains("tbsp"))
                        {
                            decimal? measurement = ConvertFromTbsp(drink.strMeasure4);
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure4.ToLower().Contains("splash"))
                        {
                            decimal? measurement = 0.5m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else
                        {
                            string num = "";
                            foreach (char letter in drink.strMeasure1)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                            }

                            if (String.IsNullOrEmpty(num))
                            {
                                netCost += item.UnitCost * 1.00m;
                                item.Quantity -= 1.00;
                                _context.Items.Update(item);
                            }
                            else
                            {
                                decimal measure1 = decimal.Parse(num);
                                netCost += item.UnitCost * measure1;
                                item.Quantity -= (double)(measure1);
                                _context.Items.Update(item);
                            }
                        }

                    }
                }
                else if (drink.strIngredient5 != null && drink.strIngredient5.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure5 != null)
                    {
                        if (!drink.strMeasure5.Contains(" "))
                        {
                            string num = "";
                            string unit = "";
                            foreach (char letter in drink.strMeasure5)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                                else
                                {
                                    unit += letter;
                                }
                            }

                            drink.strMeasure5 = num + " " + unit;

                        }

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
                            netCost += item.UnitCost * (measurement * 10);
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure5.ToLower().Contains("top") || drink.strMeasure5.ToLower().Contains("fill"))
                        {
                            decimal? measurement = 2.0m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure5.ToLower().Contains("dash"))
                        {
                            string[] measures = drink.strMeasure5.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (measures.Length > 1)
                            {
                                decimal? measurement = ConvertFromDash(drink.strMeasure5);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                            else
                            {
                                netCost += item.UnitCost * 0.021m;
                                item.Quantity -= 0.021;
                                _context.Items.Update(item);
                            }

                        }
                        else if (drink.strMeasure5.ToLower().Contains("tsp"))
                        {
                            if (drink.strMeasure5.ToLower().Contains("/"))
                            {
                                decimal? measure1 = 0;
                                string[] measures = drink.strMeasure5.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                if (measures[0].Contains("/"))
                                {
                                    string fractionFirst = measures[0];
                                    decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                                    decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                                    decimal fraction = firstDigit / secondDigit;
                                    netCost += item.UnitCost * fraction;
                                    item.Quantity -= (double)(fraction);
                                    _context.Items.Update(item);
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
                                    netCost += item.UnitCost * measure1;
                                    item.Quantity -= (double)(measure1);
                                    _context.Items.Update(item);
                                    return measure1;
                                }
                            }
                            else
                            {
                                decimal? measurement = ConvertFromTsp(drink.strMeasure5);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure5.ToLower().Contains("tbsp"))
                        {
                            decimal? measurement = ConvertFromTbsp(drink.strMeasure5);
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure5.ToLower().Contains("splash"))
                        {
                            decimal? measurement = 0.5m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else
                        {
                            string num = "";
                            foreach (char letter in drink.strMeasure5)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                            }

                            if (String.IsNullOrEmpty(num))
                            {
                                netCost += item.UnitCost * 1.00m;
                                item.Quantity -= 1.00;
                                _context.Items.Update(item);
                            }
                            else
                            {
                                decimal measure1 = decimal.Parse(num);
                                netCost += item.UnitCost * measure1;
                                item.Quantity -= (double)(measure1);
                                _context.Items.Update(item);
                            }
                        }
                    }
                }
                else if (drink.strIngredient6 != null && drink.strIngredient6.ToLower() == item.ItemName.ToLower())
                {
                    if (drink.strMeasure6 != null)
                    {
                        if (!drink.strMeasure6.Contains(" "))
                        {
                            string num = "";
                            string unit = "";
                            foreach (char letter in drink.strMeasure6)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                                else
                                {
                                    unit += letter;
                                }
                            }

                            drink.strMeasure6 = num + " " + unit;

                        }

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
                            netCost += item.UnitCost * (measurement * 10);
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure6.ToLower().Contains("top") || drink.strMeasure6.ToLower().Contains("fill"))
                        {
                            decimal? measurement = 2.0m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure6.ToLower().Contains("dash"))
                        {
                            string[] measures = drink.strMeasure6.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (measures.Length > 1)
                            {
                                decimal? measurement = ConvertFromDash(drink.strMeasure6);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                            else
                            {
                                netCost += item.UnitCost * 0.021m;
                                item.Quantity -= 0.021;
                                _context.Items.Update(item);
                            }

                        }
                        else if (drink.strMeasure6.ToLower().Contains("tsp"))
                        {
                            if (drink.strMeasure6.ToLower().Contains("/"))
                            {
                                decimal? measure1 = 0;
                                string[] measures = drink.strMeasure6.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                                if (measures[0].Contains("/"))
                                {
                                    string fractionFirst = measures[0];
                                    decimal firstDigit = decimal.Parse(fractionFirst[0].ToString());
                                    decimal secondDigit = decimal.Parse(fractionFirst[2].ToString());
                                    decimal fraction = firstDigit / secondDigit;
                                    netCost += item.UnitCost * fraction;
                                    item.Quantity -= (double)(fraction);
                                    _context.Items.Update(item);
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
                                    netCost += item.UnitCost * measure1;
                                    item.Quantity -= (double)(measure1);
                                    _context.Items.Update(item);
                                    return measure1;
                                }
                            }
                            else
                            {
                                decimal? measurement = ConvertFromTsp(drink.strMeasure6);
                                netCost += item.UnitCost * measurement;
                                item.Quantity -= (double)(measurement);
                                _context.Items.Update(item);
                            }
                        }
                        else if (drink.strMeasure6.ToLower().Contains("tbsp"))
                        {
                            decimal? measurement = ConvertFromTbsp(drink.strMeasure6);
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else if (drink.strMeasure6.ToLower().Contains("splash"))
                        {
                            decimal? measurement = 0.5m;
                            netCost += item.UnitCost * measurement;
                            item.Quantity -= (double)(measurement);
                            _context.Items.Update(item);
                        }
                        else
                        {
                            string num = "";
                            foreach (char letter in drink.strMeasure5)
                            {
                                if (Char.IsDigit(letter))
                                {
                                    num += letter;
                                }
                            }

                            if (String.IsNullOrEmpty(num))
                            {
                                netCost += item.UnitCost * 1.00m;
                                item.Quantity -= 1.00;
                                _context.Items.Update(item);
                            }
                            else
                            {
                                decimal measure1 = decimal.Parse(num);
                                netCost += item.UnitCost * measure1;
                                item.Quantity -= (double)(measure1);
                                _context.Items.Update(item);
                            }
                        }
                    }
                }
            }
            return netCost;
        }

        public decimal? ConvertFromOz(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ", StringSplitOptions.RemoveEmptyEntries);

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
            string[] measures = measurement.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            measure1 = decimal.Parse(measures[0]) / 29.574m;
            return measure1;
        }
        public decimal? ConvertFromGarnish(string measurement)
        {
            decimal measure1 = 0;
            string[] measures = measurement.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (string measure in measures)
            {
                if (measure.ToLower().Contains("/"))
                {
                    decimal firstDigit = decimal.Parse(measure[0].ToString());
                    decimal secondDigit = decimal.Parse(measure[2].ToString());
                    measure1 = firstDigit / secondDigit;
                    
                }
                else if (measure.ToLower().Contains("whole") || measure.ToLower().Contains("sprig"))
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
            string[] measures = measurement.Split(" ", StringSplitOptions.RemoveEmptyEntries);

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
        public decimal? ConvertFromDash(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            measure1 = decimal.Parse(measures[0]) * 0.021m;
            return measure1;
        }
        public decimal? ConvertFromTsp(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            measure1 = decimal.Parse(measures[0]) / 6;
            return measure1;
        }
        public decimal? ConvertFromTbsp(string measurement)
        {
            decimal? measure1 = 0;
            string[] measures = measurement.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            measure1 = decimal.Parse(measures[0]) / 2;
            return measure1;
        }

        public bool MissingIng(Drink drink)
        {

            List<string> ingredients = new List<string>();
            if (!String.IsNullOrEmpty(drink.strIngredient1)) { ingredients.Add(drink.strIngredient1.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strIngredient2)) { ingredients.Add(drink.strIngredient2.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strIngredient3)) { ingredients.Add(drink.strIngredient3.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strIngredient4)) { ingredients.Add(drink.strIngredient4.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strIngredient5)) { ingredients.Add(drink.strIngredient5.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strIngredient6)) { ingredients.Add(drink.strIngredient6.ToLower()); }
            if (drink.strIngredient7 != null) { ingredients.Add(drink.strIngredient7.ToString()); }

            List<string> measurement = new List<string>();
            if (!String.IsNullOrEmpty(drink.strMeasure1)) { measurement.Add(drink.strMeasure1.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strMeasure2)) { measurement.Add(drink.strMeasure2.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strMeasure3)) { measurement.Add(drink.strMeasure3.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strMeasure4)) { measurement.Add(drink.strMeasure4.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strMeasure5)) { measurement.Add(drink.strMeasure5.ToLower()); }
            if (!String.IsNullOrEmpty(drink.strMeasure6)) { measurement.Add(drink.strMeasure6.ToLower()); }
            if (drink.strMeasure7 != null) { measurement.Add(drink.strMeasure7.ToString()); }

            //if counts don't match do validation on whereever the list with the current drink is coming from

            int count = 0;
            List<string> temp = new List<string>();

            foreach (string ing in ingredients)
            {
                

                if (_context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).Any(x => x.ItemName.ToLower().Contains(ing)))
                {
                    count++;
                }
                else
                {
                    temp.Add(ing);
                }
            }

            if (count == ingredients.Count)
            {
                return true;
            }

            string missingString = "";

            foreach(string m in temp)
            {
                missingString += " " + m + ",";
            }
            missingString = missingString.Substring(0, missingString.Length - 1);

            TempData["Missing"] = missingString;

            return false;
        }

    }

}

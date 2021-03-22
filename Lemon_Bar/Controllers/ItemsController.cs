using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lemon_Bar.Models;
using System.Security.Claims;
using System.Data.Entity.SqlServer;

namespace Lemon_Bar.Controllers
{
    public class ItemsController : Controller
    {
        private readonly Lemon_BarContext _context;
        readonly private CocktailDAL cocktailDAL = new CocktailDAL();
        private readonly IngredientDAL _ingredient = new IngredientDAL();

        public ItemsController(Lemon_BarContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            TempData.Remove("missing");
            return View(await _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.UserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            List<Item> userItems = _context.Items.Where(u => u.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            List<Ingredient> ingredients = _ingredient.GetAllIngredients().Where(x => !userItems.Any(y => y.ItemName == x.strIngredient1)).ToList();
            //ViewBag.Ingredients = new SelectList(_ingredient.GetAllIngredients(), "strIngredient1", "strIngredient1");
            ViewBag.Ingredients = new SelectList(ingredients, "strIngredient1", "strIngredient1");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemName,TotalCost,Quantity,UnitCost,Units,Garnish,User")] Item item)
        {
            item.User = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                double factor = 1;
                if(!item.Garnish)
                {
                    switch (item.Units)
                    {
                        case "Gallon":
                            factor = 128;
                            break;
                        case "1/2Gallon":
                            factor = 64;
                            break;
                        case "Liter":
                            factor = 33.8;
                            break;
                        case "fifth":
                            factor = 25.4;
                            break;
                        
                        default:
                            break;
                    }

                    item.Quantity *= factor;
                    item.Units = "oz";
                }
                else
                {
                    item.Units = "each";
                }

                item.UnitCost = Math.Round((decimal)(item.TotalCost / (decimal)item.Quantity), 5);

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", item.User);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", item.User);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, double addTotalCost, double addQuantity, string addUnits, [Bind("Id,ItemName,TotalCost,Quantity,UnitCost,Units,Garnish,User")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                double addQty = addQuantity;
                double factor = 1;
                if (!item.Garnish)
                {
                    switch (addUnits)
                    {
                        case "Gallon":
                            factor = 128;
                            break;
                        case "1/2Gallon":
                            factor = 64;
                            break;
                        case "Liter":
                            factor = 33.8;
                            break;
                        case "fifth":
                            factor = 25.4;
                            break;

                        default:
                            break;
                    }

                    addQty = Math.Round((addQuantity * factor),2);
                }

                item.Quantity += addQty;
                //item.TotalCost += (Math.Round((decimal)addQty * (decimal)item.UnitCost, 2));
                item.TotalCost += (decimal)addTotalCost;
                //should we make this an average instead?
                item.UnitCost = Math.Round(((decimal)addTotalCost / (decimal)addQty), 5);

                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id", item.User);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.UserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RecipeList()
        {
            Rootobject recipeList = new Rootobject();

            try
            {
               recipeList =  cocktailDAL.GetPopularString();
            }
            catch
            {
                return NotFound();
            }

            Rootobject filter = new Rootobject();

            try
            {
                filter = FilterRecipes(recipeList);
            }
            catch(Exception e)
            {
                return NotFound(e);
            }

            if(filter.drinks.Count < 1)
            {
                return RedirectToAction("SearchByInventory");
            }

            return View(filter );
        }
        public async Task<IActionResult> SearchByInventory(List<Item> inventoryList)
        {
            inventoryList = await _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            return View(inventoryList);
        }
        
        public IActionResult SearchByInventoryResults(string ingredient1, string ingredient2, string ingredient3)
        {
            string searchString = $"{ingredient1}" + "," + $"{ingredient2}" + "," + $"{ingredient3}";

            Rootobject recipeList = new Rootobject();

            try
            {
                recipeList = cocktailDAL.GetInventory(searchString);
            }
            catch
            {
                return NotFound();
            }

            return View(recipeList);
        }

        private Rootobject FilterRecipes(Rootobject Drink)
        {
            int index = 0;

            Rootobject returnList= new Rootobject();
            List<Drink> filtered = new List<Drink>();
            List<Item> userInv = _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            foreach (Drink drink in Drink.drinks)
            {
                bool validDrink = false;


                List<string> ingredients= new List<string>();
                if(!String.IsNullOrEmpty(drink.strIngredient1)) { ingredients.Add(drink.strIngredient1); }
                if (!String.IsNullOrEmpty(drink.strIngredient2)) { ingredients.Add(drink.strIngredient2); }
                if (!String.IsNullOrEmpty(drink.strIngredient3)) { ingredients.Add(drink.strIngredient3); }
                if (!String.IsNullOrEmpty(drink.strIngredient4)) { ingredients.Add(drink.strIngredient4); }
                if (!String.IsNullOrEmpty(drink.strIngredient5)) { ingredients.Add(drink.strIngredient5); }
                if (!String.IsNullOrEmpty(drink.strIngredient6)) { ingredients.Add(drink.strIngredient6); }

                List<string> measurement = new List<string>();
                if (!String.IsNullOrEmpty(drink.strMeasure1)) { measurement.Add(drink.strMeasure1); }
                if (!String.IsNullOrEmpty(drink.strMeasure2)) { measurement.Add(drink.strMeasure2); }
                if (!String.IsNullOrEmpty(drink.strMeasure3)) { measurement.Add(drink.strMeasure3); }
                if (!String.IsNullOrEmpty(drink.strMeasure4)) { measurement.Add(drink.strMeasure4); }
                if (!String.IsNullOrEmpty(drink.strMeasure5)) { measurement.Add(drink.strMeasure5); }
                if (!String.IsNullOrEmpty(drink.strMeasure6)) { measurement.Add(drink.strMeasure6); }

                if (ingredients.Count != measurement.Count)
                {
                    continue;
                }

            
                int count = 0;
                
                foreach (string x in ingredients)
                {
                    for (int i = 0; i < userInv.Count; i++)
                    {
                        if (userInv[i].ItemName.Contains(x))
                        {
                            count++;
                            break;
                        }
                    }
                }

                if(count == ingredients.Count)
                {
                    validDrink = true;
                }


                if (validDrink)
                {

                    filtered.Add(drink);
                }

                index++;
                returnList.drinks = filtered;
            }
            return returnList;
        }


        public List<string> GrabUnits(string measure)
        {
            List<string> output = new List<string>();
            string unit = "";

            if (measure.Contains("/"))
            {
                string[] sent = measure.Split(" ");
                string num1 = sent[0];
                string fraction = sent[1];

                output[0] = num1 + " " + fraction;

                
                if (sent.Length > 3)
                {
                    for (int i = 2; i < sent.Length; i++)
                    {
                        unit += sent[i];
                    }
                }
                else
                {
                    unit = sent[2];
                }

                output[1] = unit;
                return output;
            }

            if (measure.Contains("."))
            {

            }


            char[] numsChar = measure.Where(Char.IsDigit).ToArray();
            string nums = numsChar.ToString();

            string[] sentence = measure.Split(" ");

            List<string> nonNum = sentence.Where(t => SqlFunctions.IsNumeric(t) == 0).ToList();

            unit = nonNum[0];

            if(nonNum.Count > 1)
            {
                for (int i = 1; i < nonNum.Count; i++)
                {
                    unit += " " + nonNum[i];
                }
            }


            output[0] = nums;
            output[1] = unit;

            return output;
        }

        public double FractionConverter(string fraction)
        {
            double integer = 0;
            double fracNum = 0;
            if (fraction.Length > 3)
            {
                integer = double.Parse(fraction.Substring(0));
                string frac = fraction.Substring(2);
                string nom = frac.Substring(0);
                string denom = frac.Last().ToString();
                double nomNum = double.Parse(nom);
                double denomNum = double.Parse(denom);
            }
            else
            {
                string nom = fraction.Substring(0);
                string denom = fraction.Last().ToString();
                double nomNum = double.Parse(nom);
                double denomNum = double.Parse(denom);
                fracNum = nomNum / denomNum;
            }

            double output = integer + fracNum;
            return output;

        }
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
       
        public async Task<IActionResult> SurplusResults()
        {
            List<Item> inventoryList = new List<Item>();
            inventoryList = await _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();

            List<Item> ordered = inventoryList.OrderByDescending(x => x.Quantity).ToList();

            Item ingredient1 = ordered[0];
            //Item ingredient2 = ordered[1];
            //Item ingredient3 = ordered[2];


            string searchString = $"{ingredient1.ItemName}";// + "," + $"{ingredient2.ItemName}" + "," + $"{ingredient3.ItemName}";

           Rootobject recipeList = new Rootobject();

            try
            {
                recipeList = cocktailDAL.GetInventory(searchString);
            }
            catch
            {
                return NotFound();
            }

            return View(recipeList);

        }

    }
}

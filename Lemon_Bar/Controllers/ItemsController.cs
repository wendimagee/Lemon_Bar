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
            TempData.Remove("Low");
            TempData.Remove("partial");
            TempData.Remove("partialAlt");
            TempData.Remove("LowQty");
            
            //Searches the user's inventory to find any low quantity items then creates a string to store in TempData to display on the page
            if (_context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).Any(x => x.Quantity < 10))
            {
                List<string> low = _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value &&
                x.Quantity < 10).Select(x => x.ItemName).ToList();
                string inggies = "";
                foreach (string item in low)
                {
                    inggies += " " + item + ",";
                }

                if(inggies.Length > 1)
                {
                    inggies = inggies.Substring(0, inggies.Length - 1);
                    string lowQty = "You are running low on" + inggies + "," + " please add more!";
                    TempData["LowQty"] = lowQty;
                }
            }

            return View(await _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            TempData.Remove("missing");
            TempData.Remove("LowQty");

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
        public IActionResult Create(string type)
        {
            TempData.Remove("missing");
            TempData.Remove("LowQty");
            ViewData["User"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            TempData["IngredType"] = type;

            //Build a list of all possible ingredients matching the selected type
            List<IngredientType> ingredientType = _context.IngredientTypes.Where(x => x.IngCategory == type).ToList();
            
            //Build a list of all inventory items specific to the current logged in user
            List<Item> userItems = _context.Items.Where(u => u.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            
            //Build a list of ingredients that are a comparison of selected type combined with current user
            //Meaning this list will be of the selected type which are not in logged in user's current inventory
            List<Ingredient> ingredients = _ingredient.GetAllIngredients().Where(x => !userItems.Any(y => y.ItemName == x.strIngredient1) && ingredientType.Any(z => z.ApistrIngredient == x.strIngredient1)).ToList();
            
            //Send the ingredients list as a dropdown selector to the view via ViewBag
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
                //Prepare for Unit and Quantity updates to force storing info relative to ounces if ingredient
                //is not a Garnish
                double factor = 1;
                if (item.Units == "each")
                {
                    item.Garnish = true;
                } else
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

                    //Finalize updates for non-Garnish ingredients
                    item.Quantity *= factor;
                    item.Units = "oz";
                }

                //Calculate and store Unit Cost in table based on Total Cost and Quantity entries from the user
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
                //Prepare updates to force storing Quantity relative to ounces if ingredient
                //is not a Garnish
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

                //Update inventory quantity 
                item.Quantity += addQty;
                
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
                recipeList = FilterRecipes(cocktailDAL.GetInventory(searchString));
            }
            catch
            {
                return View("error");
              
            }

            return View(recipeList);
        }

        private Rootobject FilterRecipes(Rootobject Drink)
        {
            Rootobject returnList = new Rootobject();
            List<Drink> filtered = new List<Drink>();

            foreach (Drink drink in Drink.drinks)
            {
                Drink drinky = new Drink();
                drinky = drink;
                bool validDrink = true;
                if (String.IsNullOrEmpty(Drink.drinks[0].strIngredient2))
                {
                    Rootobject temp = new Rootobject();
                    temp = cocktailDAL.GetIdDataString(drink.idDrink);

                    drinky = temp.drinks[0];
                }


                List<string> ingredients = new List<string>();
                if (!String.IsNullOrEmpty(drinky.strIngredient1)) { ingredients.Add(drinky.strIngredient1); }
                if (!String.IsNullOrEmpty(drinky.strIngredient2)) { ingredients.Add(drinky.strIngredient2); }
                if (!String.IsNullOrEmpty(drinky.strIngredient3)) { ingredients.Add(drinky.strIngredient3); }
                if (!String.IsNullOrEmpty(drinky.strIngredient4)) { ingredients.Add(drinky.strIngredient4); }
                if (!String.IsNullOrEmpty(drinky.strIngredient5)) { ingredients.Add(drinky.strIngredient5); }
                if (!String.IsNullOrEmpty(drinky.strIngredient6)) { ingredients.Add(drinky.strIngredient6); }
                if (drinky.strIngredient7 != null) { if (!String.IsNullOrEmpty(drinky.strIngredient7.ToString())) { ingredients.Add(drinky.strIngredient7.ToString()); } }
                if (drinky.strIngredient8 != null) { if (!String.IsNullOrEmpty(drinky.strIngredient8.ToString())) { ingredients.Add(drinky.strIngredient8.ToString()); } }
                if (drinky.strIngredient9 != null) { if (!String.IsNullOrEmpty(drinky.strIngredient9.ToString())) { ingredients.Add(drinky.strIngredient9.ToString()); } }

                List<string> measurement = new List<string>();
                if (!String.IsNullOrEmpty(drinky.strMeasure1)) { measurement.Add(drinky.strMeasure1); }
                if (!String.IsNullOrEmpty(drinky.strMeasure2)) { measurement.Add(drinky.strMeasure2); }
                if (!String.IsNullOrEmpty(drinky.strMeasure3)) { measurement.Add(drinky.strMeasure3); }
                if (!String.IsNullOrEmpty(drinky.strMeasure4)) { measurement.Add(drinky.strMeasure4); }
                if (!String.IsNullOrEmpty(drinky.strMeasure5)) { measurement.Add(drinky.strMeasure5); }
                if (!String.IsNullOrEmpty(drinky.strMeasure6)) { measurement.Add(drinky.strMeasure6); }
                if (drinky.strMeasure7 != null) { if (!String.IsNullOrEmpty(drinky.strMeasure7.ToString())) { measurement.Add(drinky.strMeasure7.ToString()); } }
                if (drinky.strMeasure8 != null) { if (!String.IsNullOrEmpty(drinky.strMeasure8.ToString())) { measurement.Add(drinky.strMeasure8.ToString()); } }
                if (drinky.strMeasure9 != null) { if (!String.IsNullOrEmpty(drinky.strMeasure9.ToString())) { measurement.Add(drinky.strMeasure9.ToString()); } }

                if (ingredients.Count > 6 || measurement.Count > 6 || measurement.Count == 0)
                {
                    validDrink = false;
                }

                //Add more conditions to test cases by inserting [validDrink = false;] to your condition, like below
                if (ingredients.Count != measurement.Count)
                {
                    validDrink = false;
                }

                foreach (string m in measurement)
                {
                    if (m.ToLower().Contains("part"))
                    {
                        validDrink = false;
                        break;
                    }
                    else if (m.ToLower().Contains("pint"))
                    {
                        validDrink = false;
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(drinky.strCategory))
                {
                    if (drinky.strCategory.ToLower().Contains("punch"))
                    {
                        validDrink = false;
                    }
                }

                if (!String.IsNullOrEmpty(drinky.strAlcoholic))
                {
                    if (drinky.strAlcoholic.ToLower().Contains("non"))
                    {
                        validDrink = false;
                    }
                }


                if (validDrink)
                {
                    filtered.Add(drinky);
                }

                returnList.drinks = filtered;

            }
            return returnList;
        }

        //Left over from before NetCost method in drink sales

        //public List<string> GrabUnits(string measure)
        //{
        //    List<string> output = new List<string>();
        //    string unit = "";

        //    if (measure.Contains("/"))
        //    {
        //        string[] sent = measure.Split(" ");
        //        string num1 = sent[0];
        //        string fraction = sent[1];

        //        output[0] = num1 + " " + fraction;


        //        if (sent.Length > 3)
        //        {
        //            for (int i = 2; i < sent.Length; i++)
        //            {
        //                unit += sent[i];
        //            }
        //        }
        //        else
        //        {
        //            unit = sent[2];
        //        }

        //        output[1] = unit;
        //        return output;
        //    }

        //    if (measure.Contains("."))
        //    {

        //    }


        //    char[] numsChar = measure.Where(Char.IsDigit).ToArray();
        //    string nums = numsChar.ToString();

        //    string[] sentence = measure.Split(" ");

        //    List<string> nonNum = sentence.Where(t => SqlFunctions.IsNumeric(t) == 0).ToList();

        //    unit = nonNum[0];

        //    if(nonNum.Count > 1)
        //    {
        //        for (int i = 1; i < nonNum.Count; i++)
        //        {
        //            unit += " " + nonNum[i];
        //        }
        //    }


        //    output[0] = nums;
        //    output[1] = unit;

        //    return output;
        //}

        //public double FractionConverter(string fraction)
        //{
        //    double integer = 0;
        //    double fracNum = 0;
        //    if (fraction.Length > 3)
        //    {
        //        integer = double.Parse(fraction.Substring(0));
        //        string frac = fraction.Substring(2);
        //        string nom = frac.Substring(0);
        //        string denom = frac.Last().ToString();
        //        double nomNum = double.Parse(nom);
        //        double denomNum = double.Parse(denom);
        //    }
        //    else
        //    {
        //        string nom = fraction.Substring(0);
        //        string denom = fraction.Last().ToString();
        //        double nomNum = double.Parse(nom);
        //        double denomNum = double.Parse(denom);
        //        fracNum = nomNum / denomNum;
        //    }

        //    double output = integer + fracNum;
        //    return output;

        //}
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
       
        public async Task<IActionResult> SurplusResults()
        {
            List<Item> inventoryList = new List<Item>();
            inventoryList = await _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Garnish == false).ToListAsync();

            List<Item> ordered = inventoryList.OrderByDescending(x => x.Quantity).ToList();

            Item ingredient1 = ordered[0];
            //Item ingredient2 = ordered[1];
            //Item ingredient3 = ordered[2];


            string searchString = $"{ingredient1.ItemName}";// + "," + $"{ingredient2.ItemName}" + "," + $"{ingredient3.ItemName}";
            searchString = searchString.Replace(" ", "_");
            Rootobject recipeList = new Rootobject();

            try
            {
                recipeList = FilterRecipes(cocktailDAL.GetInventory(searchString));
              
            }
            catch
            {
                return NotFound();
            }

            return View(recipeList);
        }
    
    }
   ;
}

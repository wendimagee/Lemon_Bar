using System;
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
            //var lemon_BarContext = _context.Items.Include(i => i.UserNavigation);

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
            ViewBag.Ingredients = new SelectList(_ingredient.GetAllIngredients(), "strIngredient1", "strIngredient1");
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,TotalCost,Quantity,UnitCost,Units,Garnish,User")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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

        public async Task<IActionResult> RecipeList()
        {
            //List<Item> inventoryList = await _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            //string com = "&&";
            //string ing = "";
            //foreach (Item item in inventoryList)
            //{
            //    string result = item.ItemName + com;
            //    ing += result;
            //}
            //string searchString = ing.Substring(0, ing.Length-1);

            Rootobject recipeList = new Rootobject();

            try
            {
               recipeList = cocktailDAL.GetDataString("old");
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

            return View(filter );
        }

        private Rootobject FilterRecipes(Rootobject Drink)
        {
            int index = 0;

            Rootobject returnList= new Rootobject();
            List<Drink> filtered = new List<Drink>();
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
                if (drink.strIngredient7 != null) { ingredients.Add(drink.strIngredient7.ToString()); }
                if (drink.strIngredient8 != null) { ingredients.Add(drink.strIngredient8.ToString()); }
                if (drink.strIngredient9 != null) { ingredients.Add(drink.strIngredient9.ToString()); }
                if (drink.strIngredient10 != null) { ingredients.Add(drink.strIngredient10.ToString()); }
                if (drink.strIngredient11 != null) { ingredients.Add(drink.strIngredient11.ToString()); }
                if (drink.strIngredient12 != null) { ingredients.Add(drink.strIngredient12.ToString()); }
                if (drink.strIngredient13 != null) { ingredients.Add(drink.strIngredient13.ToString()); }
                if (drink.strIngredient14 != null) { ingredients.Add(drink.strIngredient14.ToString()); }
                if (drink.strIngredient15 != null) { ingredients.Add(drink.strIngredient15.ToString()); }

                //if(ingredients.Count < 1)
                //{
                //    return filtered;
                //}

                List<Item> userInv = _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
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

                //filter.drinks[index] = await _context.Items.Where(x => x.ItemName.Any();
                if (validDrink)
                {

                    filtered.Add(drink);
                }

                index++;
                returnList.drinks = filtered;
            }
            return returnList;
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
        
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}

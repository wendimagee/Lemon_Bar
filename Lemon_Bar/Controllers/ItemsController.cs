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
            List<Item> inventoryList = await _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            string com = "&&";
            string ing = "";
            foreach (Item item in inventoryList)
            {
                string result = item.ItemName + com;
                ing += result;
            }
            string searchString = ing.Substring(0, ing.Length-1);

            Rootobject recipeList = new Rootobject();

            try
            {
               recipeList = cocktailDAL.GetInventory(searchString);
            }
            catch
            {
                return NotFound();
            }

            //Rootobject filter = new Rootobject();

            //foreach (Drink drink in recipeList.drinks)
            //{
                
            //}

            return View(recipeList);
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

using Lemon_Bar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lemon_Bar.Controllers
{

    public class HomeController : Controller
    {
        readonly private CocktailDAL cocktailDAL = new CocktailDAL();

        public IActionResult Index()
        {

            Rootobject r = cocktailDAL.GetDataString("manhattan");
            return View(r);
        }

        public IActionResult SearchByName(string cocktail)
        {
            Rootobject c = new Rootobject();

            if (cocktail == null)
            {
                TempData["error"] = "Please enter a valid entry";
                return RedirectToAction("Index");
            }
            try
            {
                c = cocktailDAL.GetDataString(cocktail);
            }
            catch (Exception e)
            {
                TempData["error"] = "Please enter a valid entry";
                return RedirectToAction("Index");
            }

            TempData.Remove("moveerror");
            TempData.Remove("error");

            return View(c);
        }

        public IActionResult DrinkDetails(int id)
        {
            Rootobject c = new Rootobject();
           try
            {
                c = cocktailDAL.GetIdDataString(id);
                Drink drink = c.drinks[0];
                return View(drink);
            }
            catch
            {
                return NotFound();
            }  
        }

        public IActionResult ShowMoodDrink(Drink drink)
        {// take in and show rootobject
                return View(drink);
        }

        public IActionResult DrinkByMood(string strCategory)
        {
            Rootobject c = new Rootobject();
            Random r = new Random();
            c = cocktailDAL.GetMood(strCategory);
            int rInt = r.Next(0, 10);
            //This gives us the id of cocktail at a random index on the list of category results
            int id = Int32.Parse(c.drinks[0].idDrink);
            Rootobject d = cocktailDAL.GetIdDataString(id);
            Drink drink = d.drinks[0];
            return RedirectToAction("ShowMoodDrink", drink );
        }
        public IActionResult GetMood()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
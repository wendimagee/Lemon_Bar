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

            Rootobject d = FilterRecipes(c);

            TempData.Remove("moveerror");
            TempData.Remove("error");

            return View(d);
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

        private Rootobject FilterRecipes(Rootobject Drink)
        {

            Rootobject returnList = new Rootobject();
            List<Drink> filtered = new List<Drink>();
            foreach (Drink drink in Drink.drinks)
            {
                bool validDrink = true;


                List<string> ingredients = new List<string>();
                if (!String.IsNullOrEmpty(drink.strIngredient1)) { ingredients.Add(drink.strIngredient1); }
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
                    validDrink = false;
                }

                if (validDrink)
                {

                    filtered.Add(drink);
                }

                returnList.drinks = filtered;
            }
            return returnList;
        }

    }
}
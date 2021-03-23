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

            return View();
        }

        public IActionResult SearchByName(string cocktail)
        {
            Rootobject c = new Rootobject();

            if (String.IsNullOrEmpty(cocktail))//Validation check
            {
                return RedirectToAction("Index");
            }
            try
            {
                c = cocktailDAL.GetDataString(cocktail);

                if (c.drinks == null)
                {
                    return View("error");
                }
                
                c = FilterRecipes(c);
            }
            catch (Exception e)
            {
                TempData["error"] = e;
                return RedirectToAction("Index");
            }

            TempData.Remove("error");

            return View(c);
        }

        public IActionResult DrinkDetails(string id)
        {
           Rootobject c = new Rootobject();
           try
            {
                c = cocktailDAL.GetIdDataString(id);//Returns a list of drinks even though we are pulling the rootobject by ID
            }
            catch(Exception e)
            {
               TempData["error"] = e;
               return View("error");
            }

            TempData.Remove("error");
            Drink drink = c.drinks[0];//Grabs the single drink out of the array of drinks
            return View(drink);
        }

        public IActionResult DrinkByMood(string strCategory)
        {
            Rootobject c = new Rootobject();
            Random r = new Random();
            c = cocktailDAL.GetMood(strCategory);
            c = FilterRecipes(c);

           // c = FilterRecipes(c); -- removed because FilterRecipes takes a rootobject

            int rInt = r.Next(0, c.drinks.Count);
            //This gives us the id of cocktail at a random index on the list of category results

            string iddy = c.drinks[rInt].idDrink;
            Rootobject d = cocktailDAL.GetIdDataString(iddy);
            Drink drink = d.drinks[0];

            return RedirectToAction("DrinkDetails", new {id = drink.idDrink });
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
            //if (filtered.Count != 0)
            //{
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
                    if (drink.strIngredient7 != null) { ingredients.Add(drink.strIngredient7.ToString()); }

                List<string> measurement = new List<string>();
                    if (!String.IsNullOrEmpty(drink.strMeasure1)) { measurement.Add(drink.strMeasure1); }
                    if (!String.IsNullOrEmpty(drink.strMeasure2)) { measurement.Add(drink.strMeasure2); }
                    if (!String.IsNullOrEmpty(drink.strMeasure3)) { measurement.Add(drink.strMeasure3); }
                    if (!String.IsNullOrEmpty(drink.strMeasure4)) { measurement.Add(drink.strMeasure4); }
                    if (!String.IsNullOrEmpty(drink.strMeasure5)) { measurement.Add(drink.strMeasure5); }
                    if (!String.IsNullOrEmpty(drink.strMeasure6)) { measurement.Add(drink.strMeasure6); }
                    if (drink.strMeasure7 != null) { measurement.Add(drink.strMeasure7.ToString()); }

                //Add more conditions to test cases by inserting [validDrink = false;] to your condition, like below
                if (ingredients.Count != measurement.Count)
                    {
                        validDrink = false;
                    }

                    foreach (string m in measurement)
                    {
                        if (m.Contains("part"))
                        {
                            validDrink = false;
                            break;
                        }
                        else if (m.Contains("pint"))
                        {
                            validDrink = false;
                            break;
                        }
                    }

                    //if (drink.strAlcoholic.ToLower().Contains("non"))
                    //{
                    //    validDrink = false;
                    //}

                    if (validDrink)
                    {
                        filtered.Add(drink);
                    }

                    returnList.drinks = filtered;
                //}
            }
            return returnList;
        }
    }
}
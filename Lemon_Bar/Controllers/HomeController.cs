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
            TempData.Remove("Low");
            TempData.Remove("partial");
            TempData.Remove("partialAlt");

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
            TempData.Remove("missing");
            TempData.Remove("Low");
            TempData.Remove("partial");
            TempData.Remove("partialAlt");
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
    }
}
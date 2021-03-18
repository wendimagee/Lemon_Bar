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


namespace Lemon_Bar.Helpers
{
    public class Helper
    {
        //private readonly Lemon_BarContext _context;
        //readonly private CocktailDAL cocktailDAL = new CocktailDAL();
        //private readonly IngredientDAL _ingredient = new IngredientDAL();
        //public Helper(Lemon_BarContext context)
        //{
        //    _context = context;
        //}
        //private Rootobject FilterRecipes(Rootobject Drink)
        //{
        //    int index = 0;

        //    Rootobject returnList = new Rootobject();
        //    List<Drink> filtered = new List<Drink>();
        //    foreach (Drink drink in Drink.drinks)
        //    {
        //        bool validDrink = false;


        //        List<string> ingredients = new List<string>();
        //        if (!String.IsNullOrEmpty(drink.strIngredient1)) { ingredients.Add(drink.strIngredient1); }
        //        if (!String.IsNullOrEmpty(drink.strIngredient2)) { ingredients.Add(drink.strIngredient2); }
        //        if (!String.IsNullOrEmpty(drink.strIngredient3)) { ingredients.Add(drink.strIngredient3); }
        //        if (!String.IsNullOrEmpty(drink.strIngredient4)) { ingredients.Add(drink.strIngredient4); }
        //        if (!String.IsNullOrEmpty(drink.strIngredient5)) { ingredients.Add(drink.strIngredient5); }
        //        if (!String.IsNullOrEmpty(drink.strIngredient6)) { ingredients.Add(drink.strIngredient6); }

        //        List<string> measurement = new List<string>();
        //        if (!String.IsNullOrEmpty(drink.strMeasure1)) { measurement.Add(drink.strMeasure1); }
        //        if (!String.IsNullOrEmpty(drink.strMeasure2)) { measurement.Add(drink.strMeasure2); }
        //        if (!String.IsNullOrEmpty(drink.strMeasure3)) { measurement.Add(drink.strMeasure3); }
        //        if (!String.IsNullOrEmpty(drink.strMeasure4)) { measurement.Add(drink.strMeasure4); }
        //        if (!String.IsNullOrEmpty(drink.strMeasure5)) { measurement.Add(drink.strMeasure5); }
        //        if (!String.IsNullOrEmpty(drink.strMeasure6)) { measurement.Add(drink.strMeasure6); }

        //        if (ingredients.Count != measurement.Count)
        //        {
        //            continue;
        //        }

        //        List<Item> userInv = _context.Items.Where(x => x.User == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
        //        int count = 0;

        //        foreach (string x in ingredients)
        //        {
        //            for (int i = 0; i < userInv.Count; i++)
        //            {
        //                if (userInv[i].ItemName.Contains(x))
        //                {
        //                    count++;
        //                    break;
        //                }
        //            }
        //        }

        //        if (count == ingredients.Count)
        //        {
        //            validDrink = true;
        //        }


        //        if (validDrink)
        //        {

        //            filtered.Add(drink);
        //        }

        //        index++;
        //        returnList.drinks = filtered;
        //    }
        //    return returnList;
        //}




    }
}

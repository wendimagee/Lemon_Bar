﻿using Lemon_Bar.Models;
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
        //changing this to randomize
        public IActionResult DrinkByMood(string strCategory)
        {
            Rootobject c = cocktailDAL.GetMood(strCategory);
            Random r = new Random();
            List<Drink> drinks = c.drinks.ToList();
            int rInt = r.Next(0, drinks.Count);
            //Rootobject f = drinks[rInt];
            //return View("SearchByName", c);
            return View(c);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
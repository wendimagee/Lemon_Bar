using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lemon_Bar.Models
{
    public class CocktailDAL
    {
        /// <summary>
        /// Grabs the popular recipes from the API. Then converts the API call to json using stream reader.
        /// </summary>
        /// <param ></param>
        /// <returns>A string from the json given by the API</returns>
        public string GetPopular()
        {
            //Grabs all Popular Drinks
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/popular.php";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        /// <summary>
        /// Converts string JSON to Rootobject model (Cocktail Class) for popular drinks
        /// </summary>
        /// <param ></param>
        /// <returns>Returns a Rootobject, which is under the Cocktail class</returns>
        public Rootobject GetPopularString()
        {   
            //Grabs all popular drinks
            string json = GetPopular();
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;
        }

        /// <summary>
        /// Searches the CocktailDB API for the inputed search drink name. Then converts the API call to json using stream reader.
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns>A string from the json given by the API</returns>
        public string GetData(string searchName)
        {   
            //Searches by Name
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/search.php?s={searchName}";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        /// <summary>
        /// Passes the search term to GetData to look up drink name then converts the string JSON to Rootobject model (Cocktail Class)
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns>Returns a Rootobject, which is under the Cocktail class</returns>
        public Rootobject GetDataString(string searchName)
        {   
            //Searches by Name
            string json = GetData(searchName);
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;  
        }

        /// <summary>
        /// Searches the CocktailDB API for the inputed Drink ID. Then converts the API call to json using stream reader.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A string from the json given by the API</returns>
        public string GetIdData(string id)
        {
            //Searches by ID
            string url = $"https://www.thecocktaildb.com/api/json/v2/1/lookup.php?i={id}";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        /// <summary>
        /// Passes the search term to GetIdData to look up DrinkID then converts the string JSON to Rootobject model (Cocktail Class)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a Rootobject, which is under the Cocktail class</returns>
        public Rootobject GetIdDataString(string id)
        {
            //Searches by ID
            string json = GetIdData(id);
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;
        }

        /// <summary>
        /// Searches the CocktailDB API for the inputed search term using multiple ingredients. Then converts the API call to json using stream reader.
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>A string from the json given by the API</returns>
        public string GetInventoryData(string searchString)
        {
            //Searches by multiple ingredients
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/filter.php?i={searchString}";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        /// <summary>
        /// Passes the search term to GetInventoryData to look up recipes matching search term, then converts the string JSON to Rootobject model (Cocktail Class)
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Returns a Rootobject, which is under the Cocktail class</returns>
        public Rootobject GetInventory(string searchString)
        {
            //Searches by multiple ingredients
            string json = GetInventoryData(searchString);
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;

        }

        /// <summary>
        /// Passes the search term to GetMoodData to look up recipes matching search term, then converts the string JSON to Rootobject model (Cocktail Class)
        /// </summary>
        /// <param name="cocktail"></param>
        /// <returns>Returns a Rootobject, which is under the Cocktail class</returns>
        public Rootobject GetMood(string cocktail)
        {   
            //Searches by Mood/Category
            string json = GetMoodData(cocktail);
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;
        }

        /// <summary>
        /// Searches the CocktailDB API for the based on a category the API holds. Then converts the API call to json using stream reader.
        /// </summary>
        /// <param name="cocktail"></param>
        /// <returns>A string from the json given by the API</returns>
        public string GetMoodData(string cocktail)
        {
            //Searches by Mood/Category
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/filter.php?c={cocktail}";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;

        }

        //this link is for searching by multiple ingredients https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/filter.php?i=vodka&&lemon&&lime&&cranberry
    }
}

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
        public string GetPopular()
        {
            //Searches by Name
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/popular.php";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        public Rootobject GetPopularString()
        {
            string json = GetPopular();
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;
        }
        public string GetData(string searchName)
        {
            //Searches by Name
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/search.php?s={searchName}";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();
          
            return json;
        }

        public Rootobject GetDataString(string searchName)
        {
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
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
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
            string json = GetIdData(id);
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;
        }

        public string GetInventoryData(string searchString)
        {
            //Searches by ID
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/filter.php?i={searchString}";

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = null;

            response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        public Rootobject GetInventory(string searchString)
        {
            string json = GetInventoryData(searchString);
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;

        }
        public Rootobject GetMood(string cocktail)
        {
            string json = GetMoodData(cocktail);
            Rootobject r = JsonConvert.DeserializeObject<Rootobject>(json);
            return r;
        }
        public string GetMoodData(string cocktail)
        {
            //string url = $"https://www.thecocktaildb.com/api/json/v1/1/filter.php?c={cocktail}";
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

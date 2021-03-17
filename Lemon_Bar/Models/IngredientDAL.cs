using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lemon_Bar.Models
{
    public class IngredientDAL
    {
        public string GetIngData()
        {
            string url = $"https://www.thecocktaildb.com/api/json/v2/{Secret.apikey}/list.php?i=list";
            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string json = rd.ReadToEnd();

            return json;
        }

        public List<Ingredient> GetAllIngredients()
        {
            string json = GetIngData();
            RootobjectI i = JsonConvert.DeserializeObject<RootobjectI>(json);
            return i.drinks.OrderBy(x => x.strIngredient1).ToList();
        }
    }
}

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

        public string GetData(string searchName)
        {
            //WILL MODIFY THE SEARCH
            string url = $"https://www.thecocktaildb.com/api/json/v1/1/search.php?s={searchName}";

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

        //this link is for searching by multiple ingredients https://www.thecocktaildb.com/api/json/v2/1/filter.php?i=vodka&&lemon&&lime&&cranberry

    }
}

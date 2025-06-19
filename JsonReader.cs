using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToHtmlFileConverter
{
    public class JsonReader
    {
        public JObject ReadJson(string path)
        {
            string jsonText = File.ReadAllText(path);
            var json = JObject.Parse(jsonText);
            return json;
        }
    }
}

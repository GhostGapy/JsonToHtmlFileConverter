using JsonToHtmlFileConverter;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        var finder = new JsonFileFinder();
        List<string> JsonFiles = finder.GetAllJsonFiles();

        if (JsonFiles.Count > 0)
        {
            var jsonReader = new JsonReader();
            var htmlBuilder = new HtmlBuilder();

            Console.WriteLine("Najdene datoteke: ");
            foreach (string jsonFile in JsonFiles)
            {
                // --------- Izpis najdenih datotek ---------
                string fileName = Path.GetFileName(jsonFile);
                Console.WriteLine(" - " + fileName);

                try
                {
                    // --------- Pretvorimo json file v JObject ---------
                    JObject json = jsonReader.ReadJson(jsonFile);
                    //Console.WriteLine(json.ToString() + "\n\n");


                    // --------- Pretvorimo JObject v HTML ---------
                    string html = htmlBuilder.jsonToHtml(json);
                    //Console.WriteLine(html + "\n");


                    // --------- Shranimo string v datoteko html ---------
                    htmlBuilder.SaveToHtml(html, jsonFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Napaka pri obdelavi datoteke {fileName}: {e.Message}");
                }
            }

            Console.WriteLine("\nDatoteke so bile uspešno pretvorjene v HTML");
        }
        else
        {
            Console.WriteLine("Ni bilo najdenih datotek");
        }
    }
}

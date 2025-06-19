using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToHtmlFileConverter
{
    public class JsonFileFinder
    {
        public List<string> GetAllJsonFiles()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "JSON");
            string[] files;

            if (!Directory.Exists(folderPath)) {
                return new List<string>();
            }
            else {
                files = Directory.GetFiles(folderPath, "*.json", SearchOption.TopDirectoryOnly);
                return files.ToList();
            }
        }
    }
}

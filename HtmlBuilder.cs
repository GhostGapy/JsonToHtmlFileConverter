using Newtonsoft.Json.Linq;
using System.Text;

namespace JsonToHtmlFileConverter
{
    public class HtmlBuilder
    {
        public string jsonToHtml(JObject json)
        {
            var html = new StringBuilder();

            // dodamo doctype 
            if (json.TryGetValue("doctype", out var doctype))
            {
                html.AppendLine($"<!DOCTYPE {doctype}>");
            }
            else
            {
                html.AppendLine($"<!DOCTYPE html>");
            }


            // dodamo jezik 
            if (json.TryGetValue("language", out var langToken)) 
            {
                html.AppendLine($"<html lang=\"{langToken}\">");
            }
            else
            {
                html.AppendLine($"<html>");
            }

            // obdelamo head section
            if (json.TryGetValue("head", out var headToken) && headToken is JObject head)
            {
                html.AppendLine("\t<head>");
                foreach (var prop in head.Properties())
                {
                    if (prop.Name == "meta")
                        html.Append(MetaToHtml(prop.Value)); // pretvori meta podatke
                    else if (prop.Name == "link")
                        html.Append(LinkToHtml(prop.Value)); // pretvori link tage
                    else
                        html.AppendLine($"\t\t<{prop.Name}>{prop.Value}</{prop.Name}>"); 
                }
                html.AppendLine("\t</head>");
            }

            // obdelamo body section (tudi atribute)
            if (json.TryGetValue("body", out var bodyToken) && bodyToken is JObject body)
            {
                html.Append(BodyToHtml(body, 1, "body"));
            }

            html.AppendLine("</html>");
            return html.ToString();
        }

        private string MetaToHtml(JToken metaToken)
        {
            var sb = new StringBuilder();

            if (metaToken is JObject meta)
            {
                foreach (var item in meta.Properties())
                {
                    string name = item.Name;

                    if (name == "charset") // posebej nardimo ce je charset 
                    {
                        sb.AppendLine($"\t\t<meta charset=\"{item.Value}\">");
                    }
                    else if (item.Value is JValue)
                    {
                        // tu imamo za klasicen meta tag za value
                        sb.AppendLine($"\t\t<meta name=\"{name}\" content=\"{item.Value}\">");
                    }
                    else if (item.Value is JObject objekt)
                    {
                        // tu pa imamo za meta tag ce je object
                        string content = string.Join(", ", objekt.Properties().Select(p => $"{p.Name}={p.Value}"));
                        sb.AppendLine($"\t\t<meta name=\"{name}\" content=\"{content}\">");
                    }
                }
            }

            return sb.ToString();
        }

        private string LinkToHtml(JToken linkToken)
        {
            var sb = new StringBuilder();

            if (linkToken is JArray links)
            {
                foreach (var link in links)
                {
                    var attrs = ((JObject)link).Properties()
                        .Select(p => $"{p.Name}=\"{p.Value}\"");
                    sb.AppendLine($"\t\t<link {string.Join(" ", attrs)}>");
                }
            }

            return sb.ToString();
        }

        private string BodyToHtml(JObject objekt, int indentLevel, string? parentTag = null)
        {
            var sb = new StringBuilder();
            string indent = new string('\t', indentLevel);

            // tukaj pregledamo ce ima body kaj atributov
            string attributeString = "";
            if (objekt.TryGetValue("attributes", out var attrToken) && attrToken is JObject attrs)
            {
                var attributeList = new List<string>();

                foreach (var attr in attrs.Properties())
                {
                    // spremenimo objekt v style (inline)
                    if (attr.Value is JObject styleObj && attr.Name == "style")
                    {
                        string styleString = string.Join(";", styleObj.Properties().Select(p => $"{p.Name}:{p.Value}"));
                        attributeList.Add($"style=\"{styleString}\"");
                    }
                    else
                    {
                        attributeList.Add($"{attr.Name}=\"{attr.Value}\"");
                    }
                }

                attributeString = " " + string.Join(" ", attributeList);
                objekt.Remove("attributes"); // odstranimo da se ne upodobi kot element
            }

            // ce imamo ime tag-a ga dodamo in dodamo indent
            if (parentTag != null)
            {
                sb.AppendLine($"{indent}<{parentTag}{attributeString}>");
                indentLevel++;
                indent = new string('\t', indentLevel);
            }

            // gremo cez vse key-e v objektu
            foreach (var prop in objekt.Properties())
            {
                if (prop.Value is JObject nested)
                {
                    // ce vidimo da je objekt gremo rekurzivno naprej
                    sb.Append(BodyToHtml(nested, indentLevel, prop.Name));
                }
                else if (prop.Value is JValue val)
                {
                    sb.AppendLine($"{indent}<{prop.Name}>{val}</{prop.Name}>");
                }
            }

            // ko zapuscamo rekurzivnost preverimo ali smo odprli tag
            if (parentTag != null)
            {
                indentLevel--;
                indent = new string('\t', indentLevel);
                sb.AppendLine($"{indent}</{parentTag}>");
            }

            return sb.ToString();
        }


        public void SaveToHtml(string html, string fileName)
        {
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "HTML");
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string outputPath = Path.Combine(outputDir, fileNameWithoutExtension + ".html");
            File.WriteAllText(outputPath, html);
        }
    }
}

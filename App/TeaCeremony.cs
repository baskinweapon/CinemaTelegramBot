using System.Text.Json;
using HtmlAgilityPack;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace App;

public struct TeaBase {
    public List<Tea> teas = new();

    public TeaBase() {
    }
}

public struct Tea {
    public string name;
    public string description;
    public string[] images;
    public string year;
    public string country;

    public string timeSeconds;
    public string temperature;
    public string size_gramm;
    
}

public class TeaCeremony {
    TeaBase teaBase = new();

    public TeaCeremony() {
        Load();
    }
    
    public void Load() {
        if (File.Exists("/Users/enigma/RiderProjects/NightCinemaBot/App/Saves/tea_base.json")) {
            using (StreamReader file = File.OpenText("/Users/enigma/RiderProjects/NightCinemaBot/App/Saves/tea_base.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                teaBase = (TeaBase)(serializer.Deserialize(file, typeof(TeaBase)) ?? new TeaBase());
            }
        } else {
            teaBase = new TeaBase();
        }
    }
    
    public void GetTea() {
        var site_url = "https://moychay.ru/catalog/chay";
        var page_url = "https://moychay.ru/catalog/krasnyj_chaj/tszuni-syao-e-hun";
        page_url = "https://moychay.ru/catalog/gruzinskij_chaj/dikorastuschiy-gruzinskiy-krasnyy-chay-a";

        var web = new HtmlWeb();
        var doc = web.Load(page_url);

        var nodes = doc.DocumentNode.SelectNodes("//*[@id='page-float']/div/div/div/div/div[2]");
        if (nodes == null) Console.WriteLine("Not found any teas");
        var tea = new Tea();
        tea.name = nodes.FindFirst("h1").InnerText;
        
        nodes = doc.DocumentNode.SelectNodes("//*[@id='page-float']/div/div/div/div/div[2]/article/div[2]");
        tea.description = nodes[0].InnerText.Replace("&raquo;", "").Replace("&laquo;", "").Replace("&nbsp;", " ").Replace("&deg;", "");

        nodes = doc.DocumentNode.SelectNodes("//*[@id='page-float']/div/div/div/div/div[2]/article/div[1]/div");
        var split = nodes[0].InnerText.Split("\n").Where(s => s != "").ToArray();
        var result = new string[split.Length];
        for (int i = 0; i < split.Length; i++) {
            result[i] = new String(split[i].Where(c => char.IsDigit(c)).ToArray());
        }
        
        tea.timeSeconds = result[0].Trim();
        tea.temperature = result[1].Trim();
        tea.size_gramm = result[2].Trim();
        
        nodes = doc.DocumentNode.SelectNodes("//*[@id='gallery-product-thumbnail']");
        var a_nodes = nodes[0].SelectNodes("a");

        var images = new List<string>();
        foreach (var node in a_nodes) {
            var img = "https://moychay.ru" + node.ChildNodes.FindFirst("img").Attributes["src"].Value.Replace("thumb", "original");
            Console.WriteLine(img);
            images.Add(img);
        }
        
        tea.images = images.ToArray();

        if (teaBase.teas.All(t => t.name != tea.name)) {
            teaBase.teas.Add(tea);
        } else {
            Console.WriteLine("Already exists");
        }
        Save();
    }
    
    public void Save() {
        using (StreamWriter file = File.CreateText("/Users/enigma/RiderProjects/NightCinemaBot/App/Saves/tea_base.json")) {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(file, teaBase);
        }
    }
    
    public List<string> GetBlog() {
        var site_url = "https://moychay.ru/media/articles";
        var page = 2;
        var page_url = site_url + "page_" + page;

        var web = new HtmlWeb();
        var doc = web.Load(site_url);

        var nodes = doc.DocumentNode.SelectNodes("//*[@id='tab-publication']/div[1]/div[2]/div");
        if (nodes == null) Console.WriteLine("Not found any posts");

        var blog_urls = new List<string>();
        if (nodes != null) {
            var list = nodes[0];

            foreach (var node in list.ChildNodes) {
                if (node.Name == "article") {
                    blog_urls.Add("https://moychay.ru" + node.ChildNodes.FindFirst("a").Attributes["href"].Value);
                }
            }
        }
        
        doc = web.Load(blog_urls[0]);
        nodes = doc.DocumentNode.SelectNodes("//*[@id='page-float']/div/div/div/div/article");
        
        var title = nodes[0].ChildNodes.FindFirst("h1").InnerText;

        var text = nodes[0].ChildNodes.FindFirst("div").InnerText.Replace("&nbsp;", " ");
        var main_text = new List<string>();
        var reader = new StringReader(text);
        var line = reader.ReadLine();
        while (line != null) {
            if (line == "\n") {
                line = reader.ReadLine();
                continue;
            }
            main_text.Add(line);
            line = reader.ReadLine();
        }

        var final_text = new List<string>();
        var str = "";
        foreach (var t in main_text) {
            if (t.Length == 0) continue;
            if (str.Length + t.Length >= 4090) {
                final_text.Add(str);
                str = "";
            }
            str += t + "\n";
        }

        foreach (var t in final_text) {
            Console.WriteLine(t);
        }
        
        return final_text;
    }
}
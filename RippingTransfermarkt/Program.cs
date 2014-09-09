using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RippingTransfermarkt
{

    public class Trasfer
    {
        public string name;
        public string type;
        public string age;
        public string from;
        public string to;
        public DateTime date;
        public string price;
        public string option;
    }


    class Program
    {

        static void Main(string[] args)
        {
            var webGet = new HtmlWeb();
            var list = new List<Trasfer>();

            var document = webGet.Load("http://www.transfermarkt.it/statistik/letztetransfers?ajax=yw1&page=1", "GET");
            var node = document.DocumentNode.SelectSingleNode("//div[@id='yw1']/table[@class='items']/tbody");


            foreach (var n in node.ChildNodes)
            {
                var tds = n.SelectNodes("./td");
                if (tds != null)
                {
                    try
                    {
                        list.Add(new Trasfer()
                        {
                            name = tds[0].SelectSingleNode(".//a[@class='spielprofil_tooltip']").InnerText,
                            type = tds[0].SelectSingleNode(".//table[@class='inline-table']//tr[last()]//td").InnerText,
                            age = tds[1].InnerText,
                            from = tds[4].SelectSingleNode(".//td[@class='hauptlink']//a").InnerText,
                            to = tds[3].SelectSingleNode(".//td[@class='hauptlink']//a").InnerText,
                            date = DateTime.Parse(tds[5].InnerText),
                            price = tds[6].InnerText,
                            option = tds[7].InnerText
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                }

            }

            /*

            WebRequest req = HttpWebRequest.Create("http://www.transfermarkt.it/statistik/letztetransfers?ajax=yw1&page=1");
            req.Method = "GET";

            string source;
            using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }

            Console.WriteLine(source);
             * 
             * */

        }
    }
}

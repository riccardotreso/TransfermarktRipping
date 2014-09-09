using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace RippingTransfermarkt
{

    public class Transfer
    {
        public ObjectId Id { get; set; }
        public string name;
        public string type;
        public string age;
        public string teamFrom;
        public string teamTo;
        public DateTime date;
        public string price;
        public string option;
    }


    class Program
    {

        static void Main(string[] args)
        {
            var webGet = new HtmlWeb();
            var list = new List<Transfer>();
            int page = 1;
            var url = "http://www.transfermarkt.it/statistik/letztetransfers?ajax=yw1&page={0}";
            Transfer tObject = new Transfer();
            bool continua = true;

            while (continua)
            {


                var document = webGet.Load(string.Format(url, page), "GET");
                var node = document.DocumentNode.SelectSingleNode("//div[@id='yw1']/table[@class='items']/tbody");


                foreach (var n in node.ChildNodes)
                {
                    var tds = n.SelectNodes("./td");
                    if (tds != null)
                    {
                        try
                        {
                            tObject = new Transfer()
                            {
                                name = tds[0].SelectSingleNode(".//a[@class='spielprofil_tooltip']").InnerText,
                                type = tds[0].SelectSingleNode(".//table[@class='inline-table']//tr[last()]//td").InnerText,
                                age = tds[1].InnerText,
                                teamFrom = tds[4].SelectSingleNode(".//td[@class='hauptlink']//a").InnerText,
                                teamTo = tds[3].SelectSingleNode(".//td[@class='hauptlink']//a").InnerText,
                                date = DateTime.Parse(tds[5].InnerText),
                                price = tds[6].InnerText,
                                option = tds[7].InnerText
                            };
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                        }


                        var countObject = list.Where(x => x.name == tObject.name
                                                    && x.teamFrom == tObject.teamFrom
                                                    && x.teamTo == tObject.teamTo).Count();

                        if (countObject > 0)
                        {
                            continua = false;
                            break;
                        }
                        else
                        {
                            list.Add(tObject);
                        }
                    }
                }

                ++page;
            }


            InsertIntoMongoDB(list);

        }

        private static void InsertIntoMongoDB(List<Transfer> list)
        {

            //var client = new MongoClient("mongodb://localhost:27017");
            var client = new MongoClient("mongodb://transfer:Duplicato2014@kahana.mongohq.com:10074/foootballguru");
            var server = client.GetServer();
            var database = server.GetDatabase("foootballguru");
            var collection = database.GetCollection<Transfer>("transfer");

            list.ForEach(x =>
            {
                var queryCount = (from e in collection.AsQueryable<Transfer>()
                             where e.name == x.name
                             && e.teamFrom == x.teamFrom
                             && e.teamTo == x.teamTo
                             select e).Count();

                if (queryCount == 0)
                    collection.Insert(x);
            });
        }
    }
}

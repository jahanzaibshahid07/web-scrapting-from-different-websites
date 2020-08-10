using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CREATESQL
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //string gulshan_url = "https://www.zameen.com/Homes/Karachi_Gulshan_e_Iqbal-233-";
                //int pages1 = 80;
                // 2000

                string johar_url = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages2 = 110;
                // 2800

                string bahria = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages3 = 110;
                // 6000


                string dha = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages4 = 110;
                // 4000

                string gadap = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages5 = 110;
                // 2000

                string scheme_33 = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages6 = 110;
                // 1800

                string north_naz = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages7 = 110;
                // 1800

                string malir = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages8 = 110;
                // 1000

                string cant = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages9 = 110;
                // 1000

                string fb_area = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages10 = 110;
                // 1200

                string north_karachi = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages11 = 110;
                // 1000

                string jamshed_town = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages12 = 110;
                // 950

                string korangi = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages13 = 110;
                // 950

                string clifton = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages14 = 110;
                // 800

                string nazimabad = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages15 = 110;
                // 600

                string mehmonabad = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages16 = 110;
                // 400

                string liaquatabad = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages17 = 110;
                // 350

                string civil_lines = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";
                int pages18 = 110;
                // 250

                GetAsyncDataFromZameen(johar_url, pages18);


              //  GetAsyncDataFromLamudi();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Internet not working" + ex.Message);
            }

            Console.ReadKey();
            Console.ReadKey();
        }

        static void InsertDataInSql(string price, string location, string area, string postdate)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = @"Data Source=DESKTOP-EQ3A4PJ\SQLEXPRESS;Initial Catalog=PropertyData;Integrated Security=True";
                conn.Open();

                SqlCommand cmd = new SqlCommand("InsertInPropertyTable", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Location", location);
                cmd.Parameters.AddWithValue("@Area", area);
                cmd.Parameters.AddWithValue("@PostDate", postdate);

                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        static async void GetAsyncDataFromZameen(string newurl, int pages)
        {
            try
            {
                for (int i = 1; i <= pages; i++)
                {
                    var url = newurl +
                        i.ToString()
                        + ".html";

                    var http = new HttpClient();
                    var html = await http.GetStringAsync(url);

                    var htmldocument = new HtmlDocument();
                    htmldocument.LoadHtml(html);


                    var ProductList = htmldocument.DocumentNode.Descendants("ul")
                       .Where(x => x.GetAttributeValue("class", "")
                           .Equals("_357a9937")).ToList();

                    var ProductListItems = ProductList[0].Descendants("li")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("ef447dde")).ToList();


                    Console.WriteLine("============Data From Zameen.pk=============");
                    Console.WriteLine();
                    //Console.WriteLine("Product list total = {0}", ProductListItems.Count());

                    foreach (var ProductListItem in ProductListItems)
                    {
                        // price class = c4fc20ba
                        string price = ProductListItem.Descendants("div")
                             .Where(node => node.GetAttributeValue("class", "")
                             .Equals("c4fc20ba")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                        Console.WriteLine(price);

                        // location name class = _162e6469
                        string location = ProductListItem.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("_162e6469")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                        Console.WriteLine(location);

                        // area class = _1e0ca152 _026d7bff
                        string area = ProductListItem.Descendants("div")
                              .Where(node => node.GetAttributeValue("class", "")
                              .Equals("_1e0ca152 _026d7bff")).Last().InnerText.Trim('\r', '\n', '\t');

                        Console.WriteLine(area);

                        // ad postdate class = _08b01580
                        string postdate = ProductListItem.Descendants("div")
                                  .Where(node => node.GetAttributeValue("class", "")
                                  .Equals("_08b01580")).First().InnerText.Trim('\r', '\n', '\t');

                        Console.WriteLine(postdate);

                        InsertDataInSql(price, location, area, postdate);

                        Console.WriteLine();
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Internet not working" + ex.Message);
            }
        }

        static async void GetAsyncDataFromLamudi()
        {
            try
            {
                string url1 = "https://www.prop.pk/karachi/houses-for-sale-in-gulshan-e-iqbal-town-233/?price_max=100000000&area_max=1800";

                var http = new HttpClient();
                var html = await http.GetStringAsync(url1);

                var htmldocument = new HtmlDocument();
                htmldocument.LoadHtml(html);


                var ProductList = htmldocument.DocumentNode.Descendants("ul")
                   .Where(x => x.GetAttributeValue("id", "")
                       .Equals("search_listing_section")).ToList();

                var ProductListItems = ProductList[0].Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("prop-card  clearfix white-bg")).ToList();



                //Console.WriteLine("Product list total = {0}", ProductListItems.Count());
                Console.WriteLine("============Data From Lamudi.pk=============");
                Console.WriteLine();


                foreach (var ProductListItem in ProductListItems)
                {

                    // price class = price-range
                    string price = ProductListItem.Descendants("span")
                         .Where(node => node.GetAttributeValue("class", "")
                         .Contains("price-range")).FirstOrDefault().InnerText;
                    Console.WriteLine(price);

                    // location class = location d-inline-block
                    string location = ProductListItem.Descendants("li")
                         .Where(node => node.GetAttributeValue("class", "")
                         .Contains("location d-inline-block")).FirstOrDefault().InnerText;
                    Console.WriteLine(location);

                    // area class = d-inline-block pointer-events-auto
                    string area = ProductListItem.Descendants("li")
                           .Where(node => node.GetAttributeValue("class", "")
                           .Contains("d-inline-block pointer-events-auto")).FirstOrDefault().InnerText.Trim();
                    Console.WriteLine(area);


                    // ad postdate class = potedt-date left
                    string postdate = ProductListItem.Descendants("div")
                              .Where(node => node.GetAttributeValue("class", "")
                              .Equals("potedt-date left")).FirstOrDefault().InnerText;
                    Console.WriteLine(postdate);

                    InsertDataInSql(price, location, area, postdate);

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Internet not working" + ex.Message);
            }
        }


    }
}

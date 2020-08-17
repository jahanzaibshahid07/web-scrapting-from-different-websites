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

                // zameen.pk data

                int pages = 7;

                string gulshan_url = "https://www.zameen.com/Homes/Karachi_Gulshan_e_Iqbal-233-";

                string johar_url = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";

                string bahria_url = "https://www.zameen.com/Homes/Karachi_Bahria_Town_Karachi-8298-";

                string dha_url = "https://www.zameen.com/Homes/Karachi_DHA_Defence-213-";

                string north_naz_url = "https://www.zameen.com/Homes/Karachi_North_Nazimabad-11-";

                string malir_url = "https://www.zameen.com/Homes/Karachi_Malir-476-";

                string fb_area_url = "https://www.zameen.com/Homes/Karachi_Federal_B._Area-12-";

                string korangi_url = "https://www.zameen.com/Homes/Karachi_Korangi-255-";

                string clifton_url = "https://www.zameen.com/Homes/Karachi_Clifton-5-";

                GetAsyncDataFromZameen(gulshan_url, pages);


                // lamudi.pk data

                //int pages = 7;

                //string bahria_url = "https://www.prop.pk/karachi/houses-for-sale-in-bahria-town-karachi-8298/";

                //string gulshan_url = "https://www.prop.pk/karachi/houses-for-sale-in-gulshan-e-iqbal-town-6858/";

                //string johar_url = "https://www.prop.pk/karachi/houses-for-sale-in-gulistan-e-jauhar-232/";

                //string dha_url = "https://www.prop.pk/karachi/houses-for-sale-in-dha-defence-213/";

                //string north_naz_url = "https://www.prop.pk/karachi/houses-for-sale-in-north-nazimabad-11/";

                //string malir_url = "https://www.prop.pk/karachi/houses-for-sale-in-malir-476/";

                //string fb_area_url = "https://www.prop.pk/karachi/houses-for-sale-in-federal-b-area-12/";

                //string korangi_url = "https://www.prop.pk/karachi/houses-for-sale-in-korangi-255/";

                //string clifton_url = "https://www.prop.pk/karachi/houses-for-sale-in-clifton-5/";

                //GetAsyncDataFromLamudi(gulshan_url, pages);

            }

            catch (Exception ex)
            {
                Console.WriteLine("Internet not working" + ex.Message);
            }

            Console.ReadKey();
            Console.ReadKey();
        }

        static void InsertDataInSql(Int64 price, string location, int area, string postdate)
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
                cmd.Parameters.AddWithValue("@Postdate", postdate);

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
                        string price_result = ProductListItem.Descendants("div")
                             .Where(node => node.GetAttributeValue("class", "")
                             .Equals("c4fc20ba")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                        Int64 price = 0;

                        if (price_result.Contains("Crore") == true)
                        {
                            //var price = Convert.ToDouble(Regex.Replace(price_result, @"[^0-9]", ""));
                            //Console.WriteLine(price);

                            var regex = new Regex("[\\d.]+");
                            double match = Convert.ToDouble(regex.Match(price_result).Groups[0].Value);
                            var convert_price = match * 10000000;
                            price = Convert.ToInt64(convert_price);
                            Console.WriteLine(price);
                        }
                        else if (price_result.Contains("Lakh") == true)
                        {
                            //var price = Convert.ToDouble(Regex.Replace(price_result, @"[^0-9]", ""));
                            //Console.WriteLine(price);

                            var regex = new Regex("[\\d.]+");
                            double match = Convert.ToDouble(regex.Match(price_result).Groups[0].Value);
                            var convert_price = match * 10000000;
                            price = Convert.ToInt64(convert_price);
                            Console.WriteLine(price);
                        }


                        // location name class = _162e6469
                        string location = ProductListItem.Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("_162e6469")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');

                        Console.WriteLine(location);

                        // area class = _1e0ca152 _026d7bff
                        string area_result = ProductListItem.Descendants("div")
                              .Where(node => node.GetAttributeValue("class", "")
                              .Equals("_1e0ca152 _026d7bff")).Last().InnerText.Trim('\r', '\n', '\t');


                        int area = Int32.Parse(Regex.Replace(area_result, @"[^0-9]", ""));
                        Console.WriteLine(area);

                        // ad postdate class = _08b01580
                        string postdate = ProductListItem.Descendants("div")
                                  .Where(node => node.GetAttributeValue("class", "")
                                  .Equals("_08b01580")).First().InnerText.Trim('\r', '\n', '\t');

                        
                        Console.WriteLine(postdate);
                        
                      //  InsertDataInSql(price, location, area, postdate);

                        Console.WriteLine();
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Internet not working" + ex.Message);
            }
        }
        
        static async void GetAsyncDataFromLamudi(string newurl,int pages)
        {
            try
            {
                for (int i = 1; i <= pages; i++)
                {
                    var url = newurl + i.ToString() + "/";


                    var http = new HttpClient();
                    var html = await http.GetStringAsync(url);

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
                        string price_result = ProductListItem.Descendants("span")
                             .Where(node => node.GetAttributeValue("class", "")
                             .Contains("price-range")).FirstOrDefault().InnerText;

                        Int64 price = 0;

                        if (price_result.Contains("Crore") == true)
                        {
                            //var price = Convert.ToDouble(Regex.Replace(price_result, @"[^0-9]", ""));
                            //Console.WriteLine(price);

                            var regex = new Regex("[\\d.]+");
                            double match = Convert.ToDouble(regex.Match(price_result).Groups[0].Value);
                            var convert_price = match * 10000000;
                            price = Convert.ToInt64(convert_price);
                            Console.WriteLine(price);
                        }
                        else if (price_result.Contains("Lakh") == true)
                        {
                            //var price = Convert.ToDouble(Regex.Replace(price_result, @"[^0-9]", ""));
                            //Console.WriteLine(price);

                            var regex = new Regex("[\\d.]+");
                            double match = Convert.ToDouble(regex.Match(price_result).Groups[0].Value);
                            var convert_price = match * 10000000;
                            price = Convert.ToInt64(convert_price);
                            Console.WriteLine(price);
                        }


                        // location class = location d-inline-block
                        string location = ProductListItem.Descendants("li")
                             .Where(node => node.GetAttributeValue("class", "")
                             .Contains("location d-inline-block")).FirstOrDefault().InnerText;
                        Console.WriteLine(location);

                        // area class = d-inline-block pointer-events-auto
                        string area_result = ProductListItem.Descendants("li")
                               .Where(node => node.GetAttributeValue("class", "")
                               .Contains("d-inline-block pointer-events-auto")).FirstOrDefault().InnerText.Trim();

                        int area = Int32.Parse(Regex.Replace(area_result, @"[^0-9]", ""));
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Internet not working" + ex.Message);
            }
        }

    }
}

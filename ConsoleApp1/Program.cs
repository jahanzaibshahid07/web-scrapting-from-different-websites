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

              //  int pages = 8;

                //string gulshan_url = "https://www.zameen.com/Homes/Karachi_Gulshan_e_Iqbal-233-";

                // string johar_url = "https://www.zameen.com/Homes/Karachi_Gulistan_e_Jauhar-232-";

                //string bahria_url = "https://www.zameen.com/Homes/Karachi_Bahria_Town_Karachi-8298-";

                //string dha_url = "https://www.zameen.com/Homes/Karachi_DHA_Defence-213-";

                //string north_naz_url = "https://www.zameen.com/Homes/Karachi_North_Nazimabad-11-";

                //string malir_url = "https://www.zameen.com/Homes/Karachi_Malir-476-";

                //string fb_area_url = "https://www.zameen.com/Homes/Karachi_Federal_B._Area-12-";

                //string korangi_url = "https://www.zameen.com/Homes/Karachi_Korangi-255-";

                //string clifton_url = "https://www.zameen.com/Homes/Karachi_Clifton-5-";

                //GetAsyncDataFromZameen(clifton_url, pages);


                //  lamudi.pk data

                //int pages1 = 8;

              //  string bahria_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-bahria-town-karachi-8298/";
               // string bahria_flat_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-bahria-town-karachi-8298/";

               // string gulshan_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-gulshan-e-iqbal-town-6858/";
                //string gulshan_flat_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-gulshan-e-iqbal-town-6858/";

           //    string johar_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-gulistan-e-jauhar-232/";
               // string johar_flat_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-gulistan-e-jauhar-232/";

              //  string dha_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-dha-defence-213/";
                //string dha_flat_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-dha-defence-213/";

              // string north_naz_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-north-nazimabad-11/";
               // string north_flat_naz_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-north-nazimabad-11/";

               // string malir_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-malir-476/";
             // string malir_flat_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-malir-476/";

               // string fb_area_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-federal-b-area-12/";
               // string fb_flat_area_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-federal-b-area-12/";

               //string korangi_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-korangi-255/";
             //   string korangi_flat_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-korangi-255/";

               // string clifton_url1 = "https://www.lamudi.pk/karachi/houses-for-sale-in-clifton-5/";
               // string clifton_flat_url1 = "https://www.lamudi.pk/karachi/flats-apartments-for-sale-in-clifton-5/";

                //GetAsyncDataFromLamudi(clifton_url1, pages1);


            }

            catch (Exception ex)
            {
                Console.WriteLine("Internet not working" + ex.Message);
            }

            Console.ReadKey();
            Console.ReadKey();
        }

        static void InsertDataInSql(Int64 price, string location, int area, string postdate,string titleline, string adlink)
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
                cmd.Parameters.AddWithValue("@Titleline", titleline);
                cmd.Parameters.AddWithValue("@Adlink", adlink);

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
                            var convert_price = match * 100000;
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

                        // ad titleline class = c0df3811
                        string titleline = ProductListItem.Descendants("h2")
                                  .Where(node => node.GetAttributeValue("class", "")
                                  .Equals("c0df3811")).First().InnerText.Trim('\r', '\n', '\t');

                        Console.WriteLine(titleline);

                        // ad adlink class = c0df3811
                        string adlink = ProductListItem.Descendants("a")
                            .FirstOrDefault().GetAttributeValue("href", "");

                        // Where(node => node.GetAttributeValue("href", "")
                        //.Equals("c0df3811")).First().InnerText.Trim('\r', '\n', '\t');

                        string domainname = "https://www.zameen.com";
                        string adposturl = domainname + adlink;

                        Console.WriteLine(adposturl);

                        InsertDataInSql(price, location, area, postdate, titleline, adposturl);

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
                for (int i = 5; i <= pages; i++)
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
                            var convert_price = match * 100000;
                            price = Convert.ToInt64(convert_price);
                            Console.WriteLine(price);
                        }



                        // location class = location d-inline-block
                        string location = ProductListItem.Descendants("div")
                             .Where(node => node.GetAttributeValue("class", "")
                             .Contains("location-wrap text-truncate")).FirstOrDefault().InnerText.Trim(); 
                        Console.WriteLine(location);

                        //// area class = d-inline-block pointer-events-auto
                        string area_result = ProductListItem.Descendants("li")
                               .Where(node => node.GetAttributeValue("class", "")
                               .Contains("d-inline-block pointer-events-auto area")).FirstOrDefault().InnerText.Trim();

                         int area = Int32.Parse(Regex.Replace(area_result, @"[^0-9]", ""));
                       //  int area = Convert.ToInt32(area_con * 0.111111);

                        Console.WriteLine(area);

                        //// ad postdate class = potedt-date left
                        string postdate = ProductListItem.Descendants("div")
                                  .Where(node => node.GetAttributeValue("class", "")
                                  .Equals("potedt-date left")).FirstOrDefault().InnerText;
                        Console.WriteLine(postdate);

                        //// ad titleline class = c0df3811
                        string titleline = ProductListItem.Descendants("div")
                                  .Where(node => node.GetAttributeValue("class", "")
                                  .Contains("title text-truncate")).First().InnerText.Trim('\r', '\n', '\t');

                        Console.WriteLine(titleline);

                        //// ad adlink class = c0df3811
                        string adlink = ProductListItem.Descendants("a")
                            .FirstOrDefault().GetAttributeValue("href", "");


                        ////string domainname = "zameen.com";
                        ////string adposturl = domainname + adlink;

                        Console.WriteLine(adlink);


                        //string price = ProductListItem.Descendants("span")
                        //   .FirstOrDefault().GetAttributeValue("data-def_price", "");
                        //Console.WriteLine(price);

                       InsertDataInSql(price, location, area, postdate, titleline, adlink);

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

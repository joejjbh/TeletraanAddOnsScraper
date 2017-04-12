using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AddOnsScraper
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void PrintBrandDataToFile(List<BrandData> brands)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\dev\brandData.txt"))
            {
                foreach (var brand in brands)
                {
                    file.WriteLine("BrandName: " + brand.Name);
                    file.WriteLine("Addons: ");
                    foreach (var addOn in brand.AddOns)
                    {
                        file.WriteLine(
                            $"    Category: {addOn.Category}\r\n    SubType: {addOn.Subtype}\r\n    Summary: {addOn.Summary}");
                        file.WriteLine("    Included: ");
                        foreach (var included in addOn.Included)
                        {
                            file.WriteLine("      - " + included);
                        }
                        file.WriteLine("    Excluded: ");
                        foreach (var excluded in addOn.Excluded)
                        {
                            file.WriteLine("      - " + excluded);
                        }
                        file.WriteLine();
                    }
                    file.WriteLine(new String('-', 100));
                }
            }
            
        }

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8077/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("applications/json"));

            try
            {
                var url = client.BaseAddress + "api/car/brands/published";

                var listOfBrandData = await GetBrandsData(url);
                PrintBrandDataToFile(listOfBrandData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

      

        static async Task<List<BrandData>> GetBrandsData(string path)
        {
            var listOfBrandsData = new  List<BrandData>();
            
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                string brandAddOns = await response.Content.ReadAsStringAsync();

                JObject jObject = JObject.Parse(brandAddOns);
                foreach (var brand in jObject["result"])
                {
                    var brandData = new BrandData();
                    brandData.AddOns = new List<AddOn>();
                    JToken jAddons = brand["data"]["addons"];

                    
                    brandData.AddOns = GetAddons(jAddons);
                    brandData.Name = (string) brand["data"]["name"];
                    listOfBrandsData.Add(brandData);
                }
            }
            return listOfBrandsData;
        }

        private static List<AddOn> GetAddons(JToken jAddons)
        {
            var listOfAddons = new List<AddOn>();
            foreach (var jAddon in jAddons)
            {
                var addOn = new AddOn();

                addOn.Category = (string) jAddon["category"];
                addOn.Subtype = (string) jAddon["subType"];
                addOn.Summary = (string) jAddon["summary"];

                addOn.Included = new List<string>();
                foreach (var incAddon in jAddon["included"])
                {
                    addOn.Included.Add(incAddon.ToString());
                }

                addOn.Excluded = new List<string>();
                foreach (var incAddon in jAddon["excluded"])
                {
                    addOn.Excluded.Add(incAddon.ToString());
                }

                listOfAddons.Add(addOn);
            }
            return listOfAddons;
        }
    }
}

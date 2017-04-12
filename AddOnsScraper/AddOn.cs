using System.Collections.Generic;

namespace AddOnsScraper
{
    public class AddOn
    {
        public string Category { get; set; }
        public string Subtype { get; set; }
        public string Summary { get; set; }
        public List<string> Included { get; set; }
        public List<string> Excluded { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DinnerBot.Models
{
    public class RootObject
    {
        public int total_entries { get; set; }
        public int per_page { get; set; }
        public int current_page { get; set; }
        public List<Restaurant> restaurants { get; set; }
    }
}
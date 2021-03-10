using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class Product
    {
        public ulong Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        public int Stock { get; set; }

        public DateTime AtCreated { get;  set; }
        public DateTime AtUpdate { get; set; }
        public int Price { get; set; }
    }
}

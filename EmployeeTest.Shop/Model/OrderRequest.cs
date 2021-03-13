using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class OrderRequest
    {
        public ulong Product { get; set; }
        public int Count { get; set; }
    }
}

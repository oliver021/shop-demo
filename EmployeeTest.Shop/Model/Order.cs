using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class Order
    {
        public uint Id { get; set; }

        public Product Product { get; set; }
        public ulong ProductId { get; set; }

        public DateTime AtMoment { get; set; }

        public int Count { get; set; }
    }
}

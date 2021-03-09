using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTest.Shop.Model
{
    public class Product
    {
        public ulong Id { get; set; }
        public DateTime AtCreated { get;  set; }
        public DateTime AtUpdate { get; set; }
    }
}

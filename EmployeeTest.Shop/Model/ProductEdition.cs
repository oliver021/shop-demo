using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class ProductEdition
    {
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public int Stock { get; set; }
    }
}

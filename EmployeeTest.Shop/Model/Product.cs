using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class Product
    {
        public ulong Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [MaxLength(100)]
        public string Description { get; set; }

        public int Stock { get; set; }

        public DateTime AtCreated { get;  set; }
        public DateTime AtUpdate { get; set; }

        [Required]
        public int Price { get; set; }

        public User User { get; set; }
        public ulong UserId { get; set; } = default;

        public List<Order> Orders { get; set; }
    }
}

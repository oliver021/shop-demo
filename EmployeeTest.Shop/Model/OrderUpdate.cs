using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class OrderUpdate
    {
        [Required]
        public uint Id { get; set; }

        [Required]
        public OrderStatus NewStatus { get; set; }
    }
}

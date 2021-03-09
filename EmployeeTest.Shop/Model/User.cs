using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class User
    {
        public ulong Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public ulong LastName { get; set; }
        public string Password { get; set; }
    }
}

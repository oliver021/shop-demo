using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Model
{
    public class User : IdentityUser<ulong>
    {
        public string FirstName { get; set; }
        public ulong LastName { get; set; }
        public string Password { get; set; }
    }
}

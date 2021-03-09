using OliDemos.Shop.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Services
{
    public class ShopContext : DbContext
    {

        public ShopContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            base.OnModelCreating(builder);

            builder.Entity<Product>();
            builder.Entity<Order>();
        }
    }
}

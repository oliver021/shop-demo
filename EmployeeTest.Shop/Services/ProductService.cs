using Microsoft.EntityFrameworkCore;
using OliDemos.Shop.Model;
using OliDemos.Shop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Services
{
    public class ProductService : EfRepository<Product>
    {
        public ProductService(DbContext context) : base(context)
        {
        }

        public async Task UpdateAsync(ulong userId, ulong id, ProductEdition edition, bool force)
        {
            if (edition is null)
            {
                throw new ArgumentNullException(nameof(edition));
            }

            var product = await FindOne(id);

            if (!force && !product.UserId.Equals(userId))
            {
                throw new InvalidOperationException("not owner");
            }

            if (edition.Name != null)
            {
                product.Name = edition.Name;
            }

            if (edition.Description != null)
            {
                product.Description = edition.Description;
            }

            if (!edition.Stock.Equals(default))
            {
                product.Stock = edition.Stock;
            }

            await UpdateAsync(product);
        }

        public async Task<int> DeleteAsync(ulong user, ulong id, bool force)
        {
            var product = await FindOne(id);

            if (!force && !product.UserId.Equals(user))
            {
                throw new InvalidOperationException("not owner");
            }

            return await DeleteAsync(product);
        }


        public async Task<ulong> PurchaseAsync(ulong product, ulong user, int count = 1)
        {
            Order order;
            var target = await FindOne(product);
            target.Orders.Add(order = new Order { 
                ProductId = product,
                UserId = user,
                AtMoment = DateTime.Now,
                Count = count,
            });
            await UpdateAsync(target);
            return order.Id;
        }
    }
}

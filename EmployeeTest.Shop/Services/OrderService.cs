using Microsoft.EntityFrameworkCore;
using OliDemos.Shop.Model;
using OliDemos.Shop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Services
{
    public class OrderService : EfRepository<Order>
    {
        public OrderService(DbContext context) : base(context)
        {
        }

        public async Task UpdateAsync(OrderUpdate update)
        {
            var order = await FindOne(update);
            order.Status = update.NewStatus;
            await UpdateAsync(order);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OliDemos.Shop.Model;
using OliDemos.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OliDemos.Shop.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("/api/[controller]")]
    public class OrderController : Controller
    {
        private readonly OrderService service;

        public OrderController(OrderService service)
        {
            this.service = service;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdate update)
        {
            await service.UpdateAsync(update);
            return NoContent();
        }

        [HttpDelete("{order}")]
        public async Task<IActionResult> Delete([FromRoute] uint order)
        {
            await service.DeleteAsync(order);
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OliDemos.Shop.Controllers
{
    [ApiController]
    [Authorize("Admin")]
    [Route("/api/[controller]")]
    public class OrderController : Controller
    {
    }
}

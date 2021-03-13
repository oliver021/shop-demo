using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OliDemos.Shop.Model;
using OliDemos.Shop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using OliDemos.Shop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace OliDemos.Shop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [OpenApiTag("data", Description = "The product controller to manage this entity")]
    public class ProductController : Controller
    {
        private const string AuthDesc = "Require authentication to do a request";
        private readonly ProductService _serviceProduct;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ProductService serv, ILogger<ProductController> logger)
        {
            _serviceProduct = serv ?? throw new ArgumentNullException(nameof(serv));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Devuelve un listado de todos los registros de datos
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [OpenApiOperation("Bearer", AuthDesc)]
        public async Task<JsonResult> GetAllAsync()
        {
            return Json(await _serviceProduct.FindAll());
        }

        /// <summary>
        /// Devueleve un listado filtrado de registros de datos
        /// </summary>
        /// <param name="page"></param>
        /// <param name="length"></param>
        /// <param name="search"></param>
        /// <param name="sort"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        [HttpGet("index")]
        [OpenApiOperation("Bearer", AuthDesc)]
        public async Task<List<Product>> GetAsync(int minPrice = 0, int maxPrice = 0, int page = 0, int length = 25)
        {
            var result = await _serviceProduct.Find(q =>
            {
                if (minPrice > 0)
                {
                    q = q.Where(p => p.Price >= minPrice);
                }

                if (maxPrice > 0)
                {
                    q = q.Where(p => p.Price <= maxPrice);
                }

                return q;
                //lambda end
            }, page, length);

            if (result.Count < 1)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Devuelve un unico registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [OpenApiOperation("Bearer", AuthDesc)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] ulong id)
        {
            var result =  await _serviceProduct.FindOne(id);
            if (result is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        /// <summary>
        /// Agrega un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [OpenApiOperation("Bearer", AuthDesc)]
        public async Task<IActionResult> AddAsync([FromBody] Product product)
        {
            // set user onwer by claim id
            product.UserId = HttpContext.GetUserId();
            try
            {
                var result = await _serviceProduct.StoreAnsyc(product);
                return StatusCode(201, new
                {
                    product.Id,
                    product.AtCreated
                });
            }
            catch (Exception)
            {
                // ignore for now just send 400
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Agrega un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("purchase")]
        [Authorize(Roles = "Client")]
        [OpenApiOperation("Bearer", AuthDesc)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PurshaseAsync([FromBody] OrderRequest req)
        {
            try
            {
                var result = await _serviceProduct.PurchaseAsync(product: req.Product,
                    user: HttpContext.GetUserId(),
                    count: req.Count);
                return StatusCode(201, new
                {
                   orderId = result
                });
            }
            catch (InvalidOperationException err)
            when(err.Message.Equals(ProductService.NotStockMessage))
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new
                {
                    message = "No hay stock para la cantidad solicitada"
                });
            }
            catch (InvalidOperationException err)
            when (err.Message.Equals(ProductService.ProductNotFound))
            {
                return StatusCode(StatusCodes.Status404NotFound, new
                {
                    message = "No hay el producto solicitado"
                });
            }
            catch (Exception err)
            {
                _logger.LogDebug(err.Message);
                // ignore for now just send 400
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Actualiza un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [OpenApiOperation("Bearer", AuthDesc)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateAsync([FromRoute] ulong id, [FromBody] ProductEdition data)
        {
            try
            {
                await _serviceProduct.UpdateAsync(userId: HttpContext.GetUserId(),
                    id: id, 
                    edition: data,
                    force: HttpContext.IsAdmin());
            }
            catch(InvalidOperationException err)
            when(err.Message == "not owner")
            {
                return Unauthorized();
            }
            catch (Exception err)
            {
                _logger.LogDebug("Error:{0};\nMessage:{1}", err.GetType().Name, err.Message);
            }
            return NoContent();
        }

        /// <summary>
        /// Elimina un registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller,Admin")]
        [OpenApiOperation("Bearer", AuthDesc)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync([FromRoute] ulong id)
        {
            try
            {
                await _serviceProduct.DeleteAsync(HttpContext.GetUserId(), id, HttpContext.IsAdmin());
            }
            catch (InvalidOperationException err)
            when (err.Message == "not owner")
            {
                return Unauthorized();
            }
            catch (Exception err)
            {
                _logger.LogDebug("Error:{0};\nMessage:{1}", err.GetType().Name, err.Message);
            }
            return NoContent();
        }
    }
}

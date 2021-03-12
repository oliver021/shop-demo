using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OliDemos.Shop.Model;
using OliDemos.Shop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace OliDemos.Shop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [OpenApiTag("data", Description = "The product controller to manage this entity")]
    public class ProductController : Controller
    {
        private const string AuthDesc = "Require authentication to do a request";
        private readonly IRepository<Product> _repository;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IRepository<Product> repository, ILogger<ProductController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
            return Json(await _repository.FindAll());
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
            var result = await _repository.Find(q =>
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
        public async Task<Product> GetByIdAsync([FromRoute] ulong id)
        {
            return await _repository.FindOne(id);
        }

        /// <summary>
        /// Agrega un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [OpenApiOperation("Bearer", AuthDesc)]
        public async Task<IActionResult> AddAsync([FromBody] Product data)
        {
            try
            {
                var result = await _repository.StoreAnsyc(data);
                return StatusCode(201, new
                {
                    data.Id,
                    data.AtCreated
                });
            }
            catch (Exception)
            {
                // ignore for now just send 400
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Actualiza un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        [OpenApiOperation("Bearer", AuthDesc)]
        public async Task<NoContentResult> UpdateAsync([FromBody] Product data)
        {
            try
            {
                await _repository.UpdateAsync(data);
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
        [HttpDelete]
        [OpenApiOperation("Bearer", AuthDesc)]
        public async Task<NoContentResult> DeleteAsync([FromRoute] ulong id)
        {
            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception err)
            {
                _logger.LogDebug("Error:{0};\nMessage:{1}", err.GetType().Name, err.Message);
            }
            return NoContent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeTest.Shop.Model;
using EmployeeTest.Shop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeTest.Shop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
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
        public async Task<JsonResult> GetAsync([FromRoute] int page, int length)
        {
            return Json(await _repository.Find(page, length));
        }

        /// <summary>
        /// Devuelve un unico registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
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
                // ignore for now
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Actualiza un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<NoContentResult> UpdateAsync([FromBody] Product data)
        {
            try
            {
                await _repository.UpdateAsync(data);
            }
            catch (Exception)
            {
                // ignore for now
            }
            return NoContent();
        }

        /// <summary>
        /// Elimina un registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<NoContentResult> DeleteAsync([FromRoute] ulong id)
        {
            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception)
            {
                // ignore for now
            }
            return NoContent();
        }
    }
}

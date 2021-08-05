using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Domain.Models.Dtos;
using TeusControle.Infrastructure.Dtos;

namespace TeusControle.Infrastructure.Controllers
{
    /// <summary>
    /// Controlador para CRUD de produtos
    /// </summary>
    [Route("api/Products")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _service;
        public ProductsController(IProductsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Inserir um novo produto
        /// </summary>
        /// <param name="sentProduct"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert([FromBody] CreateProductsModel sentProduct)
        {
            return Ok(_service.Insert(sentProduct));
        }

        /// <summary>
        /// Atualizar um produto
        /// </summary>
        /// <param name="sentProduct"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromBody] UpdateProductsModel sentProduct)
        {
            return Ok(_service.Update(sentProduct));
        }

        /// <summary>
        /// Buscar um produto por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetById")]
        public IActionResult GetById([FromHeader] long id)
        {
            return Ok(_service.GetById(id));
        }

        /// <summary>
        /// Buscar todos os produtos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        public IActionResult GetPaged([FromQuery] DataSourceRequest request)
        {
            return Ok(_service.GetPaged(
                pageNumber: request.Page,
                pageSize: request.PageSize,
                filterDescriptors: request.Filter,
                sort: request.OrderBy
            ));
        }

        /// <summary>
        /// Excluir um produto por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete([FromHeader] long id)
        {
            return Ok(_service.DeleteById(id));
        }
    }
}

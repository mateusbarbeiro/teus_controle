using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Domain.Models.Dtos;
using TeusControle.Infrastructure.Dtos;

namespace TeusControle.Infrastructure.Controllers
{
    [Route("api/EntriesController")]
    [ApiController]
    [Authorize]
    public class EntriesController : Controller
    {
        private readonly IEntriesService _service;
        public EntriesController(IEntriesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Criar uma entrada de produto
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Insert")]
        public IActionResult CreateEntry(CreateEntryModel entryModel)
        {
            return Ok(_service.CreateEntry(entryModel));
        }

        /// <summary>
        /// Atualiza uma entrada de produto
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult UpdateEntry(UpdateEntryModel entryModel)
        {
            return Ok(_service.Update(entryModel));
        }

        /// <summary>
        /// Insere um novo item de produto a uma respectiva entrada de produtos
        /// </summary>
        /// <param name="productEntry"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertProductEntryItem")]
        public IActionResult InsertProductEntryItem(CreateProductEntriesModel productEntry)
        {
            return Ok(_service.InsertProductEntryItem(productEntry));
        }

        /// <summary>
        /// Remove um item de produto de uma respectiva entrada de produtos
        /// </summary>
        /// <param name="entity_id"></param>
        /// <param name="product_id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteProductItem")]
        public IActionResult DeleteProductItem(
            long entity_id,
            long product_id
        )
        {
            return Ok(_service.DeleteProductEntryItem(
                entity_id, 
                product_id
            ));
        }

        /// <summary>
        /// Deleta uma entrada de produto
        /// </summary>
        /// <param name="entity_id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeleteEntry(long entity_id)
        {
            return Ok(_service.DeleteEntry(entity_id));
        }

        /// <summary>
        /// Finaliza entrada de produtos
        /// </summary>
        /// <param name="entry_id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CloseEntry")]
        public IActionResult CloseEntry(long entry_id)
        {
            return Ok(_service.CloseEntry(entry_id));
        }

        /// <summary>
        /// Busca todas as entradas
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
        /// Busca informações de uma entrada
        /// </summary>
        /// <param name="entry_id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetById")]
        public IActionResult GetById(long entry_id)
        {
            return Ok(_service.GetById(entry_id));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Domain.Models.Dtos;
using TeusControle.Infrastructure.Dtos;

namespace TeusControle.Infrastructure.Controllers
{
    /// <summary>
    /// Controlador para CRUD de usuário
    /// </summary>
    [Route("api/Users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        public UsersController(IUsersService service)
        {
            _service = service;
        }

        /// <summary>
        /// Inserir um novo usuário
        /// </summary>
        /// <param name="sentUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert([FromBody] CreateUserModel sentUser)
        {
            return Ok(_service.Insert(sentUser));
        }

        /// <summary>
        /// Atualizar um usuário
        /// </summary>
        /// <param name="sentUser"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromBody] UpdateUserModel sentUser)
        {
            return Ok(_service.Update(sentUser));
        }

        /// <summary>
        /// Buscar usuário por id
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
        /// Buscar todos os usuários
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
        /// Excluir um usuário por id
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

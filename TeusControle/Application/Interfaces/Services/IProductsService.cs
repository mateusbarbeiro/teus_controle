using TeusControle.Domain.Models;
using TeusControle.Domain.Models.Dtos;
using TeusControle.Infrastructure.Dtos;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Application.Interfaces.Services
{
    /// <summary>
    /// Inteface para serviço de usuários. Declaração de métodos
    /// </summary>
    public interface IProductsService : IBaseService<Products>
    {
        /// <summary>
        /// Insere um novo usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResponseMessages<object> Insert(CreateProductsModel user);

        /// <summary>
        /// Atualiza um registro de usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResponseMessages<object> Update(UpdateProductsModel user);

        /// <summary>
        /// Busca um registro por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseMessages<object> GetById(long id);

        /// <summary>
        /// Busca lista de todos os produtos
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterDescriptors"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        ResponseMessages<object> GetPaged(
            int pageNumber,
            int pageSize,
            DynamicFilter filterDescriptors = null,
            DynamicOrderBy sort = null
        );

        /// <summary>
        /// Deleta um registro de usuário
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseMessages<object> DeleteById(long id);

        /// <summary>
        /// Incrementa quantidade em estoque em valor informado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ReturnData<object> IncremetInStock(
            long id,
            decimal value
        );

        /// <summary>
        /// Decrementa quantidade em estoque em valor informado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ReturnData<object> DecrementInStock(
            long id,
            decimal value
        );
    }
}

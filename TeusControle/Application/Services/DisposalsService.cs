using AutoMapper;
using EnumsNET;
using System;
using System.Linq;
using TeusControle.Application.Interfaces.Repository;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Application.Validators;
using TeusControle.Domain.Models;
using TeusControle.Domain.Models.Dtos;
using TeusControle.Domain.Models.Enums;
using TeusControle.Infrastructure.Dtos;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Application.Services
{
    /// <summary>
    /// Saída de produtos
    /// </summary>
    public class DisposalsService : BaseService<Disposals>, IDisposalsService
    {
        private IDisposalsRepository _baseRepository;
        private IProductDisposalsService _productsDisposalsService;
        private IProductsService _productsService;

        public DisposalsService(
            IDisposalsRepository baseRepository,
            IProductDisposalsService productsDisposalsService,
            IProductsService productsService,
            IMapper mapper
        ) : base(
            (IBaseRepository<Disposals>)baseRepository,
            mapper
        )
        {
            _baseRepository = baseRepository;
            _productsDisposalsService = productsDisposalsService;
            _productsService = productsService;
        }

        /// <summary>
        /// Cria um registro de saída de produtos
        /// </summary>
        /// <param name="disposalModel"></param>
        /// <returns></returns>
        public ResponseMessages<object> Insert(CreateDisposalModel disposalModel)
        {
            try
            {
                var data = Add<CreateDisposalModel, DisposalModel, DisposalsValidator>(disposalModel);

                return new ResponseMessages<object>(
                    status: true,
                    data: data,
                    message: "Registro para saída de produtos criado com sucesso."
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false,
                    message: $"Erro: {ex.Message}",
                    data: ex.StackTrace
                );
            }
        }

        /// <summary>
        /// Atualiza um registro de saída de produtos
        /// </summary>
        /// <param name="disposalModel"></param>
        /// <returns></returns>
        public ResponseMessages<object> Update(UpdateDisposalModel disposalModel)
        {
            try
            {
                var data = Update<UpdateDisposalModel, DisposalModel, DisposalsValidator>(disposalModel);
                // ToDo: persistir alterações em uma tabela para logs
                return new ResponseMessages<object>(
                    status: true,
                    message: "Saída de produto alterada com sucesso.",
                    data: data
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false,
                    message: $"Erro: { ex.Message }",
                    data: ex.Data
                );
            }
        }

        /// <summary>
        /// Insere um item de produto a saída
        /// </summary>
        /// <param name="productDisposal"></param>
        /// <returns></returns>
        public ResponseMessages<object> InsertProductDisposalItem(CreateProductDisposalsModel productDisposal)
        {
            return _productsDisposalsService.InsertProductDisposalsItem(productDisposal);
        }

        /// <summary>
        /// Remove um item de produto da saída
        /// </summary>
        /// <param name="disposalId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ResponseMessages<object> DeleteProductDisposalItem(
            long disposalId,
            long productId
        )
        {
            return _productsDisposalsService.DeleteProductDisposalItem(
                disposalId,
                productId
            );
        }

        /// <summary>
        /// Deleta uma saída
        /// </summary>
        /// <param name="disposalId"></param>
        /// <returns></returns>
        public ResponseMessages<object> Delete(long disposalId)
        {
            try
            {
                if (!_baseRepository.Any(x =>
                    x.Id == disposalId &&
                    !x.Deleted
                ))
                    throw new Exception("Saída não encontrada.");
                
                LogicalDelete(disposalId);

                return new ResponseMessages<object>(
                    status: true,
                    message: "Registro para saída de produtos deletado com sucesso."
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false,
                    message: $"Erro: {ex.Message}",
                    data: ex.StackTrace
                );
            }
        }

        /// <summary>
        /// Fluxo para fechar uma saída de produto e atualizar quantidade em estoque de produtos
        /// </summary>
        /// <param name="disposalId"></param>
        /// <returns></returns>
        public ResponseMessages<object> CloseDisposal(long disposalId)
        {
            try
            {
                // passar por lista de produtos adicionados a esta saída e atualizar
                // quantidade em estoque
                var disposal = QueryDb(q => q.Id == disposalId)
                    .Select(s => new Disposals
                    {
                        Id = s.Id,
                        Status = s.Status
                    })
                    .FirstOrDefault();

                if (disposal == null)
                    throw new Exception("Saída não encontrada.");

                if (disposal.Status == DisposalsStatusEnum.Canceled || disposal.Status == DisposalsStatusEnum.Closed)
                    throw new Exception("Não é possível finalizar saída. Status inválido para operação.");

                disposal.ProductDisposals = _productsDisposalsService.QueryDb(q =>
                        q.Id == disposalId &&
                        !q.Deleted &&
                        q.Active
                    )
                    .Select(s => new ProductDisposals
                    {
                        Id = s.Id,
                        Id2 = s.Id2,
                        Amount = s.Amount,
                        Active = s.Active
                    })
                    .ToList();

                var sucess = disposal.ProductDisposals.Count;
                var data = "";
                foreach (var item in disposal.ProductDisposals)
                {
                    var result = _productsService.DecrementInStock(
                        item.Id2,
                        item.Amount
                    );

                    if (result.Sucess)
                    {
                        sucess -= 1;
                        item.Active = false;
                        _productsDisposalsService.UpdateSomeFields(
                            item,
                            b => b.Active
                        );
                    }
                    else
                        data += $"Produto id:{item.Id2}. Erro: {result.Data.ToString()}\n";
                }

                if (sucess != 0)
                {
                    disposal.Status = DisposalsStatusEnum.Pending;
                    disposal.LastChange = DateTime.Now;

                    UpdateSomeFields(
                        disposal,
                        u => u.Status,
                        u => u.LastChange
                    );

                    // ToDo: persistir erros
                    return new ResponseMessages<object>(
                        status: false,
                        data: data,
                        message: "Ocorreu um erro ao atualizar estoque dos itens."
                    );
                }


                // atualizar status da saída
                disposal.Status = DisposalsStatusEnum.Closed;
                disposal.LastChange = DateTime.Now;
                disposal.ClosingDate = DateTime.Now;

                UpdateSomeFields(
                    disposal, 
                    u => u.Status, 
                    u => u.LastChange,
                    u => u.ClosingDate
                );

                return new ResponseMessages<object>(
                    status: true,
                    message: "Saída fechada com sucesso."
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false,
                    message: $"Erro: {ex.Message}",
                    data: ex.Data
                );
            }
        }

        /// <summary>
        /// Busca informações de uma saída
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseMessages<object> GetById(long id)
        {
            try
            {
                if (!_baseRepository.Any(x =>
                    x.Id == id &&
                    !x.Deleted &&
                    x.Active
                ))
                    throw new Exception("Registro não encontrado.");

                var data = _baseRepository
                    .QueryDb(x => x.Id == id)
                    .Select(s => new
                    {
                        s.Id,
                        s.Status,
                        StatusDescription = ((DisposalsStatusEnum)s.Status).AsString(EnumFormat.Description),
                        s.ClosingDate,
                        s.Active,
                        s.CreatedDate,
                        s.ProductDisposals
                    })
                    .FirstOrDefault();

                return new ResponseMessages<object>(
                    status: true,
                    message: "Saída encontrada.",
                    data: data
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false,
                    message: $"Erro: { ex.Message }"
                );
            }
        }

        /// <summary>
        /// Busca lista paginada de todas as saídas de produtos cadastradas
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterDescriptors"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public ResponseMessages<object> GetPaged(
            int pageNumber, 
            int pageSize, 
            DynamicFilter filterDescriptors = null,
            DynamicOrderBy sort = null
        )
        {
            try
            {
                var filter = new FilterBy<Disposals>(filterDescriptors);
                filter.AddExpression(x => !x.Deleted);

                var select = new Select<Disposals>(x => new
                {
                    x.Id,
                    x.Status,
                    StatusDescription = ((DisposalsStatusEnum)x.Status).AsString(EnumFormat.Description),
                    x.ClosingDate,
                    x.Active,
                    x.CreatedDate,
                });

                var products = GetPaged(
                    page: pageNumber,
                    pageSize: pageSize,
                    filter: filter,
                    select: select,
                    sortDynamic: sort
                );

                return new ResponseMessages<object>(
                    status: true,
                    message: "Busca realizada com sucesso.",
                    data: products
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false,
                    message: $"Erro: { ex.Message }"
                );
            }
        }
    }
}

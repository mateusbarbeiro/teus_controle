using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Domain.Models.CommonModels;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Application.Services
{
    /// <summary>
    /// Classe de serviço genérica. CRUD genérico.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class BaseDoubleService<TEntity> : IBaseDoubleService<TEntity> where TEntity : BaseDoubleEntity, new()
    {
        /// <summary>
        /// Retorna se para a condição, existe tal registro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool Any(FilterBy<TEntity> filter = null)
        {
            return _baseRepository.Any(filter);
        }

        /// <summary>
        /// Retorna se para a condição, existe tal registro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return _baseRepository.Any(filter);
        }

        /// <summary>
        /// Busca quantidade de registros a partir do filtro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public int Count(FilterBy<TEntity> filter = null)
        {
            return _baseRepository.Count(filter);
        }

        /// <summary>
        /// Busca quantidade de registros a partir do filtro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public int Count(Expression<Func<TEntity, bool>> filter)
        {
            return _baseRepository.Count(filter);
        }

        /// <summary>
        /// Busca paginada
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetPage(
            int startRow,
            int pageSize,
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            return _baseRepository.GetPage(
                startRow,
                pageSize,
                filter,
                orderBy,
                includes
            );
        }

        /// <summary>
        /// Calcula qual a linha de start da listagem, conforme número da pagina atual e total de paginas
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected int CalcStartRow(int pageNumber, int pageSize)
        {
            var startRow = (pageNumber - 1) * pageSize;
            if (startRow < 0)
                startRow = 0;

            return startRow;
        }


        /// <summary>
        /// Busca paginada dinamica
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="select"></param>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        private object GetPaged(
            int page,
            int pageSize,
            Select<TEntity> select,
            FilterBy<TEntity> filter,
            OrderBy<TEntity> orderBy
        )
        {
            var data = _baseRepository.GetPageAnonymous(
                startRow: CalcStartRow(page, pageSize),
                pageSize: pageSize,
                select: select,
                filter: filter,
                orderBy: orderBy
            );

            var totalCount = _baseRepository.Count(filter);

            var dataPage = new
            {
                Data = data,
                TotalRecords = totalCount,
                PageSize = pageSize,
                Page = page
            };

            return dataPage;
        }

        /// <summary>
        /// Busca paginada
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="select"></param>
        /// <param name="filterDynamic"></param>
        /// <param name="sortDynamic"></param>
        /// <returns></returns>
        public virtual object GetPaged(
            int page,
            int pageSize,
            Select<TEntity> select,
            DynamicFilter filterDynamic,
            DynamicOrderBy sortDynamic
        )
        {
            return GetPaged(
                page: page,
                pageSize: pageSize,
                select: select,
                filter: new FilterBy<TEntity>(filterDynamic),
                orderBy: new OrderBy<TEntity>(sortDynamic)
            );
        }

        /// <summary>
        /// Busca paginada
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="select"></param>
        /// <param name="filter"></param>
        /// <param name="sortDynamic"></param>
        /// <returns></returns>
        public virtual object GetPaged(
            int page,
            int pageSize,
            Select<TEntity> select,
            FilterBy<TEntity> filter,
            DynamicOrderBy sortDynamic
        )
        {
            return GetPaged(
                page: page,
                pageSize: pageSize,
                select: select,
                filter: filter,
                orderBy: new OrderBy<TEntity>(sortDynamic)
            );
        }

        /// <summary>
        /// Realiza uma busca
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> Query(
            FilterBy<TEntity> filter,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            return _baseRepository.Query(
                filter,
                orderBy,
                includes
            );
        }

        /// <summary>
        /// Constrói busca no banco
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IQueryable<TEntity> QueryDb(Expression<Func<TEntity, bool>> filter)
        {
            return _baseRepository.QueryDb(filter);
        }

        /// <summary>
        /// Constrói busca no banco
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public IQueryable<TEntity> QueryDb(
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            return _baseRepository.QueryDb(
                filter,
                orderBy,
                includes
            );
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TeusControle.Application.Interfaces.Repository;
using TeusControle.Domain.Models.CommonModels;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Infrastructure.Repository
{
    /// <summary>
    /// Classe genérica para repositório. CRUD genério.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Busca todos os registros
        /// </summary>
        /// <returns></returns>
        public IList<TEntity> Select() =>
            _context.Set<TEntity>().ToList();

        /// <summary>
        /// Busca um registro por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity Select(long id) =>
            _context.Set<TEntity>().Find(id);

        /// <summary>
        /// Retorna se para a condição, existe tal registro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual bool Any(FilterBy<TEntity> filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null && filter.Expressions != null)
                query = query.Where(filter.Expressions);

            return query.Any();
        }

        /// <summary>
        /// Retorna se para a condição, existe tal registro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual bool Any(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            return query.Any(filter);
        }

        /// <summary>
        /// Contrói uma busca
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Query(
            FilterBy<TEntity> filter,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            var result = QueryDb(
                filter,
                orderBy,
                includes
            );

            return result.ToList();
        }

        /// <summary>
        /// Constrói busca no banco
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> QueryDb(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter == null)
                throw new ArgumentNullException("É necessário informar o filtro");

            return query.Where(filter).AsNoTracking();
        }

        /// <summary>
        /// Constrói busca no banco
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> QueryDb(
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            return QueryDbBase(filter, orderBy, includes).AsNoTracking();
        }

        /// <summary>
        /// Constroi busca no banco
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        private IQueryable<TEntity> QueryDbBase(
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null && filter.Expressions != null)
                query = query.Where(filter.Expressions);

            if (includes != null && includes.Expressions != null)
                query = includes.Expressions(query);

            if (orderBy != null && orderBy.Expressions != null)
                query = orderBy.Expressions(query);

            return query;
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
        public virtual IEnumerable<TEntity> GetPage(
            int startRow,
            int pageSize,
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            if (orderBy == null) orderBy = GetDefaultOrderBy();

            var result = QueryDb(
                filter,
                orderBy,
                includes
            );

            return result
                .Skip(startRow)
                .Take(pageSize)
                .ToList();
        }

        /// <summary>
        /// Busca quantidade de registros a partir do filtro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual int Count(FilterBy<TEntity> filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null && filter.Expressions != null)
                query = query.Where(filter.Expressions);

            return query.Count();
        }

        /// <summary>
        /// Busca quantidade de registros a partir do filtro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            query = query.Where(filter);

            return query.Count();
        }

        /// <summary>
        /// Busca paginada anonima
        /// </summary>
        /// <param name="startRow">Registro inicial</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="select">Objeto a ser buscado</param>
        /// <param name="filter">Filtro</param>
        /// <param name="orderBy">Ordenação</param>
        /// <param name="includes">Incluir</param>
        /// <returns></returns>
        public virtual IEnumerable<object> GetPageAnonymous(
            int startRow,
            int pageSize,
            Select<TEntity> select,
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        )
        {
            if (orderBy == null) orderBy = GetDefaultOrderBy();

            var result = QueryDb(
                filter,
                orderBy,
                includes
            );

            var ret = result
                .Skip(startRow)
                .Take(pageSize)
                .Select(select.Expressions);

            return ret.ToList();
        }
    }
}
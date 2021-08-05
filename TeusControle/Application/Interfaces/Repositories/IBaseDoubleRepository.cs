using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TeusControle.Domain.Models.CommonModels;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Application.Interfaces.Repository
{
    /// <summary>
    /// Interface genérica para repositório para entidade com chave composta. Declaração dos métodos para CRUD.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseDoubleRepository<TEntity> where TEntity : BaseDoubleEntity
    {
        /// <summary>
        /// Insere um novo registro
        /// </summary>
        /// <param name="obj"></param>
        void Insert(TEntity obj);

        /// <summary>
        /// Atualiza um registro
        /// </summary>
        /// <param name="obj"></param>
        void Update(TEntity obj);

        /// <summary>
        /// Deleta um registro a partir do Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id"></param>
        void PhysicalDelete(
            long id,
            long id2
        );

        /// <summary>
        /// Atualizar alguns campos
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeProperties"></param>
        void UpdateFields(
            TEntity entity, 
            params Expression<Func<TEntity, object>>[] includeProperties
        );

        /// <summary>
        /// Busca um registro por Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        TEntity Select(
            long id,
            long id2
        );

        /// <summary>
        /// Retorna se para a condição, existe tal registro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool Any(FilterBy<TEntity> filter = null);

        /// <summary>
        /// Retorna se para a condição, existe tal registro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool Any(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Constrói busca no banco
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IQueryable<TEntity> QueryDb(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Constrói busca no banco
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        IQueryable<TEntity> QueryDb(
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        );

        /// <summary>
        /// Contrói uma busca
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        IEnumerable<TEntity> Query(
            FilterBy<TEntity> filter,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        );

        /// <summary>
        /// Busca paginada
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetPage(
            int startRow,
            int pageSize,
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        );

        /// <summary>
        /// Busca quantidade de registros a partir do filtro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        int Count(FilterBy<TEntity> filter = null);

        /// <summary>
        /// Busca quantidade de registros a partir do filtro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> filter);

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
        IEnumerable<object> GetPageAnonymous(
            int startRow,
            int pageSize,
            Select<TEntity> select,
            FilterBy<TEntity> filter = null,
            OrderBy<TEntity> orderBy = null,
            Includes<TEntity> includes = null
        );
    }
}
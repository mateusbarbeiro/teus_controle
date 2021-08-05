using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TeusControle.Domain.Models.CommonModels;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Application.Interfaces.Services
{
    /// <summary>
    /// Interface da classe genérica dos serviços para entidades com chave composta. Declaração dos métodos para CRUD.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseDoubleService<TEntity> where TEntity : BaseDoubleEntity
    {
        /// <summary>
        /// Exclui logicamente um registro a partir do id e id2
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id2"></param>
        void LogicalDelete(long id, long id2);

        /// <summary>
        /// Cria um novo registro
        /// </summary>
        /// <typeparam name="TInputModel"></typeparam>
        /// <typeparam name="TOutputModel"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        TOutputModel Add<TInputModel, TOutputModel, TValidator>(TInputModel inputModel)
            where TValidator : AbstractValidator<TEntity>
            where TInputModel : class
            where TOutputModel : class;

        /// <summary>
        /// Cria um novo registro
        /// </summary>
        /// <typeparam name="TOutputModel"></typeparam>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public TOutputModel Add<TOutputModel>(TEntity inputModel)
            where TOutputModel : class;

        /// <summary>
        /// Exclui fisicamente um registro a partir do id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id2"></param>
        void PhysicalDelete(
            long id, 
            long id2
        );

        /// <summary>
        /// Atualiza um registro
        /// </summary>
        /// <typeparam name="TInputModel"></typeparam>
        /// <typeparam name="TOutputModel"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        TOutputModel Update<TInputModel, TOutputModel, TValidator>(TInputModel inputModel)
            where TValidator : AbstractValidator<TEntity>
            where TInputModel : class
            where TOutputModel : class;

        /// <summary>
        /// Realiza uma busca
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
        /// Busca paginada
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="select"></param>
        /// <param name="filterDynamic"></param>
        /// <param name="sortDynamic"></param>
        /// <returns></returns>
        object GetPaged(
            int page,
            int pageSize,
            Select<TEntity> select,
            DynamicFilter filterDynamic,
            DynamicOrderBy sortDynamic
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
        /// Atualiza alguns campos
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updatedProperties"></param>
        void UpdateSomeFields(
            TEntity entity,
            params Expression<Func<TEntity, object>>[] updatedProperties
        );
    }
}
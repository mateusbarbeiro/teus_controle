﻿using System;
using System.Linq;
using System.Linq.Expressions;
using TeusControle.Application.Interfaces.Repository;
using TeusControle.Domain.Models.CommonModels;
using TeusControle.Infrastructure.Context;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Infrastructure.Repository
{
    /// <summary>
    /// Classe genérica para repositório. CRUD genério.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class BaseDoubleRepository<TEntity> : IBaseDoubleRepository<TEntity> where TEntity : BaseDoubleEntity
    {
        protected readonly ApiContext _context;
        public BaseDoubleRepository(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Insere um novo registro
        /// </summary>
        /// <param name="obj"></param>
        public void Insert(TEntity obj)
        {
            _context.Set<TEntity>().Add(obj);
            _context.SaveChanges();
        }

        /// <summary>
        /// Atualiza um registro
        /// </summary>
        /// <param name="obj"></param>
        public void Update(TEntity obj)
        {
            _context.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }

        /// <summary>
        /// Deleta um registro a partir do Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id2"></param>
        public void PhysicalDelete(
            long id,
            long id2
        )
        {
            _context.Set<TEntity>().Remove(Select(id, id2));
            _context.SaveChanges();
        }

        /// <summary>
        /// Atualizar alguns campos
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeProperties"></param>
        public void UpdateFields(TEntity entity, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var dbEntry = _context.Entry(entity);

            foreach (var includeProperty in includeProperties)
            {
                dbEntry.Property(includeProperty).IsModified = true;
            }

            _context.SaveChanges();
        }
    }
}
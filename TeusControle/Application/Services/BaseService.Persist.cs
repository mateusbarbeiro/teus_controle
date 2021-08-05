﻿using AutoMapper;
using FluentValidation;
using System;
using System.Linq.Expressions;
using TeusControle.Application.Interfaces.Repository;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Domain.Models.CommonModels;

namespace TeusControle.Application.Services
{
    /// <summary>
    /// Classe de serviço genérica. CRUD genérico.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly IBaseRepository<TEntity> _baseRepository;
        private readonly IMapper _mapper;

        public BaseService(
            IBaseRepository<TEntity> baseRepository,
            IMapper mapper
        )
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo registro
        /// </summary>
        /// <typeparam name="TInputModel"></typeparam>
        /// <typeparam name="TOutputModel"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public virtual TOutputModel Add<TInputModel, TOutputModel, TValidator>(TInputModel inputModel)
            where TValidator : AbstractValidator<TEntity>
            where TInputModel : class
            where TOutputModel : class
        {
            TEntity entity = _mapper.Map<TEntity>(inputModel);

            Validate(entity, Activator.CreateInstance<TValidator>());
            // ToDo: buscar usuario q inseriu direto das informações do token
            entity.CreatedBy = 2;
            _baseRepository.Insert(entity);
            /*var userId = HttpContextAccessor.User.FindFirst(CustomClaimTypes.Id);*/

            TOutputModel outputModel = _mapper.Map<TOutputModel>(entity);

            return outputModel;
        }

        /// <summary>
        /// Cria um novo registro
        /// </summary>
        /// <typeparam name="TOutputModel"></typeparam>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public TOutputModel Add<TOutputModel>(TEntity inputModel)
            where TOutputModel : class
        {
            // ToDo: buscar usuario q inseriu direto das informações do token
            inputModel.CreatedBy = 2;
            _baseRepository.Insert(inputModel);
            /*var userId = HttpContextAccessor.User.FindFirst(CustomClaimTypes.Id);*/

            TOutputModel outputModel = _mapper.Map<TOutputModel>(inputModel);

            return outputModel;
        }

        /// <summary>
        /// Exclui fisicamente um registro a partir do id
        /// </summary>
        /// <param name="id"></param>
        public void PhysicalDelete(long id) => _baseRepository.PhysicalDelete((int)id);

        /// <summary>
        /// Exclui logicamente um registro a partir do id
        /// </summary>
        /// <param name="id"></param>
        public void LogicalDelete(long id)
        {
            if (!_baseRepository.Any(x => 
                x.Id == id && 
                !x.Deleted
            ))
                throw new Exception("Registro não encontrado.");

            var entity = new TEntity
            {
                Id = id,
                Deleted = true
            };

            _baseRepository.UpdateFields(entity, b => b.Deleted);
        }

        /// <summary>
        /// Atualiza um registro
        /// </summary>
        /// <typeparam name="TInputModel"></typeparam>
        /// <typeparam name="TOutputModel"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public TOutputModel Update<TInputModel, TOutputModel, TValidator>(TInputModel inputModel)
            where TValidator : AbstractValidator<TEntity>
            where TInputModel : class
            where TOutputModel : class
        {
            TEntity entity = _mapper.Map<TEntity>(inputModel);

            Validate(
                entity,
                Activator.CreateInstance<TValidator>()
            );
            entity.CreatedBy = 2; // buscar da entidade
            _baseRepository.Update(entity);

            TOutputModel outputModel = _mapper.Map<TOutputModel>(entity);

            return outputModel;
        }

        /// <summary>
        /// Valida o objeto a ser cadastrado
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="validator"></param>
        private void Validate(TEntity obj, AbstractValidator<TEntity> validator)
        {
            if (obj == null)
                throw new Exception("Registros não detectados!");

            validator.ValidateAndThrow(obj);
        }

        /// <summary>
        /// Atualiza alguns campos
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updatedProperties"></param>
        public void UpdateSomeFields(
            TEntity entity, 
            params Expression<Func<TEntity, object>>[] updatedProperties
        )
        {
            _baseRepository.UpdateFields(
                entity,
                updatedProperties
            );
        }
    }
}

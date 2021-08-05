using AutoMapper;
using System;
using System.Linq;
using TeusControle.Application.Interfaces.Repository;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Application.Validators;
using TeusControle.Domain.Models;
using TeusControle.Domain.Models.Dtos;
using TeusControle.Infrastructure.Dtos;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Application.Services
{
    /// <summary>
    /// Usuários
    /// </summary>
    public class UsersService : BaseService<Users>, IUsersService
    {
        private IUsersRepository _baseRepository;

        public UsersService(
            IUsersRepository baseRepository, 
            IMapper mapper
        ) : base(
            (IBaseRepository<Users>)baseRepository, 
            mapper
        )
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Insere um novo usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResponseMessages<object> Insert(CreateUserModel user)
        {
            try
            {
                var data = Add<CreateUserModel, UserModel, UsersValidator>(user);
                return new ResponseMessages<object>(
                    status: true, 
                    data: data, 
                    message: "Usuário cadastrado com sucesso."
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false, 
                    message: $"Ocorreu um erro: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Atualiza um registro de usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResponseMessages<object> Update(UpdateUserModel user)
        {
            try
            {
                var data = Update<UpdateUserModel, UserModel, UsersValidator>(user);
                return new ResponseMessages<object>(
                    status: true,
                    message: "Usuário alterado com sucesso."
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessages<object>(
                    status: false,
                    message: $"Ocorreu um erro: { ex.Message }"
                );  
            }
        }

        /// <summary>
        /// Busca registro por id
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
                        s.Name,
                        s.CpfCnpj,
                        s.DocumentType,
                        s.ProfileImage,
                        s.ProfileType,
                        s.Email,
                        s.UserName,
                        s.Active,
                        s.CreatedDate,
                        s.BirthDate
                    })
                    .FirstOrDefault();

                return new ResponseMessages<object>(
                    status: true,
                    message: "Usuário encontrado.",
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
        /// Busca lista de todos os usuários
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
                var filter = new FilterBy<Users>(filterDescriptors);
                filter.AddExpression(x => !x.Deleted);

                var select = new Select<Users>(x => new
                {
                    x.Id,
                    x.Name,
                    x.CpfCnpj,
                    x.DocumentType,
                    x.ProfileImage,
                    x.ProfileType,
                    x.Email,
                    x.UserName,
                    x.Active,
                    x.CreatedDate,
                    x.BirthDate
                });

                var users = GetPaged(
                    page: pageNumber,
                    pageSize: pageSize,
                    filter: filter,
                    select: select,
                    sortDynamic: sort
                );

                return new ResponseMessages<object>(
                    status: true,
                    message: "Busca realizada com sucesso.",
                    data: users
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
        /// Deleta um registro de usuário
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseMessages<object> DeleteById(long id)
        {
            try
            {
                LogicalDelete(id);

                return new ResponseMessages<object>(
                    status: true,
                    message: "Usuário deletado com sucesso."
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

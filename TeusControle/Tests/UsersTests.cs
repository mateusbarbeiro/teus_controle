/*using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Application.Services;
using TeusControle.Domain.Models;
using TeusControle.Domain.Models.Dtos;

namespace TeusControle.Tests
{
    [TestFixture]
    public class UsersTests
    {
        private UsersService _usersService;
        public Users USER { get; set; }

        public UsersTests()
        {
            var service = new ServiceCollection();
            service.AddTransient<IUsersService, UsersService>();

            var provider = service.BuildServiceProvider();
            _usersService = (UsersService)provider.GetService<IUsersService>();
            USER = new Users
            {
                Name = "Teste",
                CpfCnpj = "254.084.120-16",
                DocumentType = 1,
                BirthDate = DateTime.Now,
                Email = "teste@test.com",
                UserName = "Teste_test",
                Password = "1234",
                ProfileType = 1,
                LastChange = DateTime.Now,
                CreatedDate = DateTime.Now,
                Active = true,
                Deleted = false,
                CreatedBy = 2
            };
        }

        [Test]
        public void InsertUser()
        {
            try
            {
                _usersService.Insert(new CreateUserModel {
                
                });
                Assert.False(false);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Test]
        public void UpdateUser()
        {
            try
            {
                _usersService.Update(new UpdateUserModel { 
                });
                Assert.False(false);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Test]
        public void GetUserById()
        {
            try
            {
                _usersService.GetById(1);
                Assert.False(false);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Test]
        public void GetAllUsers()
        {
            try
            {
                _usersService.GetPaged(1,10);
                Assert.False(false);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Test]
        public void Delete()
        {
            try
            {
                _usersService.DeleteById(1);
                Assert.False(false);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }
    }
}
*/